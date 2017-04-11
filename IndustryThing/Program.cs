using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

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
       public const string installDir = "E:/C#/Industrything/";
    }

}
