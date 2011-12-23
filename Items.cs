using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using MogreNewt;

namespace Gra
{
    public class Items
    {
        static public DescribedProfile butelkaProfile;
        static public ItemSword swordProfile;
        static public DescribedProfile vaseProfile;
        static public DescribedProfile stolProfile;
        static public DescribedProfile krzesloProfile;
        static public DescribedProfile skrzyniaProfile;
        static public DescribedProfile kufelProfile;

        public Items()
        {
            butelkaProfile = new DescribedProfile();
            butelkaProfile.BodyScaleFactor = new Vector3(1, 1, 1);
            butelkaProfile.MeshName = "Butelka.mesh";
            butelkaProfile.DisplayName = "Butelka";
            butelkaProfile.Description = "Zielona, szklana butelka.";
            butelkaProfile.DisplayNameOffset = new Vector3(0.0f, 0.7f, 0.0f);
            butelkaProfile.IsPickable = true;
            butelkaProfile.Mass = 100;
            butelkaProfile.InventoryPictureMaterial = "SpriteButelka";

            swordProfile = new ItemSword();
            swordProfile.BodyScaleFactor = new Vector3(1.0f, 1.0f, 1.0f);
            swordProfile.MeshName = "Sword.mesh";
            swordProfile.DisplayName = "Miecz";
            swordProfile.Description ="Metalowy miecz.";
            swordProfile.InventoryPictureMaterial = "SpriteSword";
            swordProfile.DisplayNameOffset = new Vector3(0.0f, 0.2f, 0.0f);
            swordProfile.Mass = 50;
            swordProfile.IsPickable = true;
            swordProfile.Damage = 1;
            swordProfile.HandleOffset = new Vector3(0, 0.3f, 0);

            vaseProfile = new DescribedProfile();
            vaseProfile.BodyScaleFactor = new Vector3(0.7f, 1, 0.7f);
            vaseProfile.MeshName = "Vase.mesh";
            vaseProfile.DisplayName = "Waza";
            vaseProfile.Description = "Waza wykonana z gliny.";
            vaseProfile.DisplayNameOffset = new Vector3(0.0f, 0.7f, 0.0f);
            vaseProfile.IsPickable = true;
            vaseProfile.InventoryPictureMaterial = "VaseInventoryMaterial";

            stolProfile = new DescribedProfile();
            stolProfile.BodyScaleFactor = new Vector3(1, 1, 1);
            stolProfile.MeshName = "Stolik.mesh";
            stolProfile.DisplayName = "Stol";
            stolProfile.Description = "Drewniany stol";
            stolProfile.DisplayNameOffset = new Vector3(0.0f, 0.7f, 0.0f);
            stolProfile.IsPickable = false;
            stolProfile.Mass = 100;

            krzesloProfile = new DescribedProfile();
            krzesloProfile.BodyScaleFactor = new Vector3(1, 1, 1);
            krzesloProfile.MeshName = "Krzeslo.mesh";
            krzesloProfile.DisplayName = "Krzeslo";
            krzesloProfile.Description = "Drewniane krzeslo";
            krzesloProfile.DisplayNameOffset = new Vector3(0.0f, 0.7f, 0.0f);
            krzesloProfile.IsPickable = false;
            krzesloProfile.Mass = 100;
            krzesloProfile.InventoryPictureMaterial = "SpriteButelka";

            skrzyniaProfile = new DescribedProfile();
            skrzyniaProfile.BodyScaleFactor = new Vector3(1, 1, 1);
            skrzyniaProfile.MeshName = "Skrzynia.mesh";
            skrzyniaProfile.DisplayName = "Drewniana skrzynia";
            skrzyniaProfile.Description = "Drewniana skrzynia! LOL!";
            skrzyniaProfile.DisplayNameOffset = new Vector3(0, 1, 0);
            skrzyniaProfile.IsPickable = false;
            skrzyniaProfile.Mass = 0;

            kufelProfile = new DescribedProfile();
            kufelProfile.BodyScaleFactor = new Vector3(1, 1, 1);
            kufelProfile.MeshName = "Kufel.mesh";
            kufelProfile.DisplayName = "Drewniany kufel";
            kufelProfile.Description = "Pusty drewniany karczemny kufel.";
            kufelProfile.DisplayNameOffset = new Vector3(0, 0.4f, 0);
            kufelProfile.IsPickable = true;
            kufelProfile.Mass = 20;
            kufelProfile.InventoryPictureMaterial = "SpriteKufel";

        }
    }
}
