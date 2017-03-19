using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndustryThing.ApiImport
{
    class Assets : Misc.UsefullMethods
    {
        string currentTime;
        string cachedUntil;
        public ContainerII assets;

        public Assets(string api)
        {
            string line;
            api = RemoveNextLine(api); line = GetNextLine(api); // just removes the first line as its useless and gets the next line.
            if (line.StartsWith("<eveapi version=\"2\">")) { api = RemoveNextLine(api); line = GetNextLine(api); }
            if (line.StartsWith("<currentTime>")) { currentTime = GetValue(line, "<currentTime>", "</currentTime>"); api = RemoveNextLine(api); line = GetNextLine(api); }
            if (line.StartsWith("<result>")) { api = RemoveNextLine(api); line = GetNextLine(api); }
            if (line.StartsWith("<rowset"))
            {
                assets = new ContainerII();
                api = assets.AddItems(api);
                line = GetNextLine(api);
            }
            if (line.StartsWith("</result>")) { api = RemoveNextLine(api); line = GetNextLine(api); }
            if (line.StartsWith("<cachedUntil>")) { cachedUntil = GetValue(line, "<cachedUntil>", "</cachedUntil>"); api = RemoveNextLine(api); line = GetNextLine(api); }
            if (line.StartsWith("</eveapi>")) { } // makes sure its done reading 
        }
    }

    class ContainerII : Misc.UsefullMethods
    {
        List<string> columnList = new List<string>();
        string[][] assetList = new string[1000000][]; // hardcoded to 1mil items max until i work out a better system.
        int assetCount = 0;
        ContainerII[] containers = new ContainerII[1000];
        string[] containerName = new string[1000];
        int containerCount = 0;

        public string AddItems(string api)
        {
            string line;
            line = GetNextLine(api);
            if (line.StartsWith("<rowset"))
            {
                columnList.Add("itemID"); columnList.Add("locationID"); columnList.Add("typeID");
                columnList.Add("quantity"); columnList.Add("flag"); columnList.Add("singleton"); columnList.Add("rawQuantity");
                while (true)
                {
                    api = RemoveNextLine(api); line = GetNextLine(api);
                    if (line.StartsWith("<row "))
                    {
                        assetList[assetCount] = new string[columnList.Count];
                        int i = 0;
                        while (columnList.Count > i)
                        {
                            if (line.IndexOf(columnList.ElementAt(i)) != -1) { assetList[assetCount][i] = GetValue(line, columnList.ElementAt(i) + "=\"", "\""); i++; }
                            else { assetList[assetCount][i] = "0"; i++; }
                        }
                        if (!line.EndsWith("/>\r")) // stuff to handle containers 
                        {
                            containerName[containerCount] = assetList[assetCount][columnList.IndexOf("itemID")];
                            containers[containerCount] = new ContainerII();
                            api = RemoveNextLine(api); line = GetNextLine(api);
                            api = containers[containerCount].AddItems(api);
                            line = GetNextLine(api);
                            containerCount++;
                        }
                        assetCount++;
                    }
                    if (line.StartsWith("</rowset")) { api = RemoveNextLine(api); line = GetNextLine(api); break; }
                }
            }
            return api;
        }

        /// <summary>
        /// Method that finds the row names of the asset list. "0" is the container itself, itemid for the contents of a container and "itemid,itemid" for the contents of a sub container.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public List<string> GetReference(string input)
        {
            if (input == "0") { return columnList; }
            else
            {
                if (input.Contains(","))
                {
                    string id = input.Substring(0, input.LastIndexOf(","));
                    return containers[findContainer(id)].GetReference(input.Substring(input.IndexOf(",") + 1));
                }
                else
                {
                    return containers[findContainer(input)].GetReference("0");
                }
            }
        }

        /// <summary>
        /// Method that finds the contents of a container in the asset list. "0" is the container itself, itemid for the contents of a container and "itemid,itemid" for the contents of a sub container.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string[][] GetDetails(string input)
        {
            if (input == "0") { return assetList; }
            else
            {
                if (input.Contains(","))
                {
                    string id = input.Substring(0, input.LastIndexOf(","));
                    return containers[findContainer(id)].GetDetails(input.Substring(input.IndexOf(",") + 1));
                }
                else
                {
                    return containers[findContainer(input)].GetDetails("0");
                }
            }
        }

        /// <summary>
        /// Method for locating the index of a container
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public int findContainer(string input)
        {
            int i = 0;
            while (i < containerCount)
            {
                if (containerName[i] == input)
                {
                    break;
                }
                i++;
            }
            return i;
        }

        /// <summary>
        /// Takes the container ID as a string(because im bad)  and returns the container
        /// </summary>
        /// <param name="id">Container ID</param>
        /// <returns>The countainer</returns>
        public ContainerII GetContainer(string id)
        {
            return containers[findContainer(id)];
        }

        public long FindItem(int typeID)
        {
            long[] items = new long[100];
            int itemsCount = 0;
            int i = 0;
            while (i < assetCount)
            {
                if (Convert.ToString(typeID) == assetList[i][columnList.IndexOf("typeID")])
                {
                 items[itemsCount]=  Convert.ToInt32( assetList[i][columnList.IndexOf("quantity")]);
                 itemsCount++;
                }
                    i++;
            }
           long totalItems = items.Sum();
            i = 0;
            while (i < containerCount)
            {
                totalItems += containers[i].FindItem(typeID);
                i++;
            }
            return totalItems;
        }
    }
}
