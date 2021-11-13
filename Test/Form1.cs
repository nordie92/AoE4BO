using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace TestGlobalMouseEvents
{
    public partial class Form1 : Form
    {
        MouseHookHelper _mouseHookHelper;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _mouseHookHelper = new MouseHookHelper("Spotify");
            _mouseHookHelper.OnClick += MouseHook_OnClick;
            _mouseHookHelper.OnDrag += MouseHook_OnDrag;

            ForegroundTracker.Start();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            _mouseHookHelper.InstallHooks();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            _mouseHookHelper.DeinstallHooks();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _mouseHookHelper.DeinstallHooks();
            ForegroundTracker.Stop();
        }

        private void MouseHook_OnClick(object source, MouseClickEventArgs e)
        {
            richTextBox1.AppendText("Click at (" + e.X + "," + e.Y + ")\n");
        }

        private void MouseHook_OnDrag(object source, MouseDragEventArgs e)
        {
            richTextBox1.AppendText("Drag from (" + e.OrigX + "," + e.OrigY + ") to (" + e.X + "," + e.Y + ")\n");
        }
    }
}
