using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gra
{
    public class Prize
    {
        public String PrizeID;
        public int AmountExp;
        public int AmountGold;
        public List<DescribedProfile> ItemsList;

        public Prize(int Exp, int Gold, List<DescribedProfile> List)
        {
            AmountExp = Exp;
            AmountGold = Gold;
            ItemsList = new List<DescribedProfile>();
            ItemsList = List;
        }

        public Prize()
        {
            ItemsList = new List<DescribedProfile>();
        }

        public Prize(int Exp, int Gold, DescribedProfile Item)
        {
            AmountExp = Exp;
            AmountGold = Gold;
            ItemsList = new List<DescribedProfile>();
            ItemsList.Add(Item);
        }

        public Prize prize_Clone()
        {
            return (Prize)MemberwiseClone();
        }
    }
}
