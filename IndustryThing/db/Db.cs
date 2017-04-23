using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IndustryThing.db
{
    class Db
    {
        public ItemTypeName itemName;
        public Bpo bpo ;
        public Types types ;
        public GroupIDs groupIDs;
        public CategoryIDs categoryIDs;
        public T2ModBpoOwned t2bpoOwned ;
        public Settings settings ;

        public Db()
        {
            Console.WriteLine("....Reading ItemTypeName file...");
            itemName = new ItemTypeName();
            Console.WriteLine("....Done reading ItemTypeName file");
            Console.WriteLine("....Reading the BPO file...");
            bpo = new Bpo();
            Console.WriteLine("....Done reading the BPO file");
            Console.WriteLine("....Reading the Types file...");
            types = new Types();
            Console.WriteLine("....Done reading the Types file");
            Console.WriteLine("....Reading the GroupID's file...");
            groupIDs = new GroupIDs();
            Console.WriteLine("....Done reading the GroupID's file");
            Console.WriteLine("....Reading the CategoryID's file...");
            categoryIDs = new CategoryIDs();
            Console.WriteLine("....Done reading the GroupID's file");
            Console.WriteLine("....Reading the file for owned t2bpo's...");
            t2bpoOwned = new T2ModBpoOwned();
            Console.WriteLine("....Done reading the owned t2bpo file");
            Console.WriteLine("....Reading the settings file...");
            settings = new Settings();
            Console.WriteLine("....Done reading the settings file");
        }
    }
}
