using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AoE4BO
{
    public static class Global
    {
        // General variables
        public static GameData GameData { get; set; }
        public static Settings Settings { get; set; }

        // Settings helper variables
        public static Bitmap SettingImageSupply { get; set; }
        public static Bitmap SettingImageFood { get; set; }
        public static Bitmap SettingImageWood { get; set; }
        public static Bitmap SettingImageGold { get; set; }
        public static Bitmap SettingImageStone { get; set; }
        public static string SettingOCRTextSupply { get; set; }
        public static string SettingOCRTextFood { get; set; }
        public static string SettingOCRTextWood { get; set; }
        public static string SettingOCRTextGold { get; set; }
        public static string SettingOCRTextStone { get; set; }
        public static bool SettingsFormOpen { get; set; }

        // BuildOrder variables
        public static BoState BoState { get; set; }

        // Overlay variables
        public static OverlayState OverlayState { get; set; }

        // OCR variables
        public static OCRState OCRState { get; set; }
        public static float OCRDowntime { get; set; }
        public static int OCRProcessTime { get; set; }
    }

    public enum BoState
    {
        Idle,
        Running,
        Finish
    }

    public enum OCRState
    {
        Success,
        Warning,
        Error
    }

    public enum OverlayState
    {
        Idle,
        Running,
        WaitForAEO,
        Error
    }
}
