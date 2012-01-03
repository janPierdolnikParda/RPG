using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

namespace Gra
{
    public class CharacterAnimBlender : AnimBlender
    {
        public static string[] MaskLegs = 
        { "Main", "Leg1.L", "Leg1.R", "Leg2.L", "Leg2.R", "Leg3.L", "Leg3.R", 
            "Foot.L", "Foot.R",  "SwordSheath" };
        public static string[] MaskTorso =
        { "Spine1", "Spine2", "Spine3", "Arm1.L", "Arm1.R", "Arm2.L", "Arm2.R", 
            "Arm3.L", "Arm3.R", "Arm4.L", "Arm4.R", "Hand1.L", "Hand1.R", "Hand2.L", 
            "Hand2.R", "Hand3.L", "Hand3.R", "Hand4.L", "Hand4.R", "Thumb1.L", 
            "Thumb1.R", "Thumb2.L", "Thumb2.R", "Thumb3.L", "Thumb3.R", "Thumb1.L",
            "Neck", "HeadFront", "Head", "Jaw", "Mouth.L", "Mouth.R", 
            "LongswordSheath", "Sword" };

        public CharacterAnimBlender()
            : base()
        {
            AddGroup("Legs", MaskLegs);
            AddGroup("Torso", MaskTorso);

            RegisterAnim("IdleLegs");
            RegisterAnim("IdleTorso");
            RegisterAnim("WalkLegs");
            RegisterAnim("WalkTorso");
            RegisterAnim("PickItemDownLegs", false);
            RegisterAnim("PickItemDownTorso", false);
            RegisterAnim("RunLegs");
            RegisterAnim("RunTorso");
            RegisterAnim("DrawSwordLegs", false);
            RegisterAnim("DrawSwordTorso", false);
            RegisterAnim("HideSwordLegs", false);
            RegisterAnim("HideSwordTorso", false);
            RegisterAnim("IdleSwordLegs");
            RegisterAnim("IdleSwordTorso");
            RegisterAnim("WalkSwordTorso");


            RegisterSet("GetSword", "DrawSwordLegs", "DrawSwordTorso");
            RegisterSet("HideSword", "HideSwordLegs", "HideSwordTorso");
            RegisterSet("IdleSword", "IdleSwordLegs", "IdleSwordTorso");
            RegisterSet("WalkSword", "WalkLegs", "WalkSwordTorso");
            RegisterSet("Idle", "IdleLegs", "IdleTorso");
            RegisterSet("Walk", "WalkLegs", "WalkTorso");
            RegisterSet("PickItemDown", "PickItemDownLegs", "PickItemDownTorso");
            RegisterSet("WalkBack", "WalkLegs", "WalkTorso");
            RegisterSet("Run", "RunLegs", "RunTorso");
            //
            // RegisterSet("Attack01", "Attack01Legs", "Attack01Torso");
            //


            RegisterLink("WalkLegs", "WalkTorso");
            RegisterLink("IdleLegs", "IdleTorso");
            RegisterLink("RunLegs", "RunTorso");

            Groups["Legs"].AddAnim("IdleLegs");
            Groups["Torso"].AddAnim("IdleTorso");
            Groups["Legs"].AddAnim("PickItemDownLegs");
            Groups["Torso"].AddAnim("PickItemDownTorso");
            Groups["Legs"].AddAnim("WalkLegs");
            Groups["Torso"].AddAnim("WalkTorso");
            Groups["Legs"].AddAnim("RunLegs");
            Groups["Torso"].AddAnim("RunTorso");
            Groups["Legs"].AddAnim("DrawSwordLegs");
            Groups["Torso"].AddAnim("DrawSwordTorso");
            Groups["Legs"].AddAnim("HideSwordLegs");
            Groups["Torso"].AddAnim("HideSwordTorso");
            Groups["Legs"].AddAnim("IdleSwordLegs");
            Groups["Torso"].AddAnim("IdleSwordTorso");
            Groups["Torso"].AddAnim("WalkSwordTorso");


            //   Groups["Legs"].AddAnim("Attack01Legs");
            //   Groups["Torso"].AddAnim("Attack01Torso");

            DefaultAnimSetName = "Idle";
        }
    }
}
