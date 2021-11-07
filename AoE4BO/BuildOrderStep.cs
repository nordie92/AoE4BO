using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoE4BO
{
    public class BuildOrderStep
    {
        // prev and next build order step variables
        public BuildOrderStep PrevBuildOrderStep { get; set; }
        public BuildOrderStep NextBuildOrderStep { get; set; }

        // requirements variables
        public int SupplyRequired { get; set; }
        public int SupplyCapRequired { get; set; }
        public int FoodRequired { get; set; }
        public int WoodRequired { get; set; }
        public int GoldRequired { get; set; }
        public int StoneRequired { get; set; }
        public int TimeRequired { get; set; }

        // instruction variable
        public List<string> Instructions { get; set; }

        // state variables
        public bool IsActive { get; set; }
        public bool IsDone { get; set; }
        public float TimeActive = 0f;

        public BuildOrderStep()
        {
            Instructions = new List<string>();

            SupplyRequired = -1;
            SupplyCapRequired = -1;
            FoodRequired = -1;
            WoodRequired = -1;
            GoldRequired = -1;
            StoneRequired = -1;
            TimeRequired = -1;
        }

        public bool RequirementsEqual(BuildOrderStep buildOrderStep)
        {
            bool b1 = SupplyRequired == buildOrderStep.SupplyRequired;
            bool b2 = SupplyCapRequired == buildOrderStep.SupplyCapRequired;
            bool b3 = FoodRequired == buildOrderStep.FoodRequired;
            bool b4 = WoodRequired == buildOrderStep.WoodRequired;
            bool b5 = GoldRequired == buildOrderStep.GoldRequired;
            bool b6 = StoneRequired == buildOrderStep.StoneRequired;
            bool b7 = TimeRequired == buildOrderStep.TimeRequired;

            return b1 && b2 && b3 && b4 && b5 && b6 && b7;
        }

        public bool RequirementsMet(int supply, int supplyCap, int food, int wood, int gold, int stone)
        {
            bool b1 = SupplyRequired == -1 || supply >= SupplyRequired;
            bool b2 = SupplyCapRequired == -1 || supplyCap >= SupplyCapRequired;
            bool b3 = FoodRequired == -1 || food >= FoodRequired;
            bool b4 = WoodRequired == -1 || wood >= WoodRequired;
            bool b5 = GoldRequired == -1 || gold >= GoldRequired;
            bool b6 = StoneRequired == -1 || stone >= StoneRequired;
            bool b7 = TimeRequired == -1 || TimeActive >= TimeRequired;

            return b1 && b2 && b3 && b4 && b5 && b6 && b7;
        }

        public List<string> GetRequirementStrings()
        {
            List<string> requirements = new List<string>();

            if (SupplyRequired != -1)
                requirements.Add(SupplyRequired + ";Supply");
            if (SupplyCapRequired != -1)
                requirements.Add(SupplyCapRequired + ";SupplyCap");
            if (FoodRequired != -1)
                requirements.Add(FoodRequired + ";Food");
            if (WoodRequired != -1)
                requirements.Add(WoodRequired + ";Wood");
            if (GoldRequired != -1)
                requirements.Add(GoldRequired + ";Gold");
            if (StoneRequired != -1)
                requirements.Add(StoneRequired + ";Stone");
            if (TimeRequired != -1)
                requirements.Add(TimeRequired + ";Sec");

            return requirements;
        }

        public string GetRequirementsStringPretty()
        {
            string ret = "";

            foreach (string s in GetRequirementStrings())
            {
                if (ret.Length > 0)
                    ret += Environment.NewLine;

                string[] s1 = s.Split(';');
                ret += Fill(s1[0], 4) + " " + Fill(s1[1], 6);
            }

            return ret;
        }

        public string GetInstructionPretty()
        {
            string ret = "";

            foreach (string s in Instructions)
            {
                if (ret.Length > 0)
                    ret += Environment.NewLine;

                ret += s;
            }

            return ret;
        }

        public int RequirementsCount()
        {
            int counter = 0;

            if (SupplyRequired != -1)
                counter++;
            if (SupplyCapRequired != -1)
                counter++;
            if (FoodRequired != -1)
                counter++;
            if (WoodRequired != -1)
                counter++;
            if (GoldRequired != -1)
                counter++;
            if (StoneRequired != -1)
                counter++;
            if (TimeRequired != -1)
                counter++;

            return counter;
        }

        private string Fill(string input, int length)
        {
            string ret = input;
            for (int i = 0; i < length - input.Length; i++)
                ret = " " + ret;
            return ret;
        }
    }
}
