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
        QuestTypes QuestType;
        Prize QuestPrize;

        int killAmount;
        //Enemy killWhat;

        int bringAmount;
        DescribedProfile bringWhat;

        int visitsAmount;
        //List<Activator> visitWhat;

        public bool isDone;

        public Quest(string newQuestName, QuestTypes Type, int Amount, Prize prize)
        {
            QuestType = Type;
            Name = newQuestName;
            isDone = false;

            if (Type == QuestTypes.KILL)
                killAmount = Amount;

            else if (Type == QuestTypes.BRING)
                bringAmount = Amount;

            else if (Type == QuestTypes.VISIT)
                visitsAmount = Amount;

            QuestPrize = prize;

        }

        public void AddItem(DescribedProfile newItem)
        {
            bringWhat = newItem;
        }

        public void MakeDone()
        {
            if (QuestType == QuestTypes.BRING)
            {
                foreach (DescribedProfile Item in Engine.Singleton.HumanController.Character.Inventory)
                {
                    if (Item.DisplayName == bringWhat.DisplayName)
                    {
                        isDone = true;
                    }
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
            if (QuestType == QuestTypes.BRING)
            {
                foreach (DescribedProfile Item in Engine.Singleton.HumanController.Character.Inventory)
                {
                    if (Item.DisplayName == bringWhat.DisplayName)
                    {
                        //Console.WriteLine(Item.DisplayName);
                        Flaga = true;
                    }
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

    }
}
