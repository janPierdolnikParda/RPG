using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace Gra
{
    public class PrizeManager
    {
        static public Prize Prize1;

        static public Dictionary<String, Prize> P;

        public PrizeManager()
        {
            P = new Dictionary<String, Prize>();

            if (File.Exists("Media\\Profiles\\Prizes.xml"))
            {
                XmlDocument File1 = new XmlDocument();
                File1.Load("Media\\Profiles\\Prizes.xml");
                XmlElement root = File1.DocumentElement;
                XmlNodeList Items = root.SelectNodes("//prizes//prize");

                foreach (XmlNode item in Items)
                {
                    Prize newChar = new Prize();

                    newChar.PrizeID = item["PrizeID"].InnerText;
                    newChar.AmountExp = int.Parse(item["Exp"].InnerText);
                    newChar.AmountGold = int.Parse(item["Gold"].InnerText);

                    XmlNodeList Itemy = item["items"].ChildNodes;

                    foreach (XmlNode it in Itemy)
                    {
                        newChar.ItemsList.Add((Gra.Items.I[it["idstring"].InnerText]));
                    }

                    P.Add(newChar.PrizeID, newChar);
                }
            }
        }


    }
}
