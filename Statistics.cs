using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gra
{
    public class Statistics
    {
        public int WalkaWrecz;
        public int Krzepa;
	    public int Ataki;
        public int Opanowanie;
        public int Odpornosc;
        public int Zrecznosc;
        public int Charyzma;
        public int Zywotnosc;
        public int aktualnaZywotnosc;
		public int Sila;
		public int Wytrzymalosc;

        public int Ile_WW = 0;
        public int Ile_KR = 0;
        public int Ile_OP = 0;
        public int Ile_ODP = 0;
        public int Ile_ZR = 0;
        public int Ile_CH = 0;
        public int Ile_ZY = 0;

        public Statistics(int ww, int k, int op, int odp, int zr, int ch, int zy, int at)
        {
            WalkaWrecz = ww;
			Krzepa = k;
			Sila = Krzepa / 10;
            Opanowanie = op;
            Odpornosc = odp;
            Zrecznosc = zr;
            Charyzma = ch;
            Zywotnosc = zy;
            aktualnaZywotnosc = zy;
			Ataki = at;
			Wytrzymalosc = Odpornosc / 10;
        }

        public void PrzypiszIlosci(int ww, int k, int op, int odp, int zr, int ch, int zy)
        {
            Ile_WW = ww;
            Ile_KR = k;
            Ile_OP = op;
            Ile_ODP = odp;
            Ile_ZR = zr;
            Ile_CH = ch;
            Ile_ZY = zy;
        }

        public Statistics()
        {
            WalkaWrecz = 20;
            Krzepa = 20;
            Opanowanie = 20;
            Odpornosc = 20;
            Zywotnosc = 4;
            Charyzma = 20;
            Zrecznosc = 20;
            aktualnaZywotnosc = 4;
			Ataki = 1;
			Sila = 2;
			Wytrzymalosc = 2;
        }

        public void UpdateStatistics()
        {
            Sila = Krzepa / 10;
            Wytrzymalosc = Odpornosc / 10;
        }

        public Statistics statistics_Clone()
        {
            return (Statistics)MemberwiseClone();
        }
    }
}
