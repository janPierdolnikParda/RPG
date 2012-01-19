using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

namespace Gra
{
    public class DescribedProfile
    {
        public String MeshName;
        public String Description;
        public String DisplayName;
        public Vector3 DisplayNameOffset;
        public Vector3 BodyScaleFactor;
        public Single Mass;
        public Boolean IsPickable;
        public String InventoryPictureMaterial;
        public bool IsEquipment;
        public String ProfileName;



        public DescribedProfile Clone()
        {
            return (DescribedProfile)MemberwiseClone();
        }
    }
}
