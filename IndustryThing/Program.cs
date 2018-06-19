using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Globalization;

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

    }

}
