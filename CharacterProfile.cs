using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

namespace Gra
{
    public class CharacterProfile
    {
        public String ProfileName;
        public String MeshName;
        public float BodyMass;
        public float WalkSpeed;
        public Vector3 BodyScaleFactor;
        public Vector3 HeadOffset;

        public String DisplayName;
        public String Description;
        public String DialogRoot;
        public Vector3 DisplayNameOffset;
        public String PictureMaterial;

        public ulong Gold;
		public int Exp;
        public Statistics Statistics;
        public float ZasiegWzroku;
        public float ZasiegOgolny;
        public float MnoznikDlaShopa = 1.0f;
        public Character.FriendType FriendlyType;
        public List<DescribedProfile> Inventory;

        public CharacterProfile Clone()
        {
            return (CharacterProfile)MemberwiseClone();
        }

        public CharacterProfile()
        {
            Statistics = new Statistics();
            Inventory = new List<DescribedProfile>();
            Gold = 0;
			Exp = 0;
            ZasiegWzroku = 20;
            ZasiegOgolny = 20;
        }

    }
}
