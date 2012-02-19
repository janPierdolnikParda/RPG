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
                    Kriper.DisplayNameOffset = Vector3.ZERO;
                    Kriper.DisplayNameOffset.x = float.Parse(item["nameoffsetx"].InnerText);
                    Kriper.DisplayNameOffset.y = float.Parse(item["nameoffsety"].InnerText);
                    Kriper.DisplayNameOffset.z = float.Parse(item["nameoffsetz"].InnerText);
                    Kriper.ProfileName = item["idstring"].InnerText;
                    Kriper.Price = int.Parse(item["price"].InnerText);
                    Kriper.IsContainer = bool.Parse(item["iscontainer"].InnerText);
                    Kriper.Activator = item["activator"].InnerText;

                    if (Kriper.IsContainer)
                        Kriper.PrizeID = item["prizeid"].InnerText;

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
                    Kriper.DisplayNameOffset = Vector3.ZERO;
                    Kriper.DisplayNameOffset.x = float.Parse(item["nameoffsetx"].InnerText);
                    Kriper.DisplayNameOffset.y = float.Parse(item["nameoffsety"].InnerText);
                    Kriper.DisplayNameOffset.z = float.Parse(item["nameoffsetz"].InnerText);
                    Kriper.ProfileName = item["idstring"].InnerText;
                    Kriper.Price = int.Parse(item["price"].InnerText);
                    Kriper.Activator = item["activator"].InnerText;

                    I.Add(item["idstring"].InnerText, Kriper);
                }
            }
        }
    }
}
