using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

namespace Gra
{
    class Stat
    {
        public SimpleQuad NameBg;
        public TextLabel Name;
        public SimpleQuad AddPoint;
        public SimpleQuad ValueBg;
        public TextLabel Value;
        public bool AddAble;
        bool _IsVisible;

        public Stat(bool addAble, String name, String value, float top, float left)
        {
            
            AddAble = addAble;

            Name = Engine.Singleton.Labeler.NewTextLabel("Primitive", 0.05f, new ColourValue(0.7f, 0.4f, 0), new ColourValue(1, 1.0f, 0.6f), 3);
            Name.Caption = name;
            Name.SetPosition(left + 0.01f, top + 0.01f);

            Value = Engine.Singleton.Labeler.NewTextLabel("Primitive", 0.05f, new ColourValue(0.7f, 0.4f, 0), new ColourValue(1, 1.0f, 0.6f), 3);
            Value.Caption = value;
            Value.SetPosition(left + 0.22f, top + 0.01f);

            //if (AddAble)
                //AddPoint = Engine.Singleton.Labeler.NewSimpleQuad("AddPoint", left + 0.15f, top + 0.15f, width, height, new ColorValue(1, 1, 1), 2);

            NameBg = Engine.Singleton.Labeler.NewSimpleQuad("QuadMaterial", left, top, 0.2f, 0.05f, new ColourValue(1, 1, 1), 2);

            ValueBg = Engine.Singleton.Labeler.NewSimpleQuad("QuadMaterial", left + 0.21f, top, 0.07f, 0.05f, new ColourValue(1, 1, 1), 2);
        }

        public void UpdateValue(String value)
        {
            Value.Caption = value;
        }

        public bool IsVisible
        {
            get
            {
                return _IsVisible;
            }

            set
            {
                _IsVisible = value;
                Value.IsVisible = value;
                ValueBg.IsVisible = value;
                //AddPoint.IsVisible = value;
                NameBg.IsVisible = value;
                Name.IsVisible = value;                
            }
        }
    }

    class HUDStats
    {
        Stat[] Stats;
        bool _IsVisible;
        TextLabel Header;
        SimpleQuad Bg;

        public Statistics CharStats
        {
            get
            {
                return Engine.Singleton.HumanController.Character.Statistics;
            }
        }

        public HUDStats()
        {
            Stats = new Stat[10];
            Stats[0] = new Stat(true, "Walka wrecz", CharStats.WalkaWrecz.ToString(), 0.32f, 0.15f);
            Stats[1] = new Stat(true, "Sila", CharStats.Sila.ToString(), 0.38f, 0.15f);
            Stats[2] = new Stat(true, "Zrecznosc", CharStats.Zrecznosc.ToString(), 0.44f, 0.15f);
            Stats[3] = new Stat(true, "Zywotnosc", CharStats.Zywotnosc.ToString(), 0.5f, 0.15f);
            Stats[4] = new Stat(true, "Charyzma", CharStats.Charyzma.ToString(), 0.56f, 0.15f);
            Stats[5] = new Stat(true, "Opanowanie", CharStats.Opanowanie.ToString(), 0.32f, 0.55f);
            Stats[6] = new Stat(true, "Odpornosc", CharStats.Odpornosc.ToString(), 0.38f, 0.55f);
            Stats[7] = new Stat(true, "Wytrzymalosc", CharStats.Wytrzymalosc.ToString(), 0.44f, 0.55f);
            Stats[8] = new Stat(false, "Krzepa", CharStats.Krzepa.ToString(), 0.5f, 0.55f);
            Stats[9] = new Stat(false, "Ataki", CharStats.Ataki.ToString(), 0.56f, 0.55f);

            Bg = Engine.Singleton.Labeler.NewSimpleQuad("InventoryBgMaterial", 0.01f, 0.01f, 0.98f, 0.98f, new ColourValue(1, 1, 1), 0);
            IsVisible = false;
        }

        public void Update()
        {
            Stats[0].UpdateValue(CharStats.WalkaWrecz.ToString());
            Stats[1].UpdateValue(CharStats.Sila.ToString());
            Stats[2].UpdateValue(CharStats.Zrecznosc.ToString());
            Stats[3].UpdateValue(CharStats.Zywotnosc.ToString());
            Stats[4].UpdateValue(CharStats.Charyzma.ToString());
            Stats[5].UpdateValue(CharStats.Opanowanie.ToString());
            Stats[6].UpdateValue(CharStats.Odpornosc.ToString());
            Stats[7].UpdateValue(CharStats.Wytrzymalosc.ToString());
            Stats[8].UpdateValue(CharStats.Krzepa.ToString());
            Stats[9].UpdateValue(CharStats.Ataki.ToString());
        }

        public bool IsVisible
        {
            get
            {
                return _IsVisible;
            }

            set
            {
                _IsVisible = value;
                //Header.IsVisible = value;
                Bg.IsVisible = value;

                foreach (Stat s in Stats)
                    s.IsVisible = value;
            }
        }
        
    }
}
