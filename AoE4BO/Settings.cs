using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoE4BO
{
    public class Settings
    {
        // config varibales
        private string _filename = "config.json";

        // general settings
        public bool PlaySound = true;
        public float OCRMaxDowntime = 5f;
        public int OCRInterval = 1000;

        // display settings
        public int BuildOrderContainerX = 15;
        public int BuildOrderContainerY = 450;
        public int BuildOrderContainerWidth = 350;
        public int BuildOrderContainerHeight = 300;

        // ocr settings
        public int SupplyX = 36;
        public int SupplyY = 855;
        public int SupplyWidth = 77;
        public int SupplyHeight = 25;
        public float SupplyScale = 2f;
        public float SupplyContrast = 5f;
        public bool SupplyGrayscale = true;
        public int FoodX = 36;
        public int FoodY = 913;
        public int FoodWidth = 77;
        public int FoodHeight = 25;
        public float FoodScale = 2f;
        public float FoodContrast = 5f;
        public bool FoodGrayscale = true;
        public int WoodX = 36;
        public int WoodY = 952;
        public int WoodWidth = 77;
        public int WoodHeight = 25;
        public float WoodScale = 2f;
        public float WoodContrast = 5f;
        public bool WoodGrayscale = true;
        public int GoldX = 36;
        public int GoldY = 992;
        public int GoldWidth = 77;
        public int GoldHeight = 25;
        public float GoldScale = 2f;
        public float GoldContrast = 5f;
        public bool GoldGrayscale = true;
        public int StoneX = 36;
        public int StoneY = 1031;
        public int StoneWidth = 77;
        public int StoneHeight = 25;
        public float StoneScale = 2f;
        public float StoneContrast = 5f;
        public bool StoneGrayscale = true;

        public void Save()
        {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(_filename, json, Encoding.UTF8);
        }

        public void Load()
        {
            if (!File.Exists(_filename))
                Save();

            string json = File.ReadAllText(_filename);
            Settings settings = JsonConvert.DeserializeObject<Settings>(json);

            // general settings
            PlaySound = settings.PlaySound;
            OCRInterval = settings.OCRInterval;
            OCRMaxDowntime = settings.OCRMaxDowntime;

            // display settings
            BuildOrderContainerX = settings.BuildOrderContainerX;
            BuildOrderContainerY = settings.BuildOrderContainerY;
            BuildOrderContainerWidth = settings.BuildOrderContainerWidth;
            BuildOrderContainerHeight = settings.BuildOrderContainerHeight;

            // ocr settings
            SupplyX = settings.SupplyX;
            SupplyY = settings.SupplyY;
            SupplyWidth = settings.SupplyWidth;
            SupplyHeight = settings.SupplyHeight;
            SupplyScale = settings.SupplyScale;
            SupplyContrast = settings.SupplyContrast;
            SupplyGrayscale = settings.SupplyGrayscale;
            FoodX = settings.FoodX;
            FoodY = settings.FoodY;
            FoodWidth = settings.FoodWidth;
            FoodHeight = settings.FoodHeight;
            FoodScale = settings.FoodScale;
            FoodContrast = settings.FoodContrast;
            FoodGrayscale = settings.FoodGrayscale;
            WoodX = settings.WoodX;
            WoodY = settings.WoodY;
            WoodWidth = settings.WoodWidth;
            WoodHeight = settings.WoodHeight;
            WoodScale = settings.WoodScale;
            WoodContrast = settings.WoodContrast;
            WoodGrayscale = settings.WoodGrayscale;
            GoldX = settings.GoldX;
            GoldY = settings.GoldY;
            GoldWidth = settings.GoldWidth;
            GoldHeight = settings.GoldHeight;
            GoldScale = settings.GoldScale;
            GoldContrast = settings.GoldContrast;
            GoldGrayscale = settings.GoldGrayscale;
            StoneX = settings.StoneX;
            StoneY = settings.StoneY;
            StoneWidth = settings.StoneWidth;
            StoneHeight = settings.StoneHeight;
            StoneScale = settings.StoneScale;
            StoneContrast = settings.StoneContrast;
            StoneGrayscale = settings.StoneGrayscale;
        }
    }
}
