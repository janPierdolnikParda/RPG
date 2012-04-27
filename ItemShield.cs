using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

namespace Gra
{
	public class ItemShield : DescribedProfile
	{
		public bool InUse;
		public float Blok;
		public Vector3 HandleOffset;

		public new ItemShield Clone()
		{
			return (ItemShield)MemberwiseClone();
		}

		public ItemShield(DescribedProfile prof)
		{
			MeshName = prof.MeshName;
		}
	}
}
