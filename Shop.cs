using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gra
{
    class Shop
    {
        public List<DescribedProfile> Items;
        public String ShopName;
        public int Gold;
		public float Mnoznik;


        public Shop()
        {
            Items = new List<DescribedProfile>();
        }

        public Shop(List<DescribedProfile> items, int gold, String shopname, float mnoznik)
        {
            Items = new List<DescribedProfile>();

            foreach (DescribedProfile d in items)
                Items.Add(d);

            Gold = gold;
            ShopName = shopname;
			Mnoznik = mnoznik;
        }
    }
}
