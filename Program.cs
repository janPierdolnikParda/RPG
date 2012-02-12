﻿using System;
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

			Engine.Singleton.SoundManager.BGMPlaylist.Add("Achaidh Cheide.mp3");
			Engine.Singleton.SoundManager.BGMPlaylist.Add("Thatched Villagers.mp3");
            //Engine.Singleton.SoundManager.PlayBGM();


            Engine.Singleton.CurrentLevel = new Level();
           // Engine.Singleton.CurrentLevel.SetGraphicsMesh("World.mesh");
           // Engine.Singleton.CurrentLevel.SetCollisionMesh("World.mesh");

            Engine.Singleton.CurrentLevel.LoadLevel("Karczma", "KarczmaNav");
            Engine.Singleton.Load();

            TriggerVolume triggerVolume = new TriggerVolume();
            triggerVolume.BeginShapeBuild();
            triggerVolume.AddBoxPart(Vector3.ZERO, Quaternion.IDENTITY, new Vector3(2, 2, 2));
            triggerVolume.EndShapeBuild();
            Engine.Singleton.ObjectManager.Add(triggerVolume);
            triggerVolume.Position = new Vector3(9, 3.2f, 8);
            triggerVolume.OnCharacterEntered += new TriggerVolume.CharacterEnteredHandler(triggerVolume_OnCharacterEntered);          

            Character player = new Character(CharacterProfileManager.character);
            player.Position = new Vector3(7.4251f, 0.2231f, -1.0019f);
            Engine.Singleton.ObjectManager.Add(player);

            Engine.Singleton.GameCamera.Character = player;
            Engine.Singleton.GameCamera.Distance = 4;
            Engine.Singleton.GameCamera.Angle = new Degree(20);

            Light light = Engine.Singleton.SceneManager.CreateLight();
            light.Type = Light.LightTypes.LT_DIRECTIONAL;
            light.Direction = new Vector3(1, -3, 1).NormalisedCopy;
            light.DiffuseColour = new ColourValue(0.2f, 0.2f, 0.2f);

            Engine.Singleton.SceneManager.ShadowTechnique = ShadowTechnique.SHADOWTYPE_STENCIL_MODULATIVE;

            Engine.Singleton.HumanController.Character = player;
            
            Engine.Singleton.HumanController.Character.Inventory.Add(Items.I["iKufel"]);
            Engine.Singleton.HumanController.ToggleHud();

            Engine.Singleton.Mysz = Engine.Singleton.Mouse.MouseState;

            while (true)
            {
                Engine.Singleton.Update();

                if (Engine.Singleton.Keyboard.IsKeyDown(MOIS.KeyCode.KC_ESCAPE))
                    break;

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

                if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_P))
                {
                    Engine.Singleton.CurrentLevel.navMesh.AStar(NPCManager.npc.Position, player.Position);
                    if (Engine.Singleton.CurrentLevel.navMesh.TriPath.Count > 1)
                    {
						Console.WriteLine("-------NAV-------");
                        Engine.Singleton.CurrentLevel.navMesh.GetPortals();
                        NPCManager.npc.WalkPath = Engine.Singleton.CurrentLevel.navMesh.Funnel();

                        NPCManager.npc.FollowPathOrder = true;
                    }
                }
            }

            Engine.Singleton.Root.Dispose();

        }

        static void triggerVolume_OnCharacterEntered(TriggerVolume sender, Character character)
        {
			Engine.Singleton.CurrentLevel.DeleteLevel();
			Engine.Singleton.CurrentLevel.LoadLevel("Karczmalvl2", "KarczmaNav");
			Engine.Singleton.Load();
        }

        static void triggerVolume_Teleport(TriggerVolume sender, Character character)
        {
            character.Position = new Vector3(-10, 10, -10);
        }
    }
}
