using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using Mogre;

namespace Gra
{
    class TriggerManager
    {
        public static Dictionary<String, TriggerVolume> T;

        public TriggerManager()
        {
            T = new Dictionary<string, TriggerVolume>();
            Load();
        }

        public void RemoveAll()
        {
            T.Clear();
        }

        public void Load()
        {
            if (File.Exists("Media\\Maps\\" + Engine.Singleton.CurrentLevel.Name + "\\Triggers.xml"))
            {
                XmlDocument Filee = new XmlDocument();
                Filee.Load("Media\\Maps\\" + Engine.Singleton.CurrentLevel.Name + "\\Triggers.xml");

                XmlElement root = Filee.DocumentElement;
                XmlNodeList Items = root.SelectNodes("//Triggers/Trigger");

                foreach (XmlNode item in Items)
                {
                    TriggerVolume newTrigg = new TriggerVolume();
                    newTrigg.ID = item["ID"].InnerText;
                    newTrigg.EnterActivator = item["EnteredActivator"].InnerText;
                    newTrigg.LeftActivator = item["LeftActivator"].InnerText;

                    float Size = float.Parse(item["Size"].InnerText);

                    newTrigg.BeginShapeBuild();
                    newTrigg.AddBoxPart(Vector3.ZERO, Quaternion.IDENTITY, new Vector3(Size, Size, Size));
                    newTrigg.EndShapeBuild();

                    newTrigg.Position = new Vector3(float.Parse(item["Position_x"].InnerText), float.Parse(item["Position_y"].InnerText), float.Parse(item["Position_z"].InnerText));

                    newTrigg.PrzypiszMetody();

                    T.Add(newTrigg.ID, newTrigg);
                    Engine.Singleton.ObjectManager.Add(newTrigg);
                }
            }
        }
    }
}
