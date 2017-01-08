using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IndustryThing.db
{
    class Db
    {
        public ItemTypeName itemName = new ItemTypeName();
        public Bpo bpo = new Bpo();
        public Types types = new Types();
        public GroupIDs groupIDs = new GroupIDs();
        public CategoryIDs categoryIDs = new CategoryIDs();
        public T2ModBpoOwned t2bpoOwned = new T2ModBpoOwned();
        public Settings settings = new Settings();

        public Db()
        {

        }
    }
}
