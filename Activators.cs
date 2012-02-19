using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gra
{
	public class Activators
	{
		public static void Null()
		{}

		public static void soundOddawajPiec()
		{
			Engine.Singleton.SoundManager.PlayDialog("oddawaj_moj_piec.mp3");
		}

        public static void ZejscieDoPiwnicy()
        {
            Engine.Singleton.CurrentLevel.LoadNewMap = true;
            Engine.Singleton.CurrentLevel.NewMapName = "Piwnica";
            Engine.Singleton.CurrentLevel.NewMapNav = "Karczmanav";
            Engine.Singleton.HumanController.Character.Position = new Mogre.Vector3(3.5f, 0.5f, -3.4f);
        }

        public static void WyjscieZPiwnicy()
        {
            Engine.Singleton.CurrentLevel.LoadNewMap = true;
            Engine.Singleton.CurrentLevel.NewMapName = "Karczma";
            Engine.Singleton.CurrentLevel.NewMapNav = "Karczmanav";
            Engine.Singleton.HumanController.Character.Position = new Mogre.Vector3(9, 1, -12);
        }
	}
}
