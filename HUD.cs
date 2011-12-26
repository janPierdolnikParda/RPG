using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

namespace Gra
{
    class HUD
    {
        SimpleQuad CompassBg;
        TextLabel CompassLabel;

        bool _isVisible;

        public HUD()
        {

            CompassBg = Engine.Singleton.Labeler.NewSimpleQuad("QuadMaterial", 0.1f, 0.1f, 0.2f, 0.1f, new ColourValue(1, 1, 1), 1);
            CompassLabel = Engine.Singleton.Labeler.NewTextLabel("Primitive", 0.05f, new ColourValue(0.7f, 0.4f, 0), new ColourValue(1, 1.0f, 0.6f), 2);
            CompassLabel.SetPosition(0.11f, 0.13f);


            IsVisible = false;
        }

        public Character Character
        {
            get
            {
                return Engine.Singleton.HumanController.Character;
            }
        }

        public void UpdateView()
        {
            
            Degree deg;
            Vector3 v;
            Character.Orientation.ToAngleAxis(out deg, out v);
            
            CompassLabel.Caption = deg.ValueAngleUnits.ToString();  
        }

        public bool IsVisible
        {
            set
            {

                CompassBg.IsVisible = value;
                CompassLabel.IsVisible = value;

                if (value)
                {
                    UpdateView();
                }

                _isVisible = value;
            }
        }


        public void ToggleVisibility()
        {
            if (_isVisible == true)
                IsVisible = false;
            else
                IsVisible = true;
        }

    }
}
