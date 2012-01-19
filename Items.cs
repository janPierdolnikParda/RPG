using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using MogreNewt;
using System.Xml;

namespace Gra
{
    public class Items
    {
        static public Dictionary<string, DescribedProfile> I;

        public Items()
        {
            I = new Dictionary<string, DescribedProfile>();

            ItemSword swordProfile = new ItemSword();
            swordProfile.BodyScaleFactor = new Vector3(1.0f, 1.0f, 1.0f);
            swordProfile.MeshName = "Sword.mesh";
            swordProfile.DisplayName = "Miecz";
            swordProfile.Description = "Metalowy miecz.";
            swordProfile.InventoryPictureMaterial = "SpriteSword";
            swordProfile.DisplayNameOffset = new Vector3(0.0f, 0.2f, 0.0f);
            swordProfile.Mass = 50;
            swordProfile.IsPickable = true;
            swordProfile.Damage = 1;
            swordProfile.HandleOffset = new Vector3(0, 0.3f, 0);
            swordProfile.ProfileName = "sSword";
            I.Add("sSword", swordProfile);


            XmlDocument File = new XmlDocument();
			File.Load("Media\\Profiles\\Items.xml");

            XmlElement root = File.DocumentElement;
            XmlNodeList Items = root.SelectNodes("//items/item");

            foreach (XmlNode item in Items)
            {
                if (item["type"].InnerText == "DescribedProfile")
                {
                    DescribedProfile Kriper = new DescribedProfile();
                    Kriper.DisplayName = item["name"].InnerText;
                    Kriper.Description = item["description"].InnerText;
                    Kriper.MeshName = item["mesh"].InnerText;
                    Kriper.InventoryPictureMaterial = item["inventory_material"].InnerText;
                    Kriper.Mass = int.Parse(item["mass"].InnerText);
                    Kriper.IsPickable = bool.Parse(item["ispickable"].InnerText);
                    Kriper.IsEquipment = bool.Parse(item["isequipment"].InnerText);
                    Kriper.DisplayNameOffset = Vector3.ZERO;
                    Kriper.DisplayNameOffset.x = float.Parse(item["nameoffsetx"].InnerText);
                    Kriper.DisplayNameOffset.y = float.Parse(item["nameoffsety"].InnerText);
                    Kriper.DisplayNameOffset.z = float.Parse(item["nameoffsetz"].InnerText);
                    Kriper.ProfileName = item["idstring"].InnerText;

                    I.Add(item["idstring"].InnerText, Kriper);
                }
                else if (item["type"].InnerText == "ItemSword")
                {
                    ItemSword Kriper = new ItemSword();
                    Kriper.DisplayName = item["name"].InnerText;
                    Kriper.Description = item["description"].InnerText;
                    Kriper.MeshName = item["mesh"].InnerText;
                    Kriper.InventoryPictureMaterial = item["inventory_material"].InnerText;
                    Kriper.Mass = int.Parse(item["mass"].InnerText);
                    Kriper.IsPickable = bool.Parse(item["ispickable"].InnerText);
                    Kriper.IsEquipment = bool.Parse(item["isequipment"].InnerText);
                    Kriper.DisplayNameOffset = Vector3.ZERO;
                    Kriper.DisplayNameOffset.x = float.Parse(item["nameoffsetx"].InnerText);
                    Kriper.DisplayNameOffset.y = float.Parse(item["nameoffsety"].InnerText);
                    Kriper.DisplayNameOffset.z = float.Parse(item["nameoffsetz"].InnerText);
                    Kriper.ProfileName = item["idstring"].InnerText;

                    I.Add(item["idstring"].InnerText, Kriper);
                }
            }
        }
    }
}
