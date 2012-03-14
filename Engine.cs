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

        public bool Mysza;
        public MOIS.MouseButtonID Przycisk;

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
            Mouse = (MOIS.Mouse)InputManager.CreateInputObject(MOIS.Type.OISMouse, true);

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
            Random = new Random();
            HumanController = new HumanController();

            TypedInput = new TypedInput();


            SoundManager = new SoundManager();

            Dialog = new Dialog();

            Mysz = new MOIS.MouseState_NativePtr();
			Conversations = new Conversations();
            
            TriggerManager = new TriggerManager();

            Mouse.MouseReleased += new MOIS.MouseListener.MouseReleasedHandler(MouseReleased);
        }

        public bool MouseReleased(MOIS.MouseEvent e, MOIS.MouseButtonID button)
        {
            Mysza = true;
            Przycisk = button;
            return true;
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
                Engine.Singleton.Load(null);
            }
        }

        public void Load(String Slot)
        {
            if (Slot == null)
                Slot = "AutoSave";
            bool WasSaved = System.IO.File.Exists("Saves\\" + Slot +"\\" + Engine.Singleton.CurrentLevel.Name + "\\Saved.xml");

            TriggerManager.RemoveAll();
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

            if (System.IO.File.Exists("Saves\\" + Slot +"\\" + CurrentLevel.Name + "\\Items.xml"))
            {
                XmlDocument File = new XmlDocument();
                File.Load("Saves\\" + Slot +"\\" + CurrentLevel.Name + "\\Items.xml");

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

                        if (newDescribed.IsContainer && WasSaved)
                        {
                            newDescribed.Container.Gold = int.Parse(item["ContainerGold"].InnerText);

                            XmlNodeList No_oN = item["ContainerItems"].ChildNodes;

                            newDescribed.Container.Contains = new List<DescribedProfile>();

                            foreach (XmlNode o_o in No_oN)
                                newDescribed.Container.Contains.Add(Gra.Items.I[o_o["ContainerItem"].InnerText]);

                        }

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

            if (System.IO.File.Exists("Saves\\" + Slot +"\\" + CurrentLevel.Name + "\\NPCs.xml"))
            {
                XmlDocument File = new XmlDocument();
                File.Load("Saves\\" + Slot +"\\" + CurrentLevel.Name + "\\NPCs.xml");

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

                    if (WasSaved)
                    {
                        newCharacter.Statistics = new Statistics(
                            int.Parse(item["WalkaWrecz"].InnerText), int.Parse(item["Krzepa"].InnerText),
                            int.Parse(item["Opanowanie"].InnerText), int.Parse(item["Odpornosc"].InnerText),
                            int.Parse(item["Zrecznosc"].InnerText), int.Parse(item["Charyzma"].InnerText),
                            int.Parse(item["Zywotnosc"].InnerText), int.Parse(item["Ataki"].InnerText));
                        newCharacter.Statistics.aktualnaZywotnosc = int.Parse(item["aktualnaZywotnosc"].InnerText);
                        newCharacter.State = (Enemy.StateTypes)int.Parse((item["State"].InnerText));
                        newCharacter.Profile.Gold = ulong.Parse(item["Gold"].InnerText);
                        List<DescribedProfile> Inventory = new List<DescribedProfile>();
                        XmlNodeList inv = item["Inventory"].ChildNodes;
                        foreach (XmlNode invItem in inv)
                        {
                            Inventory.Add(Gra.Items.I[invItem["InventoryItem"].InnerText]);
                        }
                        newCharacter.Inventory = Inventory;
                    }

                    Engine.Singleton.ObjectManager.Add(newCharacter);
                }
            }

            //*************************************************************//
            //                                                             //
            //                           ENEMIES                           //
            //                                                             //
            //*************************************************************//

            if (System.IO.File.Exists("Saves\\" + Slot +"\\" + CurrentLevel.Name + "\\Enemies.xml"))
            {
                XmlDocument File = new XmlDocument();
                File.Load("Saves\\" + Slot +"\\" + CurrentLevel.Name + "\\Enemies.xml");

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

                    if (WasSaved)
                    {
                        newCharacter.Statistics = new Statistics(
                            int.Parse(item["WalkaWrecz"].InnerText), int.Parse(item["Krzepa"].InnerText),
                            int.Parse(item["Opanowanie"].InnerText), int.Parse(item["Odpornosc"].InnerText),
                            int.Parse(item["Zrecznosc"].InnerText), int.Parse(item["Charyzma"].InnerText),
                            int.Parse(item["Zywotnosc"].InnerText), int.Parse(item["Ataki"].InnerText));
                        newCharacter.Statistics.aktualnaZywotnosc = int.Parse(item["aktualnaZywotnosc"].InnerText);
                        newCharacter.State = (Enemy.StateTypes)int.Parse((item["State"].InnerText));
                    }

                    Engine.Singleton.ObjectManager.Add(newCharacter);
                }
            }

            //*************************************************************//
            //                                                             //
            //                        TWOJ PROFIL                          //
            //                                                             //
            //*************************************************************//

            if (WasSaved)
            {
                XmlDocument File = new XmlDocument();
                File.Load("Saves\\" + Slot +"\\Profile.xml");

                XmlElement root = File.DocumentElement;
                XmlNode Item = root.SelectSingleNode("//Profile");

                Character ch = HumanController.Character;
                ch.Profile.Gold = ulong.Parse(Item["Gold"].InnerText);
                ch.Profile.Exp = int.Parse(Item["Exp"].InnerText);
                ch.Position = new Vector3(float.Parse(Item["Position_x"].InnerText),
                    float.Parse(Item["Position_y"].InnerText),
                    float.Parse(Item["Position_z"].InnerText));
                ch.Orientation = new Quaternion(float.Parse(Item["Orientation_w"].InnerText),
                    float.Parse(Item["Orientation_x"].InnerText),
                    float.Parse(Item["Orientation_y"].InnerText),
                    float.Parse(Item["Orientation_z"].InnerText));
                ch.Statistics = new Statistics(
                    int.Parse(Item["WalkaWrecz"].InnerText), int.Parse(Item["Krzepa"].InnerText),
                    int.Parse(Item["Opanowanie"].InnerText), int.Parse(Item["Odpornosc"].InnerText),
                    int.Parse(Item["Zrecznosc"].InnerText), int.Parse(Item["Charyzma"].InnerText),
                    int.Parse(Item["Zywotnosc"].InnerText), int.Parse(Item["Ataki"].InnerText));
                ch.Statistics.aktualnaZywotnosc = int.Parse(Item["aktualnaZywotnosc"].InnerText);
                ch.State = (Enemy.StateTypes)int.Parse((Item["State"].InnerText));
                ch.Inventory = new List<DescribedProfile>();

                XmlNodeList invItems = Item["Inventory"].ChildNodes;

                foreach (XmlNode invItem in invItems)
                {
                    ch.Inventory.Add(Gra.Items.I[invItem["ProfileName"].InnerText]);

                    if (bool.Parse(invItem["IsEquipment"].InnerText))
                    {
                        ch.Inventory[ch.Inventory.Count - 1].IsEquipment = true;
                        ch.Sword = ch.Inventory[ch.Inventory.Count - 1] as ItemSword;
                    }
                }
            }

            TriggerManager.Load();
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

        public void CreateNewChar()
        {
            if (System.IO.Directory.Exists("Saves\\AutoSave"))
                System.IO.Directory.Delete("Saves\\AutoSave", true);
            CopyAll(new DirectoryInfo("Media\\Maps"), new DirectoryInfo("Saves\\AutoSave"));
        }

        public void AutoSave(String Slot)
        {
            if (Slot == null)
                Slot = "AutoSave";
            XmlTextWriter Saved = new XmlTextWriter("Saves\\" + Slot +"\\" + CurrentLevel.Name + "\\Saved.xml", (Encoding)null);
            Saved.WriteStartElement("Saved");
            Saved.WriteStartElement("IsSaved");
            Saved.WriteElementString("IsItFkinSaved", "True");
            Saved.WriteEndElement();
            Saved.WriteEndElement();
            Saved.Flush();
            Saved.Close();

            //*************************************************************//
            //                                                             //
            //                            ITEMY                            //
            //                                                             //
            //*************************************************************//

            XmlTextWriter w = new XmlTextWriter("Saves\\" + Slot +"\\" + CurrentLevel.Name + "\\Items.xml", (Encoding)null);

            w.WriteStartElement("items");

            foreach (GameObject GO in Engine.Singleton.ObjectManager.Objects)
            {
                if (GO.GetType().ToString() == "Gra.Described" && (GO as Described).Profile.ProfileName[0] != 's')
                {
                    w.WriteStartElement("item");
                    w.WriteElementString("DescribedProfile", (GO as Described).Profile.ProfileName);
                    w.WriteElementString("ItemSword", "");
                    w.WriteElementString("Position_x", (GO as Described).Position.x.ToString());
                    w.WriteElementString("Position_y", (GO as Described).Position.y.ToString());
                    w.WriteElementString("Position_z", (GO as Described).Position.z.ToString());
                    w.WriteElementString("Orientation_w", (GO as Described).Orientation.w.ToString());
                    w.WriteElementString("Orientation_x", (GO as Described).Orientation.x.ToString());
                    w.WriteElementString("Orientation_y", (GO as Described).Orientation.y.ToString());
                    w.WriteElementString("Orientation_z", (GO as Described).Orientation.z.ToString());
                    w.WriteElementString("Activator", (GO as Described).Activatorr);

                    if ((GO as Described).IsContainer)
                    {
                        w.WriteElementString("ContainerGold", (GO as Described).Container.Gold.ToString());
                        w.WriteStartElement("ContainerItems");

                        foreach (DescribedProfile DP in (GO as Described).Container.Contains)
                        {
                            w.WriteStartElement("BLABLA");
                            w.WriteElementString("ContainerItem", DP.ProfileName);
                            w.WriteEndElement();
                        }

                        w.WriteEndElement();
                    }
                    

                    w.WriteEndElement();
                }

                if (GO.GetType().ToString() == "Gra.Described" && (GO as Described).Profile.ProfileName[0] == 's')
                {
                    w.WriteStartElement("item");
                    w.WriteElementString("ItemSword", (GO as Described).Profile.ProfileName);
                    w.WriteElementString("DescribedProfile", "");
                    w.WriteElementString("Position_x", (GO as Described).Position.x.ToString());
                    w.WriteElementString("Position_y", (GO as Described).Position.y.ToString());
                    w.WriteElementString("Position_z", (GO as Described).Position.z.ToString());
                    w.WriteElementString("Orientation_w", (GO as Described).Orientation.w.ToString());
                    w.WriteElementString("Orientation_x", (GO as Described).Orientation.x.ToString());
                    w.WriteElementString("Orientation_y", (GO as Described).Orientation.y.ToString());
                    w.WriteElementString("Orientation_z", (GO as Described).Orientation.z.ToString());
                    w.WriteElementString("Activator", (GO as Described).Activatorr);
                    w.WriteEndElement();
                }
            }

            w.WriteEndElement();
            w.Flush();
            w.Close();

            //*************************************************************//
            //                                                             //
            //                             NPCS                            //
            //                                                             //
            //*************************************************************//

            XmlTextWriter NPCs = new XmlTextWriter("Saves\\" + Slot +"\\" + CurrentLevel.Name + "\\NPCs.xml", (Encoding)null);

            NPCs.WriteStartElement("npcs");

            foreach (GameObject GO in Engine.Singleton.ObjectManager.Objects)
            {
                if (GO.GetType().ToString() == "Gra.Character" && (GO as Character) != Engine.Singleton.HumanController.Character
                    && (GO as Character).State != Enemy.StateTypes.DEAD)
                {
                    NPCs.WriteStartElement("npc");
                    NPCs.WriteElementString("ProfileName", (GO as Character).Profile.ProfileName);
                    NPCs.WriteElementString("Position_x", (GO as Character).Position.x.ToString());
                    NPCs.WriteElementString("Position_y", (GO as Character).Position.y.ToString());
                    NPCs.WriteElementString("Position_z", (GO as Character).Position.z.ToString());
                    NPCs.WriteElementString("Orientation_w", (GO as Character).Orientation.w.ToString());
                    NPCs.WriteElementString("Orientation_x", (GO as Character).Orientation.x.ToString());
                    NPCs.WriteElementString("Orientation_y", (GO as Character).Orientation.y.ToString());
                    NPCs.WriteElementString("Orientation_z", (GO as Character).Orientation.z.ToString());
                    NPCs.WriteElementString("Gold", (GO as Character).Profile.Gold.ToString());
                    NPCs.WriteStartElement("Inventory");

                    foreach (DescribedProfile invItem in (GO as Character).Inventory)
                    {
                        NPCs.WriteStartElement("BLABLA");
                        NPCs.WriteElementString("InventoryItem", invItem.ProfileName);
                        NPCs.WriteEndElement();
                    }

                    NPCs.WriteEndElement();
                    (GO as Character).Statistics.WriteToFile(NPCs);
                    NPCs.WriteElementString("State", ((int)(GO as Character).State).ToString());
                    NPCs.WriteEndElement();
                }
            }

            NPCs.WriteEndElement();
            NPCs.Flush();
            NPCs.Close();

            //*************************************************************//
            //                                                             //
            //                            ENEMY                            //
            //                                                             //
            //*************************************************************//

            XmlTextWriter Enemies = new XmlTextWriter("Saves\\" + Slot +"\\" + CurrentLevel.Name + "\\Enemies.xml", (Encoding)null);

            Enemies.WriteStartElement("enemies");

            foreach (GameObject GO in Engine.Singleton.ObjectManager.Objects)
            {
                if (GO.GetType().ToString() == "Gra.Enemy" && (GO as Enemy).State != Enemy.StateTypes.DEAD)
                {
                    Enemies.WriteStartElement("enemy");
                    Enemies.WriteElementString("ProfileName", (GO as Enemy).Profile.ProfileName);
                    Enemies.WriteElementString("Position_x", (GO as Enemy).Position.x.ToString());
                    Enemies.WriteElementString("Position_y", (GO as Enemy).Position.y.ToString());
                    Enemies.WriteElementString("Position_z", (GO as Enemy).Position.z.ToString());
                    Enemies.WriteElementString("Orientation_w", (GO as Enemy).Orientation.w.ToString());
                    Enemies.WriteElementString("Orientation_x", (GO as Enemy).Orientation.x.ToString());
                    Enemies.WriteElementString("Orientation_y", (GO as Enemy).Orientation.y.ToString());
                    Enemies.WriteElementString("Orientation_z", (GO as Enemy).Orientation.z.ToString());
                    (GO as Enemy).Statistics.WriteToFile(Enemies);
                    Enemies.WriteElementString("State", ((int)(GO as Enemy).State).ToString());
                    Enemies.WriteEndElement();
                }
            }

            Enemies.WriteEndElement();
            Enemies.Flush();
            Enemies.Close();

            //*************************************************************//
            //                                                             //
            //                         TWOJ PROFIL                         //
            //                                                             //
            //*************************************************************//

            Character ch = HumanController.Character;
            XmlTextWriter Profile = new XmlTextWriter("Saves\\" + Slot +"\\Profile.xml", (Encoding)null);

            Profile.WriteStartElement("Profile");
            Profile.WriteElementString("Exp", ch.Profile.Exp.ToString());
            Profile.WriteElementString("Gold", ch.Profile.Gold.ToString());
            Profile.WriteElementString("Position_x", ch.Position.x.ToString());
            Profile.WriteElementString("Position_y", ch.Position.y.ToString());
            Profile.WriteElementString("Position_z", ch.Position.z.ToString());
            Profile.WriteElementString("Orientation_w", ch.Orientation.w.ToString());
            Profile.WriteElementString("Orientation_x", ch.Orientation.x.ToString());
            Profile.WriteElementString("Orientation_y", ch.Orientation.y.ToString());
            Profile.WriteElementString("Orientation_z", ch.Orientation.z.ToString());
            Profile.WriteStartElement("Inventory");

            foreach (DescribedProfile DP in ch.Inventory)
            {
                Profile.WriteStartElement("BLABLA");
                Profile.WriteElementString("ProfileName", DP.ProfileName);
                Profile.WriteElementString("IsEquipment", DP.IsEquipment.ToString());
                Profile.WriteEndElement();
            }

            Profile.WriteEndElement();

            ch.Statistics.WriteToFile(Profile);
            Profile.WriteElementString("State", ((int)ch.State).ToString());
            Profile.WriteElementString("MapName", CurrentLevel.Name);

            Profile.WriteEndElement();
            Profile.Flush();
            Profile.Close();

        }

        public void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into it’s new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
    }
}
