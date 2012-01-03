using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gra
{
    public class QuestManager
    {
        public List<Quest> Quests;

        public QuestManager()
        {
            Quests = new List<Quest>();
        }

        public void Add(Quest aQuest)
        {
            Quests.Add(aQuest);
        }

        public void Update()
        {
            for (int i = Quests.Count - 1; i >= 0; i--)
                Quests[i].Update();
        }

        public void Destroy(Quest aQuest)
        {
            Quests.Remove(aQuest);
        }

        public void MakeFinished(Quest quest)
        {
            Quests[Quests.IndexOf(quest)].IsFinished = true;
            Quests[Quests.IndexOf(quest)].GivePrize();
            Quests[Quests.IndexOf(quest)].RemoveItems();
        }
    }
}
