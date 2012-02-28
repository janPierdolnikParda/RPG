using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gra
{
    class SubMenu
    {
        public String MenuName;
        public bool Enabled;
        public bool Selected;
        public SubMenu Parent;
        public List<SubMenu> SubMenus;
        public event Action Actions;
        public bool Ending;

        public SubMenu()
        {
            SubMenus = new List<SubMenu>();
            Enabled = true;
            Selected = false;
            Ending = false;
        }

        public void AddSub(SubMenu subMenu)
        {
            SubMenus.Add(subMenu);
        }

        public void AddAction(Action action)
        {
            Actions += action;
        }

        public void CallActions()
        {
            Actions();
        }
    }
}
