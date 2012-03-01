using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

namespace Gra
{
    class GameCamera
    {
        public Character Character;
        public float Distance;
        public Degree Angle;

        public Vector3 InterPosition;

        public void Update()
        {
			if (!Engine.Singleton.HumanController.HUDMenu.IsVisible)
			{
				Vector3 offset =
				Character.Node.Orientation * (-Vector3.UNIT_Z +
				  (Vector3.UNIT_Y * (float)System.Math.Tan(Angle.ValueRadians))
				  ).NormalisedCopy * Distance;

				Vector3 head = Character.Node.Position + Character.Profile.HeadOffset;
				Vector3 desiredPosition = head + offset;

				InterPosition += (desiredPosition - InterPosition) * 0.1f;

				PredicateRaycast raycast = new PredicateRaycast((b => !(b.UserData is TriggerVolume)));
				raycast.Go(Engine.Singleton.NewtonWorld, head, InterPosition);
				if (raycast.Contacts.Count != 0)
				{
					raycast.SortContacts();
					Engine.Singleton.Camera.Position = head
					  + (InterPosition - head) * raycast.Contacts[0].Distance
					  + raycast.Contacts[0].Normal * 0.15f;
				}
				else
					Engine.Singleton.Camera.Position = InterPosition;

				Engine.Singleton.Camera.LookAt(head);
			}
			else
			{
				Engine.Singleton.Camera.Position = new Vector3(3.5f, 1.5f, 2.25f);
				Engine.Singleton.Camera.LookAt(new Vector3(0, 0, 0));
			}

            
        }
    }
}
