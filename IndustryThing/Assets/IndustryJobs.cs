using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace IndustryThing.ApiImport
{
    class IndustryJobs : Misc.UsefullMethods
    {
        db.Db dataBase;
        DateTime currentTime;

        private int[] jobID = new int[500];
        private int[] installerID = new int[500];
        private string[] installerName = new string[500];
        private long[] facilityID = new long[500];
        private int[] solarSystemID = new int[500];
        private string[] solarSystemName = new string[500];
        private long[] stationID = new long[500];
        private int[] activityID = new int[500];
        private long[] blueprintID = new long[500];
        private int[] blueprintTypeID = new int[500];
        private string[] blueprintTypeName = new string[500];
        private long[] blueprintLocationID = new long[500];
        private long[] outputLocationID = new long[500];
        private int[] runs = new int[500];
        private decimal[] cost = new decimal[500];
        private int[] teamID = new int[500];
        private int[] licensedRuns = new int[500];
        private decimal[] probability = new decimal[500];
        private int[] productTypeID = new int[500];
        private string[] productTypeName = new string[500];
        private int[] status = new int[500];
        private string[] timeInSeconds = new string[500];
        private DateTime[] startDate = new DateTime[500];
        private DateTime[] endDate = new DateTime[500];
        private DateTime[] pauseDate = new DateTime[500];
        private DateTime[] completedDate = new DateTime[500];
        private int[] completedCharacterID = new int[500];
        private int[] successfulRuns = new int[500];


        public IndustryJobs(StreamReader sr, db.Db dataBase)
        {
            this.dataBase = dataBase;
            sr.ReadLine(); sr.ReadLine();
            string line = sr.ReadLine(); line = line.Trim();
            line = line.Substring(line.IndexOf(">") + 1);
            currentTime = Convert.ToDateTime(line.Substring(0, line.IndexOf("<")));
            sr.ReadLine(); sr.ReadLine();
            int i = 0;
            while (!sr.EndOfStream)
            {
                line = sr.ReadLine(); line = line.Trim();
                if (line.StartsWith("<row "))
                {
                    line = line.Substring(line.IndexOf("\"") + 1); // jobID
                    jobID[i] = Convert.ToInt32(line.Substring(0, line.IndexOf("\""))); line = line.Substring(line.IndexOf("\"") + 1);

                    line = line.Substring(line.IndexOf("\"") + 1); //installerID
                    installerID[i] = Convert.ToInt32(line.Substring(0, line.IndexOf("\""))); line = line.Substring(line.IndexOf("\"") + 1);

                    line = line.Substring(line.IndexOf("\"") + 1); //installerName
                    installerName[i] = line.Substring(0, line.IndexOf("\"")); line = line.Substring(line.IndexOf("\"") + 1);

                    line = line.Substring(line.IndexOf("\"") + 1); //facilityID
                    facilityID[i] = Convert.ToInt64(line.Substring(0, line.IndexOf("\""))); line = line.Substring(line.IndexOf("\"") + 1);

                    line = line.Substring(line.IndexOf("\"") + 1); //solarSystemID
                    solarSystemID[i] = Convert.ToInt32(line.Substring(0, line.IndexOf("\""))); line = line.Substring(line.IndexOf("\"") + 1);

                    line = line.Substring(line.IndexOf("\"") + 1); //solarSystemName
                    solarSystemName[i] = line.Substring(0, line.IndexOf("\"")); line = line.Substring(line.IndexOf("\"") + 1);

                    line = line.Substring(line.IndexOf("\"") + 1); //stationID
                    stationID[i] = Convert.ToInt64(line.Substring(0, line.IndexOf("\""))); line = line.Substring(line.IndexOf("\"") + 1);

                    line = line.Substring(line.IndexOf("\"") + 1); //activityID
                    activityID[i] = Convert.ToInt32(line.Substring(0, line.IndexOf("\""))); line = line.Substring(line.IndexOf("\"") + 1);

                    line = line.Substring(line.IndexOf("\"") + 1); //blueprintID
                    blueprintID[i] = Convert.ToInt64(line.Substring(0, line.IndexOf("\""))); line = line.Substring(line.IndexOf("\"") + 1);

                    line = line.Substring(line.IndexOf("\"") + 1); //blueprintTypeID
                    blueprintTypeID[i] = Convert.ToInt32(line.Substring(0, line.IndexOf("\""))); line = line.Substring(line.IndexOf("\"") + 1);

                    line = line.Substring(line.IndexOf("\"") + 1); //blueprintTypeName
                    blueprintTypeName[i] = line.Substring(0, line.IndexOf("\"")); line = line.Substring(line.IndexOf("\"") + 1);

                    line = line.Substring(line.IndexOf("\"") + 1); //blueprintLocationID
                    blueprintLocationID[i] = Convert.ToInt64(line.Substring(0, line.IndexOf("\""))); line = line.Substring(line.IndexOf("\"") + 1);

                    line = line.Substring(line.IndexOf("\"") + 1); //outputLocationID
                    outputLocationID[i] = Convert.ToInt64(line.Substring(0, line.IndexOf("\""))); line = line.Substring(line.IndexOf("\"") + 1);

                    line = line.Substring(line.IndexOf("\"") + 1); //runs
                    runs[i] = Convert.ToInt32(line.Substring(0, line.IndexOf("\""))); line = line.Substring(line.IndexOf("\"") + 1);

                    line = line.Substring(line.IndexOf("\"") + 1); //cost
                    cost[i] = decimal.Parse(line.Substring(0, line.IndexOf("\"")),StaticInfo.ci); line = line.Substring(line.IndexOf("\"") + 1);

                    line = line.Substring(line.IndexOf("\"") + 1); //teamID
                    teamID[i] = Convert.ToInt32(line.Substring(0, line.IndexOf("\""))); line = line.Substring(line.IndexOf("\"") + 1);

                    line = line.Substring(line.IndexOf("\"") + 1); //licensedRuns
                    licensedRuns[i] = Convert.ToInt32(line.Substring(0, line.IndexOf("\""))); line = line.Substring(line.IndexOf("\"") + 1);

                    line = line.Substring(line.IndexOf("\"") + 1); //probability
                    probability[i] = decimal.Parse(line.Substring(0, line.IndexOf("\"")),StaticInfo.ci); line = line.Substring(line.IndexOf("\"") + 1);

                    line = line.Substring(line.IndexOf("\"") + 1); //productTypeID
                    productTypeID[i] = Convert.ToInt32(line.Substring(0, line.IndexOf("\""))); line = line.Substring(line.IndexOf("\"") + 1);

                    line = line.Substring(line.IndexOf("\"") + 1); //productTypeName
                    productTypeName[i] = line.Substring(0, line.IndexOf("\"")); line = line.Substring(line.IndexOf("\"") + 1);

                    line = line.Substring(line.IndexOf("\"") + 1); //status
                    status[i] = Convert.ToInt32(line.Substring(0, line.IndexOf("\""))); line = line.Substring(line.IndexOf("\"") + 1);

                    line = line.Substring(line.IndexOf("\"") + 1); //timeInSeconds
                    timeInSeconds[i] = line.Substring(0, line.IndexOf("\"")); line = line.Substring(line.IndexOf("\"") + 1);

                    line = line.Substring(line.IndexOf("\"") + 1); //startDate
                    startDate[i] = Convert.ToDateTime(line.Substring(0, line.IndexOf("\""))); line = line.Substring(line.IndexOf("\"") + 1);

                    line = line.Substring(line.IndexOf("\"") + 1); //endDate
                    endDate[i] = Convert.ToDateTime(line.Substring(0, line.IndexOf("\""))); line = line.Substring(line.IndexOf("\"") + 1);

                    line = line.Substring(line.IndexOf("\"") + 1); //pauseDate
                    pauseDate[i] = Convert.ToDateTime(line.Substring(0, line.IndexOf("\""))); line = line.Substring(line.IndexOf("\"") + 1);

                    line = line.Substring(line.IndexOf("\"") + 1); //completedDate
                    completedDate[i] = Convert.ToDateTime(line.Substring(0, line.IndexOf("\""))); line = line.Substring(line.IndexOf("\"") + 1);

                    line = line.Substring(line.IndexOf("\"") + 1); //completedCharacterID
                    completedCharacterID[i] = Convert.ToInt32(line.Substring(0, line.IndexOf("\""))); line = line.Substring(line.IndexOf("\"") + 1);

                    line = line.Substring(line.IndexOf("\"") + 1); //successfulRuns
                    successfulRuns[i] = Convert.ToInt32(line.Substring(0, line.IndexOf("\""))); line = line.Substring(line.IndexOf("\"") + 1);

                    i++;
                }
                else break;
            }

        }

        /// <summary>
        /// takes a typeID and returns the amount of items being produced currently
        /// </summary>
        /// <param name="typeID"></param>
        /// <returns></returns>
        public int GetJobs(int typeID)
        {
            int totalRuns = 0;
            int i = 0;
            while (i<jobID.Length)
            {
                if (productTypeID[i] == typeID)
                {
                    totalRuns += runs[i];
                }
                i++;
            }
            int[,] bpoOutput = dataBase.bpo.ManufacturingOutput(dataBase.bpo.FindBpoTypeIdForItem(typeID));
            return totalRuns * bpoOutput[0, 1];
        }
    }
}
