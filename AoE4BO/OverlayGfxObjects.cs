using Overlay.NET.Directx;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace AoE4BO
{
    public class GfxObject
    {
        public PointF Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
            }
        }
        private PointF _position;
        public SizeF Size
        {
            get
            {
                return _size;
            }
            set
            {
                _size = value;
            }
        }
        private SizeF _size;
        public float X
        {
            get
            {
                return Position.X;
            }
            set
            {
                Position = new PointF(value, Position.Y);
            }
        }
        public float Y
        {
            get
            {
                return Position.Y;
            }
            set
            {
                Position = new PointF(Position.X, value);
            }
        }
        public float Width
        {
            get
            {
                return Size.Width;
            }
            set
            {
                Size = new SizeF(value, Size.Height);
            }
        }
        public float Height
        {
            get
            {
                return Size.Height;
            }
            set
            {
                Size = new SizeF(Size.Width, value);
            }
        }
        public Direct2DRenderer Renderer
        {
            get
            {
                return _renderer;
            }
            set
            {
                _renderer = value;
            }
        }
        private Direct2DRenderer _renderer;

        public GfxObject(Direct2DRenderer renderer)
        {
            _renderer = renderer;
        }

        public virtual RectangleF GetRectangle()
        {
            return new RectangleF(_position, _size);
        }

        public virtual void Draw()
        {
            throw new NotImplementedException();
        }
    }

    public class GfxBuildOrder : GfxObject
    {
        public List<GfxObject> GfxObjects { get; set; }
        private int _colorBack;
        public BuildOrder BuildOrder;
        private float _pixelPerSecond = 3f;
        private float _targetY;
        private float _currentY;
        private int _font;
        private int _brushFrontRed;
        private int _brushFrontYellow;
        private int _brushBackColor;
        public bool Hide;
        public float OffsetY;
        private bool _resizeActive;
        private Point _resizeLastPos;
        private bool _positionActive;
        private Point _positionLastPos;

        public GfxBuildOrder(Direct2DRenderer renderer, BuildOrder buildOrder) : base(renderer)
        {
            BuildOrder = buildOrder;
            GfxObjects = new List<GfxObject>();
            Global.MouseHook.OnClick += MouseHook_OnClick;
            Global.MouseHook.OnDrag += MouseHook_OnDrag;
            Global.MouseHook.OnDown += MouseHook_OnDown;

            Position = new PointF(Global.Settings.BuildOrderContainerX, Global.Settings.BuildOrderContainerY);
            Size = new SizeF(Global.Settings.BuildOrderContainerWidth, Global.Settings.BuildOrderContainerHeight);

            _colorBack = renderer.CreateBrush(Color.FromArgb(200, 255, 255, 255));
            _font = renderer.CreateFont("Arial", 12);
            _brushFrontRed = renderer.CreateBrush(Color.FromArgb(200, 255, 0, 0));
            _brushFrontYellow = renderer.CreateBrush(Color.FromArgb(200, 255, 255, 0));
            _brushBackColor = renderer.CreateBrush(Color.FromArgb(200, 255, 255, 255));

            CreateBuildOrderSteps(renderer, buildOrder);
            GfxObjects.Add(new GfxButtonHide(renderer, this));
            GfxObjects.Add(new GfxButtonNextStep(renderer, this));
            GfxObjects.Add(new GfxButtonPrevStep(renderer, this));
            GfxObjects.Add(new GfxButtonRestart(renderer, this));
            GfxObjects.Add(new GfxButtonResize(renderer, this));
            GfxObjects.Add(new GfxButtonPosition(renderer, this));
        }

        private void MouseHook_OnDown(object source, MouseClickEventArgs e)
        {
            foreach (GfxObject gfxObj in GfxObjects)
            {
                if (typeof(GfxButtonResize) == gfxObj.GetType())
                {
                    _resizeActive = (gfxObj as GfxButtonResize).PointContains(e.X, e.Y);
                    _resizeLastPos = new Point(e.X, e.Y);
                }
                else if (typeof(GfxButtonPosition) == gfxObj.GetType())
                {
                    _positionActive = (gfxObj as GfxButtonPosition).PointContains(e.X, e.Y);
                    _positionLastPos = new Point(e.X, e.Y);
                }
            }
        }

        private void MouseHook_OnClick(object source, MouseClickEventArgs e)
        {
            foreach (GfxObject gfxObj in GfxObjects)
            {
                if (typeof(GfxButton).IsAssignableFrom(gfxObj.GetType()))
                {
                    (gfxObj as GfxButton).ClickIfHit(e.X, e.Y);
                }
            }

            if (_resizeActive || _positionActive)
            {
                Global.Settings.BuildOrderContainerX = (int)X;
                Global.Settings.BuildOrderContainerY = (int)Y;
                Global.Settings.BuildOrderContainerWidth = (int)Width;
                Global.Settings.BuildOrderContainerHeight = (int)Height;
                Global.Settings.Save();
            }

            _resizeActive = false;
            _positionActive = false;
        }

        private void MouseHook_OnDrag(object source, MouseDragEventArgs e)
        {
            if (_resizeActive)
            {
                Width += e.X - _resizeLastPos.X;
                Height += e.Y - _resizeLastPos.Y;
                _resizeLastPos = new Point(e.X, e.Y);
            }

            if (_positionActive)
            {
                X += e.X - _positionLastPos.X;
                Y += e.Y - _positionLastPos.Y;
                _positionLastPos = new Point(e.X, e.Y);
            }
        }

        private void CreateBuildOrderSteps(Direct2DRenderer renderer, BuildOrder buildOrder)
        {
            float gap = 5f;
            float width = Width - gap * 2f;
            float textHeight = 15f;
            float textGap = 5f;

            float lastY = Y + Height - gap;

            BuildOrderStep bos = buildOrder.FirstBuildOrderStep;
            do
            {
                float requirementCounts = (float)bos.RequirementsCount();
                float instructionsCount = bos.Instructions.Count;
                float bosLines = Math.Max(requirementCounts, instructionsCount);

                GfxBuildOrderStep gfxBos = new GfxBuildOrderStep(renderer, bos, this);
                gfxBos.Size = new SizeF(width, textHeight * bosLines + textGap * (bosLines + 1));
                gfxBos.Position = new PointF(gap, lastY - gfxBos.Size.Height);
                lastY = gfxBos.Position.Y - gap;
                GfxObjects.Add(gfxBos);

                bos = bos.NextBuildOrderStep;
            } while (bos != null);
        }

        private float GetActiveStepY()
        {
            float gap = 5f;

            foreach (GfxObject gfxObj in GfxObjects)
            {
                GfxBuildOrderStep gfxBos = (gfxObj as GfxBuildOrderStep);
                if (gfxBos.BuildOrderStep.IsActive)
                {
                    return (Y + Height - gap) - (gfxBos.Y + gfxBos.Height);
                }
            }

            return 0f;
        }

        public override void Draw()
        {
            if (!Hide && Global.BoState == BoState.Running && Global.OCRState != OCRState.WaitForMatch)
            {
                // draw container rectangle
                Renderer.FillRectangle((int)X, (int)Y, (int)Width, (int)Height, _colorBack);

                // scroll build order steps down
                _targetY = GetActiveStepY();
                OffsetY = _targetY;

                // show ocr problem text
                if (Global.OCRState == OCRState.Warning)
                {
                    Renderer.FillRectangle(15, (int)(Y + Height + 3), 200, 18, _brushBackColor);
                    Renderer.DrawText("OCR can't detect game values!", _font, _brushFrontYellow, 18, (int)(Y + Height) + 5);
                }
                else if (Global.OCRState == OCRState.Error)
                {
                    Renderer.FillRectangle(15, (int)(Y + Height + 3), 200, 18, _brushBackColor);
                    Renderer.DrawText("OCR can't detect game values!", _font, _brushFrontRed, 18, (int)(Y + Height) + 5);
                }
            }

            DrawChildren();
        }

        private void DrawChildren()
        {
            foreach (GfxObject gfxObj in GfxObjects)
            {
                if (gfxObj.GetType() == typeof(GfxButtonHide))
                {
                    gfxObj.Draw();
                }
                else if (gfxObj.GetType() == typeof(GfxButtonRestart))
                {
                    if (!Hide)
                        gfxObj.Draw();
                }
                else
                {
                    if (!Hide && Global.BoState == BoState.Running && Global.OCRState != OCRState.WaitForMatch)
                        gfxObj.Draw();
                }
            }
        }
    }

    public class GfxBuildOrderStep : GfxObject
    {
        public GfxObject Parent { get; set; }
        public BuildOrderStep BuildOrderStep;
        private int _font;
        private int _colorFont;
        private int _colorBack;
        private int _colorBackActive;

        public GfxBuildOrderStep(Direct2DRenderer renderer, BuildOrderStep buildOrderStep, GfxObject parent) : base(renderer)
        {
            BuildOrderStep = buildOrderStep;
            Parent = parent;

            _font = renderer.CreateFont("Arial", 15);
            _colorFont = renderer.CreateBrush(Color.FromArgb(0, 0, 0, 0));
            _colorBack = renderer.CreateBrush(Color.FromArgb(200, 255, 255, 255));
            _colorBackActive = renderer.CreateBrush(Color.FromArgb(200, 0, 1, 0));
        }

        public override void Draw()
        {
            int yGlobal = (int)(Parent as GfxBuildOrder).OffsetY + (int)Y;
            int width = (int)Parent.Width - 10;

            if (BuildOrderStep.IsDone)
                return;
            if (yGlobal < Parent.Y)
                return;

            int color = BuildOrderStep.IsActive ? _colorBackActive : _colorBack;

            Renderer.FillRectangle((int)X + (int)Parent.X, yGlobal, width, (int)Height, color);

            int offset = 0;
            foreach (string req in BuildOrderStep.GetRequirementStrings())
            {
                string[] s = req.Split(';');
                Renderer.DrawText(s[0], _font, _colorFont, (int)X + 5 + (int)Parent.X, yGlobal + 5 + offset);
                Renderer.DrawText(s[1], _font, _colorFont, (int)X + 45 + (int)Parent.X, yGlobal + 5 + offset);
                offset += 20;
            }

            offset = 0;
            foreach (string instruction in BuildOrderStep.Instructions)
            {
                Renderer.DrawText(instruction, _font, _colorFont, (int)X + 105 + (int)Parent.X, yGlobal + 5 + offset);
                offset += 20;
            }
        }
    }

    public class GfxButton : GfxObject
    {
        public GfxObject Parent { get; set; }
        public int Font;
        public int ColorFont;
        public int ColorBack;
        public float XGlobal
        {
            get
            {
                return Parent.X + X;
            }
            set
            {
                X = value - Parent.X;
            }
        }
        public float YGlobal
        {
            get
            {
                return Parent.Y + Y;
            }
            set
            {
                Y = value - Parent.Y;
            }
        }
        public Rectangle RectangleGlobal
        {
            get
            {
                return new Rectangle((int)XGlobal, (int)YGlobal + (int)Parent.Height, (int)Width, (int)Height);
            }
        }

        public GfxButton(Direct2DRenderer renderer, GfxObject parent) : base(renderer)
        {
            Parent = parent;

            Font = renderer.CreateFont("Arial", 12);
            ColorFont = renderer.CreateBrush(Color.FromArgb(0, 0, 0, 0));
            ColorBack = renderer.CreateBrush(Color.FromArgb(200, 225, 225, 225));

            Position = new PointF(0f, 0f);
            Size = new SizeF(20f, 20f);
        }

        public virtual bool PointContains(int x, int y)
        {
            return RectangleGlobal.Contains(x, y);
        }

        public virtual void ClickIfHit(int x, int y)
        {
            if (PointContains(x, y))
                Click();
        }

        public virtual void Click()
        {
            throw new NotImplementedException();
        }
    }

    public class GfxButtonHide : GfxButton
    {
        private SizeF _gap = new SizeF(0f, 2f);

        public GfxButtonHide(Direct2DRenderer renderer, GfxObject parent) : base(renderer, parent)
        {
            Position = new PointF(_gap.Width, _gap.Height);
            Size = new SizeF(32f, 14f);
        }

        public override void Click()
        {
            (Parent as GfxBuildOrder).Hide = !(Parent as GfxBuildOrder).Hide;
        }

        public override void Draw()
        {
            string text = (Parent as GfxBuildOrder).Hide ? "Show" : "Hide";
            Renderer.FillRectangle((int)XGlobal, (int)YGlobal + (int)Parent.Height, (int)Width, (int)Height, ColorBack);
            Renderer.DrawText(text, Font, ColorFont, (int)XGlobal + 1, (int)YGlobal + (int)Parent.Height + 1);
        }
    }

    public class GfxButtonRestart : GfxButton
    {
        private SizeF _gap = new SizeF(0f, 2f);

        public GfxButtonRestart(Direct2DRenderer renderer, GfxObject parent) : base(renderer, parent)
        {
            Position = new PointF(_gap.Width + 34f, _gap.Height);
            Size = new SizeF(43f, 14f);
        }

        public override void Click()
        {
            (Parent as GfxBuildOrder).BuildOrder.Restart();
        }

        public override void Draw()
        {
            int x = (int)(Parent.X + X);
            int y = (int)(Parent.Y + Parent.Height + Y);
            Renderer.FillRectangle(x, y, (int)Width, (int)Height, ColorBack);
            Renderer.DrawText("Restart", Font, ColorFont, x + 1, y + 1);
        }
    }

    public class GfxButtonPrevStep : GfxButton
    {
        private SizeF _gap = new SizeF(0f, 2f);

        public GfxButtonPrevStep(Direct2DRenderer renderer, GfxObject parent) : base(renderer, parent)
        {
            Position = new PointF(_gap.Width + 79f, _gap.Height);
            Size = new SizeF(30f, 14f);
        }

        public override void Click()
        {
            (Parent as GfxBuildOrder).BuildOrder.PrevStep();
        }

        public override void Draw()
        {
            int x = (int)(Parent.X + X);
            int y = (int)(Parent.Y + Parent.Height + Y);
            Renderer.FillRectangle(x, y, (int)Width, (int)Height, ColorBack);
            Renderer.DrawText("Prev", Font, ColorFont, x + 1, y + 1);
        }
    }

    public class GfxButtonNextStep : GfxButton
    {
        private SizeF _gap = new SizeF(0f, 2f);

        public GfxButtonNextStep(Direct2DRenderer renderer, GfxObject parent) : base(renderer, parent)
        {
            Position = new PointF(_gap.Width + 111f, _gap.Height);
            Size = new SizeF(30f, 14f);
        }

        public override void Click()
        {
            (Parent as GfxBuildOrder).BuildOrder.NextStep();
        }

        public override void Draw()
        {
            int x = (int)(Parent.X + X);
            int y = (int)(Parent.Y + Parent.Height + Y);

            Renderer.FillRectangle(x, y, (int)Width, (int)Height, ColorBack);
            Renderer.DrawText("Next", Font, ColorFont, x + 1, y + 1);
        }
    }

    public class GfxButtonResize : GfxButton
    {
        private SizeF _gap = new SizeF(0f, 5f);

        public GfxButtonResize(Direct2DRenderer renderer, GfxObject parent) : base(renderer, parent)
        {
            Position = new PointF(_gap.Width + 190f, _gap.Height);
            Size = new SizeF(15f, 15f);
        }

        public override void Click()
        {

        }

        public override bool PointContains(int x, int y)
        {
            int x2 = (int)(Parent.X + Parent.Width - Width);
            int y2 = (int)(Parent.Y + Parent.Height - Height);
            return new Rectangle(x2, y2, (int)Width, (int)Height).Contains(x, y);
        }

        public override void Draw()
        {
            int x = (int)(Parent.X + Parent.Width - Width);
            int y = (int)(Parent.Y + Parent.Height - Height);

            Point start = new Point(x, y + (int)Height);
            Point end = new Point(x + (int)Width, y);

            for (int i = 0; i <= Math.Min(Width, Height); i++)
            {
                Renderer.DrawLine(start.X, start.Y, end.X, end.Y, 1f, ColorBack);
                start.X += 1;
                end.Y += 1;
            }
        }
    }

    public class GfxButtonPosition : GfxButton
    {
        public GfxButtonPosition(Direct2DRenderer renderer, GfxObject parent) : base(renderer, parent)
        {
            Size = new SizeF(20f, 10f);
            ColorBack = renderer.CreateBrush(Color.FromArgb(200, 125, 125, 125));
        }

        public override bool PointContains(int x, int y)
        {
            int x2 = (int)(Parent.X);
            int y2 = (int)(Parent.Y);
            return new Rectangle(x2, y2, (int)Parent.Width, (int)Height).Contains(x, y);
        }

        public override void Click()
        {

        }

        public override void Draw()
        {
            int x = (int)Parent.X;
            int y = (int)Parent.Y;
            int height = 0;
            while (height <= Height)
            {
                Renderer.DrawLine(x, y + height, x + (int)Parent.Width, y + height, 2f, ColorBack);
                height += 4;
            }
        }
    }
}
