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
        public int Gold;
        public bool CanAdd;


        public Container()
        {
            Contains = new List<DescribedProfile>();
            MaxItems = 1000;
            CanAdd = true;
            Gold = 0;
        }

        public Container(int maxItems, bool canAdd, int gold, List<DescribedProfile> lista)
        {
            Contains = new List<DescribedProfile>();
            Contains = lista;
            MaxItems = maxItems;
            CanAdd = canAdd;
            Gold = gold;
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
