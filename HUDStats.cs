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
        public SimpleQuad RemovePoint;
        public SimpleQuad ValueBg;
        public TextLabel Value;
        public bool AddAble;
        bool _IsVisible;
        public bool AddPoint_Available = true;
        public bool RemovePoint_Available = true;

        public Stat(bool addAble, String name, String value, float top, float left)
        {
            
            AddAble = addAble;

            Name = Engine.Singleton.Labeler.NewTextLabel("Primitive", 0.05f, new ColourValue(0.7f, 0.4f, 0), new ColourValue(1, 1.0f, 0.6f), 2 );
            Name.Caption = name;
            Name.SetPosition(left + 0.01f, top + 0.01f);

            Value = Engine.Singleton.Labeler.NewTextLabel("Primitive", 0.05f, new ColourValue(0.7f, 0.4f, 0), new ColourValue(1, 1.0f, 0.6f), 2);
            Value.Caption = value;
            Value.SetPosition(left + 0.22f, top + 0.01f);

            AddPoint = Engine.Singleton.Labeler.NewSimpleQuad("AddPoint", left + 0.29f, top, Engine.Singleton.GetFloatFromPxWidth(32), Engine.Singleton.GetFloatFromPxHeight(32), new ColourValue(1, 1, 1), 1);
            RemovePoint = Engine.Singleton.Labeler.NewSimpleQuad("RemovePoint", left + 0.3f + Engine.Singleton.GetFloatFromPxWidth(32), top, Engine.Singleton.GetFloatFromPxWidth(32), Engine.Singleton.GetFloatFromPxHeight(32), new ColourValue(1, 1, 1), 1);

            if (AddAble)
            {
                AddPoint.IsVisible = false;
                RemovePoint.IsVisible = false;
            }

            NameBg = Engine.Singleton.Labeler.NewSimpleQuad("QuadMaterial", left, top, 0.2f, 0.05f, new ColourValue(1, 1, 1), 1);

            ValueBg = Engine.Singleton.Labeler.NewSimpleQuad("QuadMaterial", left + 0.21f, top, 0.07f, 0.05f, new ColourValue(1, 1, 1), 1);
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

        public void SetRemovePointAvailable()
        {
            RemovePoint_Available = true;
            RemovePoint.Panel.MaterialName = "RemovePoint";
        }

        public void SetRemovePointUnavailable()
        {
            RemovePoint_Available = false;
            RemovePoint.Panel.MaterialName = "RemovePointUnavailable";
        }

        public bool IsOverAddPoint()
        {
            return ((Engine.Singleton.GetFloatFromPxWidth(Engine.Singleton.Mouse.MouseState.X.abs) >= AddPoint.Panel.Left && Engine.Singleton.GetFloatFromPxWidth(Engine.Singleton.Mouse.MouseState.X.abs) <= AddPoint.Panel.Left + AddPoint.Panel.Width && Engine.Singleton.GetFloatFromPxHeight(Engine.Singleton.Mouse.MouseState.Y.abs) >= AddPoint.Panel.Top && Engine.Singleton.GetFloatFromPxHeight(Engine.Singleton.Mouse.MouseState.Y.abs) <= AddPoint.Panel.Top + AddPoint.Panel.Height));
        }

        public bool IsOverRemovePoint()
        {
            return ((Engine.Singleton.GetFloatFromPxWidth(Engine.Singleton.Mouse.MouseState.X.abs) >= RemovePoint.Panel.Left && Engine.Singleton.GetFloatFromPxWidth(Engine.Singleton.Mouse.MouseState.X.abs) <= RemovePoint.Panel.Left + RemovePoint.Panel.Width && Engine.Singleton.GetFloatFromPxHeight(Engine.Singleton.Mouse.MouseState.Y.abs) >= RemovePoint.Panel.Top && Engine.Singleton.GetFloatFromPxHeight(Engine.Singleton.Mouse.MouseState.Y.abs) <= RemovePoint.Panel.Top + RemovePoint.Panel.Height));
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
                {
                    AddPoint.IsVisible = value;
                    RemovePoint.IsVisible = value;
                }
                else
                {
                    AddPoint.IsVisible = false;
                    RemovePoint.IsVisible = false;
                }
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
        SimpleQuad ZmianyBg;
        public TextLabel Zmiany;
        public SimpleQuad RequiredBg;
        public TextLabel Required;

        public Statistics CharStats
        {
            get
            {
                return Engine.Singleton.HumanController.Character.Statistics;
            }
        }

		public Character Character
		{
			get
			{
				return Engine.Singleton.HumanController.Character;
			}
		}

        public HumanController HumanController
        {
            get
            {
                return Engine.Singleton.HumanController;
            }
        }

        public HUDStats()
        {
            Stats = new Stat[10];
            Stats[0] = new Stat(true, "Walka wrecz", CharStats.WalkaWrecz.ToString(), 0.32f, 0.1f);
            Stats[1] = new Stat(true, "Krzepa", CharStats.Krzepa.ToString(), 0.38f, 0.1f);
            Stats[2] = new Stat(true, "Zrecznosc", CharStats.Zrecznosc.ToString(), 0.44f, 0.1f);
            Stats[3] = new Stat(true, "Zywotnosc", CharStats.Zywotnosc.ToString(), 0.5f, 0.1f);
            Stats[4] = new Stat(true, "Charyzma", CharStats.Charyzma.ToString(), 0.56f, 0.1f);
            Stats[5] = new Stat(true, "Opanowanie", CharStats.Opanowanie.ToString(), 0.32f, 0.5f);
            Stats[6] = new Stat(true, "Odpornosc", CharStats.Odpornosc.ToString(), 0.38f, 0.5f);
            Stats[7] = new Stat(false, "Wytrzymalosc", CharStats.Wytrzymalosc.ToString(), 0.44f, 0.5f);
            Stats[8] = new Stat(false, "Sila", CharStats.Sila.ToString(), 0.5f, 0.5f);
            Stats[9] = new Stat(false, "Ataki", CharStats.Ataki.ToString(), 0.56f, 0.5f);

            HeaderBg = Engine.Singleton.Labeler.NewSimpleQuad("QuadMaterial", 0.25f, 0.04f, 0.5f, 0.1f, ColourValue.White, 1);
            Header = Engine.Singleton.Labeler.NewTextLabel("Primitive", 0.08f, new ColourValue(0.7f, 0.4f, 0), new ColourValue(1, 1.0f, 0.6f), 2);
            Header.SetPosition(0.4f, 0.06f);
            Header.Caption = "Statystyki";
            ExpBg = Engine.Singleton.Labeler.NewSimpleQuad("QuadMaterial", 0.25f, 0.2f, 0.5f, 0.06f, ColourValue.White, 2);
            Exp = Engine.Singleton.Labeler.NewTextLabel("Primitive", 0.05f, new ColourValue(0.7f, 0.4f, 0), new ColourValue(1, 1.0f, 0.6f), 3);
            Exp.SetPosition(0.26f, 0.21f);
            MouseCursor = Engine.Singleton.Labeler.NewSimpleQuad("Kursor", 0.0f, 0.0f, Engine.Singleton.GetFloatFromPxWidth(32), Engine.Singleton.GetFloatFromPxHeight(32), new ColourValue(1, 1, 1), 4);
            Bg = Engine.Singleton.Labeler.NewSimpleQuad("InventoryBgMaterial", 0.01f, 0.01f, 0.98f, 0.98f, new ColourValue(1, 1, 1), 0);
            Zmiany = Engine.Singleton.Labeler.NewTextLabel("Primitive", 0.04f, new ColourValue(0.7f, 0.4f, 0), new ColourValue(1, 1.0f, 0.6f), 2);
            Zmiany.SetPosition(0.42f, 0.71f);
            Zmiany.Caption = "Zatwierdz zmiany";
            ZmianyBg = Engine.Singleton.Labeler.NewSimpleQuad("QuadMaterial", 0.41f, 0.7f, 0.22f, 0.06f, ColourValue.White, 1);
            RequiredBg = Engine.Singleton.Labeler.NewSimpleQuad("Black", 0, 0, 0.3f, 0.05f, new ColourValue(1, 1, 1), 3);
            Required = Engine.Singleton.Labeler.NewTextLabel("Primitive", 0.05f, new ColourValue(0.7f, 0.4f, 0), new ColourValue(1, 1.0f, 0.6f), 4);

            IsVisible = false;
        }

        public void Update()
        {
            if (Character.Profile.Exp >= (Character.Statistics.Ile_WW + 1) * 100)
                Stats[0].SetAddPointAvailable();
            else
                Stats[0].SetAddPointUnavailable();
            if (HumanController.StatisticsB4.WalkaWrecz < Character.Statistics.WalkaWrecz)
                Stats[0].SetRemovePointAvailable();
            else
                Stats[0].SetRemovePointUnavailable();
            Stats[0].UpdateValue(CharStats.WalkaWrecz.ToString());


            if (Character.Profile.Exp >= (Character.Statistics.Ile_KR + 1) * 100)
                Stats[1].SetAddPointAvailable();
            else
                Stats[1].SetAddPointUnavailable();
            if (HumanController.StatisticsB4.Krzepa < Character.Statistics.Krzepa)
                Stats[1].SetRemovePointAvailable();
            else
                Stats[1].SetRemovePointUnavailable();
            Stats[1].UpdateValue(CharStats.Krzepa.ToString());


            if (Character.Profile.Exp >= (Character.Statistics.Ile_ZR + 1) * 100)
                Stats[2].SetAddPointAvailable();
            else
                Stats[2].SetAddPointUnavailable();
            if (HumanController.StatisticsB4.Zrecznosc < Character.Statistics.Zrecznosc)
                Stats[2].SetRemovePointAvailable();
            else
                Stats[2].SetRemovePointUnavailable();
            Stats[2].UpdateValue(CharStats.Zrecznosc.ToString());


            if (Character.Profile.Exp >= (Character.Statistics.Ile_ZY + 1) * 100)
                Stats[3].SetAddPointAvailable();
            else
                Stats[3].SetAddPointUnavailable();
            if (HumanController.StatisticsB4.Zywotnosc < Character.Statistics.Zywotnosc)
                Stats[3].SetRemovePointAvailable();
            else
                Stats[3].SetRemovePointUnavailable();
            Stats[3].UpdateValue(CharStats.Zywotnosc.ToString());


            if (Character.Profile.Exp >= (Character.Statistics.Ile_CH + 1) * 100)
                Stats[4].SetAddPointAvailable();
            else
                Stats[4].SetAddPointUnavailable();
            if (HumanController.StatisticsB4.Charyzma < Character.Statistics.Charyzma)
                Stats[4].SetRemovePointAvailable();
            else
                Stats[4].SetRemovePointUnavailable();
            Stats[4].UpdateValue(CharStats.Charyzma.ToString());


            if (Character.Profile.Exp >= (Character.Statistics.Ile_OP + 1) * 100)
                Stats[5].SetAddPointAvailable();
            else
                Stats[5].SetAddPointUnavailable();
            if (HumanController.StatisticsB4.Opanowanie < Character.Statistics.Opanowanie)
                Stats[5].SetRemovePointAvailable();
            else
                Stats[5].SetRemovePointUnavailable();
            Stats[5].UpdateValue(CharStats.Opanowanie.ToString());


            if (Character.Profile.Exp >= (Character.Statistics.Ile_ODP + 1) * 100)
                Stats[6].SetAddPointAvailable();
            else
                Stats[6].SetAddPointUnavailable();
            if (HumanController.StatisticsB4.Odpornosc < Character.Statistics.Odpornosc)
                Stats[6].SetRemovePointAvailable();
            else
                Stats[6].SetRemovePointUnavailable();
            Stats[6].UpdateValue(CharStats.Odpornosc.ToString());



            Stats[7].UpdateValue(CharStats.Wytrzymalosc.ToString());
            Stats[8].UpdateValue(CharStats.Sila.ToString());
            Stats[9].UpdateValue(CharStats.Ataki.ToString());
            MouseCursor.SetDimensions(Engine.Singleton.GetFloatFromPxWidth(Engine.Singleton.Mouse.MouseState.X.abs), Engine.Singleton.GetFloatFromPxHeight(Engine.Singleton.Mouse.MouseState.Y.abs), Engine.Singleton.GetFloatFromPxWidth(32), Engine.Singleton.GetFloatFromPxHeight(32));
            Exp.Caption = "Punkty doswiadczenia: " + Engine.Singleton.HumanController.Character.Profile.Exp.ToString();

            if (Required.IsVisible)
            {
                Required.SetPosition(Engine.Singleton.GetFloatFromPxWidth(Engine.Singleton.Mouse.MouseState.X.abs) + 0.06f, Engine.Singleton.GetFloatFromPxHeight(Engine.Singleton.Mouse.MouseState.Y.abs) + 0.01f);
                RequiredBg.SetDimensions(Engine.Singleton.GetFloatFromPxWidth(Engine.Singleton.Mouse.MouseState.X.abs) + 0.06f, Engine.Singleton.GetFloatFromPxHeight(Engine.Singleton.Mouse.MouseState.Y.abs), 0.3f, 0.05f);
            }

			Character.MoveLeftOrder = false;
			Character.MoveRightOrder = false;
			Character.MoveOrder = false;
			Character.MoveOrderBack = false;
        }

        public bool IsOverZmiany()
        {
            return ((Engine.Singleton.GetFloatFromPxWidth(Engine.Singleton.Mouse.MouseState.X.abs) >= ZmianyBg.Panel.Left && Engine.Singleton.GetFloatFromPxWidth(Engine.Singleton.Mouse.MouseState.X.abs) <= ZmianyBg.Panel.Left + ZmianyBg.Panel.Width && Engine.Singleton.GetFloatFromPxHeight(Engine.Singleton.Mouse.MouseState.Y.abs) >= ZmianyBg.Panel.Top && Engine.Singleton.GetFloatFromPxHeight(Engine.Singleton.Mouse.MouseState.Y.abs) <= ZmianyBg.Panel.Top + ZmianyBg.Panel.Height));
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
                Zmiany.IsVisible = value;
                ZmianyBg.IsVisible = value;

                if (!value)
                {
                    Required.IsVisible = value;
                    RequiredBg.IsVisible = value;
                }

                foreach (Stat s in Stats)
                    s.IsVisible = value;
            }
        }
        
    }
}
