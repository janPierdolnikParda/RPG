﻿using System;
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
    }
}
