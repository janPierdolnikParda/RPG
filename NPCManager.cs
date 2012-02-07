using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

namespace Gra
{
    public class NPCManager
    {
        static public Character npc;
        static public Enemy enemy0;

        public NPCManager()
        {
			npc = new Character(CharacterProfileManager.C["cAndrzej"]);
			npc.Position = new Vector3(0, 0, 0);
			Engine.Singleton.ObjectManager.Add(npc);

        }
    }
}
