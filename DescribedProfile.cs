﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

namespace Gra
{
    public class DescribedProfile
    {
        public string MeshName;
        public string Description;
        public string DisplayName;
        public Vector3 DisplayNameOffset;
        public Vector3 BodyScaleFactor;
        public float Mass;
        public Boolean IsPickable;
        public string InventoryPictureMaterial;
        public bool IsEquipment;
        public bool IsContainer;
        public string PrizeID;
        public string ProfileName;
        public int Price = 0;

        public DescribedProfile Clone()
        {
            return (DescribedProfile)MemberwiseClone();
        }
    }
}
