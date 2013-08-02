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
        public Dictionary<String, int> KilledEnemies;

        public Prize QuestPrize;

        public bool isDone;

        public String questID;

        public Quest()
        {
            BringItems = new Dictionary<String, int>();
            KillEnemies = new Dictionary<String, int>();
            KilledEnemies = new Dictionary<String, int>();
            isDone = false;
        }

        public void MakeDone()
        {
            int Licznik = 0;
            bool GotItems = false;
            foreach (String str in BringItems.Keys)
            {
                foreach (DescribedProfile Item in Engine.Singleton.HumanController.Character.Inventory)
                {
                    if (Item.ProfileName == str)
                    {
                        Licznik++;
                    }

                    if (Licznik == BringItems[str])
                        GotItems = true;
                }
            }

            bool KilledAll = true;

            foreach (String str in KillEnemies.Keys)
            {
                if (KilledEnemies[str] < KillEnemies[str])
                    KilledAll = false;
            }

            if (KilledAll && GotItems)
                isDone = true;
            else
                isDone = false;
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
            Engine.Singleton.HumanController.Character.Profile.Exp += QuestPrize.AmountExp;

            foreach (DescribedProfile I in QuestPrize.ItemsList)
                Engine.Singleton.HumanController.Character.Inventory.Add(I.Clone());
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
            {                
                Engine.Singleton.HumanController.Character.Inventory.Remove(Ajtem);
            }
        }
    }
}
