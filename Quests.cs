using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gra
{
    public class Quests
    {
        static public Quest Quest1;

        public Quests()
        {
            Quest1 = new Quest("Pierwszy", Quest.QuestTypes.BRING, 1, PrizeManager.Prize1);
            Quest1.AddItem(Items.I["iButelka"]);
        }
    }
}
