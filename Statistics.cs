using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gra
{
    public class Statistics
    {
        public int WalkaWrecz;
        public int Sila;
        public int Opanowanie;
        public int Wytrzymalosc;
        public int Zrecznosc;
        public int Charyzma;
        public int Zywotnosc;
        public int aktualnaZywotnosc;

        public Statistics(int ww, int si, int op, int wy, int zr, int ch, int zy)
        {
            WalkaWrecz = ww;
            Sila = si;
            Opanowanie = op;
            Wytrzymalosc = wy;
            Zrecznosc = zr;
            Charyzma = ch;
            Zywotnosc = zy;
            aktualnaZywotnosc = zy;
        }

        public Statistics()
        {
            WalkaWrecz = 30;
            Sila = 2;
            Opanowanie = 30;
            Wytrzymalosc = 2;
            Zywotnosc = 4;
            Charyzma = 30;
            Zrecznosc = 30;
            aktualnaZywotnosc = 4;
        }
    }
}
