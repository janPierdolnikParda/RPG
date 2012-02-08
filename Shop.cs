using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gra
{
    class Shop
    {
        public Dictionary<DescribedProfile, int> Items;
        public String ShopName;
        public int Gold;

        public Shop()
        {
            Items = new Dictionary<DescribedProfile, int>();
        }

        public Shop(List<DescribedProfile> items, int gold, String shopname, float mnoznik)
        {
            Items = new Dictionary<DescribedProfile, int>();

            foreach (DescribedProfile d in items)
                Items.Add(d, (int)(d.Price * mnoznik));

            Gold = gold;
            ShopName = shopname;
        }
    }
}
