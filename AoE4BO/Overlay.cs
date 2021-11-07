using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using Overlay.NET.Common;
using Overlay.NET.Directx;
using Process.NET;
using Process.NET.Memory;
using Process.NET.Windows;

namespace AoE4BO
{
    [RegisterPlugin("DirectXverlayDemo-1", "NAME", "DirectXOverlayDemo", "0.0",
        "A basic demo of the DirectXoverlay.")]
    public class Overlay : DirectXOverlayPlugin
    {
        private readonly TickEngine _tickEngine = new TickEngine();
        private Thread _thread;
        private ProcessSharp _processSharp;
        private Stopwatch _gameTime;
        private GfxObject _gfxObject;
        private bool _stopDrawThread = false;
        private BuildOrder _buildOrder;

        public Overlay()
        {
            Global.OverlayState = OverlayState.Idle;
        }

        public void StartBuildOrder(BuildOrder buildOrder)
        {
            _buildOrder = buildOrder;

            // start stopwatch to get draw delta time
            _gameTime = new Stopwatch();
            _gameTime.Start();

            // start draw thread
            _stopDrawThread = false;
            _thread = new Thread(new ParameterizedThreadStart(DoWork));
            _thread.IsBackground = true;
            _thread.Start();

            Global.OverlayState = OverlayState.Running;
        }

        public void StopBuildOrder()
        {
            _stopDrawThread = true;
            Global.OverlayState = OverlayState.Idle;
        }

        public void RestartBuildOrder()
        {
            if (OverlayWindow != null)
            {
                // create gfx objects
                _gfxObject = new GfxBuildOrder(OverlayWindow.Graphics, null, _buildOrder);
            }
        }

        public override void Initialize(IWindow targetWindow)
        {
            // Set target window by calling the base method
            base.Initialize(targetWindow);

            OverlayWindow = new DirectXOverlayWindow(targetWindow.Handle, false);

            _tickEngine.PreTick += OnPreTick;
            _tickEngine.Tick += OnTick;
        }

        private void DoWork(object state)
        {
            try
            {
                Global.OverlayState = OverlayState.WaitForAEO;
                while (System.Diagnostics.Process.GetProcessesByName("RelicCardinal").Length == 0)
                {
                    if (_stopDrawThread)
                        return;

                    Thread.Sleep(500);
                }
                var process = System.Diagnostics.Process.GetProcessesByName("RelicCardinal")[0];
                _processSharp = new ProcessSharp(process, MemoryType.Remote);

                var d3DOverlay = (Overlay)this;
                d3DOverlay.Initialize(_processSharp.WindowFactory.MainWindow);
                d3DOverlay.Enable();

                // create gfx objects
                _gfxObject = new GfxBuildOrder(OverlayWindow.Graphics, null, _buildOrder);

                Global.OverlayState = OverlayState.Running;
                while (!_stopDrawThread)
                    d3DOverlay.Update();
            }
            catch (Exception)
            {
                Global.OverlayState = OverlayState.Error;
            }
            finally
            {
                if (!_stopDrawThread)
                    DoWork(state);
            }
        }

        private void OnTick(object sender, EventArgs e)
        {
            if (!OverlayWindow.IsVisible)
            {
                return;
            }

            OverlayWindow.Update();
            InternalRender();
        }

        private void OnPreTick(object sender, EventArgs e)
        {
            var targetWindowIsActivated = TargetWindow.IsActivated;
            if (!targetWindowIsActivated && OverlayWindow.IsVisible)
            {
                ClearScreen();
                OverlayWindow.Hide();
            }
            else if (targetWindowIsActivated && !OverlayWindow.IsVisible)
            {
                OverlayWindow.Show();
            }
        }

        // ReSharper disable once RedundantOverriddenMember
        public override void Enable()
        {
            _tickEngine.Interval = TimeSpan.FromMilliseconds(1000.0 / 30.0);
            _tickEngine.IsTicking = true;
            base.Enable();
        }

        // ReSharper disable once RedundantOverriddenMember
        public override void Disable()
        {
            _tickEngine.IsTicking = false;
            base.Disable();
        }

        public override void Update() => _tickEngine.Pulse();

        protected void InternalRender()
        {
            // calc delta time
            float deltaTime = (float)_gameTime.ElapsedMilliseconds / 1000f;
            _gameTime.Restart();

            OverlayWindow.Graphics.BeginScene();
            OverlayWindow.Graphics.ClearScene();

            // start recusive draw function
            if (_gfxObject != null)
                DrawGfxObjects(_gfxObject);

            OverlayWindow.Graphics.EndScene();
        }

        private void DrawGfxObjects(GfxObject gfxObject)
        {
            gfxObject.Draw();

            foreach (GfxObject gfxObj in gfxObject.GfxObjects)
                DrawGfxObjects(gfxObj);
        }

        public override void Dispose()
        {
            OverlayWindow.Dispose();
            base.Dispose();
        }

        private void ClearScreen()
        {
            OverlayWindow.Graphics.BeginScene();
            OverlayWindow.Graphics.ClearScene();
            OverlayWindow.Graphics.EndScene();
        }
    }
}