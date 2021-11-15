using System;
using System.Diagnostics;
using System.Threading;
using Overlay.NET.Common;
using Overlay.NET.Directx;
using Process.NET;
using Process.NET.Memory;
using Process.NET.Windows;

namespace AoE4BO
{
    [RegisterPlugin("DirectXverlayDemo-1", "NAME", "DirectXOverlayDemo", "0.0", "A basic demo of the DirectXoverlay.")]
    public class Overlay : DirectXOverlayPlugin
    {
        private readonly TickEngine _tickEngine = new TickEngine();
        private Thread _thread;
        private ProcessSharp _processSharp;
        private Stopwatch _gameTime;
        private GfxBuildOrder _gfxBuildOrder;
        private bool _stopDrawThread = false;
        private BuildOrder _buildOrder;

        public Overlay()
        {
            Global.OverlayState = OverlayState.Idle;
        }

        public void StartBuildOrder(BuildOrder buildOrder)
        {
            if (_thread != null && _thread.ThreadState == System.Threading.ThreadState.Background)
                StopBuildOrder();

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
            while (_stopDrawThread)
                Thread.Sleep(100);
            Global.OverlayState = OverlayState.Idle;
        }

        public void RestartBuildOrder()
        {
            if (_buildOrder != null)
            {
                StopBuildOrder();
                StartBuildOrder(_buildOrder);
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
            while (!_stopDrawThread)
            {
                try
                {
                    Global.OverlayState = OverlayState.WaitForAEO;

                    System.Diagnostics.Process process = null;
                    while (process == null)
                    {
                        if (_stopDrawThread)
                            return;

                        foreach (string processName in Global.Settings.ProcessName)
                            if (System.Diagnostics.Process.GetProcessesByName(processName).Length > 0)
                                process = System.Diagnostics.Process.GetProcessesByName(processName)[0];

                        Thread.Sleep(500);
                    }
                    _processSharp = new ProcessSharp(process, MemoryType.Remote);

                    var d3DOverlay = (Overlay)this;
                    Console.WriteLine(_processSharp.WindowFactory.MainWindow);
                    d3DOverlay.Initialize(_processSharp.WindowFactory.MainWindow);
                    d3DOverlay.Enable();

                    // create gfx objects
                    _gfxBuildOrder = new GfxBuildOrder(OverlayWindow.Graphics, _buildOrder);

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
                    Thread.Sleep(100);
                }
            }
            _stopDrawThread = false;
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
            if (_gfxBuildOrder != null)
                _gfxBuildOrder.Draw();

            OverlayWindow.Graphics.EndScene();
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