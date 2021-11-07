using Process.NET;
using Process.NET.Memory;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AoE4BO
{
    public partial class FormMain : Form
    {
        private OCR _ocr;
        private Overlay _overlay;
        private BuildOrder _buildOrder;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            Global.Settings = new Settings();
            Global.Settings.Load();
            Global.GameData = new GameData();

            _ocr = new OCR();
            _overlay = new Overlay();
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
            }
            else if (Global.BoState == BoState.Finish)
            {
                lbState.Text = "Build order finished!";
                lbState.ForeColor = Color.Green;
            }
        }

        private void btnOpenBO_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "AoE4BO Files|*.aoe4bo";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string buildOrderString = File.ReadAllText(ofd.FileName, Encoding.UTF8);

                    _buildOrder = new BuildOrder(buildOrderString);
                    _buildOrder.Start();
                    rtbBuildOrder.Text = buildOrderString;

                    _overlay.StartBuildOrder(_buildOrder);
                    Text = "AoE4BO - " + ofd.SafeFileName;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "parsing error")
                {
                    MessageBox.Show("Error while parsing build order file", "Parse failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    throw ex;
                }
            }
        }

        private void btnBoFromClipboard_Click(object sender, EventArgs e)
        {
            try
            {
                string buildOrderString = Clipboard.GetText();

                _buildOrder = new BuildOrder(buildOrderString);
                _buildOrder.Start();
                rtbBuildOrder.Text = buildOrderString;

                _overlay.StartBuildOrder(_buildOrder);
                Text = "AoE4BO - unknown.aoe4bo";
            }
            catch (Exception ex)
            {
                if (ex.Message == "parsing error")
                {
                    MessageBox.Show("Error while parsing build order file", "Parse failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    throw ex;
                }
            }
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
            }
        }
    }
}
