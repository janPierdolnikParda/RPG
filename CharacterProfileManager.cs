using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using Mogre;

namespace Gra
{
    public class CharacterProfileManager
    {
        static public CharacterProfile character;
        static public Dictionary<String, CharacterProfile> C;
        static public Dictionary<String, CharacterProfile> E;

        public CharacterProfileManager()
        {
            C = new Dictionary<String, CharacterProfile>();
            E = new Dictionary<string, CharacterProfile>();

            character = new CharacterProfile();
            character.BodyMass = 70;
            character.BodyScaleFactor = new Vector3(1.5f, 1, 1.5f);
            character.HeadOffset = new Vector3(0, 0.8f, 0);
            character.MeshName = "Man.mesh";
            character.WalkSpeed = 1.85f;
            character.PictureMaterial = "AdamMaterial";

            if (System.IO.File.Exists("Media\\Profiles\\Characters.xml"))
            {
                XmlDocument File = new XmlDocument();
                File.Load("Media\\Profiles\\Characters.xml");

                XmlElement root = File.DocumentElement;
                XmlNodeList Items = root.SelectNodes("//npcs//npc");

                foreach (XmlNode item in Items)
                {

                    CharacterProfile Kriper = new CharacterProfile();
                    Kriper.DisplayName = item["DisplayName"].InnerText;
                    Kriper.MeshName = item["MeshName"].InnerText;
                    Kriper.BodyMass = int.Parse(item["BodyMass"].InnerText);
                    Kriper.WalkSpeed = float.Parse(item["WalkSpeed"].InnerText);
                    Kriper.DisplayNameOffset = Vector3.ZERO;
                    Kriper.DisplayNameOffset.x = float.Parse(item["DisplayNameOffset_x"].InnerText);
                    Kriper.DisplayNameOffset.y = float.Parse(item["DisplayNameOffset_y"].InnerText);
                    Kriper.DisplayNameOffset.z = float.Parse(item["DisplayNameOffset_z"].InnerText);
                    Kriper.HeadOffset = Vector3.ZERO;
                    Kriper.HeadOffset.x = float.Parse(item["HeadOffset_x"].InnerText);
                    Kriper.HeadOffset.y = float.Parse(item["HeadOffset_y"].InnerText);
                    Kriper.HeadOffset.z = float.Parse(item["HeadOffset_z"].InnerText);
                    Kriper.ProfileName = item["ProfileName"].InnerText;

                    C.Add(Kriper.ProfileName, Kriper);
                }

            }

            if (System.IO.File.Exists("Media\\Profiles\\Enemies.xml"))
            {
                XmlDocument File = new XmlDocument();
                File.Load("Media\\Profiles\\Enemies.xml");

                XmlElement root = File.DocumentElement;
                XmlNodeList Items = root.SelectNodes("//enemies//enemy");

                foreach (XmlNode item in Items)
                {

                    CharacterProfile Kriper = new CharacterProfile();
                    Kriper.DisplayName = item["DisplayName"].InnerText;
                    Kriper.MeshName = item["MeshName"].InnerText;
                    Kriper.BodyMass = int.Parse(item["BodyMass"].InnerText);
                    Kriper.WalkSpeed = float.Parse(item["WalkSpeed"].InnerText);
                    Kriper.DisplayNameOffset = Vector3.ZERO;
                    Kriper.DisplayNameOffset.x = float.Parse(item["DisplayNameOffset_x"].InnerText);
                    Kriper.DisplayNameOffset.y = float.Parse(item["DisplayNameOffset_y"].InnerText);
                    Kriper.DisplayNameOffset.z = float.Parse(item["DisplayNameOffset_z"].InnerText);
                    Kriper.HeadOffset = Vector3.ZERO;
                    Kriper.HeadOffset.x = float.Parse(item["HeadOffset_x"].InnerText);
                    Kriper.HeadOffset.y = float.Parse(item["HeadOffset_y"].InnerText);
                    Kriper.HeadOffset.z = float.Parse(item["HeadOffset_z"].InnerText);
                    Kriper.ProfileName = item["ProfileName"].InnerText;

                    E.Add(item["ProfileName"].InnerText, Kriper);
                }

            }
        }
    }
}
