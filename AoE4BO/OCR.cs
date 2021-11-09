using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tesseract;

namespace AoE4BO
{
    class OCR
    {
        private TesseractEngine _ocr;
        private PrintScreen _printScreen = new PrintScreen();
        private DateTime _lastOcrTime = DateTime.MinValue;
        private Thread _threadOCR;
        private bool _stopOCRThread;
        private Rectangle _screenArea;

        public OCR()
        {
            _ocr = new TesseractEngine("./tessdata", "eng", EngineMode.Default);
            _ocr.SetDebugVariable("debug_file", "tesseract.log");
            _ocr.SetVariable("tessedit_char_whitelist", "0123456789/");
        }

        public void Start()
        {
            _screenArea = GetOCRAreaRec();

            Global.OCRState = OCRState.WaitForMatch;

            _stopOCRThread = false;
            _threadOCR = new Thread(new ParameterizedThreadStart(DoOCRWork));
            _threadOCR.IsBackground = true;
            _threadOCR.Start();
        }

        public void Restart()
        {
            Global.OCRState = OCRState.WaitForMatch;
        }

        public void Stop()
        {
            _threadOCR.Abort();
            _stopOCRThread = true;
        }

        private void DoOCRWork(object obj)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            while (!_stopOCRThread)
            {
                try
                {
                    Bitmap screenshot = TakeScreenshot();

                    Rectangle area1 = GetRelativeRectangle(new Rectangle(Global.Settings.SupplyX, Global.Settings.SupplyY, Global.Settings.SupplyWidth, Global.Settings.SupplyHeight));
                    string s = Recognize(screenshot, "Supply", area1, Global.Settings.SupplyContrast, Global.Settings.SupplyScale, Global.Settings.SupplyGrayscale, true);
                    string[] strSupply = s.Split('/');

                    if (strSupply.Length != 2)
                    {
                        ErrorHandling(false);
                        continue;
                    }

                    Rectangle area2 = GetRelativeRectangle(new Rectangle(Global.Settings.FoodX, Global.Settings.FoodY, Global.Settings.FoodWidth, Global.Settings.FoodHeight));
                    string strFood = Recognize(screenshot, "Food", area2, Global.Settings.FoodContrast, Global.Settings.FoodScale, Global.Settings.FoodGrayscale, true).Trim();
                    Rectangle area3 = GetRelativeRectangle(new Rectangle(Global.Settings.WoodX, Global.Settings.WoodY, Global.Settings.WoodWidth, Global.Settings.WoodHeight));
                    string strWood = Recognize(screenshot, "Wood", area3, Global.Settings.WoodContrast, Global.Settings.WoodScale, Global.Settings.WoodGrayscale, true).Trim();
                    Rectangle area4 = GetRelativeRectangle(new Rectangle(Global.Settings.GoldX, Global.Settings.GoldY, Global.Settings.GoldWidth, Global.Settings.GoldHeight));
                    string strGold = Recognize(screenshot, "Gold", area4, Global.Settings.GoldContrast, Global.Settings.GoldScale, Global.Settings.GoldGrayscale, true).Trim();
                    Rectangle area5 = GetRelativeRectangle(new Rectangle(Global.Settings.StoneX, Global.Settings.StoneY, Global.Settings.StoneWidth, Global.Settings.StoneHeight));
                    string strStone = Recognize(screenshot, "Stone", area5, Global.Settings.StoneContrast, Global.Settings.StoneScale, Global.Settings.StoneGrayscale, true).Trim();

                    screenshot.Dispose();

                    if (strFood.Length <= 0 || strWood.Length <= 0 || strGold.Length <= 0 || strStone.Length <= 0 || strFood.Length <= 0 || strFood.Length <= 0)
                    {
                        ErrorHandling(false);
                        continue;
                    }

                    bool b1, b2, b3, b4, b5, b6;
                    int i1, i2, i3, i4, i5, i6;
                    b1 = int.TryParse(strSupply[0], out i1);
                    b2 = int.TryParse(strSupply[1], out i2);
                    b3 = int.TryParse(strFood, out i3);
                    b4 = int.TryParse(strWood, out i4);
                    b5 = int.TryParse(strGold, out i5);
                    b6 = int.TryParse(strStone, out i6);

                    if (b1 && b2 && b3 && b4 && b5 && b6)
                    {
                        Global.GameData.Supply = i1;
                        Global.GameData.SupplyCap = i2;
                        Global.GameData.Food = i3;
                        Global.GameData.Wood = i4;
                        Global.GameData.Gold = i5;
                        Global.GameData.Stone = i6;

                        _lastOcrTime = DateTime.Now;
                        Global.OCRDowntime = 0f;
                        Global.OCRState = OCRState.Success;
                    }
                    else
                    {
                        ErrorHandling(false);
                    }
                }
                catch (Exception)
                {
                    ErrorHandling(true);
                }
                finally
                {
                    Global.OCRProcessTime = (int)sw.ElapsedMilliseconds;
                    Thread.Sleep(Math.Max(0, Global.Settings.OCRInterval - (int)sw.ElapsedMilliseconds));
                    sw.Restart();
                }
            }
        }

        private void ErrorHandling(bool exception)
        {
            if (_lastOcrTime != DateTime.MinValue)
                Global.OCRDowntime = (float)(DateTime.Now - _lastOcrTime).TotalSeconds;

            if (exception)
            {
                Global.OCRState = OCRState.Error;
                return;
            }

            if (Global.OCRState != OCRState.WaitForMatch)
            {
                if (Global.OCRDowntime > Global.Settings.OCRMaxDowntime)
                    Global.OCRState = OCRState.Error;
                else if (Global.OCRDowntime > 0f)
                    Global.OCRState = OCRState.Warning;
            }
        }

        private string Recognize(Bitmap screenshot, string valueName, Rectangle area, float contrast, float scale, bool grayscale, bool trySingleMode = true)
        {
            Bitmap bm = CropImage(screenshot, area);
            if (scale != 1f)
                bm = ResizeImage(bm, (int)((float)bm.Width * scale), (int)((float)bm.Height * scale));
            if (grayscale)
                bm = RemoveNoise(bm);
            if (contrast > 0f)
                bm = AdjustContrast(bm, contrast);
            bm = InvertColors(bm);

            Page page = _ocr.Process(bm);
            string text = page.GetText();
            page.Dispose();

            if (text.Length == 0 && trySingleMode)
            {
                page = _ocr.Process(bm, PageSegMode.SingleChar);
                text = page.GetText();
                page.Dispose();
            }

            SetSettingValues(valueName, bm, text);

            bm.Dispose();

            return text;
        }

        private Bitmap InvertColors(Bitmap bm)
        {
            Bitmap pic = new Bitmap(bm);
            for (int y = 0; (y <= (pic.Height - 1)); y++)
            {
                for (int x = 0; (x <= (pic.Width - 1)); x++)
                {
                    Color inv = pic.GetPixel(x, y);
                    inv = Color.FromArgb(255, (255 - inv.R), (255 - inv.G), (255 - inv.B));
                    pic.SetPixel(x, y, inv);
                }
            }
            return pic;
        }

        private void SetSettingValues(string valueName, Bitmap bitmap, string ocrText)
        {
            switch (valueName)
            {
                case "Supply":
                    Global.SettingImageSupply = new Bitmap(bitmap);
                    Global.SettingOCRTextSupply = ocrText;
                    break;
                case "Food":
                    Global.SettingImageFood = new Bitmap(bitmap);
                    Global.SettingOCRTextFood = ocrText;
                    break;
                case "Wood":
                    Global.SettingImageWood = new Bitmap(bitmap);
                    Global.SettingOCRTextWood = ocrText;
                    break;
                case "Gold":
                    Global.SettingImageGold = new Bitmap(bitmap);
                    Global.SettingOCRTextGold = ocrText;
                    break;
                case "Stone":
                    Global.SettingImageStone = new Bitmap(bitmap);
                    Global.SettingOCRTextStone = ocrText;
                    break;
            }
        }

        private Bitmap TakeScreenshot()
        {
            Image img = _printScreen.CaptureScreen(_screenArea);
            return new Bitmap(img);
        }

        public static Bitmap AdjustContrast(Bitmap Image, float Value)
        {
            Value = (100.0f + Value) / 100.0f;
            Value *= Value;
            Bitmap NewBitmap = (Bitmap)Image.Clone();
            BitmapData data = NewBitmap.LockBits(
                new Rectangle(0, 0, NewBitmap.Width, NewBitmap.Height),
                ImageLockMode.ReadWrite,
                NewBitmap.PixelFormat);
            int Height = NewBitmap.Height;
            int Width = NewBitmap.Width;

            unsafe
            {
                for (int y = 0; y < Height; ++y)
                {
                    byte* row = (byte*)data.Scan0 + (y * data.Stride);
                    int columnOffset = 0;
                    for (int x = 0; x < Width; ++x)
                    {
                        byte B = row[columnOffset];
                        byte G = row[columnOffset + 1];
                        byte R = row[columnOffset + 2];

                        float Red = R / 255.0f;
                        float Green = G / 255.0f;
                        float Blue = B / 255.0f;
                        Red = (((Red - 0.5f) * Value) + 0.5f) * 255.0f;
                        Green = (((Green - 0.5f) * Value) + 0.5f) * 255.0f;
                        Blue = (((Blue - 0.5f) * Value) + 0.5f) * 255.0f;

                        int iR = (int)Red;
                        iR = iR > 255 ? 255 : iR;
                        iR = iR < 0 ? 0 : iR;
                        int iG = (int)Green;
                        iG = iG > 255 ? 255 : iG;
                        iG = iG < 0 ? 0 : iG;
                        int iB = (int)Blue;
                        iB = iB > 255 ? 255 : iB;
                        iB = iB < 0 ? 0 : iB;

                        row[columnOffset] = (byte)iB;
                        row[columnOffset + 1] = (byte)iG;
                        row[columnOffset + 2] = (byte)iR;

                        columnOffset += 4;
                    }
                }
            }

            NewBitmap.UnlockBits(data);

            return NewBitmap;
        }

        private static Bitmap CropImage(Bitmap source, Rectangle section)
        {
            var bitmap = new Bitmap(section.Width, section.Height);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);
                return bitmap;
            }
        }

        public static Bitmap MakeGrayscale(Bitmap original)
        {
            Bitmap temp = (Bitmap)original;
            Bitmap bmap = (Bitmap)temp.Clone();
            Color c;
            for (int i = 0; i < bmap.Width; i++)
            {
                for (int j = 0; j < bmap.Height; j++)
                {
                    c = bmap.GetPixel(i, j);
                    byte gray = (byte)(.299 * c.R + .587 * c.G + .114 * c.B);

                    bmap.SetPixel(i, j, Color.FromArgb(gray, gray, gray));
                }
            }
            return (Bitmap)bmap.Clone();
        }

        public Bitmap RemoveNoise(Bitmap bmap)
        {
            for (var x = 0; x < bmap.Width; x++)
            {
                for (var y = 0; y < bmap.Height; y++)
                {
                    var pixel = bmap.GetPixel(x, y);
                    if (pixel.R < 162 && pixel.G < 162 && pixel.B < 162)
                        bmap.SetPixel(x, y, Color.Black);
                    else if (pixel.R > 162 && pixel.G > 162 && pixel.B > 162)
                        bmap.SetPixel(x, y, Color.White);
                }
            }

            return bmap;
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        private Rectangle GetOCRAreaRec()
        {
            int xMin, xMax, yMin, yMax;

            xMin = Global.Settings.SupplyX;
            xMax = Global.Settings.SupplyX + Global.Settings.SupplyWidth;
            yMin = Global.Settings.SupplyY;
            yMax = Global.Settings.SupplyY + Global.Settings.SupplyHeight;

            if (Global.Settings.FoodX < xMin)
                xMin = Global.Settings.FoodX;
            if (Global.Settings.FoodX + Global.Settings.FoodWidth > xMax)
                xMax = Global.Settings.FoodX + Global.Settings.FoodWidth;
            if (Global.Settings.WoodX < xMin)
                xMin = Global.Settings.WoodX;
            if (Global.Settings.WoodX + Global.Settings.WoodWidth > xMax)
                xMax = Global.Settings.WoodX + Global.Settings.WoodWidth;
            if (Global.Settings.GoldX < xMin)
                xMin = Global.Settings.GoldX;
            if (Global.Settings.GoldX + Global.Settings.GoldWidth > xMax)
                xMax = Global.Settings.GoldX + Global.Settings.GoldWidth;
            if (Global.Settings.StoneX < xMin)
                xMin = Global.Settings.StoneX;
            if (Global.Settings.StoneX + Global.Settings.StoneWidth > xMax)
                xMax = Global.Settings.StoneX + Global.Settings.StoneWidth;

            if (Global.Settings.FoodY < yMin)
                yMin = Global.Settings.FoodY;
            if (Global.Settings.FoodY + Global.Settings.FoodHeight > yMax)
                yMax = Global.Settings.FoodY + Global.Settings.FoodHeight;
            if (Global.Settings.WoodY < yMin)
                yMin = Global.Settings.WoodY;
            if (Global.Settings.WoodY + Global.Settings.WoodHeight > yMax)
                yMax = Global.Settings.WoodY + Global.Settings.WoodHeight;
            if (Global.Settings.GoldY < yMin)
                yMin = Global.Settings.GoldY;
            if (Global.Settings.GoldY + Global.Settings.GoldHeight > yMax)
                yMax = Global.Settings.GoldY + Global.Settings.GoldHeight;
            if (Global.Settings.StoneY < yMin)
                yMin = Global.Settings.StoneY;
            if (Global.Settings.StoneY + Global.Settings.StoneHeight > yMax)
                yMax = Global.Settings.StoneY + Global.Settings.StoneHeight;

            Rectangle r = new Rectangle(xMin, yMin, xMax - xMin, yMax - yMin);
            return r;
        }

        private Rectangle GetRelativeRectangle(Rectangle substractor)
        {
            return new Rectangle(
                substractor.X - _screenArea.X,
                substractor.Y - _screenArea.Y,
                substractor.Width,
                substractor.Height
                );
        }
    }
}
