using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Mogre;

namespace Gra
{
    class Program
    {
        static void Main(string[] args)
        {
            bool DebugMode = false;

            Engine.Singleton.Initialise();

            Engine.Singleton.SoundManager.BGMPlaylist.Add("Soundtrack1.mp3");
            //Engine.Singleton.SoundManager.PlayBGM();


            Engine.Singleton.CurrentLevel = new Level();
           // Engine.Singleton.CurrentLevel.SetGraphicsMesh("World.mesh");
           // Engine.Singleton.CurrentLevel.SetCollisionMesh("World.mesh");

            Engine.Singleton.CurrentLevel.LoadLevel("World", "NavMesh", true);

            TriggerVolume triggerVolume = new TriggerVolume();
            triggerVolume.BeginShapeBuild();
            triggerVolume.AddBoxPart(Vector3.ZERO, Quaternion.IDENTITY, new Vector3(2, 2, 2));
            triggerVolume.EndShapeBuild();
            Engine.Singleton.ObjectManager.Add(triggerVolume);
            triggerVolume.Position = new Vector3(4, 1, 0);
            triggerVolume.OnCharacterEntered += new TriggerVolume.CharacterEnteredHandler(triggerVolume_OnCharacterEntered);          

            Described skrzynia0 = new Described(Items.skrzyniaProfile);
            skrzynia0.Position = new Vector3(-2.43f, 0, 0.04f);
            skrzynia0.IsContainer = true;
            skrzynia0.Container = new Container(2, true, PrizeManager.Prize1.ItemsList);
            Engine.Singleton.ObjectManager.Add(skrzynia0);

            Described stol0 = new Described(Items.stolProfile);
            stol0.Position = new Vector3(-1.0615f, 0.1268f, 6.7239f);
            Engine.Singleton.ObjectManager.Add(stol0);

            Described krzeslo0 = new Described(Items.krzesloProfile);
            krzeslo0.Position = new Vector3(-3.9273f, -0.1260f, -3.1647f);
            Engine.Singleton.ObjectManager.Add(krzeslo0);

            Described krzeslo1 = new Described(Items.krzesloProfile);
            krzeslo1.Position = new Vector3(-1.3125f, -0.13f, -3.6235f);
            Engine.Singleton.ObjectManager.Add(krzeslo1);

            Described butelka0 = new Described(Items.butelkaProfile);
            butelka0.Position = new Vector3(6.0f, 1.6f, 0.0f);
            Engine.Singleton.ObjectManager.Add(butelka0);

            Described vase0 = new Described(Items.vaseProfile);
            vase0.Position = new Vector3(5.5f, 1.55f, 3.5f);
            Engine.Singleton.ObjectManager.Add(vase0);

            Described vase1 = new Described(Items.vaseProfile);
            vase1.Position = new Vector3(5.5f, 1.55f, -3.5f);
            Engine.Singleton.ObjectManager.Add(vase1);

            Described sword = new Described(Items.swordProfile);
            sword.Position = new Vector3(7.5f, 2.0f, 3.0f);
            Engine.Singleton.ObjectManager.Add(sword);

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
            
            Engine.Singleton.HumanController.Character.Inventory.Add(Items.kufelProfile);
            Engine.Singleton.HumanController.ToggleHud();

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
            Console.WriteLine("Bohater wszedł na teren wyzwalacza");
        }

        static void triggerVolume_Teleport(TriggerVolume sender, Character character)
        {
            character.Position = new Vector3(-10, 10, -10);
        }
    }
}
