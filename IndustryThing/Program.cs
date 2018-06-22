using Flurl;
using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace IndustryThing
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            StaticInfo.ci.NumberFormat.NumberDecimalSeparator = ".";
            Console.WriteLine("Starting Program...");
            Stopwatch timer = Stopwatch.StartNew();
            calculator.Calculator calc = new calculator.Calculator();
            timer.Stop();
            Console.WriteLine("Program finished successfully in " + timer.Elapsed);
            Console.WriteLine("Press any key to close the console...");
            Console.ReadKey();
        }
    }

    public class AuthToken
    {
        public int CharacterID { get; set; }
        public string CharacterName { get; set; }
        public string CharacterOwnerHash { get; set; }

        public string AuthorizationToken { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int Expires { get; set; }

        public string Scopes { get; set; }
    }

    public class ESIResponse<T>
    {
        public DateTime? CachedUntil { get; set; }
        public T Result { get; set; }
    }

    public static class StaticInfo
    {
        public delegate void AuthCompletedDelegate(AuthToken token);
        public static event AuthCompletedDelegate AuthCompleted;

        public static void Completed(AuthToken token)
        {
            AuthCompleted?.Invoke(token);
        }

        public const string installDir = "";
        public static CultureInfo ci = CultureInfo.InvariantCulture.Clone() as CultureInfo;
        public static StreamReader GetStream(string url)
        {
            int failedAttempts = 0;
            while (failedAttempts < 10)
            {
                try
                {
                    WebRequest wrGetXml;
                    wrGetXml = WebRequest.Create(url);
                    return new StreamReader(wrGetXml.GetResponse().GetResponseStream());
                }
                catch (Exception)
                {
                    failedAttempts++;
                    Console.WriteLine("Caught an error accesing (#" + failedAttempts + ")" + url);
                }
            }
            Console.WriteLine("Program gave up accessing " + url);
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return null; // this is just here to make the error checker shut up, it should never run
        }

        public static ESIResponse<T> GetESIResponse<T>(string route, ESI.CharacterEnum typeenum = ESI.CharacterEnum.None, Dictionary<string, object> parms = null, string version = null)
        {
            if (version == null)
                version = "latest";

            route = SetPathParameters(route, typeenum, parms);

            string token = null;

            switch (typeenum)
            {
                case ESI.CharacterEnum.BuildCorp:
                    token = db.Settings.BuildCorpAccessToken;
                    break;
                case ESI.CharacterEnum.EmpireDonkey:
                    token = db.Settings.EmpireDonkeyAccessToken;
                    break;
            }

            var url = db.Settings.ESIURL
                .AppendPathSegments(version, route);

            if (parms != null && parms.Count > 0)
                url.SetQueryParams(parms);

            int retry = 0;
            while (retry < 3)
            {
                try
                {
                    HttpResponseMessage result;
                    // If we got a token, use it
                    if (!string.IsNullOrEmpty(token))
                        result = url.WithOAuthBearerToken(token).GetAsync().Result;
                    else // Otherwise just query directly
                        result = url.GetAsync().Result;

                    var json = result.Content.ReadAsStringAsync().Result;

                    var response = new ESIResponse<T>()
                    {
                        CachedUntil = result.Content.Headers?.Expires?.UtcDateTime.AddSeconds(10),
                        Result = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json)
                    };

                    return response;
                }
                catch (Exception ex)
                {
                    if (ex is AggregateException e)
                        ex = e.InnerException;

                    retry++;
                    Console.WriteLine("Caught an error accesing (#" + retry + ")" + url);
                    Console.WriteLine("Exception: " + ex.GetType().Name + " - " + ex.Message);
                }
            }
            Console.WriteLine("Failed to call ESI");
            Console.ReadKey();
            throw new Exception("Dead");
        }

        static string SetPathParameters(string route, ESI.CharacterEnum e, Dictionary<string, object> parms)
        {
            int character_id = 0, corporation_id = 0;

            switch(e)
            {
                case ESI.CharacterEnum.BuildCorp:
                    character_id = db.Settings.BuildCorpCharacterId;
                    corporation_id = db.Settings.BuildCorpCorporationId;
                    break;
                case ESI.CharacterEnum.EmpireDonkey:
                    character_id = db.Settings.EmpireDonkeyCharacterId;
                    corporation_id = db.Settings.EmpireDonkeyCorporationId;
                    break;
            }

            route = route.Replace("{character_id}", character_id.ToString());
            route = route.Replace("{corporation_id}", corporation_id.ToString());

            if (parms != null)
            {
                var parmkeys = parms.Keys.ToList();

                foreach (var key in parmkeys)
                {
                    var parm = "{" + key + "}";
                    if (route.Contains(parm))
                    {
                        route = route.Replace(parm, parms[key].ToString());
                        parms.Remove(key);
                    }
                }
            }

            return route;
        }

        public static ESIResponse<List<T>> ESIImportCrawl<T>(string route, ESI.CharacterEnum type = ESI.CharacterEnum.None, Dictionary<string, object> parms = null, int pageSize = 1000)
        {
            if (parms == null)
                parms = new Dictionary<string, object>();

            int page = 1;
            ESIResponse<List<T>> response = null, result = null;
            do
            {
                var myparms = new Dictionary<string, object>(parms);
                myparms.Add("page", page);
                response = StaticInfo.GetESIResponse<List<T>>(route, type, parms);

                if (result == null)
                    result = response;
                else
                    result.Result.AddRange(response.Result);

                page++;
            }
            while (response.Result.Count >= pageSize);

            return result;
        }
    }

}
