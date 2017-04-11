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
        private decimal moduleRigMaterialBonus;
        private decimal componentRigSpeedBonus;
        private decimal componentRigMaterialBonus;
        private decimal shipRigSpeedBonus;
        private decimal shipRigMaterialBonus;
        public decimal RigSpeedModifier(string rig) 
        {
            if (rig == "module") return 1 - (moduleRigSpeedBonus * locationModifier);
            else if (rig == "component") return 1 - (componentRigSpeedBonus * locationModifier);
            else if (rig == "ship") return 1 - (shipRigSpeedBonus * locationModifier);
            else return 1;
        }
    
        public decimal RigMaterialModifier(string rig) 
        {
            if (rig == "module") return 1 - (moduleRigMaterialBonus * locationModifier);
            else if (rig == "component") return 1 - (componentRigMaterialBonus * locationModifier);
            else if (rig == "ship") return 1 - (shipRigMaterialBonus * locationModifier);
            else return 1;
        }

        private decimal materialEfficiency;
        public decimal MaterialEfficiencyModifier { get { return 1 - (materialEfficiency / 100); } }
        private decimal timeEfficiency;
        public decimal TimeEfficiencyModifier { get { return 1 - (timeEfficiency / 100); } }

        private string marketRegion;
        public string MarketRegion { get { return marketRegion; } }
        private int productionSystem;
        public int ProductionSystem { get { return productionSystem; } }
        private decimal facilityTax;
        public decimal FacilityTax { get { return facilityTax; } }

        private string[] buildCorpApi;
        /// <summary>
        /// First entry is key ID, second entry is vCode
        /// </summary>
        public string[] BuildCorpApi { get { return buildCorpApi; } }
        private string[] empireCorpApi;
        /// <summary>
        /// First entry is key ID, second entry is vCode
        /// </summary>
        public string[] EmpireCorpApi { get { return empireCorpApi; } }
        private string[] empireDonkey;
        /// <summary>
        /// First entry is key ID, second entry is vCode, 3rd entry is character ID
        /// </summary>
        public string[] EmpireDonkey { get { return empireDonkey; } }

        public Settings()
        {
            StreamReader sr = new StreamReader(StaticInfo.installDir+"settings.txt");
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
                else if (line.Substring(0, line.IndexOf(":")) == "marketRegion") marketRegion = line.Substring(line.IndexOf(":") + 1);
                else if (line.Substring(0, line.IndexOf(":")) == "productionSystem") productionSystem = Convert.ToInt32(line.Substring(line.IndexOf(":") + 1));
                else if (line.Substring(0, line.IndexOf(":")) == "facilityTax") facilityTax = Convert.ToDecimal(line.Substring(line.IndexOf(":") + 1));
                else if (line.Substring(0, line.IndexOf(":")) == "buildCorpApi")
                {
                    buildCorpApi = new string[2];
                    line = line.Substring(line.IndexOf(":") + 1);
                    buildCorpApi[0] = line.Substring(0, line.IndexOf(":"));
                    buildCorpApi[1] = line.Substring(line.IndexOf(":") + 1);
                }
                else if (line.Substring(0, line.IndexOf(":")) == "empireCorpApi")
                {
                    empireCorpApi = new string[2];
                    line = line.Substring(line.IndexOf(":") + 1);
                    empireCorpApi[0] = line.Substring(0, line.IndexOf(":"));
                    empireCorpApi[1] = line.Substring(line.IndexOf(":") + 1);
                }
                else if (line.Substring(0, line.IndexOf(":")) == "empireDonkey")
                {
                    empireDonkey = new string[3];
                    line = line.Substring(line.IndexOf(":") + 1);
                    empireDonkey[0] = line.Substring(0, line.IndexOf(":"));
                    line = line.Substring(line.IndexOf(":") + 1);
                    empireDonkey[1] = line.Substring(0, line.IndexOf(":"));
                    empireDonkey[2] = line.Substring(line.IndexOf(":") + 1);
                }
            }
        }
    }
}
