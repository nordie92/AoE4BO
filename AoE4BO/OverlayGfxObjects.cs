using Overlay.NET.Directx;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public List<GfxObject> GfxObjects { get; set; }
        public GfxObject Parent { get; set; }

        public GfxObject(Direct2DRenderer renderer, GfxObject parent)
        {
            _renderer = renderer;
            GfxObjects = new List<GfxObject>();
            Parent = parent;
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
        private int _colorBack;
        private BuildOrder _buildOrder;
        private float _pixelPerSecond = 3f;
        private float _targetY;
        private float _currentY;
        private int _font;
        private int _brushFrontRed;
        private int _brushFrontYellow;
        private int _brushBackColor;

        public GfxBuildOrder(Direct2DRenderer renderer, GfxObject parent, BuildOrder buildOrder) : base(renderer, parent)
        {
            _buildOrder = buildOrder;

            Position = new PointF(Global.Settings.BuildOrderContainerX, Global.Settings.BuildOrderContainerY);
            Size = new SizeF(Global.Settings.BuildOrderContainerWidth, Global.Settings.BuildOrderContainerHeight);

            _colorBack = renderer.CreateBrush(Color.FromArgb(200, 255, 255, 255));
            _font = renderer.CreateFont("Arial", 12);
            _brushFrontRed = renderer.CreateBrush(Color.FromArgb(200, 255, 0, 0));
            _brushFrontYellow = renderer.CreateBrush(Color.FromArgb(200, 255, 255, 0));
            _brushBackColor = renderer.CreateBrush(Color.FromArgb(200, 255, 255, 255));

            CreateBuildOrderSteps(renderer, buildOrder);
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
                gfxBos.Position = new PointF(gap + X, lastY - gfxBos.Size.Height);
                lastY = gfxBos.Position.Y - gap;
                GfxObjects.Add(gfxBos);

                bos = bos.NextBuildOrderStep;
            } while (bos != null);
        }

        private void MoveGfx(float value)
        {
            foreach (GfxObject gfxObj in GfxObjects)
            {
                (gfxObj as GfxBuildOrderStep).Y += value;
            }
        }

        private float GetYFromActiveBuildOrderStep()
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
            if (Global.BoState != BoState.Running || Global.OCRState == OCRState.WaitForMatch)
                return;

            // draw container rectangle
            Renderer.FillRectangle((int)X, (int)Y, (int)Width, (int)Height, _colorBack);

            // scroll build order steps down
            _targetY = GetYFromActiveBuildOrderStep();
            if (_targetY <= _currentY)
                _currentY = _targetY;
            else
                _currentY += _pixelPerSecond;
            MoveGfx(_currentY);

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
    }

    public class GfxBuildOrderStep : GfxObject
    {
        private int _font;
        private int _colorFont;
        private int _colorBack;
        private int _colorBackActive;
        public BuildOrderStep BuildOrderStep;

        public GfxBuildOrderStep(Direct2DRenderer renderer, BuildOrderStep buildOrderStep, GfxObject parent) : base(renderer, parent)
        {
            BuildOrderStep = buildOrderStep;

            _font = renderer.CreateFont("Arial", 15);
            _colorFont = renderer.CreateBrush(Color.FromArgb(0, 0, 0, 0));
            _colorBack = renderer.CreateBrush(Color.FromArgb(200, 255, 255, 255));
            _colorBackActive = renderer.CreateBrush(Color.FromArgb(200, 0, 1, 0));
        }

        public override void Draw()
        {
            if (BuildOrderStep.IsDone)
                return;
            if (Global.BoState == BoState.Finish)
                return;
            if (Y < Parent.Y)
                return;
            if (Global.OCRState == OCRState.WaitForMatch)
                return;

            int color = BuildOrderStep.IsActive ? _colorBackActive : _colorBack;

            Renderer.FillRectangle((int)X, (int)Y, (int)Width, (int)Height, color);

            int offset = 0;
            foreach (string req in BuildOrderStep.GetRequirementStrings())
            {
                string[] s = req.Split(';');
                Renderer.DrawText(s[0], _font, _colorFont, (int)X + 5, (int)Y + 5 + offset);
                Renderer.DrawText(s[1], _font, _colorFont, (int)X + 45, (int)Y + 5 + offset);
                offset += 20;
            }

            offset = 0;
            foreach (string instruction in BuildOrderStep.Instructions)
            {
                Renderer.DrawText(instruction, _font, _colorFont, (int)X + 105, (int)Y + 5 + offset);
                offset += 20;
            }
        }
    }
}
