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
using GlobalLowLevelHooks;

namespace Test
{
    public partial class Form1 : Form
    {
        MouseHook mouseHook;
        KeyboardHook keyboardHook;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Create the Mouse Hook
            mouseHook = new MouseHook();

            // Create the Keyboard Hook
            keyboardHook = new KeyboardHook();

            // Capture the events
            mouseHook.MouseMove += new MouseHook.MouseHookCallback(mouseHook_MouseMove);


            //Installing the Mouse Hooks
            mouseHook.Install();
            // Using the Keyboard Hook:

            // Capture the events
            keyboardHook.KeyDown += new KeyboardHook.KeyboardHookCallback(keyboardHook_KeyDown);
            keyboardHook.KeyUp += new KeyboardHook.KeyboardHookCallback(keyboardHook_KeyUp);

            //Installing the Keyboard Hooks
            keyboardHook.Install();
        }

        private void keyboardHook_KeyUp(KeyboardHook.VKeys key)
        {
            Console.WriteLine("c");
        }

        private void keyboardHook_KeyDown(KeyboardHook.VKeys key)
        {
            Console.WriteLine("b");
        }

        private void mouseHook_MouseMove(MouseHook.MSLLHOOKSTRUCT mouseStruct)
        {
            Console.WriteLine("a");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            keyboardHook.KeyDown -= new KeyboardHook.KeyboardHookCallback(keyboardHook_KeyDown);
            keyboardHook.KeyUp -= new KeyboardHook.KeyboardHookCallback(keyboardHook_KeyUp);
            keyboardHook.Uninstall();
            mouseHook.MouseMove -= new MouseHook.MouseHookCallback(mouseHook_MouseMove);
            mouseHook.Uninstall();
        }
    }
}
