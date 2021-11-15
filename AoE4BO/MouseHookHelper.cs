using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Timers;

namespace AoE4BO
{
    public class MouseHookHelper
    {
        public delegate void MouseClickEventHandler(object source, MouseClickEventArgs e);
        public delegate void MouseDragEventHandler(object source, MouseDragEventArgs e);
        public event MouseClickEventHandler OnClick;
        public event MouseDragEventHandler OnDrag;
        public event MouseClickEventHandler OnDown;
        private MouseHook _mouseHook;
        private bool _isMouseDown;
        private int _clickX;
        private int _clickY;
        private string[] _processNames;

        public MouseHookHelper(string[] processNames)
        {
            _mouseHook = new MouseHook();
            _processNames = processNames;
            ForegroundTracker.Start();
        }

        public void InstallHooks()
        {
            _mouseHook.MouseMove += _mouseHook_MouseMove;
            _mouseHook.LeftButtonDown += _mouseHook_LeftButtonDown;
            _mouseHook.LeftButtonUp += _mouseHook_LeftButtonUp;
            _mouseHook.Install();
        }

        public void DeinstallHooks()
        {
            _mouseHook.MouseMove -= _mouseHook_MouseMove;
            _mouseHook.LeftButtonDown -= _mouseHook_LeftButtonDown;
            _mouseHook.LeftButtonUp -= _mouseHook_LeftButtonUp;
            _mouseHook.Uninstall();
        }

        private void _mouseHook_LeftButtonDown(MouseHook.MSLLHOOKSTRUCT mouseStruct)
        {
            if (!ProcessAlive())
                return;

            if (OnDown != null)
                OnDown(null, new MouseClickEventArgs(mouseStruct.pt.x, mouseStruct.pt.y));

            _clickX = mouseStruct.pt.x;
            _clickY = mouseStruct.pt.y;
            _isMouseDown = true;
        }

        private void _mouseHook_LeftButtonUp(MouseHook.MSLLHOOKSTRUCT mouseStruct)
        {
            if (!ProcessAlive())
                return;

            if (OnClick != null && _isMouseDown)
                OnClick(null, new MouseClickEventArgs(mouseStruct.pt.x, mouseStruct.pt.y));

            _isMouseDown = false;
        }

        private void _mouseHook_MouseMove(MouseHook.MSLLHOOKSTRUCT mouseStruct)
        {
            if (!ProcessAlive())
                return;

            if (OnDrag != null && _isMouseDown)
                OnDrag(null, new MouseDragEventArgs(_clickX, _clickY, mouseStruct.pt.x, mouseStruct.pt.y));
        }

        private bool ProcessAlive()
        {
            return Array.IndexOf(_processNames, ForegroundTracker.ProcessName) > -1;
        }
    }

    public class MouseClickEventArgs : EventArgs
    {
        public int X { get; set; }
        public int Y { get; set; }

        public MouseClickEventArgs(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class MouseDragEventArgs : EventArgs
    {
        public int OrigX { get; set; }
        public int OrigY { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public MouseDragEventArgs(int origX, int origY, int x, int y)
        {
            OrigX = origX;
            OrigY = origY;
            X = x;
            Y = y;
        }
    }
}