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

        public static ESIResponse<T> GetESIResponse<T>(string route, ESI.CharacterEnum typeenum, string version = null)
        {
            if (version == null)
                version = "latest";

            route = SetRouteParameters(route, typeenum);

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

            var request = url.WithOAuthBearerToken(token);

            int retry = 0;
            while (retry < 3)
            {
                try
                {
                    var result = request.GetAsync().Result;
                    var json = result.Content.ReadAsStringAsync().Result;

                    var response = new ESIResponse<T>()
                    {
                        CachedUntil = result.Content.Headers?.Expires?.UtcDateTime,
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

        static string SetRouteParameters(string route, ESI.CharacterEnum e)
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

            return route;
        }
    }

}
