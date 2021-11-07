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
        private Timer _timerScreenshot;
        private DateTime _lastOcrTime = DateTime.MinValue;

        public OCR()
        {
            _ocr = new TesseractEngine("./tessdata", "eng", EngineMode.Default);
            _ocr.SetDebugVariable("debug_file", "tesseract.log");
            _ocr.SetVariable("tessedit_char_whitelist", "0123456789/");
        }

        public void Start()
        {
            Global.OCRState = OCRState.WaitForMatch;
            _timerScreenshot = new Timer(new TimerCallback(Tick), null, 500, 500);
        }

        public void Stop()
        {
            if (_timerScreenshot != null)
            {
                _timerScreenshot.Dispose();
                _timerScreenshot = null;
            }
        }

        private void Tick(object state)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Bitmap screenshot = TakeScreenshot();

            string s = Recognize(screenshot, "Supply",
                new Rectangle(Global.Settings.SupplyX, Global.Settings.SupplyY, Global.Settings.SupplyWidth, Global.Settings.SupplyHeight),
                Global.Settings.SupplyContrast, Global.Settings.SupplyScale, Global.Settings.SupplyGrayscale, true);
            string[] strSupply = s.Split('/');

            if (strSupply.Length != 2)
            {
                ErrorHandling();
                return;
            }

            string strFood = Recognize(screenshot, "Food",
                new Rectangle(Global.Settings.FoodX, Global.Settings.FoodY, Global.Settings.FoodWidth, Global.Settings.FoodHeight),
                Global.Settings.FoodContrast, Global.Settings.FoodScale, Global.Settings.FoodGrayscale, true);
            string strWood = Recognize(screenshot, "Wood",
                new Rectangle(Global.Settings.WoodX, Global.Settings.WoodY, Global.Settings.WoodWidth, Global.Settings.WoodHeight),
                Global.Settings.WoodContrast, Global.Settings.WoodScale, Global.Settings.WoodGrayscale, true);
            string strGold = Recognize(screenshot, "Gold",
                new Rectangle(Global.Settings.GoldX, Global.Settings.GoldY, Global.Settings.GoldWidth, Global.Settings.GoldHeight),
                Global.Settings.GoldContrast, Global.Settings.GoldScale, Global.Settings.GoldGrayscale, true);
            string strStone = Recognize(screenshot,  "Stone",
                new Rectangle(Global.Settings.StoneX, Global.Settings.StoneY, Global.Settings.StoneWidth, Global.Settings.StoneHeight),
                Global.Settings.StoneContrast, Global.Settings.StoneScale, Global.Settings.StoneGrayscale, true);

            screenshot.Dispose();

            bool b1, b2, b3, b4, b5, b6;
            int i1, i2, i3, i4, i5, i6;
            b1 = int.TryParse(strSupply[0], out i1);
            b2 = int.TryParse(strSupply[1], out i2);
            b3 = int.TryParse(strFood, out i3);
            b4 = int.TryParse(strWood, out i4);
            b5 = int.TryParse(strGold, out i5);
            b6 = int.TryParse(strStone, out i6);

            sw.Stop();
            Global.OCRProcessTime = (int)sw.ElapsedMilliseconds;

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
                ErrorHandling();
            }
        }

        private void ErrorHandling()
        {
            Console.WriteLine("#################");
            Console.WriteLine(_lastOcrTime);
            Console.WriteLine(Global.OCRDowntime);
            Console.WriteLine((DateTime.Now - _lastOcrTime));
            if (_lastOcrTime != DateTime.MinValue)
                Global.OCRDowntime = (float)(DateTime.Now - _lastOcrTime).TotalSeconds;

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
            //bm = RemoveNoise(bm);
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
            Image img = _printScreen.CaptureScreen();
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

    }
}
