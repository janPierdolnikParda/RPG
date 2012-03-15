using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

namespace Gra
{
    class HUD
    {

		class LogSlot
		{
			public TextLabel Label;

			public LogSlot(float left, float top)
			{
				Label = Engine.Singleton.Labeler.NewTextLabel("Primitive", 0.02f, new ColourValue(0.7f, 0.4f, 0), new ColourValue(1, 1.0f, 0.6f), 2);
				//Label.Caption = "dzambersmok";
				Label.SetPosition(0.81f, top);
			}

			public bool IsVisible
			{
				set
				{
					Label.IsVisible = value;

				}
			}

		}

		LogSlot[] LogSlots;

        SimpleQuad CompassBg;
        TextLabel CompassLabel;
		SimpleQuad Crosshair;

		SimpleQuad LoadScreen;

		SimpleQuad HPHero, HPEnemy, Log;
		TextLabel HPHeroLabel, HPEnemyLabel, LogLabel;

		List<Pair<string, ColourValue>> LogList;

        bool _isVisible;

        public HUD()
        {
			LogList = new List<Pair<string, ColourValue>>(8);
			for (int i = 0; i < 8; i++)
			{
				LogList.Add(new Pair<string, ColourValue>("", ColourValue.Black));
			}

			CompassBg = Engine.Singleton.Labeler.NewSimpleQuad("QuadMaterial", 0.1f, 0.1f, 0.2f, 0.1f, new ColourValue(1, 1, 1), 1);
            CompassLabel = Engine.Singleton.Labeler.NewTextLabel("Primitive", 0.05f, new ColourValue(0.7f, 0.4f, 0), new ColourValue(1, 1.0f, 0.6f), 2);
            CompassLabel.SetPosition(0.11f, 0.13f);

			Crosshair = Engine.Singleton.Labeler.NewSimpleQuad("CrosshairMat", Engine.Singleton.GetFloatFromPxWidth(((int)Engine.Singleton.Root.AutoCreatedWindow.Width / 2) - 10), Engine.Singleton.GetFloatFromPxHeight(((int)Engine.Singleton.Root.AutoCreatedWindow.Height / 2) - 10), Engine.Singleton.GetFloatFromPxWidth(20), Engine.Singleton.GetFloatFromPxHeight(20), new ColourValue(1, 1, 1), 2);


			HPHero = Engine.Singleton.Labeler.NewSimpleQuad("CzerwonyMaterial", 0.05f, 0.05f, 0.1f, 0.05f, new ColourValue(1, 1, 1), 1);
			HPEnemy = Engine.Singleton.Labeler.NewSimpleQuad("ZoltyMaterial", 0.45f, 0.05f, 0.1f, 0.05f, new ColourValue(1, 1, 1), 1);
			Log = Engine.Singleton.Labeler.NewSimpleQuad("QuadMaterial", 0.8f, 0.8f, 0.18f, 0.18f, new ColourValue(1, 1, 1), 1);

			HPHeroLabel = Engine.Singleton.Labeler.NewTextLabel("Primitive", 0.05f, new ColourValue(1, 0, 0), new ColourValue(1, 0, 0), 2);
			HPEnemyLabel = Engine.Singleton.Labeler.NewTextLabel("Primitive", 0.05f, new ColourValue(1, 0, 0), new ColourValue(1, 0, 0), 2);
			LogLabel = Engine.Singleton.Labeler.NewTextLabel("Primitive", 0.05f, new ColourValue(0.7f, 0.4f, 0), new ColourValue(1, 1.0f, 0.6f), 2);


			LoadScreen = Engine.Singleton.Labeler.NewSimpleQuad("Loading1", 0, 0, 1.0f, 1.0f, ColourValue.White, 4);

			HPHeroLabel.SetPosition(0.055f, 0.055f);
			HPEnemyLabel.SetPosition(0.455f, 0.055f);
			LogLabel.SetPosition(0.82f, 0.82f);
			
			LogSlots = new LogSlot[8];
			for (int i = 0; i < 8; i++)
			{
				LogSlots[i] = new LogSlot(0.81f, 0.81f + i * 0.02f);
			}
			
			IsVisible = false;
			DrawEnemyHP = false;
			DrawLog = false;
			LoadScreen.IsVisible = false;

			

        }

		bool _drawEnemyHP;
		public bool DrawEnemyHP
		{
			get { return _drawEnemyHP; }
			set 
			{
				_drawEnemyHP = value;
				HPEnemy.IsVisible = value;
				HPEnemyLabel.IsVisible = value;
			}
		}

		public void ToggleLoadScreen()
		{
			LoadScreen.IsVisible = !LoadScreen.IsVisible;
		}

		bool _drawLog;
		public bool DrawLog
		{
			get { return _drawLog; }
			set
			{
				_drawLog = value;
				Log.IsVisible = value;
				LogLabel.IsVisible = value;
				for (int i = 0; i < 8; i++)
				{
					LogSlots[i].IsVisible = value;
				}
			}
		}

        public Character Character
        {
            get
            {
                return Engine.Singleton.HumanController.Character;
            }
        }

		public Radian getRotationY()
		{
			Matrix3 orientMatrix;
			orientMatrix = Character.Orientation.ToRotationMatrix();

			Radian yRad, pRad, rRad;
			orientMatrix.ToEulerAnglesYXZ(out yRad, out pRad, out rRad);

			return yRad;
		}

        public void UpdateView()
        {
			CompassLabel.Caption = getRotationY().ValueDegrees.ToString();
			if (Character.FocusedEnemy != null)
				HPEnemyLabel.Caption = Character.FocusedEnemy.Statistics.aktualnaZywotnosc + "/" + Character.FocusedEnemy.Statistics.Zywotnosc;

			HPHeroLabel.Caption = Character.Statistics.aktualnaZywotnosc + "/" + Character.Statistics.Zywotnosc;

			if (DrawLog)
			{
				for (int i = 0; i < 8; i++)
				{
					LogSlots[i].Label.Caption = LogList[i].first;
					LogSlots[i].Label.SetColor(LogList[i].second, LogList[i].second);
				}

			}
        }

        public bool IsVisible
        {
            set
            {

                CompassBg.IsVisible = value;
                CompassLabel.IsVisible = value;
				Crosshair.IsVisible = value;
				HPHero.IsVisible = value;
				HPHeroLabel.IsVisible = value;

                if (value)
                {
                    UpdateView();
                }

                _isVisible = value;
            }
        }


        public void ToggleVisibility()
        {
            if (_isVisible == true)
                IsVisible = false;
            else
                IsVisible = true;
        }

		public void ToggleLog()
		{
			if (_drawLog)
				DrawLog = false;
			else
				DrawLog = true;
		}

		public void LogAdd(string what, ColourValue color)
		{
			
			LogList.Insert(0, new Pair<string, ColourValue>(what, color));

			LogList.RemoveAt(8);

		}

    }
}
