using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

namespace Gra
{
    class HumanController
    {
        public enum HumanControllerState
        {
            FREE,
            TALK,
            INVENTORY,
            CONTAINER
        }

        public enum HumanTalkState
        {
            CHOOSING_REPLY,
            REPLYING,
            PAUSE,
            LISTENING
        }

		// config
		public bool InvertMouse = false;
		public float MouseSpeed = 1.0f;

        public Character Character;
        SelectableObject FocusObject;
        TextLabel3D TargetLabel;

        SimpleQuad TalkBox;
        List<TextLabel> TalkLabels;

        HumanControllerState State;
        HumanTalkState TalkState;

        List<TalkReply> ValidReplies;
        int SelectedReply = 0;

        float TextRemainingTime = 0.0f;
        int TextIndex;
        TalkReply CurrentReply;
        TalkNode CurrentNode;

        HUDInventory HUDInventory;
        HUDContainer HUDContainer;
        HUD HUD;

        int FocusObjectId = 0;


        public HumanController()
        {
            TargetLabel = Engine.Singleton.Labeler.NewTextLabel3D("Primitive", 0.04f, new ColourValue(0.7f, 0.4f, 0), new ColourValue(1, 1.0f, 0.6f), 0);

            TalkBox = Engine.Singleton.Labeler.NewSimpleQuad("QuadMaterial", 0.05f, 0.85f, 0.9f, 0.1f, new ColourValue(1, 1, 1), 1);
            TalkBox.IsVisible = false;

            TalkLabels = new List<TextLabel>();
            for (int i = 0; i < 8; i++)
            {
                TalkLabels.Add(Engine.Singleton.Labeler.NewTextLabel("Primitive", 0.04f, new ColourValue(0.7f, 0.4f, 0), new ColourValue(1, 1.0f, 0.6f), 2));
                TalkLabels[i].SetPosition(0.07f, 0.63f + i * 0.037f);
            }
            ValidReplies = new List<TalkReply>();

            HUDInventory = new HUDInventory();
            HUDContainer = new HUDContainer();
            HUD = new HUD();
        }


        public void HideTalkOverlay()
        {
            foreach (TextLabel lab in TalkLabels) lab.IsVisible = false;
            TalkBox.IsVisible = false;
        }


        public void ShowReplies()
        {
            TalkBox.IsVisible = true;
            foreach (TextLabel lab in TalkLabels) lab.IsVisible = true;
            TalkBox.SetDimensions(0.05f, 0.6f, 0.9f, 0.35f);
            TalkLabels.Last().SetPosition(0.07f, TalkLabels.Last().TextArea.Top);
        }

        public void ShowTalkBox()
        {
            TalkBox.IsVisible = true;
            foreach (TextLabel lab in TalkLabels) lab.IsVisible = false;
            TalkLabels.Last().IsVisible = true;
            TalkBox.SetDimensions(0.05f, 0.85f, 0.9f, 0.1f);
        }

        public void BeginTextDisplay(TalkText text)
        {
            TalkLabels.Last().Caption = text.Text;
            TalkLabels.Last().SetPosition(0.5f - TalkLabels.Last().GetTextWidth() * 0.5f, 0.889f);
            TextRemainingTime = text.Duration;
        }

        public void SwitchState(HumanControllerState newState)
        {
            if (State == HumanControllerState.FREE)
            {
                if (newState == HumanControllerState.TALK)
                    SwitchTalkState(HumanTalkState.LISTENING);
                if (newState == HumanControllerState.INVENTORY)
                    HUDInventory.IsVisible = true;
                if (newState == HumanControllerState.CONTAINER)
                {

                    HUDContainer.IsVisible = true;
                    if (HUDContainer.Container.Contains.Count > 0)
                    {
                        HUDContainer.ActiveEq = 1;
                        HUDContainer.SelectIndex2 = 0;
                        HUDContainer.SelectIndex1 = -1;
                    }
                    else
                    {
                        HUDContainer.ActiveEq = 0;
                        HUDContainer.SelectIndex1 = 0;
                        HUDContainer.SelectIndex2 = -1;
                    }
                    
                    
                    
                    HUDContainer.UpdateViewAll();
                    
                    
                }
            }
            else if (State == HumanControllerState.TALK)
            {
                if (newState == HumanControllerState.FREE)
                    HideTalkOverlay();
            }
            else if (State == HumanControllerState.CONTAINER)
            {
                if (newState == HumanControllerState.FREE)
                {
                   

                    HUDContainer.IsVisible = false;
                    
                }
            }

            else if (State == HumanControllerState.INVENTORY)
                if (newState == HumanControllerState.FREE)
                    HUDInventory.IsVisible = false;


            State = newState;
        }

        public void SwitchTalkState(HumanTalkState newState)            // zmiana stanu bohatera w trakcie rozmowy
        {
            if (newState == HumanTalkState.CHOOSING_REPLY)
            {
                ShowReplies();
            }
            else if (newState == HumanTalkState.PAUSE)
            {
                HideTalkOverlay();
                TextRemainingTime = 0.5f;
            }
            else if (newState == HumanTalkState.LISTENING || newState == HumanTalkState.REPLYING)
            {
                TextIndex = 0;
                if (newState == HumanTalkState.LISTENING)
                {
                    TalkLabels.Last().SetColor(new ColourValue(0.7f, 0.4f, 0), new ColourValue(1, 1.0f, 0.6f));
                    BeginTextDisplay(CurrentNode.Text[0]);
                }
                else
                {
                    TalkLabels.Last().SetColor(new ColourValue(0.4f, 0.4f, 0.4f), new ColourValue(1.0f, 1.0f, 1.0f));
                    BeginTextDisplay(CurrentReply.Text[0]);
                }

                ShowTalkBox();
            }
            TalkState = newState;
        }

        public void UpdateRepliesColours()                  // podświetlenie aktualnie wybranej odpowiedzi w dialogu
        {
            for (int i = 0; i < ValidReplies.Count; i++)
            {
                if (i == SelectedReply)
                    TalkLabels[i].SetColor(new ColourValue(1, 1, 1), new ColourValue(1, 1, 1));
                else
                    TalkLabels[i].SetColor(new ColourValue(0.7f, 0.4f, 0), new ColourValue(1, 1.0f, 0.6f));
            }
        }

        public void SetRepliesText(TalkNode node)
        {
            SelectedReply = 0;
            ValidReplies.Clear();
            foreach (TextLabel label in TalkLabels)
                label.Caption = "";

            foreach (TalkReply reply in CurrentNode.Replies)
            {
                if (reply.IsConditionFulfilled())
                {
                    if (ValidReplies.Count > TalkLabels.Count)
                        throw new Exception("Zbyt dużo możliwych odpowiedzi");
                    TalkLabels[ValidReplies.Count].Caption = reply.Text[0].Text;
                    ValidReplies.Add(reply);
                }
            }
        }

        public void HandleConversation()                        // @@ funkcja odpowiedzialna za obsługę rozmowy
        {   
            TextRemainingTime -= Engine.FixedTimeStep;

            if (TalkState == HumanTalkState.LISTENING)
            {
                if (TextRemainingTime <= 0)
                {
                    TextIndex++;
                    if (TextIndex == CurrentNode.Text.Count)
                    {
                        if (CurrentNode.Replies.Count == 0)
                        {
                            SwitchState(HumanControllerState.FREE); // zakończenie rozmowy przez npc'a
                        }
                        else
                        {
                            SetRepliesText(CurrentNode);
                            SwitchTalkState(HumanTalkState.CHOOSING_REPLY);
                        }
                    }
                    else
                    {
                        BeginTextDisplay(CurrentNode.Text[TextIndex]);
                    }
                }
            }
            else if (TalkState == HumanTalkState.REPLYING)
            {
                if (TextRemainingTime <= 0)
                {
                    TextIndex++;
                    if (TextIndex == CurrentReply.Text.Count)
                    {
                        if (CurrentReply.IsEnding)
                        {
                            SwitchState(HumanControllerState.FREE);
                        }
                        else
                        {
                            CurrentNode = CurrentReply.Reaction.PickNode();
                            SwitchTalkState(HumanTalkState.PAUSE);
                        }
                    }
                    else
                    {
                        BeginTextDisplay(CurrentReply.Text[TextIndex]);
                    }
                }
            }
            else if (TalkState == HumanTalkState.PAUSE)
            {
                if (TextRemainingTime <= 0)
                {
                    SwitchTalkState(HumanTalkState.LISTENING);
                }
            }
            else if (TalkState == HumanTalkState.CHOOSING_REPLY)
            {
                if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_S))
                {
                    SelectedReply++;
                    if (SelectedReply == ValidReplies.Count)
                        SelectedReply = 0;
                }
                if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_W))
                {
                    SelectedReply--;
                    if (SelectedReply == -1)
                        SelectedReply = ValidReplies.Count - 1;
                }
                UpdateRepliesColours();

                if (Engine.Singleton.Keyboard.IsKeyDown(MOIS.KeyCode.KC_SPACE))
                {
                    CurrentReply = ValidReplies[SelectedReply];
                    SwitchTalkState(HumanTalkState.REPLYING);
                }
            }
        }

        private void HandleInventory()                               // @@ funkcja odpowiedzialna za obsługę ekwipunku
        {
            if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_S))      // następny przedmiot z listy
                HUDInventory.SelectIndex++;
            if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_W))        // poprzedni przedmiot z listy
                HUDInventory.SelectIndex--;

            if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_Q))     // wyrzucenie wybranego przedmiotu
            {
                if (HUDInventory.SelectIndex != -1)
                {
                    if (Character.DropItem(HUDInventory.SelectIndex))
                    {
                        HUDInventory.SelectIndex = HUDInventory.SelectIndex;
                        HUDInventory.UpdateView();
                    }
                }
            }

            if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_F))              // założenie / zdjęcie wybranego przedmiotu
            {
                if (HUDInventory.SelectIndex != -1)
                {
                    if (Character.Inventory[HUDInventory.SelectIndex] is ItemSword)
                    {
                        if (Character.Sword != Character.Inventory[HUDInventory.SelectIndex])
                            Character.Sword = Character.Inventory[HUDInventory.SelectIndex] as ItemSword;
                        else
                            Character.Sword = null;
                        HUDInventory.UpdateItem(HUDInventory.SelectIndex);
                    }
                }
            }

            if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_TAB))       // opuszczenie ekranu ekwipunku
                SwitchState(HumanControllerState.FREE);
        }

        public void Update()
        {
            if (Character != null)
            {
                HUD.IsVisible = false;

                if (State == HumanControllerState.FREE)
                {
                    HandleMovement();
                    HUD.IsVisible = true;
                }
                else if (State == HumanControllerState.TALK)
                    HandleConversation();
                else if (State == HumanControllerState.INVENTORY)
                    HandleInventory();
                else if (State == HumanControllerState.CONTAINER)
                    HandleContainer();
            }
        }

        private void HandleContainer()      // @@ funkcja odpowiedzialna za obsługę kontenerów
        {
            if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_S))      // następny przedmiot z listy
            {
                if (HUDContainer.ActiveEq == 1)
                    HUDContainer.SelectIndex2++;
                else if (HUDContainer.ActiveEq == 0)
                    HUDContainer.SelectIndex1++;

                HUDContainer.UpdateView();
            }
            if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_W))        // poprzedni przedmiot z listy
            {
                if (HUDContainer.ActiveEq == 1)
                    HUDContainer.SelectIndex2--;
                else if (HUDContainer.ActiveEq == 0)
                    HUDContainer.SelectIndex1--;

                HUDContainer.UpdateView();
            }
            if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_A))  // zmiana aktualnego ekwipunku
            {
                if (HUDContainer.ActiveEq == 1)
                {
                    HUDContainer.ActiveEq = 0;
                    HUDContainer.SelectIndex2 = -1;
                    HUDContainer.SelectIndex1 = 0;
                }
            }
            if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_D)) // zmiana aktualnego ekwipunku
            {
                if (HUDContainer.ActiveEq == 0)
                {
                    HUDContainer.ActiveEq = 1;
                    HUDContainer.SelectIndex2 = 0;
                    HUDContainer.SelectIndex1 = -1;
                }

                HUDContainer.UpdateView();
            }
            
            if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_F))              // przerzucenie przedmiotu między ekwipunkami
            {
                if (HUDContainer.ActiveEq == 1 && HUDContainer.SelectIndex2 != -1)
                {
                    Character.Inventory.Add(HUDContainer.Container.Contains[HUDContainer.SelectIndex2]);
                    HUDContainer.Container.Contains.RemoveAt(HUDContainer.SelectIndex2);
                    HUDContainer.SelectIndex2 = 0;
                }
                else if (HUDContainer.ActiveEq == 0 && HUDContainer.Container.CanAdd && HUDContainer.Container.Contains.Count < HUDContainer.Container.MaxItems && HUDContainer.SelectIndex1 != -1)
                {
                    HUDContainer.Container.Contains.Add(Character.Inventory[HUDContainer.SelectIndex1]);
                    Character.Inventory.RemoveAt(HUDContainer.SelectIndex1);
                    HUDContainer.SelectIndex1 = 0;
                }
                


                HUDContainer.UpdateViewAll();
            }


            if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_TAB))       // opuszczenie ekranu kontenera
            {
                HUDContainer.Container = new Container();
                SwitchState(HumanControllerState.FREE);
            }
        }

        private void HandleMovement()                             // @@ funkcja odpowiedzialna za całokształt poruszania się
        {
			Character.TurnDelta = -Engine.Singleton.Mouse.MouseState.X.rel * 0.1f * MouseSpeed;			// obracanie postaci

			Degree akt = Engine.Singleton.GameCamera.Angle;
			Degree zmiana;
	
			if (!InvertMouse)																	// ruszanie kamerą (góra i dół)
				zmiana = new Degree(Engine.Singleton.Mouse.MouseState.Y.rel * 0.1f * MouseSpeed);
			else
				zmiana = new Degree(-Engine.Singleton.Mouse.MouseState.Y.rel * 0.1f * MouseSpeed);
			
			if (zmiana + akt < new Degree(70) && zmiana + akt > new Degree(-45))
				Engine.Singleton.GameCamera.Angle += zmiana;
			
			if (Engine.Singleton.Keyboard.IsKeyDown(MOIS.KeyCode.KC_F))     // podnoszenie, otwieranie, itp. 
            {
                if (FocusObject != null)
                {
                    if (FocusObject is Described)
                    {
						if ((FocusObject as Described).IsPickable)
							Character.TryPick(FocusObject as Described);
						else if ((FocusObject as Described).IsContainer)             // czy jest kontenerem
						{
							HUDContainer.Container = (FocusObject as Described).Container;
							SwitchState(HumanControllerState.CONTAINER);
						}
						else														// AKTYWATOR!!!!!! WOLOLO!
						{
							(FocusObject as Described).PerformAkt = true;
						}
                    }
                    if (FocusObject is Character)
                    {
                        if ((FocusObject as Character).IsContainer)             // czy człowiek jest kontenerem
                        {
                            HUDContainer.Container.Contains = (FocusObject as Character).Inventory;
                            SwitchState(HumanControllerState.CONTAINER);
                        }
                    }

                }
            }

            if (Engine.Singleton.Keyboard.IsKeyDown(MOIS.KeyCode.KC_Z))         // tzw. skok
                Character.Position = new Vector3(Character.Position.x, Character.Position.y + 1, Character.Position.z);

            if (Engine.Singleton.Keyboard.IsKeyDown(MOIS.KeyCode.KC_X))          // wypisanie w konsoli aktualnej pozycji postaci
            {
                Console.Write("Pozycja: ");
                Console.Write(Character.Position.x);
                Console.Write(", ");
                Console.Write(Character.Position.y);
                Console.Write(", ");
                Console.WriteLine(Character.Position.z);
            }

            if (Engine.Singleton.Keyboard.IsKeyDown(MOIS.KeyCode.KC_B))           // wypisanie w konsoli aktywnych kłestów
            {
                Console.WriteLine("Aktywne Questy: ");
                foreach (Quest Q in Engine.Singleton.HumanController.Character.ActiveQuests.Quests)
                {
                    if (!Q.IsFinished)
                    {
                        Console.Write("- ");
                        Console.Write(Q.Name + "\n");
                    }

                }
            }

            if (Engine.Singleton.Keyboard.IsKeyDown(MOIS.KeyCode.KC_C))
            {
                Console.WriteLine("q: ");
                Console.Write("- ");
                Console.Write(Engine.Singleton.Dialog.Nodes.ElementAt(0).Value.Quest + "\n");
            }

            if (Engine.Singleton.Keyboard.IsKeyDown(MOIS.KeyCode.KC_SPACE))       // @@ rozpoczęcie rozmowy
            {
                if (FocusObject != null)
                {
                    if (Character.TalkPerm && FocusObject.TalkRoot != null)
                    {
                        CurrentNode = FocusObject.TalkRoot.PickNode();
                        SwitchState(HumanControllerState.TALK);
                    }
                }
            }

            if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_TAB)                    // @@ Otwarcie ekwipunku
                && Character.InventoryPerm)
                SwitchState(HumanControllerState.INVENTORY);

            if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_N))                     // zmiana aktualnie wybranego obiektu z sensora
            {
                FocusObjectId--;
            }
            else if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_M))                // j.w.
            {
                FocusObjectId++;
            }


			if (Engine.Singleton.Keyboard.IsKeyDown(MOIS.KeyCode.KC_A))          // obrót postaci
			{
				Character.MoveRightOrder = false;
				Character.MoveLeftOrder = true;
				Character.MoveOrder = false;
				Character.RunOrder = false;
				Character.MoveOrderBack = false;
			}
			else
				Character.MoveLeftOrder = false;

			if (Engine.Singleton.Keyboard.IsKeyDown(MOIS.KeyCode.KC_D))
			{
				Character.MoveRightOrder = true;
				Character.MoveLeftOrder = false;
				Character.MoveOrder = false;
				Character.RunOrder = false;
				Character.MoveOrderBack = false;
			}
			else
				Character.MoveRightOrder = false;

            if (Engine.Singleton.Keyboard.IsKeyDown(MOIS.KeyCode.KC_W))            // chodzenie do przodu +bieganie
            {
                Character.MoveOrder = true;
                Character.MoveOrderBack = false;
				Character.MoveRightOrder = false;
				Character.MoveLeftOrder = false;
                if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_LSHIFT))
                {
                    if (!Character.RunOrder)
                        Character.RunOrder = true;
                    else
                        Character.RunOrder = false;
                }
            }
            else
            {
                Character.MoveOrder = false;
                Character.RunOrder = false;
            }
				
            if (Engine.Singleton.Keyboard.IsKeyDown(MOIS.KeyCode.KC_S))			    // "chodzenie" do tyłu. W aktualnej wersji 
            {                                                                       // przesuwanie o wektor przeciwny w osiach 
                Character.MoveOrder = false;                                        // x i z do wektora skierowanego do przodu
                Character.MoveOrderBack = true;                                     // postaci (zwyczajny brak odpowiedniej animacji)
				Character.MoveRightOrder = false;
				Character.MoveLeftOrder = false;
            }
            else
                Character.MoveOrderBack = false;

            if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_COMMA))     // poprzednia piosenka
                Engine.Singleton.SoundManager.PreviousBGM();
            else if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_PERIOD)) // następna piosenka
                Engine.Singleton.SoundManager.NextBGM();

            if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_SLASH))
                Engine.Singleton.SoundManager.TogglePauseBGM();

            if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_H))             // wyświetlanie i chowanie HUD'a
            {
                HUD.ToggleVisibility();
            }

            if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_E))                 // @@@ Wyciąganie i chowanie miecza.
            {
                if (Character.Sword != null && !Character.Sword.InUse)
                {
                    Character.MoveOrder = false;
                    Character.RunOrder = false;
                    Character.MoveOrderBack = false;
                    Character.GetSwordOrder = true;
                }
                else if (Character.Sword != null && Character.Sword.InUse)
                {
                    Character.MoveOrder = false;
                    Character.RunOrder = false;
                    Character.MoveOrderBack = false;
                    Character.HideSwordOrder = true;
                }
            }


            //if (Character.Contacts.Count > 0)
            if (Character.Contact != null)
            {
                
                /*if (FocusObjectId < 0)                                  // przełączanie między kilkoma obiektami znajdującymi
                {                                                       // się w sensorze
                    FocusObjectId = Character.Contacts.Count - 1;
                }
                else if (FocusObjectId >= Character.Contacts.Count)
                {
                    FocusObjectId = 0;
                }

                
                SelectableObject contact = Character.Contacts[FocusObjectId] as SelectableObject;
                FocusObject = contact;
                string lol = null;

				if (contact is Enemy)
				{
					//lol = "\nHp: " + (contact as Enemy).Statistics.Hp + "\\" + (contact as Enemy).Statistics.MaxHp;
				}
                else if (contact.IsContainer == true)            // dodawanie Otwórz do nazwy obiektu, jeśli jest kontenerem
					lol = "\n(Otworz)";

                TargetLabel.Caption = contact.DisplayName + lol;
                TargetLabel.Position3D = contact.Position + contact.DisplayNameOffset;
                TargetLabel.IsVisible = true;*/

                FocusObject = Character.Contact as SelectableObject;
                string lol = null;

                if (FocusObject is ISomethingMoving)
                {
                }

                if (FocusObject.IsContainer == true)
                    lol = "\n(Otworz)";

                TargetLabel.Caption = FocusObject.DisplayName + lol;
                TargetLabel.Position3D = FocusObject.Position + FocusObject.DisplayNameOffset;
                TargetLabel.IsVisible = true;
            }
            else
            {
                FocusObject = null;
                TargetLabel.IsVisible = false;
            }
        }

        public void ToggleHud()
        {
            HUD.ToggleVisibility();
        }
    }
}
