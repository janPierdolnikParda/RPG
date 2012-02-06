using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gra
{
    public class Quest
    {
        public bool IsFinished = false;

        public enum QuestTypes
        {
            KILL,
            BRING,
            VISIT
        }

        public string Name;

        public Dictionary<String, int> BringItems;
        public Dictionary<String, int> KillEnemies;

        //public QuestTypes QuestType;
        public Prize QuestPrize;

        //int killAmount;
        //Enemy killWhat;

        //public int bringAmount;
        DescribedProfile bringWhat;

        //int visitsAmount;
        //List<Activator> visitWhat;

        public bool isDone;

        public Quest()
        {
            BringItems = new Dictionary<String, int>();
            KillEnemies = new Dictionary<String, int>();
        }

        public void MakeDone()
        {
            int Licznik = 0;

            foreach (String str in BringItems.Keys)
            {
                foreach (DescribedProfile Item in Engine.Singleton.HumanController.Character.Inventory)
                {
                    if (Item.ProfileName == str)
                    {
                        Licznik++;
                    }

                    if (Licznik == BringItems[str])
                        isDone = true;
                }
            }
        }

        public void Update()
        {
            if (isDone == false)
                MakeDone();
            else
                Check();
        }

        public void Check()
        {
            bool Flaga = false;

            int Licznik = 0;

            foreach (String str in BringItems.Keys)
            {
                foreach (DescribedProfile Item in Engine.Singleton.HumanController.Character.Inventory)
                {
                    if (Item.ProfileName == str)
                    {
                        Licznik++;
                    }

                    if (Licznik == BringItems[str])
                        Flaga = true;
                }

                if (!Flaga)
                    isDone = false;
            }
        }

        public void GivePrize()
        {
            Engine.Singleton.HumanController.Character.Profile.Gold += (ulong)QuestPrize.AmountGold;
            //Engine.Singleton.HumanController.Character.Exp += Prize.AmountExp;

            foreach (DescribedProfile I in QuestPrize.ItemsList)
                Engine.Singleton.HumanController.Character.Inventory.Add(I);
        }

        public void RemoveItems()
        {
            int Licznik = 0;

            List<DescribedProfile> Ajtemy = new List<DescribedProfile>();

            foreach (String str in BringItems.Keys)
            {
                foreach (DescribedProfile Item in Engine.Singleton.HumanController.Character.Inventory)
                {
                    if (Licznik == BringItems[str])
                        break;

                    if (Item.ProfileName == str)
                    {
                        Ajtemy.Add(Item);
                        Licznik++;
                    }
                }
            }

            foreach (DescribedProfile Ajtem in Ajtemy)
                Engine.Singleton.HumanController.Character.Inventory.Remove(Ajtem);
        }

    }
}
