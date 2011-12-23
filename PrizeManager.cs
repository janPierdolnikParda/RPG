using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gra
{
    public class PrizeManager
    {
        static public Prize Prize1;

        public PrizeManager()
        {
            Prize1 = new Prize(10, 3, Items.swordProfile);

        }


    }
}
