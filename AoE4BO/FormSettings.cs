using System;
using System.Windows.Forms;

namespace AoE4BO
{
    public partial class FormSettings : Form
    {
        public FormSettings()
        {
            InitializeComponent();
        }

        private void FormSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            Global.SettingsFormOpen = false;
        }

        private void FormSettings_Load(object sender, EventArgs e)
        {
            Global.SettingsFormOpen = true;

            // overlay
            nudPosX.Value = Global.Settings.BuildOrderContainerX;
            nudPosY.Value = Global.Settings.BuildOrderContainerY;
            nudWidth.Value = Global.Settings.BuildOrderContainerWidth;
            nudHeight.Value = Global.Settings.BuildOrderContainerHeight;
            cbEnableCound.Checked = Global.Settings.PlaySound;

            // ocr supply
            nudSupplyX.Value = Global.Settings.SupplyX;
            nudSupplyY.Value = Global.Settings.SupplyY;
            nudSupplyWidth.Value = Global.Settings.SupplyWidth;
            nudSupplyHeight.Value = Global.Settings.SupplyHeight;
            nudSupplyContrast.Value = (decimal)Global.Settings.SupplyContrast;
            nudSupplyScale.Value = (decimal)Global.Settings.SupplyScale;
            cbSupplyGrayscale.Checked = Global.Settings.SupplyGrayscale;

            // ocr Food
            nudFoodX.Value = Global.Settings.FoodX;
            nudFoodY.Value = Global.Settings.FoodY;
            nudFoodWidth.Value = Global.Settings.FoodWidth;
            nudFoodHeight.Value = Global.Settings.FoodHeight;
            nudFoodContrast.Value = (decimal)Global.Settings.FoodContrast;
            nudFoodScale.Value = (decimal)Global.Settings.FoodScale;
            cbFoodGrayscale.Checked = Global.Settings.FoodGrayscale;

            // ocr Wood
            nudWoodX.Value = Global.Settings.WoodX;
            nudWoodY.Value = Global.Settings.WoodY;
            nudWoodWidth.Value = Global.Settings.WoodWidth;
            nudWoodHeight.Value = Global.Settings.WoodHeight;
            nudWoodContrast.Value = (decimal)Global.Settings.WoodContrast;
            nudWoodScale.Value = (decimal)Global.Settings.WoodScale;
            cbWoodGrayscale.Checked = Global.Settings.WoodGrayscale;

            // ocr Gold
            nudGoldX.Value = Global.Settings.GoldX;
            nudGoldY.Value = Global.Settings.GoldY;
            nudGoldWidth.Value = Global.Settings.GoldWidth;
            nudGoldHeight.Value = Global.Settings.GoldHeight;
            nudGoldContrast.Value = (decimal)Global.Settings.GoldContrast;
            nudGoldScale.Value = (decimal)Global.Settings.GoldScale;
            cbGoldGrayscale.Checked = Global.Settings.GoldGrayscale;

            // ocr Stone
            nudStoneX.Value = Global.Settings.StoneX;
            nudStoneY.Value = Global.Settings.StoneY;
            nudStoneWidth.Value = Global.Settings.StoneWidth;
            nudStoneHeight.Value = Global.Settings.StoneHeight;
            nudStoneContrast.Value = (decimal)Global.Settings.StoneContrast;
            nudStoneScale.Value = (decimal)Global.Settings.StoneScale;
            cbStoneGrayscale.Checked = Global.Settings.StoneGrayscale;

            // set change events
            nudPosX.ValueChanged += SaveSettings;
            nudPosY.ValueChanged += SaveSettings;
            nudWidth.ValueChanged += SaveSettings;
            nudHeight.ValueChanged += SaveSettings;
            cbEnableCound.CheckedChanged += SaveSettings;
            nudSupplyX.ValueChanged += SaveSettings;
            nudSupplyY.ValueChanged += SaveSettings;
            nudSupplyWidth.ValueChanged += SaveSettings;
            nudSupplyHeight.ValueChanged += SaveSettings;
            nudSupplyContrast.ValueChanged += SaveSettings;
            nudSupplyScale.ValueChanged += SaveSettings;
            cbSupplyGrayscale.CheckedChanged += SaveSettings;
            nudFoodX.ValueChanged += SaveSettings;
            nudFoodY.ValueChanged += SaveSettings;
            nudFoodWidth.ValueChanged += SaveSettings;
            nudFoodHeight.ValueChanged += SaveSettings;
            nudFoodContrast.ValueChanged += SaveSettings;
            nudFoodScale.ValueChanged += SaveSettings;
            cbFoodGrayscale.CheckedChanged += SaveSettings;
            nudWoodX.ValueChanged += SaveSettings;
            nudWoodY.ValueChanged += SaveSettings;
            nudWoodWidth.ValueChanged += SaveSettings;
            nudWoodHeight.ValueChanged += SaveSettings;
            nudWoodContrast.ValueChanged += SaveSettings;
            nudWoodScale.ValueChanged += SaveSettings;
            cbWoodGrayscale.CheckedChanged += SaveSettings;
            nudGoldX.ValueChanged += SaveSettings;
            nudGoldY.ValueChanged += SaveSettings;
            nudGoldWidth.ValueChanged += SaveSettings;
            nudGoldHeight.ValueChanged += SaveSettings;
            nudGoldContrast.ValueChanged += SaveSettings;
            nudGoldScale.ValueChanged += SaveSettings;
            cbGoldGrayscale.CheckedChanged += SaveSettings;
            nudStoneX.ValueChanged += SaveSettings;
            nudStoneY.ValueChanged += SaveSettings;
            nudStoneWidth.ValueChanged += SaveSettings;
            nudStoneHeight.ValueChanged += SaveSettings;
            nudStoneContrast.ValueChanged += SaveSettings;
            nudStoneScale.ValueChanged += SaveSettings;
            cbStoneGrayscale.CheckedChanged += SaveSettings;
        }

        private void SaveSettings(object sender, EventArgs e)
        {
            Global.Settings.BuildOrderContainerX = (int)nudPosX.Value;
            Global.Settings.BuildOrderContainerY = (int)nudPosY.Value;
            Global.Settings.BuildOrderContainerWidth = (int)nudWidth.Value;
            Global.Settings.BuildOrderContainerHeight = (int)nudHeight.Value;
            Global.Settings.PlaySound = cbEnableCound.Checked;

            Global.Settings.SupplyX = (int)nudSupplyX.Value;
            Global.Settings.SupplyY = (int)nudSupplyY.Value;
            Global.Settings.SupplyWidth = (int)nudSupplyWidth.Value;
            Global.Settings.SupplyHeight = (int)nudSupplyHeight.Value;
            Global.Settings.SupplyContrast = (int)nudSupplyContrast.Value;
            Global.Settings.SupplyScale = (float)nudSupplyScale.Value;
            Global.Settings.SupplyGrayscale = cbSupplyGrayscale.Checked;

            Global.Settings.FoodX = (int)nudFoodX.Value;
            Global.Settings.FoodY = (int)nudFoodY.Value;
            Global.Settings.FoodWidth = (int)nudFoodWidth.Value;
            Global.Settings.FoodHeight = (int)nudFoodHeight.Value;
            Global.Settings.FoodContrast = (int)nudFoodContrast.Value;
            Global.Settings.FoodScale = (float)nudFoodScale.Value;
            Global.Settings.FoodGrayscale = cbFoodGrayscale.Checked;

            Global.Settings.WoodX = (int)nudWoodX.Value;
            Global.Settings.WoodY = (int)nudWoodY.Value;
            Global.Settings.WoodWidth = (int)nudWoodWidth.Value;
            Global.Settings.WoodHeight = (int)nudWoodHeight.Value;
            Global.Settings.WoodContrast = (int)nudWoodContrast.Value;
            Global.Settings.WoodScale = (float)nudWoodScale.Value;
            Global.Settings.WoodGrayscale = cbWoodGrayscale.Checked;

            Global.Settings.GoldX = (int)nudGoldX.Value;
            Global.Settings.GoldY = (int)nudGoldY.Value;
            Global.Settings.GoldWidth = (int)nudGoldWidth.Value;
            Global.Settings.GoldHeight = (int)nudGoldHeight.Value;
            Global.Settings.GoldContrast = (int)nudGoldContrast.Value;
            Global.Settings.GoldScale = (float)nudGoldScale.Value;
            Global.Settings.GoldGrayscale = cbGoldGrayscale.Checked;

            Global.Settings.StoneX = (int)nudStoneX.Value;
            Global.Settings.StoneY = (int)nudStoneY.Value;
            Global.Settings.StoneWidth = (int)nudStoneWidth.Value;
            Global.Settings.StoneHeight = (int)nudStoneHeight.Value;
            Global.Settings.StoneContrast = (int)nudStoneContrast.Value;
            Global.Settings.StoneScale = (float)nudStoneScale.Value;
            Global.Settings.StoneGrayscale = cbStoneGrayscale.Checked;

            Global.Settings.Save();
        }

        private void timerImages_Tick(object sender, EventArgs e)
        {
            pbSupply.Image = Global.SettingImageSupply;
            lbSupply.Text = Global.SettingOCRTextSupply;
            pbFood.Image = Global.SettingImageFood;
            lbFood.Text = Global.SettingOCRTextFood;
            pbWood.Image = Global.SettingImageWood;
            lbWood.Text = Global.SettingOCRTextWood;
            pbGold.Image = Global.SettingImageGold;
            lbGold.Text = Global.SettingOCRTextGold;
            pbStone.Image = Global.SettingImageStone;
            lbStone.Text = Global.SettingOCRTextStone;
        }
    }
}
