using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

namespace Gra
{
    class KwadratLosu
    {
        public TextLabel Wartosc;
        SimpleQuad Kwadrat;
        SimpleQuad CzerwonaPoswiata;
        public bool Uzyty = false;
        public bool Zaznaczony = false;
        bool _IsVisible;

        public KwadratLosu(String wartosc, float top, float left)
        {
            Kwadrat = Engine.Singleton.Labeler.NewSimpleQuad("QuadMaterial", left, top, 0.08f, 0.08f, ColourValue.White, 1);
            Wartosc = Engine.Singleton.Labeler.NewTextLabel("Primitive", 0.05f, new ColourValue(0.7f, 0.4f, 0), new ColourValue(1, 1.0f, 0.6f), 4);
            Wartosc.SetPosition(left + 0.02f, top + 0.01f);
            Wartosc.Caption = wartosc;
            CzerwonaPoswiata = Engine.Singleton.Labeler.NewSimpleQuad("QuadMaterial", left - 0.01f, top - 0.01f, 0.1f, 0.1f, ColourValue.Red, 2);
        }

        public void Update()
        {
            if (Uzyty)
                Kwadrat = Kwadrat;
            else
                Kwadrat = Kwadrat;

            if (Zaznaczony)
                CzerwonaPoswiata.IsVisible = true;
            else
                CzerwonaPoswiata.IsVisible = false;
        }

        public bool IsVisible
        {
            get
            {
                return _IsVisible;
            }

            set
            {
                Wartosc.IsVisible = value;
                Kwadrat.IsVisible = value;

                if (Zaznaczony)
                    CzerwonaPoswiata.IsVisible = false;
                else
                    CzerwonaPoswiata.IsVisible = value;
            }
        }
    }

    class StatLosu
    {
        public SimpleQuad NameBg;
        public TextLabel Name;
        public SimpleQuad AddPoint;
        public SimpleQuad ValueBg;
        public TextLabel Value;
        bool _IsVisible;
        public bool AddPoint_Available = true;

        public StatLosu(String name, String value, float top, float left)
        {

            Name = Engine.Singleton.Labeler.NewTextLabel("Primitive", 0.05f, new ColourValue(0.7f, 0.4f, 0), new ColourValue(1, 1.0f, 0.6f), 4);
            Name.Caption = name;
            Name.SetPosition(left + 0.01f, top + 0.01f);

            Value = Engine.Singleton.Labeler.NewTextLabel("Primitive", 0.05f, new ColourValue(0.7f, 0.4f, 0), new ColourValue(1, 1.0f, 0.6f), 4);
            Value.Caption = value;
            Value.SetPosition(left + 0.22f, top + 0.01f);

            AddPoint = Engine.Singleton.Labeler.NewSimpleQuad("AddPoint", left + 0.34f, top, Engine.Singleton.GetFloatFromPxWidth(32), Engine.Singleton.GetFloatFromPxHeight(32), new ColourValue(1, 1, 1), 2);

            AddPoint.IsVisible = false;

            NameBg = Engine.Singleton.Labeler.NewSimpleQuad("QuadMaterial", left, top, 0.2f, 0.05f, new ColourValue(1, 1, 1), 2);

            ValueBg = Engine.Singleton.Labeler.NewSimpleQuad("QuadMaterial", left + 0.21f, top, 0.12f, 0.05f, new ColourValue(1, 1, 1), 2);
        }

        public void SetAddPointAvailable()
        {
            AddPoint_Available = true;
            AddPoint.Panel.MaterialName = "AddPoint";
        }

        public void SetAddPointUnavailable()
        {
            AddPoint_Available = false;
            AddPoint.Panel.MaterialName = "AddPointUnavailable";
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
                AddPoint.IsVisible = value;
                NameBg.IsVisible = value;
                Name.IsVisible = value;
            }
        }
    }

    class HUDNewCharacterStats
    {
        SimpleQuad Bg;
        SimpleQuad HeaderBg;
        TextLabel Header;
        SimpleQuad MouseCursor;
        SimpleQuad DownArrow;
        public SimpleQuad ResetBg;
        TextLabel Reset;
        public KwadratLosu[] KwadratyLosu;
        public StatLosu[] StatyLosu;
        public bool DownArrowEnabled = false;
        bool _IsVisible;
        public int KtoryZaznaczony;

        public HUDNewCharacterStats()
        {
            Bg = Engine.Singleton.Labeler.NewSimpleQuad("InventoryBgMaterial", 0.01f, 0.01f, 0.98f, 0.98f, new ColourValue(1, 1, 1), 0);
            MouseCursor = Engine.Singleton.Labeler.NewSimpleQuad("Kursor", 0.0f, 0.0f, Engine.Singleton.GetFloatFromPxWidth(32), Engine.Singleton.GetFloatFromPxHeight(32), new ColourValue(1, 1, 1), 4);
            HeaderBg = Engine.Singleton.Labeler.NewSimpleQuad("QuadMaterial", 0.25f, 0.04f, 0.5f, 0.1f, ColourValue.White, 1);
            Header = Engine.Singleton.Labeler.NewTextLabel("Primitive", 0.08f, new ColourValue(0.7f, 0.4f, 0), new ColourValue(1, 1.0f, 0.6f), 2);
            Header.SetPosition(0.4f, 0.06f);
            Header.Caption = "Statystyki";
            ResetBg = Engine.Singleton.Labeler.NewSimpleQuad("QuadMaterial", 0.8f, 0.85f, 0.09f, 0.06f, ColourValue.White, 1);
            Reset = Engine.Singleton.Labeler.NewTextLabel("Primitive", 0.04f, new ColourValue(0.7f, 0.4f, 0), new ColourValue(1, 1.0f, 0.6f), 2);
            Reset.SetPosition(0.81f, 0.86f);
            Reset.Caption = "RESET";

            DownArrow = Engine.Singleton.Labeler.NewSimpleQuad("DownArrow", 0.45f, 0.75f, Engine.Singleton.GetFloatFromPxWidth(64), Engine.Singleton.GetFloatFromPxHeight(128), ColourValue.White, 3);
            DownArrow.IsVisible = DownArrowEnabled;

            KwadratyLosu = new KwadratLosu[6];

            for (int i = 0; i < 6; i++)
            {
                KwadratyLosu[i] = new KwadratLosu(Engine.Singleton.Kostka(2, 10).ToString(), 0.2f, 0.215f + i * 0.1f);
            }

            StatyLosu = new StatLosu[6];
            StatyLosu[0] = new StatLosu("Walka wrecz", CharStats.WalkaWrecz.ToString(), 0.32f, 0.1f);
            StatyLosu[1] = new StatLosu("Krzepa", CharStats.Krzepa.ToString(), 0.38f, 0.1f);
            StatyLosu[2] = new StatLosu("Zrecznosc", CharStats.Zrecznosc.ToString(), 0.44f, 0.1f);
            StatyLosu[3] = new StatLosu("Charyzma", CharStats.Charyzma.ToString(), 0.5f, 0.1f);
            StatyLosu[4] = new StatLosu("Opanowanie", CharStats.Opanowanie.ToString(), 0.56f, 0.1f);
            StatyLosu[5] = new StatLosu("Odpornosc", CharStats.Odpornosc.ToString(), 0.62f, 0.1f);

            IsVisible = false;
        }

        public void Update()
        {
            MouseCursor.SetDimensions(Engine.Singleton.GetFloatFromPxWidth(Engine.Singleton.Mouse.MouseState.X.abs), Engine.Singleton.GetFloatFromPxHeight(Engine.Singleton.Mouse.MouseState.Y.abs), Engine.Singleton.GetFloatFromPxWidth(32), Engine.Singleton.GetFloatFromPxHeight(32));

            StatyLosu[0].UpdateValue(CharStats.WalkaWrecz.ToString() + "(+" + (CharStats.WalkaWrecz - 20).ToString() + ")");
            StatyLosu[1].UpdateValue(CharStats.Krzepa.ToString() + "(+" + (CharStats.Krzepa - 20).ToString() + ")");
            StatyLosu[2].UpdateValue(CharStats.Zrecznosc.ToString() + "(+" + (CharStats.Zrecznosc - 20).ToString() + ")");
            StatyLosu[3].UpdateValue(CharStats.Charyzma.ToString() + "(+" + (CharStats.Charyzma - 20).ToString() + ")");
            StatyLosu[4].UpdateValue(CharStats.Opanowanie.ToString() + "(+" + (CharStats.Opanowanie - 20).ToString() + ")");
            StatyLosu[5].UpdateValue(CharStats.Odpornosc.ToString() + "(+" + (CharStats.Odpornosc - 20).ToString() + ")");

            if (IsOverQuad(ResetBg))
                Reset.SetColor(new ColourValue(1.0f, 0, 0.2f), new ColourValue(1, 1.0f, 0.6f));
            else
                Reset.SetColor(new ColourValue(0.7f, 0.4f, 0), new ColourValue(1, 1.0f, 0.6f));

            for (int i = 0; i < 6; i++)
            {
                KwadratyLosu[i].Update();
            }

            DownArrow.IsVisible = DownArrowEnabled;
        }

        public Statistics CharStats
        {
            get
            {
                return Engine.Singleton.HumanController.Character.Statistics;
            }
        }

        public bool IsOverDownArrow()
        {
            return ((Engine.Singleton.GetFloatFromPxWidth(Engine.Singleton.Mouse.MouseState.X.abs) >= DownArrow.Panel.Left && Engine.Singleton.GetFloatFromPxWidth(Engine.Singleton.Mouse.MouseState.X.abs) <= DownArrow.Panel.Left + DownArrow.Panel.Width && Engine.Singleton.GetFloatFromPxHeight(Engine.Singleton.Mouse.MouseState.Y.abs) >= DownArrow.Panel.Top && Engine.Singleton.GetFloatFromPxHeight(Engine.Singleton.Mouse.MouseState.Y.abs) <= DownArrow.Panel.Top + DownArrow.Panel.Height));
        }

        public bool IsOverQuad(SimpleQuad quad)
        {
            return ((Engine.Singleton.GetFloatFromPxWidth(Engine.Singleton.Mouse.MouseState.X.abs) >= quad.Panel.Left && Engine.Singleton.GetFloatFromPxWidth(Engine.Singleton.Mouse.MouseState.X.abs) <= quad.Panel.Left + quad.Panel.Width && Engine.Singleton.GetFloatFromPxHeight(Engine.Singleton.Mouse.MouseState.Y.abs) >= quad.Panel.Top && Engine.Singleton.GetFloatFromPxHeight(Engine.Singleton.Mouse.MouseState.Y.abs) <= quad.Panel.Top + quad.Panel.Height));
        }

        public bool IsVisible
        {
            get
            {
                return _IsVisible;
            }

            set
            {
                Bg.IsVisible = value;
                HeaderBg.IsVisible = value;
                Header.IsVisible = value;
                MouseCursor.IsVisible = value;
                Reset.IsVisible = value;
                ResetBg.IsVisible = value;

                if (DownArrowEnabled)
                    DownArrow.IsVisible = value;
                else
                    DownArrow.IsVisible = false;

                for (int i = 0; i < 6; i++)
                {
                    KwadratyLosu[i].IsVisible = value;
                    StatyLosu[i].IsVisible = value;
                }
            }
        }
    }
}
