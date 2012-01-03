using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gra
{
    public class Container
    {

        public List<DescribedProfile> Contains;
        public int MaxItems;
        public bool CanAdd;


        public Container()
        {
            Contains = new List<DescribedProfile>();
            MaxItems = 1000;
            CanAdd = true;
        }

        public Container(int maxItems, bool canAdd, List<DescribedProfile> lista)
        {
            Contains = new List<DescribedProfile>();
            Contains = lista;
            MaxItems = maxItems;
            CanAdd = canAdd;

        }

        public bool Add(DescribedProfile item)
        {
            if (CanAdd)
            {
                if (Contains.Count < MaxItems)
                {
                    Contains.Add(item);
                    return true;
                }

                return false;
            }
            else
                return false;


        }

        public void Delete(int id)
        {
            Contains.RemoveAt(id);

        }

    }
}
