using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gra
{
    public class Statistics
    {
        public int Hp;
        public int MaxHp;
        public int Mp;
        public int MaxMp;

        public Statistics(int hp, int mp)
        {
            Hp = hp;
            Mp = mp;
            MaxHp = hp;
            MaxMp = mp;
        }

        public Statistics()
        {
        }
    }
}
