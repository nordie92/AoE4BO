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
        private string _filename = "config.json";
        public int BuildOrderContainerX = 15;
        public int BuildOrderContainerY = 450;
        public int BuildOrderContainerWidth = 600;
        public int BuildOrderContainerHeight = 300;
        public bool PlaySound = true;
        public float OCRMaxDowntime = 2f;
        public int SupplyX = 33;
        public int SupplyY = 855;
        public int SupplyWidth = 77;
        public int SupplyHeight = 25;
        public float SupplyScale = 4f;
        public float SupplyContrast = 30f;
        public bool SupplyGrayscale = true;
        public int FoodX = 33;
        public int FoodY = 913;
        public int FoodWidth = 77;
        public int FoodHeight = 25;
        public float FoodScale = 4f;
        public float FoodContrast = 30f;
        public bool FoodGrayscale = true;
        public int WoodX = 33;
        public int WoodY = 952;
        public int WoodWidth = 77;
        public int WoodHeight = 25;
        public float WoodScale = 4f;
        public float WoodContrast = 30f;
        public bool WoodGrayscale = true;
        public int GoldX = 33;
        public int GoldY = 992;
        public int GoldWidth = 77;
        public int GoldHeight = 25;
        public float GoldScale = 4f;
        public float GoldContrast = 30f;
        public bool GoldGrayscale = true;
        public int StoneX = 33;
        public int StoneY = 1031;
        public int StoneWidth = 77;
        public int StoneHeight = 25;
        public float StoneScale = 4f;
        public float StoneContrast = 30f;
        public bool StoneGrayscale = true;

        public void Save()
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;

            using (StreamWriter sw = new StreamWriter(_filename))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, this);
            }
        }

        public void Load()
        {
            if (!File.Exists(_filename))
                Save();

            string json = File.ReadAllText(_filename);
            Settings settings = JsonConvert.DeserializeObject<Settings>(json);

            BuildOrderContainerX = settings.BuildOrderContainerX;
            BuildOrderContainerY = settings.BuildOrderContainerY;
            BuildOrderContainerWidth = settings.BuildOrderContainerWidth;
            BuildOrderContainerHeight = settings.BuildOrderContainerHeight;
            PlaySound = settings.PlaySound;
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
