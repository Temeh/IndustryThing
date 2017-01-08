using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace IndustryThing.db
{
    class Settings
    {
        private int industryLevel;
        public int IndustryLevel { get { return industryLevel; } }
        private int advancedIndustryLevel;
        public int AdvancedIndustryLevel { get { return advancedIndustryLevel; } }
        private int scienceSkillOneLevel;
        public int ScienceSkillOneLevel { get { return scienceSkillOneLevel; } }
        private int scienceSkillTwoLevel;
        public int ScienceSkillTwoLevel { get { return scienceSkillTwoLevel; } }
        private int buildCycle;
        /// <summary>
        /// returns the planed buildtime in days
        /// </summary>
        public int BuildCycle { get { return 1 * 60 * 60 * 24 * buildCycle; } }
        private decimal facilitySpeedModifier;
        public decimal FacilitySpeedModifier { get { return facilitySpeedModifier; } }
        private decimal facilityMaterialModifier;
        public decimal FacilityMaterialModifier { get { return facilityMaterialModifier; } }

        private decimal locationModifier;

        private decimal moduleRigSpeedBonus;
        public decimal ModuleRigSpeedModifier { get { return 1 - (moduleRigSpeedBonus * locationModifier); } }
        private decimal moduleRigMaterialBonus;
        public decimal ModuleRigMaterialModifier { get { return 1 - (moduleRigMaterialBonus * locationModifier); } }

        private decimal componentRigSpeedBonus;
        public decimal ComponentRigSpeedModifier { get { return 1 - (componentRigSpeedBonus * locationModifier); } }
        private decimal componentRigMaterialBonus;
        public decimal ComponentRigMaterialModifier { get { return 1 - (componentRigMaterialBonus * locationModifier); } }

        private decimal shipRigSpeedBonus;
        public decimal ShipRigSpeedModifier { get { return 1 - (shipRigSpeedBonus * locationModifier); } }
        private decimal shipRigMaterialBonus;
        public decimal ShipRigMaterialModifier { get { return 1 - (shipRigMaterialBonus * locationModifier); } }

        private decimal materialEfficiency;
        public decimal MaterialEfficiencyModifier { get { return 1 - (materialEfficiency / 100); } }
        private decimal timeEfficiency;
        public decimal TimeEfficiencyModifier { get { return 1 - (timeEfficiency / 100); } }


        public Settings()
        {
            StreamReader sr = new StreamReader("settings.txt");
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                if (line.Substring(0, line.IndexOf(":")) == "Industry") industryLevel = Convert.ToInt32(line.Substring(line.IndexOf(":") + 1));
                else if (line.Substring(0, line.IndexOf(":")) == "AdvancedIndustry") advancedIndustryLevel = Convert.ToInt32(line.Substring(line.IndexOf(":") + 1));
                else if (line.Substring(0, line.IndexOf(":")) == "ScienceSkillOne") scienceSkillOneLevel = Convert.ToInt32(line.Substring(line.IndexOf(":") + 1));
                else if (line.Substring(0, line.IndexOf(":")) == "ScienceSkillTwo") scienceSkillTwoLevel = Convert.ToInt32(line.Substring(line.IndexOf(":") + 1));
                else if (line.Substring(0, line.IndexOf(":")) == "buildtime") buildCycle = Convert.ToInt32(line.Substring(line.IndexOf(":") + 1));
                else if (line.Substring(0, line.IndexOf(":")) == "facility")
                {
                    if (line.Substring(line.IndexOf(":") + 1) == "Sotiyo") { facilitySpeedModifier = Convert.ToDecimal(0.7); facilityMaterialModifier = Convert.ToDecimal(.99); }
                    else { facilitySpeedModifier = 1; facilityMaterialModifier = 1; }
                }
                else if (line.Substring(0, line.IndexOf(":")) == "location")
                {
                    if ((line.Substring(line.IndexOf(":") + 1) == "null") || (line.Substring(line.IndexOf(":") + 1) == "worm")) locationModifier = Convert.ToDecimal(2.1);
                    else if (line.Substring(line.IndexOf(":") + 1) == "low") locationModifier = Convert.ToDecimal(1.9);
                    else locationModifier = 1;
                }
                else if (line.Substring(0, line.IndexOf(":")) == "moduleRig")
                {
                    int tech = Convert.ToInt32(line.Substring(line.IndexOf(":") + 1));
                    if (tech == 1) { moduleRigSpeedBonus = Convert.ToDecimal(.2); moduleRigMaterialBonus = Convert.ToDecimal(.02); }
                    else if (tech == 2) { moduleRigSpeedBonus = Convert.ToDecimal(.24); moduleRigMaterialBonus = Convert.ToDecimal(.024); }
                    else { moduleRigSpeedBonus = Convert.ToDecimal(0); moduleRigMaterialBonus = Convert.ToDecimal(0); }
                }
                else if (line.Substring(0, line.IndexOf(":")) == "componentRig")
                {
                    int tech = Convert.ToInt32(line.Substring(line.IndexOf(":") + 1));
                    if (tech == 1) { componentRigSpeedBonus = Convert.ToDecimal(.2); componentRigMaterialBonus = Convert.ToDecimal(.02); }
                    else if (tech == 2) { componentRigSpeedBonus = Convert.ToDecimal(.24); componentRigMaterialBonus = Convert.ToDecimal(.024); }
                    else { componentRigSpeedBonus = Convert.ToDecimal(0); componentRigMaterialBonus = Convert.ToDecimal(0); }
                }
                else if (line.Substring(0, line.IndexOf(":")) == "shipRig")
                {
                    int tech = Convert.ToInt32(line.Substring(line.IndexOf(":") + 1));
                    if (tech == 1) { shipRigSpeedBonus = Convert.ToDecimal(.2); shipRigMaterialBonus = Convert.ToDecimal(.02); }
                    else if (tech == 2) { shipRigSpeedBonus = Convert.ToDecimal(.24); shipRigMaterialBonus = Convert.ToDecimal(.024); }
                    else { shipRigSpeedBonus = Convert.ToDecimal(0); shipRigMaterialBonus = Convert.ToDecimal(0); }
                }
                else if (line.Substring(0, line.IndexOf(":")) == "bpoME") materialEfficiency = Convert.ToInt32(line.Substring(line.IndexOf(":") + 1));
                else if (line.Substring(0, line.IndexOf(":")) == "bpoTE") timeEfficiency = Convert.ToInt32(line.Substring(line.IndexOf(":") + 1));
            }
        }
    }
}
