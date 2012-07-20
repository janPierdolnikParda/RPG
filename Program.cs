/*



				\   |   /
				 \  |  /
				  \ | /
		                _________
			       /    	 \
			      /           \
			     /             \
			    /  ___     ___  \
			   |                 |
			   |  |   |   |   |  |
			   |  | - |   | - |  |
			   |  |   |   |   |  |
		\|/        |   ___     ___   |
		 |         |                 |     \|/
		  \	   |   \  /   \	 /   |      |
		   \_______|    \/     \/    |     /
			   |                 |____/
			   |                 |
			   \                 \
			    \                 \ 
                             \                 \
                              \                	|
	                       \_______________/
				   /      \
				  /        \
				 /          \
				 |           |
			        _|           |_


MR. Poryciak!!!


*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Mogre;
using System.IO;
using System.Xml;

namespace Gra
{
    class Program
    {
        static void Main(string[] args)
        {
            bool DebugMode = false;

            Engine.Singleton.Initialise();

            //MENUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUu

            SubMenu MainMenu = new SubMenu();
            SubMenu Continue = new SubMenu();
            SubMenu NewGame = new SubMenu();
            SubMenu LoadGame = new SubMenu();
            SubMenu SaveGame = new SubMenu();
            SubMenu Options = new SubMenu();
            SubMenu Credits = new SubMenu();
            SubMenu End = new SubMenu();

            MainMenu.MenuName = "MENU";
            MainMenu.AddSub(NewGame);
            MainMenu.AddSub(Continue);
            MainMenu.AddSub(LoadGame);
            MainMenu.AddSub(SaveGame);
            MainMenu.AddSub(Options);
            //MainMenu.AddSub(Credits);
            MainMenu.AddSub(End);

            Continue.MenuName = "Kontynuuj";
            Continue.Ending = true;
            Continue.Enabled = false;
            Continue.Parent = MainMenu;
            Continue.AddAction(ContinueGame);

            NewGame.Ending = true;
            NewGame.MenuName = "Nowa gra";
            NewGame.Parent = MainMenu;
            NewGame.AddAction(New);

            LoadGame.MenuName = "Laduj gre";
            LoadGame.Enabled = false;
            LoadGame.Parent = MainMenu;

            SaveGame.MenuName = "Zapisz gre";
            SaveGame.Enabled = false;
            SaveGame.Parent = MainMenu;

            Options.MenuName = "Opcje";
            Options.Enabled = false;
            Options.Parent = MainMenu;

            Credits.MenuName = "Autorzy";
            Credits.Enabled = false;
            Credits.Parent = MainMenu;

            End.MenuName = "Koniec";
            End.Enabled = true;
            End.Parent = MainMenu;
            End.Ending = true;                  //CZASEM BYWA, ZE END NIE JEST ENDING .... POZDRO RABIS
            End.AddAction(Exit);

            Engine.Singleton.Menu = MainMenu;
            Engine.Singleton.HumanController.SwitchState(HumanController.HumanControllerState.MENU);

            //KONIEC MENUUUUUUUUUUUUUUUUUUUUUUUUUUUu

			Engine.Singleton.SoundManager.BGMPlaylist.Add("title.mp3");
			Engine.Singleton.SoundManager.BGMPlaylist.Add("Folk Round.mp3");
			Engine.Singleton.SoundManager.BGMPlaylist.Add("Achaidh Cheide.mp3");
			Engine.Singleton.SoundManager.BGMPlaylist.Add("Thatched Villagers.mp3");
			
            //Engine.Singleton.SoundManager.PlayBGM();

            Light light = Engine.Singleton.SceneManager.CreateLight();
            light.Type = Light.LightTypes.LT_DIRECTIONAL;
            light.Direction = new Vector3(1, -3, 1).NormalisedCopy;
            light.DiffuseColour = new ColourValue(0.2f, 0.2f, 0.2f);

            Engine.Singleton.SceneManager.ShadowTechnique = ShadowTechnique.SHADOWTYPE_STENCIL_MODULATIVE;

            Engine.Singleton.Mysz = Engine.Singleton.Mouse.MouseState;

            Character player = new Character(CharacterProfileManager.character);
            player.Position = new Vector3(-3.5f,0,1);
			player.Orientation = new Quaternion(new Radian(new Degree(90)), Vector3.UNIT_Y);

            player.Statistics = new Statistics();
			player.Sword = (ItemSword)Items.I["sSword"];
			player.GetSwordOrder = true;

            Engine.Singleton.ObjectManager.Add(player);
            Engine.Singleton.HumanController.Character = player;

            Engine.Singleton.CurrentLevel = new Level();
            Engine.Singleton.CurrentLevel.LoadLevel("MenuLevel", "KarczmaNav", true); // MENU LVL

            Engine.Singleton.GameCamera.Character = player;
			Engine.Singleton.GameCamera.Distance = 7;
            Engine.Singleton.GameCamera.Angle = new Degree(33);


			String resourceGroupName = "debugger";
			if (ResourceGroupManager.Singleton.ResourceGroupExists(resourceGroupName) == false)
				ResourceGroupManager.Singleton.CreateResourceGroup(resourceGroupName);

			// create material (colour)
			MaterialPtr moMaterial = Mogre.MaterialManager.Singleton.Create("line_material", resourceGroupName);
			moMaterial.ReceiveShadows = false;
			moMaterial.GetTechnique(0).SetLightingEnabled(true);
			moMaterial.GetTechnique(0).GetPass(0).SetDiffuse(0, 0, 1, 0);
			moMaterial.GetTechnique(0).GetPass(0).SetAmbient(0, 0, 1);
			moMaterial.GetTechnique(0).GetPass(0).SetSelfIllumination(0, 0, 1);
			moMaterial.Dispose();

			SceneNode moNode = Engine.Singleton.SceneManager.RootSceneNode.CreateChildSceneNode("line_node");
			ManualObject manOb = Engine.Singleton.SceneManager.CreateManualObject("line");

            while (true)
            {
                Engine.Singleton.Update();

                if (Engine.Singleton.Keyboard.IsKeyDown(MOIS.KeyCode.KC_ESCAPE) || Engine.Singleton.GameEnder)
                    break;

                if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_GRAVE))
                {
                    Engine.Singleton.Pause = !Engine.Singleton.Pause;
                }

                if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_F3))
                {
                    if (DebugMode)
                        DebugMode = false;
                    else
                        DebugMode = true;
                }

                if (DebugMode)
                    Engine.Singleton.NewtonDebugger.ShowDebugInformation();
                else
                    Engine.Singleton.NewtonDebugger.HideDebugInformation();
				/*
                if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_P))
                {
                    Engine.Singleton.CurrentLevel.navMesh.AStar(NPCManager.npc.Position, Engine.Singleton.HumanController.Character.Position);
                    if (Engine.Singleton.CurrentLevel.navMesh.TriPath.Count > 1)
                    {
                        Engine.Singleton.CurrentLevel.navMesh.GetPortals();
                        NPCManager.npc.WalkPath = Engine.Singleton.CurrentLevel.navMesh.Funnel();

                        NPCManager.npc.FollowPathOrder = true;
                    }
                }
				*/
            }

            Engine.Singleton.Root.Dispose();

        }

        static void New()
        {
            //Piotra ulubione tworzenie postaci :) @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

			Engine.Singleton.HumanController.HUD.ToggleLoadScreen();
			Engine.Singleton.Root.RenderOneFrame();

            Engine.Singleton.HumanController.Character.Position = new Vector3(-3.5f, 0, 1);
            Engine.Singleton.HumanController.Character.Orientation = new Quaternion(new Radian(new Degree(90)), Vector3.UNIT_Y);

            Engine.Singleton.GameCamera.Character = Engine.Singleton.HumanController.Character;
            Engine.Singleton.GameCamera.Distance = 4;
            Engine.Singleton.GameCamera.Angle = new Degree(20);
            Engine.Singleton.HumanController.HUDStats = new HUDStats();

			Engine.Singleton.HumanController.Character.Sword = null;

            while (Engine.Singleton.HumanController.Character.Inventory.Count > 0)
                Engine.Singleton.HumanController.Character.Inventory.RemoveAt(0);

            Engine.Singleton.HumanController.Character.Inventory.Add(Items.I["iSwieczka"]);
            Engine.Singleton.HumanController.ToggleHud();
            Engine.Singleton.HumanController.HUDNewCharacterStats = new HUDNewCharacterStats();
            //Engine.Singleton.HumanController.SwitchState(HumanController.HumanControllerState.CREATOR_STATS);
            Engine.Singleton.HumanController.SwitchState(HumanController.HumanControllerState.FREE);

            Engine.Singleton.CurrentLevel.DeleteLevel();
            Engine.Singleton.CreateNewChar();

            Engine.Singleton.CurrentLevel.LoadLevel("Karczma", "KarczmaNav");
			//Engine.Singleton.CurrentLevel.LoadLevel("LabWerter", "KarczmaNav", true);
			//Engine.Singleton.HumanController.Character.Position = new Vector3(-23.13174f, 1 , -0.80251f);
			
            Engine.Singleton.Load(null);
			Engine.Singleton.HumanController.HUD.ToggleLoadScreen();

            Engine.Singleton.Menu.SubMenus[1].Enabled = true;

            if (Engine.Singleton.Pause)
                Engine.Singleton.Pause = false;
        }

        static void Load()
        {
        }

        static void Save()
        {

        }

        static void ContinueGame()
        {
            Engine.Singleton.Pause = !Engine.Singleton.Pause;
        }

        static void Exit()
        {
            Engine.Singleton.GameEnder = true;
        }
    }
}
