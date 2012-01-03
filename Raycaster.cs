using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using MogreNewt;

namespace Gra
{
    public class PredicateRaycast : Raycast
    {
      public class ContactInfo
      {
        public float Distance;
        public Body Body;
        public Vector3 Normal;
      }

          public Predicate<Body> Predicate;
          public List<ContactInfo> Contacts;
 
          public PredicateRaycast(Predicate<Body> pred)
          {
            Predicate = pred;
            Contacts = new List<ContactInfo>();
          }
 
          public override bool UserPreFilterCallback(Body body)
          {
            return Predicate(body);
          }
          public void SortContacts()
          {
              Contacts.Sort((a, b) => a.Distance.CompareTo(b.Distance));
          }

          public override bool UserCallback(Body body, float distance, Vector3 normal, int collisionID)
          {
              ContactInfo contact = new ContactInfo();
              contact.Distance = distance;
              contact.Body = body;
              contact.Normal = normal;
              Contacts.Add(contact);
              return true;
          }
    }
}
