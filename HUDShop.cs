using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

namespace Gra
{
	class HUDShop
	{
		public class Slot
		{
			public const float Size = 0.11f;
			public static float Width;
			public SimpleQuad BgQuad;
			public SimpleQuad BlueQuad;
            public SimpleQuad ItemPicture;
            public bool isSelected;

			public Slot(float left, float top)
			{
				BgQuad = Engine.Singleton.Labeler.NewSimpleQuad("QuadMaterial", left, top, Width, Size, new ColourValue(1, 1, 1), 1);
				BlueQuad = Engine.Singleton.Labeler.NewSimpleQuad("HighlightBlueMaterial", left, top, Width, Size, new ColourValue(1, 1, 1), 3);
                ItemPicture = Engine.Singleton.Labeler.NewSimpleQuad("QuadMaterial", left, top, Width, Size, new ColourValue(1, 1, 1), 2);

				BlueQuad.IsVisible = false;
                ItemPicture.IsVisible = false;
                isSelected = false;
			}

			public bool IsVisible
			{
				set
				{
					BgQuad.IsVisible = value;
					ItemPicture.IsVisible = value;
					BlueQuad.IsVisible = value;
				}
			}

			public void SetItem(DescribedProfile item)
			{
				if (item != null)
				{
					BlueQuad.IsVisible = item.IsEquipment;
                    ItemPicture.Panel.MaterialName = item.InventoryPictureMaterial;
				}
				else
				{
					BlueQuad.IsVisible = false;
                    ItemPicture.Panel.MaterialName = "QuadMaterial";
				}
			}
		}

		public Shop Shop = new Shop();

		public const int SlotsCount = 7;
		const float SlotsSpacing = 0.02f;

		public Slot[] Slots;
		public Slot[] Slots2;

		public int AktywnaStrona;
        public int SelectedOne;
        public int SelectedOneB4;
        public int KtoraStrona;

		SimpleQuad DescriptionBg;
		SimpleQuad SelectedPicture;
		TextLabel DescriptionLabel;

        SimpleQuad YourGoldBg;
        TextLabel YourGoldLabel;
        SimpleQuad NPCGoldBg;
        TextLabel NPCGoldLabel;

        SimpleQuad ShopNameBg;
        TextLabel ShopNameLabel;

		//SimpleQuad CompassBg;
		//TextLabel CompassLabel;

		SimpleQuad InventoryBg;

		public SimpleQuad MouseCursor;

		bool _isVisible;

		public HUDShop()
		{
			AktywnaStrona = 0;
			Slot.Width = Slot.Size / Engine.Singleton.Camera.AspectRatio;
			Slots = new Slot[SlotsCount];
			Slots2 = new Slot[SlotsCount];
			for (int i = 0; i < SlotsCount; i++)
			{
				Slots[i] = new Slot(SlotsSpacing, SlotsSpacing + i * (Slot.Size + SlotsSpacing));
				Slots2[i] = new Slot(SlotsSpacing + 0.87f, SlotsSpacing + i * (Slot.Size + SlotsSpacing));
			}

			DescriptionBg = Engine.Singleton.Labeler.NewSimpleQuad("QuadMaterial", 0.2f, 0.5f, 0.6f, 0.45f, ColourValue.White, 1);
			SelectedPicture = Engine.Singleton.Labeler.NewSimpleQuad("QuadMaterial",
				0.21f,
				0.58f,
				0.3f / Engine.Singleton.Camera.AspectRatio,
				0.3f, ColourValue.White, 2);
			DescriptionLabel = Engine.Singleton.Labeler.NewTextLabel("Primitive", 0.03f, new ColourValue(0.7f, 0.4f, 0), new ColourValue(1, 1.0f, 0.6f), 2);
			DescriptionLabel.SetPosition(0.45f, 0.51f);

            YourGoldBg = Engine.Singleton.Labeler.NewSimpleQuad("QuadMaterial", 0.2f, 0.2f, 0.2f, 0.08f, ColourValue.White, 1);
            YourGoldLabel = Engine.Singleton.Labeler.NewTextLabel("Primitive", 0.03f, new ColourValue(0.7f, 0.4f, 0), new ColourValue(1, 1.0f, 0.6f), 2);
            YourGoldLabel.SetPosition(0.2f, 0.23f);

            NPCGoldBg = Engine.Singleton.Labeler.NewSimpleQuad("QuadMaterial", 0.6f, 0.2f, 0.2f, 0.08f, ColourValue.White, 1);
            NPCGoldLabel = Engine.Singleton.Labeler.NewTextLabel("Primitive", 0.03f, new ColourValue(0.7f, 0.4f, 0), new ColourValue(1, 1.0f, 0.6f), 2);
            NPCGoldLabel.SetPosition(0.6f, 0.23f);

            ShopNameBg = Engine.Singleton.Labeler.NewSimpleQuad("QuadMaterial", 0.25f, 0.04f, 0.5f, 0.1f, ColourValue.White, 1);
            ShopNameLabel = Engine.Singleton.Labeler.NewTextLabel("Primitive", 0.08f, new ColourValue(0.7f, 0.4f, 0), new ColourValue(1, 1.0f, 0.6f), 2);
            ShopNameLabel.SetPosition(0.25f, 0.06f);

			//CompassBg = Engine.Singleton.Labeler.NewSimpleQuad("QuadMaterial", 0.1f, 0.1f, 0.2f, 0.1f, new ColourValue(1, 1, 1), 1);
			//CompassLabel = Engine.Singleton.Labeler.NewTextLabel("Primitive", 0.05f, new ColourValue(0.7f, 0.4f, 0), new ColourValue(1, 1.0f, 0.6f), 2);
			//CompassLabel.SetPosition(0.11f, 0.13f);

			InventoryBg = Engine.Singleton.Labeler.NewSimpleQuad("InventoryBgMaterial", 0.01f, 0.01f, 0.98f, 0.98f, new ColourValue(1, 1, 1), 0);
            MouseCursor = Engine.Singleton.Labeler.NewSimpleQuad("Kursor", 0.0f, 0.0f, Engine.Singleton.GetFloatFromPxWidth(32), Engine.Singleton.GetFloatFromPxHeight(32), new ColourValue(1, 1, 1), 4);
			IsVisible = false;

            KtoraStrona = 0;
            SelectedOne = -1;

			//DescriptionLabel.Caption = screenx.ToString();

		}

        public void UnselectAll()
        {
            foreach (Slot S in Slots)
                if (S.isSelected)
                    S.isSelected = false;
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
			if (AktywnaStrona == 0)
			{
                for (int i = KtoraStrona * SlotsCount; i < KtoraStrona * SlotsCount + SlotsCount; i++)
                {
                    if (i < Character.Inventory.Count)
                        Slots[i - KtoraStrona * SlotsCount].SetItem(Character.Inventory.ElementAt(i));
                    else
                        Slots[i - KtoraStrona * SlotsCount].SetItem(null);
                }

			}

			if (AktywnaStrona == 1)
			{
                for (int i = KtoraStrona * SlotsCount; i < KtoraStrona * SlotsCount + SlotsCount; i++)
					if (i < Shop.Items.Count)
                        Slots2[i - KtoraStrona * SlotsCount].SetItem(Shop.Items.ElementAt(i));
					else
                        Slots2[i - KtoraStrona * SlotsCount].SetItem(null);
			}

            YourGoldLabel.Caption = "Zloto: " + Character.Profile.Gold.ToString();
            NPCGoldLabel.Caption = "Zloto: " + Shop.Gold.ToString();
            ShopNameLabel.Caption = "Sklep " + Shop.ShopName;
            MouseCursor.SetDimensions(Engine.Singleton.GetFloatFromPxWidth(Engine.Singleton.Mouse.MouseState.X.abs), Engine.Singleton.GetFloatFromPxHeight(Engine.Singleton.Mouse.MouseState.Y.abs), Engine.Singleton.GetFloatFromPxWidth(32), Engine.Singleton.GetFloatFromPxHeight(32));
		}

		public void UpdateDescription()
		{
			if (AktywnaStrona == 0)
			{
                if (SelectedOne != -1 && SelectedOne < Character.Inventory.Count)
                {
                    float CenaSprzedazy = Character.Inventory[SelectedOne].Price * 0.5f;
                    CenaSprzedazy += CenaSprzedazy * (Character.Statistics.Charyzma * 0.25f) / 100.0f;
                    DescriptionLabel.Caption =
                        Character.Inventory[SelectedOne].DisplayName
                        + "\n\n"
                        + Character.Inventory[SelectedOne].Description
                        + "\nCena sprzedazy: "
                        + ((int)CenaSprzedazy).ToString();

                    if (Character.Inventory[SelectedOne] is ItemSword)
                        DescriptionLabel.Caption += "\nObrazenia: "
                            + (Character.Inventory[SelectedOne] as ItemSword).IloscRzutow.ToString() + "k"
                            + (Character.Inventory[SelectedOne] as ItemSword).JakoscRzutow.ToString();

                    

                    if (Character.Inventory[SelectedOne].InventoryPictureMaterial != null && Character.Inventory[SelectedOne].InventoryPictureMaterial != "-")
                        SelectedPicture.Panel.MaterialName = Character.Inventory[SelectedOne].InventoryPictureMaterial;
                    else
                        SelectedPicture.Panel.MaterialName = "QuadMaterial";
                }
                else
                {
                    DescriptionLabel.Caption = "";
                    SelectedPicture.Panel.MaterialName = "QuadMaterial";
                }
			}

			if (AktywnaStrona == 1)
			{
                if (SelectedOne != -1 && SelectedOne < Shop.Items.Count)
                {
                    float CenaKupna = Shop.Items[SelectedOne].Price * Shop.Mnoznik;
                    CenaKupna -= CenaKupna * (Character.Statistics.Charyzma * 0.25f) / 100.0f;
                    DescriptionLabel.Caption =
                        Shop.Items[SelectedOne].DisplayName
                        + "\n\n"
                        + Shop.Items[SelectedOne].Description
                        + "\n Cena: "
                        + ((int)CenaKupna).ToString();
                        

                    if (Shop.Items[SelectedOne] is ItemSword)
                        DescriptionLabel.Caption += "\nObrazenia: "
                        + (Character.Inventory[SelectedOne] as ItemSword).IloscRzutow.ToString() + "k"
                        + (Character.Inventory[SelectedOne] as ItemSword).JakoscRzutow.ToString();

                    DescriptionLabel.Caption += "\nMasa: " + Shop.Items[SelectedOne].Mass;

                    if (Shop.Items[SelectedOne].InventoryPictureMaterial != null && Shop.Items[SelectedOne].InventoryPictureMaterial != "-")
                        SelectedPicture.Panel.MaterialName = Shop.Items[SelectedOne].InventoryPictureMaterial;
                    else
                        SelectedPicture.Panel.MaterialName = "QuadMaterial";
                }
                else
                {
                    DescriptionLabel.Caption = "";
                    SelectedPicture.Panel.MaterialName = "QuadMaterial";
                }
			}
		}

		public void UpdateViewAll()
		{
                for (int i = KtoraStrona * SlotsCount; i < KtoraStrona * SlotsCount + SlotsCount; i++)
					if (i < Character.Inventory.Count)
                        Slots[i - KtoraStrona * SlotsCount].SetItem(Character.Inventory.ElementAt(i));
					else
                        Slots[i - KtoraStrona * SlotsCount].SetItem(null);

                for (int i = KtoraStrona * SlotsCount; i < KtoraStrona * SlotsCount + SlotsCount; i++)
					if (i < Shop.Items.Count)
                        Slots2[i - KtoraStrona * SlotsCount].SetItem(Shop.Items.ElementAt(i));
					else
                        Slots2[i - KtoraStrona * SlotsCount].SetItem(null);
		}

		public bool IsVisible
		{
			set
			{
                foreach (var slot in Slots) slot.IsVisible = value;
                foreach (var slot in Slots2) slot.IsVisible = value;
				DescriptionBg.IsVisible = value;
				DescriptionLabel.IsVisible = value;
				SelectedPicture.IsVisible = value;
				InventoryBg.IsVisible = value;
                YourGoldLabel.IsVisible = value;
                YourGoldBg.IsVisible = value;
                NPCGoldLabel.IsVisible = value;
                NPCGoldBg.IsVisible = value;
                ShopNameBg.IsVisible = value;
                ShopNameLabel.IsVisible = value;
				MouseCursor.IsVisible = value;

				if (value)
				{
					UpdateView();
				}
			}
		}


		public void ToggleVisibility()
		{
			if (_isVisible == true)
				IsVisible = false;
			else
				IsVisible = true;
		}

        public bool IsOver(SimpleQuad quad)
        {
            if (Engine.Singleton.GetFloatFromPxWidth(Engine.Singleton.Mouse.MouseState.X.abs) >= quad.Panel.Left && Engine.Singleton.GetFloatFromPxWidth(Engine.Singleton.Mouse.MouseState.X.abs) <= quad.Panel.Left + quad.Panel.Width && Engine.Singleton.GetFloatFromPxHeight(Engine.Singleton.Mouse.MouseState.Y.abs) >= quad.Panel.Top && Engine.Singleton.GetFloatFromPxHeight(Engine.Singleton.Mouse.MouseState.Y.abs) <= quad.Panel.Top + quad.Panel.Height)
                return true;
            return false;
        }
	}
}
