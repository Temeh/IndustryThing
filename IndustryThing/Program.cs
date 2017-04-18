using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Net;

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
            calculator.Calculator calc = new calculator.Calculator();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
 
        }
    }

    public static class StaticInfo
    {
       public const string installDir = "";

        public static StreamReader GetStream(string url)
        {
            int failedAttempts=0;
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
                }
            }
            MessageBox.Show("There was many errors, eve, or temi is being shit. Try again, now, or later!");
            Application.Exit();
            return null; // this is just here to make the error checker shut up, it should never run
        }
    }

}
