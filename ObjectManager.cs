using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gra
{
    class ObjectManager
    {
        public List<GameObject> Objects;

        public ObjectManager()
        {
            Objects = new List<GameObject>();
        }

        public void Add(GameObject gameObject)
        {
            Objects.Add(gameObject);
        }

        public void Update()
        {
			for (int i = Objects.Count - 1; i >= 0; i--)
			{
				Objects[i].Update();
			}
			
        }

        public void Destroy(GameObject gameObject)
        {
            Objects.Remove(gameObject);
            gameObject.Destroy();
        }
    }
}
