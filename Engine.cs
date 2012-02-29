using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using MogreNewt;
using System.IO;
using System.Xml;

namespace Gra
{
    sealed class Engine
    {
        public Root Root;
        public RenderWindow RenderWindow;
        public SceneManager SceneManager;
        public Camera Camera;
        public Viewport Viewport;
        public MOIS.Keyboard Keyboard;
        public MOIS.Mouse Mouse;
        public MOIS.InputManager InputManager;
        public World NewtonWorld;
        public Debugger NewtonDebugger;

        public Level CurrentLevel;
        public GameCamera GameCamera;
        public ObjectManager ObjectManager;
        int BodyId;

        public const float FixedFPS = 60.0f;
        public const float FixedTimeStep = 1.0f / FixedFPS;
        float TimeAccumulator;
        public long LastTime;
        public float TimeStep;

        public MaterialManager MaterialManager;
        public TextLabeler Labeler;

        public HumanController HumanController;
        public TypedInput TypedInput;

        CharacterProfileManager CharacterProfileManager;
        public NPCManager NPCManager;
        public Items Items;
        public PrizeManager PrizeManager;
        public Quests Quests;
        public TriggerManager TriggerManager;

        public SoundManager SoundManager;

        public Dialog Dialog;

        public MOIS.MouseState_NativePtr Mysz;
		public Conversations Conversations;

        public Random Random;

        public SubMenu Menu;

        public bool GameEnder = false;

        public void Initialise()
        {
            Root = new Root();
            ConfigFile cf = new ConfigFile();
            cf.Load("Resources.cfg", "\t:=", true);

            ConfigFile.SectionIterator seci = cf.GetSectionIterator();

            while (seci.MoveNext())
            {
                ConfigFile.SettingsMultiMap settings = seci.Current;
                foreach (KeyValuePair<string, string> pair in settings)
                    ResourceGroupManager.Singleton.AddResourceLocation(pair.Value, pair.Key, seci.CurrentKey);
            }

            if (!Root.RestoreConfig())
                if (!Root.ShowConfigDialog())
                    return;

			Root.RenderSystem.SetConfigOption("VSync", "Yes");
            RenderWindow = Root.Initialise(true, "Kolejny epicki erpeg");  // @@@@@@@@@@@@@@@ Nazwa okna gry.
			
			ResourceGroupManager.Singleton.InitialiseAllResourceGroups();
            
            SceneManager = Root.CreateSceneManager(SceneType.ST_GENERIC);
            Camera = SceneManager.CreateCamera("MainCamera");
            Viewport = RenderWindow.AddViewport(Camera);
            Camera.NearClipDistance = 0.1f;
            Camera.FarClipDistance = 1000.0f;
            Camera.AspectRatio = ((float)RenderWindow.Width / (float)RenderWindow.Height);

            MOIS.ParamList pl = new MOIS.ParamList();
            IntPtr windowHnd;
            RenderWindow.GetCustomAttribute("WINDOW", out windowHnd);
            pl.Insert("WINDOW", windowHnd.ToString());

            InputManager = MOIS.InputManager.CreateInputSystem(pl);

            Keyboard = (MOIS.Keyboard)InputManager.CreateInputObject(MOIS.Type.OISKeyboard, false);
            Mouse = (MOIS.Mouse)InputManager.CreateInputObject(MOIS.Type.OISMouse, false);

			NewtonWorld = new World();
            NewtonDebugger = new Debugger(NewtonWorld);
            NewtonDebugger.Init(SceneManager);

            GameCamera = new GameCamera();
            ObjectManager = new ObjectManager();

            MaterialManager = new MaterialManager();
            MaterialManager.Initialise();

            
            Items = new Items();
            PrizeManager = new PrizeManager();  //////////////////// @@ Brand nju staff. Nawet trochę działa :Δ
            CharacterProfileManager = new CharacterProfileManager();
            Quests = new Quests();
            NPCManager = new NPCManager();
            
            Labeler = new TextLabeler(5);
            HumanController = new HumanController();

            TypedInput = new TypedInput();


            SoundManager = new SoundManager();

            Dialog = new Dialog();

            Mysz = new MOIS.MouseState_NativePtr();
			Conversations = new Conversations();
            Random = new Random();
        }

        public void Update()
        {
            Mysz.height = (int)Root.AutoCreatedWindow.Height;
            Mysz.width = (int)Root.AutoCreatedWindow.Width;
            long currentTime = Root.Timer.Milliseconds;
            TimeStep = (currentTime - LastTime) / 1000.0f;
            LastTime = currentTime;
            TimeAccumulator += TimeStep;
            TimeAccumulator = System.Math.Min(TimeAccumulator, FixedTimeStep * (FixedFPS / 15));

            Keyboard.Capture();
            Mouse.Capture();
            Root.RenderOneFrame();
            Labeler.Update();

            while (TimeAccumulator >= FixedTimeStep)
            {
                TypedInput.Update();
                
                NewtonWorld.Update(FixedTimeStep);
                HumanController.Update();
                ObjectManager.Update();
                GameCamera.Update();
                TimeAccumulator -= FixedTimeStep;

                        //// mjuzik status i ogarnięcie żeby przełączało na następną piosenkę z plejlisty po zakończeniu poprzedniej


            }
            WindowEventUtilities.MessagePump();

            if (CurrentLevel.LoadNewMap)
            {
                HumanController.Character.Contact = null;
                CurrentLevel.DeleteLevel();
                CurrentLevel.LoadLevel(CurrentLevel.NewMapName, CurrentLevel.NewMapNav, false);
                CurrentLevel.LoadNewMap = false;
                CurrentLevel.NewMapName = "";
                CurrentLevel.NewMapNav = "";
                Engine.Singleton.Load();
            }
        }

        public void Load()
        {
            while (Engine.Singleton.ObjectManager.Objects.Count > 0)
            {
                int q = 0;

                if (Engine.Singleton.ObjectManager.Objects[0] is Character && (Engine.Singleton.ObjectManager.Objects[0] as Character) == Engine.Singleton.HumanController.Character)
                {
                    q = 1;

                    if (Engine.Singleton.ObjectManager.Objects.Count == 1)
                        break;
                }

               
                Engine.Singleton.ObjectManager.Destroy(Engine.Singleton.ObjectManager.Objects[q]);
            }

            //*************************************************************//
            //                                                             //
            //                            ITEMY                            //
            //                                                             //
            //*************************************************************//

            if (System.IO.File.Exists("Media\\Maps\\" + CurrentLevel.Name + "\\Items.xml"))
            {
                XmlDocument File = new XmlDocument();
                File.Load("Media\\Maps\\" + CurrentLevel.Name + "\\Items.xml");

                XmlElement root = File.DocumentElement;
                XmlNodeList Items = root.SelectNodes("//items/item");

                foreach (XmlNode item in Items)
                {
                    if (item["DescribedProfile"].InnerText != "")
                    {
                        Described newDescribed = new Described(Gra.Items.I[item["DescribedProfile"].InnerText]);
                        Vector3 Position = new Vector3();

                        Quaternion Orientation = new Quaternion(float.Parse(item["Orientation_w"].InnerText), float.Parse(item["Orientation_x"].InnerText), float.Parse(item["Orientation_y"].InnerText), float.Parse(item["Orientation_z"].InnerText));
                        newDescribed.Orientation = Orientation;

                        Position.x = float.Parse(item["Position_x"].InnerText);
                        Position.y = float.Parse(item["Position_y"].InnerText);
                        Position.z = float.Parse(item["Position_z"].InnerText);
                        newDescribed.Position = Position;
                        newDescribed.Activatorr = item["Activator"].InnerText;
                        newDescribed.PrzypiszMetode();

                        Engine.Singleton.ObjectManager.Add(newDescribed);
                    }

                    if (item["ItemSword"].InnerText != "")
                    {
                        Described newDescribed = new Described(Gra.Items.I[item["ItemSword"].InnerText]);
                        Vector3 Position = new Vector3();

                        Quaternion Orientation = new Quaternion(float.Parse(item["Orientation_w"].InnerText), float.Parse(item["Orientation_x"].InnerText), float.Parse(item["Orientation_y"].InnerText), float.Parse(item["Orientation_z"].InnerText));
                        newDescribed.Orientation = Orientation;

                        Position.x = float.Parse(item["Position_x"].InnerText);
                        Position.y = float.Parse(item["Position_y"].InnerText);
                        Position.z = float.Parse(item["Position_z"].InnerText);
                        newDescribed.Position = Position;
                        newDescribed.Activatorr = item["Activator"].InnerText;
                        newDescribed.PrzypiszMetode();

                        Engine.Singleton.ObjectManager.Add(newDescribed);
                    }
                }
            }

            //*************************************************************//
            //                                                             //
            //                            NPCs                             //
            //                                                             //
            //*************************************************************//

            if (System.IO.File.Exists("Media\\Maps\\" + CurrentLevel.Name + "\\NPCs.xml"))
            {
                XmlDocument File = new XmlDocument();
                File.Load("Media\\Maps\\" + CurrentLevel.Name + "\\NPCs.xml");

                XmlElement root = File.DocumentElement;
                XmlNodeList Items = root.SelectNodes("//npcs//npc");

                foreach (XmlNode item in Items)
                {
                    Character newCharacter = new Character(CharacterProfileManager.C[item["ProfileName"].InnerText]);
                    Vector3 Position = new Vector3();

                    Quaternion Orientation = new Quaternion(float.Parse(item["Orientation_w"].InnerText), float.Parse(item["Orientation_x"].InnerText), float.Parse(item["Orientation_y"].InnerText), float.Parse(item["Orientation_z"].InnerText));
                    newCharacter.Orientation = Orientation;

                    Position.x = float.Parse(item["Position_x"].InnerText);
                    Position.y = float.Parse(item["Position_y"].InnerText);
                    Position.z = float.Parse(item["Position_z"].InnerText);
                    newCharacter.Position = Position;

                    Engine.Singleton.ObjectManager.Add(newCharacter);
                }
            }

            //*************************************************************//
            //                                                             //
            //                           ENEMIES                           //
            //                                                             //
            //*************************************************************//

            if (System.IO.File.Exists("Media\\Maps\\" + CurrentLevel.Name + "\\Enemies.xml"))
            {
                XmlDocument File = new XmlDocument();
                File.Load("Media\\Maps\\" + CurrentLevel.Name + "\\Enemies.xml");

                XmlElement root = File.DocumentElement;
                XmlNodeList Items = root.SelectNodes("//enemies//enemy");

                foreach (XmlNode item in Items)
                {
                    Enemy newCharacter = new Enemy(Gra.CharacterProfileManager.E[item["ProfileName"].InnerText], false, Gra.CharacterProfileManager.E[item["ProfileName"].InnerText].ZasiegWzroku, Gra.CharacterProfileManager.E[item["ProfileName"].InnerText].ZasiegOgolny);
                    Vector3 Position = new Vector3();

                    Quaternion Orientation = new Quaternion(float.Parse(item["Orientation_w"].InnerText), float.Parse(item["Orientation_x"].InnerText), float.Parse(item["Orientation_y"].InnerText), float.Parse(item["Orientation_z"].InnerText));
                    newCharacter.Orientation = Orientation;

                    Position.x = float.Parse(item["Position_x"].InnerText);
                    Position.y = float.Parse(item["Position_y"].InnerText);
                    Position.z = float.Parse(item["Position_z"].InnerText);
                    newCharacter.Position = Position;

                    Engine.Singleton.ObjectManager.Add(newCharacter);
                }
            }
        }

        public bool IsKeyTyped(MOIS.KeyCode code)
        {
            return TypedInput.IsKeyTyped[(int)code];
        }

        public int GetUniqueBodyId()
        {
            return BodyId++;
        }

        static Engine instance;

        Engine()
        {
        }

        static Engine()
        {
            instance = new Engine();
        }

        public static Engine Singleton
        {
            get
            {
                return instance;
            }
        }

        public static double Distance(Vector3 v1, Vector3 v2)
        {
            return
            (
               System.Math.Sqrt
               (
                   (v1.x - v2.x) * (v1.x - v2.x) +
                   (v1.y - v2.y) * (v1.y - v2.y) +
                   (v1.z - v2.z) * (v1.z - v2.z)
               )
            );
        }

		public float GetFloatFromPxHeight(int px)
		{
			float ret;

			ret = ((float)px / (float)Root.AutoCreatedWindow.Height);

			return ret;
		}

		public float GetFloatFromPxWidth(int px)
		{
			float ret;

			ret = ((float)px / (float)Root.AutoCreatedWindow.Width);

			return ret;
		}

        public int Kostka(int n, int k)
        {
            int Wynik = 0;

            for (int i = 0; i < n; i++)
                Wynik += Random.Next(1, k+1);

            return Wynik;
        }

        public bool Procenty(int n)
        {
            List<int> TabB4Shuffle = new List<int>();
            List<int> TabAfterShuffle = new List<int>();

            for (int i = 0; i < 100; i++)
            {
                if (n > 0)
                {
                    n--;
                    TabB4Shuffle.Add(1);
                }

                else
                    TabB4Shuffle.Add(0);
            }

            while (TabB4Shuffle.Count > 0)
            {
                int Rand = Random.Next(TabB4Shuffle.Count);
                TabAfterShuffle.Add(TabB4Shuffle[Rand]);
                TabB4Shuffle.RemoveAt(Rand);
            }

            if (TabAfterShuffle[Random.Next(100)] == 0)
                return false;

            else
                return true;
        }
    }
}
