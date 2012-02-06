using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace Gra
{
    public class Quests
    {
        static public Dictionary<String, Quest> Q;

        public Quests()
        {
            Q = new Dictionary<String, Quest>();

            if (File.Exists("Media\\Others\\Quests.xml"))
            {
                XmlDocument File1 = new XmlDocument();
                File1.Load("Media\\Others\\Quests.xml");
                XmlElement root = File1.DocumentElement;
                XmlNodeList Items = root.SelectNodes("//Quests//Quest");

                foreach (XmlNode item in Items)
                {
                    Quest justQuest = new Quest();
                    justQuest.Name = item["Name"].InnerText;
                    justQuest.QuestPrize = PrizeManager.P[item["PrizeID"].InnerText];

                    XmlNodeList Enemies = item["Enemies"].ChildNodes;

                    foreach(XmlNode e in Enemies)
                    {
                        justQuest.KillEnemies.Add(e["EnemyID"].InnerText, int.Parse(e["EnemyAmount"].InnerText));
                    }

                    XmlNodeList Itemy = item["Items"].ChildNodes;

                    foreach (XmlNode i in Itemy)
                    {
                        justQuest.BringItems.Add(i["ItemID"].InnerText, int.Parse(i["ItemAmount"].InnerText));
                    }

                    Q.Add(item["QuestID"].InnerText, justQuest);
                }
            }
        }
    }
}
