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

            AddPoint = Engine.Singleton.Labeler.NewSimpleQuad("AddPoint", left + 0.29f, top, Engine.Singleton.GetFloatFromPxWidth(32), Engine.Singleton.GetFloatFromPxHeight(32), new ColourValue(1, 1, 1), 2);

            if (AddAble)
                AddPoint.IsVisible = false;

            NameBg = Engine.Singleton.Labeler.NewSimpleQuad("QuadMaterial", left, top, 0.2f, 0.05f, new ColourValue(1, 1, 1), 2);

            ValueBg = Engine.Singleton.Labeler.NewSimpleQuad("QuadMaterial", left + 0.21f, top, 0.07f, 0.05f, new ColourValue(1, 1, 1), 2);
        }

        public bool IsOverAddPoint()
        {
            return ((Engine.Singleton.GetFloatFromPxWidth(Engine.Singleton.Mouse.MouseState.X.abs) >= AddPoint.Panel.Left && Engine.Singleton.GetFloatFromPxWidth(Engine.Singleton.Mouse.MouseState.X.abs) <= AddPoint.Panel.Left + AddPoint.Panel.Width && Engine.Singleton.GetFloatFromPxHeight(Engine.Singleton.Mouse.MouseState.Y.abs) >= AddPoint.Panel.Top && Engine.Singleton.GetFloatFromPxHeight(Engine.Singleton.Mouse.MouseState.Y.abs) <= AddPoint.Panel.Top + AddPoint.Panel.Height));
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
                if (AddAble)
                    AddPoint.IsVisible = value;
                else
                    AddPoint.IsVisible = false;
                NameBg.IsVisible = value;
                Name.IsVisible = value;                
            }
        }
    }

    class HUDStats
    {
        public Stat[] Stats;
        bool _IsVisible;
        SimpleQuad HeaderBg;
        TextLabel Header;
        SimpleQuad ExpBg;
        TextLabel Exp;
        SimpleQuad Bg;
        SimpleQuad MouseCursor;

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
            Stats[7] = new Stat(false, "Wytrzymalosc", CharStats.Wytrzymalosc.ToString(), 0.44f, 0.55f);
            Stats[8] = new Stat(false, "Krzepa", CharStats.Krzepa.ToString(), 0.5f, 0.55f);
            Stats[9] = new Stat(false, "Ataki", CharStats.Ataki.ToString(), 0.56f, 0.55f);

            HeaderBg = Engine.Singleton.Labeler.NewSimpleQuad("QuadMaterial", 0.25f, 0.04f, 0.5f, 0.1f, ColourValue.White, 1);
            Header = Engine.Singleton.Labeler.NewTextLabel("Primitive", 0.08f, new ColourValue(0.7f, 0.4f, 0), new ColourValue(1, 1.0f, 0.6f), 2);
            Header.SetPosition(0.4f, 0.06f);
            Header.Caption = "Statystyki";
            ExpBg = Engine.Singleton.Labeler.NewSimpleQuad("QuadMaterial", 0.25f, 0.2f, 0.5f, 0.06f, ColourValue.White, 1);
            Exp = Engine.Singleton.Labeler.NewTextLabel("Primitive", 0.05f, new ColourValue(0.7f, 0.4f, 0), new ColourValue(1, 1.0f, 0.6f), 2);
            Exp.SetPosition(0.26f, 0.21f);
            MouseCursor = Engine.Singleton.Labeler.NewSimpleQuad("Kursor", 0.0f, 0.0f, Engine.Singleton.GetFloatFromPxWidth(32), Engine.Singleton.GetFloatFromPxHeight(32), new ColourValue(1, 1, 1), 4);
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
            MouseCursor.SetDimensions(Engine.Singleton.GetFloatFromPxWidth(Engine.Singleton.Mouse.MouseState.X.abs), Engine.Singleton.GetFloatFromPxHeight(Engine.Singleton.Mouse.MouseState.Y.abs), Engine.Singleton.GetFloatFromPxWidth(32), Engine.Singleton.GetFloatFromPxHeight(32));
            Exp.Caption = "Punkty doswiadczenia: " + Engine.Singleton.HumanController.Character.Profile.Exp.ToString();
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
                Header.IsVisible = value;
                HeaderBg.IsVisible = value;
                Bg.IsVisible = value;
                MouseCursor.IsVisible = value;
                Exp.IsVisible = value;
                ExpBg.IsVisible = value;

                foreach (Stat s in Stats)
                    s.IsVisible = value;
            }
        }
        
    }
}
