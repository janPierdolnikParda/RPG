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
		SimpleQuad Crosshair;

        bool _isVisible;

        public HUD()
        {

            CompassBg = Engine.Singleton.Labeler.NewSimpleQuad("QuadMaterial", 0.1f, 0.1f, 0.2f, 0.1f, new ColourValue(1, 1, 1), 1);
            CompassLabel = Engine.Singleton.Labeler.NewTextLabel("Primitive", 0.05f, new ColourValue(0.7f, 0.4f, 0), new ColourValue(1, 1.0f, 0.6f), 2);
            CompassLabel.SetPosition(0.11f, 0.13f);

			Crosshair = Engine.Singleton.Labeler.NewSimpleQuad("CrosshairMat", Engine.Singleton.GetFloatFromPxWidth(((int)Engine.Singleton.Root.AutoCreatedWindow.Width / 2) - 10), Engine.Singleton.GetFloatFromPxHeight(((int)Engine.Singleton.Root.AutoCreatedWindow.Height / 2) - 10), Engine.Singleton.GetFloatFromPxWidth(20), Engine.Singleton.GetFloatFromPxHeight(20), new ColourValue(1, 1, 1), 2);

			IsVisible = false;
        }

        public Character Character
        {
            get
            {
                return Engine.Singleton.HumanController.Character;
            }
        }

		public Radian getRotationY()
		{
			Matrix3 orientMatrix;
			orientMatrix = Character.Orientation.ToRotationMatrix();

			Radian yRad, pRad, rRad;
			orientMatrix.ToEulerAnglesYXZ(out yRad, out pRad, out rRad);

			return yRad;
		}

        public void UpdateView()
        {
			CompassLabel.Caption = getRotationY().ValueDegrees.ToString(); 
        }

        public bool IsVisible
        {
            set
            {

                CompassBg.IsVisible = value;
                CompassLabel.IsVisible = value;
				Crosshair.IsVisible = value;

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
