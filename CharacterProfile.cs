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
        // Nazwa pliku siatki postaci
        public String MeshName;
        // Masa ciała potrzebna Newtonowi
        public float BodyMass;
        // Prędkość chodu
        public float WalkSpeed;
        // Współczynnik, przez który skalowana będzie siatka kolizyjna
        public Vector3 BodyScaleFactor;
        // Punkt, w którym znajduje się głowa postaci względem środka jej ciężkości
        public Vector3 HeadOffset;

        public String DisplayName;
        public String Description;
        public Vector3 DisplayNameOffset;
        public String PictureMaterial;

        public ulong Gold;


        public CharacterProfile Clone()
        {
            return (CharacterProfile)MemberwiseClone();
        }

    }
}
