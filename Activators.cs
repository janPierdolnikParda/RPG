using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gra
{
	public class Activators
	{
		public static void soundOddawajPiec()
		{
			Engine.Singleton.SoundManager.PlayDialog("oddawaj_moj_piec.mp3");
		}

		public static void ChangeMap(string name = "Karczma", string nav = "Karczmanav", string x = "0", string y = "0", string z = "0")
		{
			Engine.Singleton.CurrentLevel.LoadNewMap = true;
            Engine.Singleton.CurrentLevel.NewMapName = name;
            Engine.Singleton.CurrentLevel.NewMapNav = nav;
            Engine.Singleton.HumanController.Character.Position = new Mogre.Vector3(float.Parse(x), float.Parse(y), float.Parse(z));
		}

        public static void SaveGame(string SlotName = "QuickSave")
        {
            Engine.Singleton.AutoSave(SlotName);
        }

        public static void LoadGame(string SlotName = "QuickSave")
        {
            Engine.Singleton.Load(SlotName);
        }

		public static void playSound(string play)
		{
			Engine.Singleton.SoundManager.PlaySound(play);
		}

        public static void ZejscieDoPiwnicy()
        {
            Engine.Singleton.CurrentLevel.LoadNewMap = true;
            Engine.Singleton.CurrentLevel.NewMapName = "Piwnica";
            Engine.Singleton.CurrentLevel.NewMapNav = "Karczmanav";
            Engine.Singleton.HumanController.Character.Position = new Mogre.Vector3(3.5f, 0.5f, -3.4f);
        }

        public static void teleportKarczma2()
        {
            Engine.Singleton.CurrentLevel.LoadNewMap = true;
            Engine.Singleton.CurrentLevel.NewMapName = "Karczmalvl2";
            Engine.Singleton.CurrentLevel.NewMapNav = "Karczmanav";
            Engine.Singleton.HumanController.Character.Position = new Mogre.Vector3(-4.29f, -0.57f, -9.09f);
        }

        public static void teleportKarczma()
        {
            Engine.Singleton.CurrentLevel.LoadNewMap = true;
            Engine.Singleton.CurrentLevel.NewMapName = "Karczma";
            Engine.Singleton.CurrentLevel.NewMapNav = "Karczmanav";
            Engine.Singleton.HumanController.Character.Position = new Mogre.Vector3(9.10f, -1.01f, 3.09f);
        }

        public static void WyjscieZPiwnicy()
        {
            Engine.Singleton.CurrentLevel.LoadNewMap = true;
            Engine.Singleton.CurrentLevel.NewMapName = "Karczma";
            Engine.Singleton.CurrentLevel.NewMapNav = "Karczmanav";
            Engine.Singleton.HumanController.Character.Position = new Mogre.Vector3(9, 1, -12);
        }

		public static void DajMieczZeSciany()
		{
			Engine.Singleton.ObjectManager.Destroy(Engine.Singleton.HumanController.FocusObject);
			Engine.Singleton.HumanController.Character.Inventory.Add(Items.I["sSword"]);
		}

		public static void Exit()
		{
			Engine.Singleton.GameEnder = true;
		}

        public static void Null()
        {
        }
	}
}
