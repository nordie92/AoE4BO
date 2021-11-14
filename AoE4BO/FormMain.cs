using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace AoE4BO
{
    public partial class FormMain : Form
    {
        private OCR _ocr;
        private Overlay _overlay;
        private BuildOrder _buildOrder;
        private bool _stepFinished;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            Global.Settings = new Settings();
            Global.Settings.Load();
            Global.GameData = new GameData();
            Global.MouseHook = new MouseHookHelper(Global.Settings.ProcessName);
            Global.MouseHook.InstallHooks();

            _ocr = new OCR();
            _overlay = new Overlay();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Global.MouseHook.DeinstallHooks();
        }

        private void timerUI_Tick(object sender, EventArgs e)
        {
            if (Global.BoState == BoState.Idle)
            {
                lbState.Text = "Waiting for build order!";
                lbState.ForeColor = Color.Black;
            }
            else if (Global.BoState == BoState.Running)
            {
                if (Global.OCRState == OCRState.Success)
                {
                    lbState.Text = "Build order running!";
                    lbState.ForeColor = Color.Green;
                }
                else if (Global.OCRState == OCRState.Warning)
                {
                    lbState.Text = "Text recognition warning!";
                    lbState.ForeColor = Color.Orange;
                }
                else if (Global.OCRState == OCRState.Error)
                {
                    lbState.Text = "Text recognition error!";
                    lbState.ForeColor = Color.Red;
                }
                else
                {
                    lbState.Text = "Waiting for match";
                    lbState.ForeColor = Color.Green;
                }
            }
            else if (Global.BoState == BoState.Finish)
            {
                lbState.Text = "Build order finished!";
                lbState.ForeColor = Color.Green;
            }

            if (_stepFinished)
            {
                _stepFinished = false;
                FillRichTextBox(_buildOrder);
            }
        }

        private void LoadBuildOrder(string buildOrderString)
        {
            try
            {
                _buildOrder = new BuildOrder(buildOrderString);
                _buildOrder.Start();
                _buildOrder.StepRefresh += _buildOrder_StepFinished;

                _overlay.StartBuildOrder(_buildOrder);
                _ocr.Start();

                FillRichTextBox(_buildOrder);
            }
            catch (Exception ex)
            {
                if (ex.Message == "parsing error")
                    MessageBox.Show("Error while parsing build order file", "Parse failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    throw ex;
            }
        }

        private void _buildOrder_StepFinished(object source, EventArgs e)
        {
            _stepFinished = true;
        }

        private void btnOpenBO_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "AoE4BO Files|*.aoe4bo";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string buildOrderString = File.ReadAllText(ofd.FileName, Encoding.UTF8);
                LoadBuildOrder(buildOrderString);
                Text = "AoE4BO - " + ofd.SafeFileName;
            }
        }

        private void btnBoFromClipboard_Click(object sender, EventArgs e)
        {
            string buildOrderString = Clipboard.GetText();
            LoadBuildOrder(buildOrderString);
            Text = "AoE4BO - unknown.aoe4bo";
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            FormSettings frm = new FormSettings();
            frm.Show();
        }

        private void btnRestartBO_Click(object sender, EventArgs e)
        {
            if (_buildOrder != null)
            {
                _buildOrder.Restart();
                _overlay.RestartBuildOrder();
                _ocr.Restart();
            }
        }

        private void FillRichTextBox(BuildOrder buildOrder)
        {
            rtbBuildOrder.Text = "";

            int reqWidth = GetRequirementsWidth(buildOrder);

            BuildOrderStep bos = buildOrder.FirstBuildOrderStep;
            do
            {
                if (bos.Comment.Length > 0)
                    AppendText(rtbBuildOrder, bos.Comment + "\n", Color.Gray);

                int i = 0;
                List<string> reqs = bos.GetRequirementStrings();
                List<string> instructs = bos.Instructions;
                string req = " ";
                string ins = " ";
                Color color = bos.IsActive ? Color.Green : Color.Black;
                while (req.Length != 0 && ins.Length != 0)
                {
                    req = reqs.Count > i ? reqs[i] : "";
                    ins = instructs.Count > i ? instructs[i] : "";
                    AppendText(rtbBuildOrder, FillLetters(req.Replace(";", " "), reqWidth) + " " + ins + "\n", color);
                    i++;
                }

                bos = bos.NextBuildOrderStep;
            } while (bos != null);
        }

        private int GetRequirementsWidth(BuildOrder buildOrder)
        {
            int width = 0;
            BuildOrderStep bos = buildOrder.FirstBuildOrderStep;
            do
            {
                List<string> reqs = bos.GetRequirementStrings();
                foreach (string req in reqs)
                    if (width < req.Length)
                        width = req.Length;

                bos = bos.NextBuildOrderStep;
            } while (bos != null);

            return width;
        }

        private void AppendText(RichTextBox box, string text, Color color)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
        }

        private string FillLetters(string input, int length)
        {
            string ret = input;
            for (int i = 0; i < length - input.Length; i++)
                ret = " " + ret;
            return ret;
        }
    }
}
