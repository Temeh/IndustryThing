using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndustryThing.ApiImport
{
    class MainImport
    {
        string apiDomain;
        db.Db dataBase;
        public POS pos;
        public POSDetail posDetail;
        public Assets buildCorpAssets;
        public Assets empireDonkey;
        public MarketOrders marketOrders;
        public IndustryJobs jobs;

        public MainImport(db.Db dataBase)
        {
            this.dataBase = dataBase;

            apiDomain = "https://api.eveonline.com//";

            new ESI.Login(ESI.typeenum.BuildCorp);
            new ESI.Login(ESI.typeenum.EmpireDonkey);

            Console.ReadKey();

            //  StarbaseListImport();
            AssetImport();
            IndustryJobsImport();
            MarketOrdersImport();
        }

        /* void StarbaseListImport()///Gets The StarbaseList xml details
       {
           //makes the url, and gets the xml file
           string url = apiDomain + "corp/StarbaseList.xml.aspx?" + "KeyID=" + keyID + "&vCode=" + vCode;
           WebRequest wrGetXml;
           wrGetXml = WebRequest.Create(url);
           Stream objStream;
           objStream = wrGetXml.GetResponse().GetResponseStream();
           StreamReader objReader = new StreamReader(objStream);

           //reads the xml file
           pos = new POS();
           string nextline = "not null";
           while (true)
           {
               nextline = RemoveSpaceFromStartOfLine(objReader.ReadLine());
               if (nextline.StartsWith("<eveapi")) { pos.apiVersion = nextline.Substring(17, 1); }
               if (nextline.StartsWith("<currentTime>")) { pos.eveTimeOfApi = nextline.Substring(13, 19); }
               if (nextline.StartsWith("<result>"))
               {
                   nextline = RemoveSpaceFromStartOfLine(objReader.ReadLine());
                   if (nextline.StartsWith("<rowset"))
                   {
                       pos.SetColums(nextline);
                       while (true)
                       {
                           nextline = RemoveSpaceFromStartOfLine(objReader.ReadLine());
                           if (nextline.StartsWith("<row ")) pos.AddPOS(nextline);
                           else break;
                       }
                   }
               }
               if (nextline.StartsWith("<cachedUntil>")) pos.cachedUntil = nextline.Substring(13, 19);
               if (nextline.StartsWith("</eveapi>")) break;
           }
           StarbaseDetailImport();
       }
       */

        /*  void StarbaseDetailImport() //Gets the StarbaseDetail xml details
          {
              int i = 0;
              while (i < pos.posCount)
              {
                  //makes the url, and gets the xml file
                  string url = apiDomain + "corp/StarbaseDetail.xml.aspx?" + "KeyID=" + keyID + "&vCode=" + vCode + "&itemID=" + pos.posDetails[i].itemID;
                  posDetail = new POSDetail(pos.posCount);
                  posDetail.GetPOSDetail(url, i);
                  i++;
              }

          }
       */

        void AssetImport()
        {
            string[] apiCode = dataBase.settings.BuildCorpApi;
            string keyID = apiCode[0];
            string vCode = apiCode[1];
            string api;

            string url = apiDomain + "corp/AssetList.xml.aspx?" + "KeyID=" + keyID + "&vCode=" + vCode;
         /*   wrGetXml = WebRequest.Create(apiDomain + "corp/AssetList.xml.aspx?" + "KeyID=" + keyID + "&vCode=" + vCode);
            Stream objStream;
            objStream = wrGetXml.GetResponse().GetResponseStream();
            StreamReader objReader = new StreamReader(objStream); */
            StreamReader objReader = IndustryThing.StaticInfo.GetStream(url);
            api = objReader.ReadToEnd();
            buildCorpAssets = new Assets(api);

            apiCode = dataBase.settings.EmpireDonkey;
            keyID = apiCode[0];
            vCode = apiCode[1];
            int charID = Convert.ToInt32(apiCode[2]);
            /*
            wrGetXml = WebRequest.Create(apiDomain + "char/AssetList.xml.aspx?" + "KeyID=" + keyID + "&vCode=" + vCode + "&characterID=" + charID);
            objStream = wrGetXml.GetResponse().GetResponseStream();
            objReader = new StreamReader(objStream); */
            url = apiDomain + "char/AssetList.xml.aspx?" + "KeyID=" + keyID + "&vCode=" + vCode + "&characterID=" + charID;
            objReader = IndustryThing.StaticInfo.GetStream(url);
            api = objReader.ReadToEnd();
            empireDonkey = new Assets(api);
        }

        void IndustryJobsImport()
        {
            string[] apiCode = dataBase.settings.BuildCorpApi;
            string keyID = apiCode[0];
            string vCode = apiCode[1];
            WebRequest wrGetXml;
            string temp = apiDomain + "corp/IndustryJobs.xml.aspx?" + "KeyID=" + keyID + "&vCode=" + vCode;
            wrGetXml = WebRequest.Create(apiDomain + "corp/IndustryJobs.xml.aspx?" + "KeyID=" + keyID + "&vCode=" + vCode);
            Stream objStream;
            objStream = wrGetXml.GetResponse().GetResponseStream();
            StreamReader objReader = new StreamReader(objStream);
            jobs = new IndustryJobs(objReader, dataBase);
        }

        void MarketOrdersImport()
        {
            string[] apiCode = dataBase.settings.EmpireCorpApi;
            string keyID = apiCode[0];
            string vCode = apiCode[1];
            WebRequest wrGetXml;
            string temp = apiDomain + "corp/MarketOrders.xml.aspx?" + "KeyID=" + keyID + "&vCode=" + vCode;
            wrGetXml = WebRequest.Create(apiDomain + "corp/MarketOrders.xml.aspx?" + "KeyID=" + keyID + "&vCode=" + vCode);
            Stream objStream;
            objStream = wrGetXml.GetResponse().GetResponseStream();
            StreamReader objReader = new StreamReader(objStream);
            marketOrders = new MarketOrders(objReader);
        }

        string RemoveSpaceFromStartOfLine(string line) // Clears the start of a line of empty spaces to make it easier to read
        {
            bool hasSpaces = true;
            while (hasSpaces == true) { if (line.StartsWith(" ")) { line = line.Remove(0, 1); } else { hasSpaces = false; } }
            return line;
        }
    }

    class POS // class to hold pos info
    {

        public string apiVersion;
        public string eveTimeOfApi;
        public string cachedUntil;
        public string name;
        public string key;
        public POSDetails[] posDetails = new POSDetails[100];
        public int posCount = 0;

        public POS() { }

        List<string> columns = new List<string>();//Does not actually serve a purpose atm.
        public void SetColums(string line)
        {
            line = line.Substring(line.IndexOf("name=\"") + 6);
            name = line.Substring(0, line.IndexOf("\""));

            line = line.Substring(line.IndexOf("key=\"") + 5);
            key = line.Substring(0, line.IndexOf("\""));

            line = line.Substring(line.IndexOf("columns=\"") + 9);
            line = line.Substring(0, line.IndexOf("\""));
            while (line != "done")
            {
                int j = line.IndexOf(",");
                if (j != -1)
                {
                    columns.Add(line.Substring(0, j));
                    line = line.Remove(0, j + 1);

                }
                else { columns.Add(line); line = "done"; }
            }
        }

        public void AddPOS(string line)
        {
            posDetails[posCount] = new POSDetails();
            string findValue;
            findValue = line.Substring(line.IndexOf("itemID") + 8);
            posDetails[posCount].itemID = findValue.Substring(0, findValue.IndexOf("\""));

            findValue = line.Substring(line.IndexOf("typeID") + 8);
            posDetails[posCount].typeID = findValue.Substring(0, findValue.IndexOf("\""));

            findValue = line.Substring(line.IndexOf("locationID") + 12);
            posDetails[posCount].locationID = findValue.Substring(0, findValue.IndexOf("\""));

            findValue = line.Substring(line.IndexOf("moonID") + 8);
            posDetails[posCount].moonID = findValue.Substring(0, findValue.IndexOf("\""));

            findValue = line.Substring(line.IndexOf("state") + 7);
            posDetails[posCount].state = findValue.Substring(0, findValue.IndexOf("\""));

            findValue = line.Substring(line.IndexOf("stateTimestamp") + 16);
            posDetails[posCount].stateTimestamp = findValue.Substring(0, findValue.IndexOf("\""));

            findValue = line.Substring(line.IndexOf("onlineTimestamp") + 17);
            posDetails[posCount].onlineTimestamp = findValue.Substring(0, findValue.IndexOf("\""));

            findValue = line.Substring(line.IndexOf("standingOwnerID") + 17);
            posDetails[posCount].standingOwnerID = findValue.Substring(0, findValue.IndexOf("\""));

            posCount++;
        }
    }

    class POSDetails
    {
        public string itemID;
        public string typeID;
        public string locationID;
        public string moonID;
        public string state;
        public string stateTimestamp;
        public string onlineTimestamp;
        public string standingOwnerID;
    }

    class POSDetail : Misc.UsefullMethods
    {
        string[] version;
        string[] currentTime;
        string[] cachedUntil;
        string[] state;
        string[] stateTimestamp;
        string[] onlineTimestamp;
        string[] usageFlags;
        string[] deployFlags;
        string[] allowCorporationMembers;
        string[] allowAllianceMembers;
        string[] useStandingsFrom;
        string[] onStandingDrop;
        string[] onAggression;
        string[] onCorprationWar;
        string api;
        Container[] container;
        int containerCount;

        public POSDetail(int posCount)
        {
            version = new string[posCount];
            currentTime = new string[posCount];
            cachedUntil = new string[posCount];
            state = new string[posCount];
            stateTimestamp = new string[posCount];
            onlineTimestamp = new string[posCount];
            usageFlags = new string[posCount];
            deployFlags = new string[posCount];
            allowCorporationMembers = new string[posCount];
            allowAllianceMembers = new string[posCount];
            useStandingsFrom = new string[posCount];
            onStandingDrop = new string[posCount];
            onAggression = new string[posCount];
            onCorprationWar = new string[posCount];
            container = new Container[posCount];
            containerCount = 0;
        }

        public void GetPOSDetail(string url, int pos)
        {
            WebRequest wrGetXml;
            wrGetXml = WebRequest.Create(url);
            Stream objStream;
            objStream = wrGetXml.GetResponse().GetResponseStream();
            StreamReader objReader = new StreamReader(objStream);
            api = objReader.ReadToEnd();

            string line = GetNextLine(api); api = RemoveNextLine(api);
            while (line != "")
            {
                if (line.StartsWith("<eveapi"))
                {
                    version[pos] = GetValue(line, "version=\"", "\"");
                    while (true)
                    {
                        line = GetNextLine(api);
                        if (line.StartsWith("<currentTime>")) currentTime[pos] = GetValue(line, "<currentTime>", "</currentTime>");
                        if (line.StartsWith("<state>")) state[pos] = GetValue(line, "<state>", "</state>");
                        if (line.StartsWith("<stateTimestamp>")) stateTimestamp[pos] = GetValue(line, "<stateTimestamp>", "</stateTimestamp>");
                        if (line.StartsWith("<onlineTimestamp>")) onlineTimestamp[pos] = GetValue(line, "<onlineTimestamp>", "</onlineTimestamp>");
                        if (line.StartsWith("<usageFlags>")) usageFlags[pos] = GetValue(line, "<usageFlags>", "</usageFlags>");
                        if (line.StartsWith("<deployFlags>")) deployFlags[pos] = GetValue(line, "<deployFlags>", "</deployFlags>");
                        if (line.StartsWith("<allowCorporationMembers>")) allowCorporationMembers[pos] = GetValue(line, "<allowCorporationMembers>", "</allowCorporationMembers>");
                        if (line.StartsWith("<allowAllianceMembers>")) allowAllianceMembers[pos] = GetValue(line, "<allowAllianceMembers>", "</allowAllianceMembers>");
                        if (line.StartsWith("<useStandingsFrom ")) useStandingsFrom[pos] = GetValue(line, "ownerID=\"", "\"");
                        if (line.StartsWith("<onStandingDrop ")) onStandingDrop[pos] = GetValue(line, "standing=\"", "\"");
                        if (line.StartsWith("<onStatusDrop ")) { } //not saved atm
                        if (line.StartsWith("<onAggression ")) { } //not saved atm
                        if (line.StartsWith("<onCorporationWar ")) { } //not saved atm
                        if (line.StartsWith("<cachedUntil>")) cachedUntil[pos] = GetValue(line, "<cachedUntil>", "</cachedUntil>");
                        if (line.StartsWith("<rowset"))
                        {
                            container[containerCount] = new Container(); containerCount++;
                            container[pos].AddContainer(api);
                        }
                        if (line.StartsWith("</eveapi>")) { api = ""; break; }
                        api = RemoveNextLine(api);
                    }
                }
                line = GetNextLine(api); api = RemoveNextLine(api);
            }
        }
    }

    /// <summary>
    /// Simple class that holds information about containers. Holds a maximum of 1000 containers.
    /// </summary>
    class Container : Misc.UsefullMethods
    {
        ContainerContents[] contents;
        int containerCount;

        public Container()
        {
            contents = new ContainerContents[1000];
            containerCount = 0;
        }

        public void AddContainer(string api)
        {
            contents[containerCount] = new ContainerContents(api);
        }
    }
    /// <summary>
    /// Holds information about an item's contents.
    /// Might break if the container is empty.
    /// </summary>
    class ContainerContents : Misc.UsefullMethods
    {
        string name;
        string key;
        List<string> columnList = new List<string>();
        string[][] contents;

        public ContainerContents(string api)
        {
            string line = GetNextLine(api); api = RemoveNextLine(api);
            name = GetValue(line, "name=\"", "\"");
            key = GetValue(line, "key=\"", "\"");

            string columns = GetValue(line, "columns=\"", "\">");
            while (true)
            {
                if (columns.IndexOf(",") != -1) columnList.Add(columns.Substring(0, columns.IndexOf(",")));
                else { columnList.Add(columns); break; }
                columns = columns.Substring(columns.IndexOf(",") + 1);
            }

            string findNumberOfItems = api;
            int counter = 0;
            while (true)
            {
                line = GetNextLine(findNumberOfItems); findNumberOfItems = RemoveNextLine(findNumberOfItems);
                if (line.StartsWith("<row ")) counter++;
                else { contents = new string[counter][]; break; }
            }

            int i = 0;
            while (i < counter)
            {
                line = GetNextLine(api); api = RemoveNextLine(api);
                contents[i] = new string[columnList.Count];
                int j = 0;
                while (j < columnList.Count)
                {
                    contents[i][j] = GetValue(line, columnList[j] + "=\"", "\"");
                    j++;
                }
                i++;
            }

        }

    }
}
