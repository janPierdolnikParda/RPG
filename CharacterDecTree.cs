using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using MogreNewt;

namespace Gra
{
    public class CharacterDecTree : DecTree.FirstSucc
    {
        DecTree.Node PickItemDownSeq;
        DecTree.Node GetSwordSeq;
        DecTree.Node HideSwordSeq;
        DecTree.Job TurnJob;

        public CharacterDecTree()
        {
            // #1
            DecTree.FirstSucc resetAnim = new DecTree.FirstSucc(
              new DecTree.Assert(ch => ch.AnimBlender.CurrentAnimSet == "PickItemDown"),
              new DecTree.Job(ch =>
              {
                  ch.AnimBlender.ResetAnimSet("PickItemDown");
                  ch.AnimBlender.SetAnimSet("PickItemDown");
                  ch.Velocity = Vector3.ZERO;
                  ch.TurnTo(ch.PickingTarget.Position);
                  return true;
              }));
            // #2
            Func<Character, bool> animBendDown = (
              ch => ch.AnimBlender.AnimSetPhase("PickItemDown") > 0.5f
              );
            DecTree.FirstSucc bendDown = new DecTree.FirstSucc(
              new DecTree.Assert(animBendDown),
              new DecTree.FirstFail(
                new DecTree.Assert(ch => ch.PickingTarget.Exists), // #2.1
                new DecTree.Job(animBendDown)
                )
              );
            // #3
            DecTree.FirstSucc pickItem = new DecTree.FirstSucc(
              new DecTree.Assert(ch => ch.PickingTarget == null || !ch.PickingTarget.Exists),
              new DecTree.Job(ch =>
              {
                  ch.Inventory.Add(ch.PickingTarget.Profile);
                  Engine.Singleton.ObjectManager.Destroy(ch.PickingTarget);
                  ch.PickingTarget = null;
                  return true; // #3.1
              }));
            // #4
            DecTree.Job bendUp = new DecTree.Job(
              ch => ch.AnimBlender.AnimSetPhase("PickItemDown") > 0.99f
              );

            PickItemDownSeq = new DecTree.FirstFail(
                resetAnim,
                bendDown,
                pickItem,
                bendUp);

            DecTree.Job cleanUp = new DecTree.Job(ch => { ch.PickItemOrder = false; ch.Contact = null; return true; });
            DecTree.FirstFail pickItemNode = new DecTree.FirstFail(
              new DecTree.Assert(ch => ch.PickItemOrder),
              new DecTree.FirstSucc(PickItemDownSeq, cleanUp), // #1
              cleanUp); // #2

            TurnJob = new DecTree.Job(ch =>
            {
                if (ch.TurnDelta != 0)
                {
                    Quaternion rotation = Quaternion.IDENTITY;
                    rotation.FromAngleAxis(new Degree(ch.TurnDelta), Vector3.UNIT_Y);
                    ch.Orientation *= rotation;
                    ch.TurnDelta = 0;
                }
                return true;
            });

            DecTree.FirstFail walkNode = new DecTree.FirstFail(
                new DecTree.Assert(ch => ch.MoveOrder),
                new DecTree.Job(ch =>
                {
                    ch.Velocity = ch.Orientation * Vector3.UNIT_Z * ch.Profile.WalkSpeed;


                    if (ch.RunOrder)
                        ch.AnimBlender.SetAnimSet("Run");
                    else if (ch.Sword != null)
                        if (ch.Sword.InUse)
                            ch.AnimBlender.SetAnimSet("WalkSword");
                        else
                            ch.AnimBlender.SetAnimSet("Walk");
                    else
                        ch.AnimBlender.SetAnimSet("Walk");

                    return true;
                }),
                TurnJob);

            DecTree.FirstFail walkNodeBack = new DecTree.FirstFail(
               new DecTree.Assert(ch => ch.MoveOrderBack),
               new DecTree.Job(ch =>
               {
                   ch.Velocity = -(ch.Orientation * Vector3.UNIT_Z * ch.Profile.WalkSpeed);
                   ch.AnimBlender.SetAnimSet("WalkBack");

                   return true;
               }),
               TurnJob);

            DecTree.FirstFail idleNode = new DecTree.FirstFail(
              new DecTree.Job(ch =>
              {
                  ch.TalkPerm = true;
                  ch.InventoryPerm = true;
                  ch.Velocity = Vector3.ZERO;
                  ch.AnimBlender.SetAnimSet("Idle");
                  return true;
              }),
              TurnJob);

            DecTree.FirstFail followPathNode = new DecTree.FirstFail(
                  new DecTree.Assert(ch => ch.FollowPathOrder),
                  new DecTree.Job(ch =>
                  {
                      if (Op2D.Dist(ch.Position, ch.WalkPath[0]) < 0.5f)
                      {
                          ch.WalkPath.RemoveAt(0);
                      }
                      if (ch.WalkPath.Count == 0)
                      {
                          ch.FollowPathOrder = false;
                          return true;
                      }
                      else
                      {
                          ch.Orientation = Quaternion.Slerp(
                            0.2f,
                            ch.Orientation,
                            Vector3.UNIT_Z.GetRotationTo(
                              (Op2D.XZ * (ch.WalkPath[0] - ch.Position)).NormalisedCopy),
                            true
                          );
                          ch.Velocity = ch.Orientation * Vector3.UNIT_Z * ch.Profile.WalkSpeed;
                          ch.AnimBlender.SetAnimSet("Walk");
                      }
                      return false;
                  }
                ));

            // get sword

            DecTree.FirstSucc resetAnimGetSword = new DecTree.FirstSucc(
                new DecTree.Assert(ch => ch.AnimBlender.CurrentAnimSet == "GetSword"),
                new DecTree.Job(ch =>
                {
                    ch.Velocity = Vector3.ZERO;
                    ch.AnimBlender.ResetAnimSet("GetSword");
                    ch.AnimBlender.SetAnimSet("GetSword");

                    return true;
                }));

            Func<Character, bool> animGetSword1 = (ch => ch.AnimBlender.AnimSetPhase("GetSword") > 0.5f);
            DecTree.FirstSucc getSwordWait1 = new DecTree.FirstSucc(
                new DecTree.Assert(animGetSword1),
                new DecTree.FirstFail(
                    new DecTree.Assert(ch => ch.GetSwordOrder),
                    new DecTree.Job(animGetSword1)
                    )
                );

            Func<Character, bool> animGetSword2 = (ch => ch.AnimBlender.AnimSetPhase("GetSword") > 0.99f);
            DecTree.FirstSucc getSwordWait2 = new DecTree.FirstSucc(
                new DecTree.Assert(animGetSword2),
                new DecTree.FirstFail(
                    new DecTree.Assert(ch => ch.GetSwordOrder),
                    new DecTree.Job(animGetSword2)
                    )
                );

            DecTree.FirstSucc getSword = new DecTree.FirstSucc(
                new DecTree.Job(ch =>
                {
                    ItemSword sword = ch.Sword;
                    ch.UnequipSword();
                    ch.EquipSwordToSword(sword);

                    return true;
                }));

            GetSwordSeq = new DecTree.FirstFail(
                resetAnimGetSword,
                getSwordWait1,
                getSword,
                getSwordWait2
                );

            DecTree.Job cleanUpGetSword = new DecTree.Job(ch => { ch.GetSwordOrder = false; return true; });

            DecTree.FirstFail getSwordNode = new DecTree.FirstFail(
                new DecTree.Assert(ch => ch.GetSwordOrder),
                new DecTree.FirstSucc(GetSwordSeq, cleanUpGetSword),
                cleanUpGetSword);


            // hide sword

            DecTree.FirstSucc resetAnimHideSword = new DecTree.FirstSucc(
                new DecTree.Assert(ch => ch.AnimBlender.CurrentAnimSet == "HideSword"),
                new DecTree.Job(ch =>
                {
                    ch.Velocity = Vector3.ZERO;
                    ch.AnimBlender.ResetAnimSet("HideSword");
                    ch.AnimBlender.SetAnimSet("HideSword");
                    return true;
                }));

            Func<Character, bool> animHideSword1 = (ch => ch.AnimBlender.AnimSetPhase("HideSword") > 0.5f);
            DecTree.FirstSucc hideSwordWait1 = new DecTree.FirstSucc(
                new DecTree.Assert(animHideSword1),
                new DecTree.FirstFail(
                    new DecTree.Assert(ch => ch.HideSwordOrder),
                    new DecTree.Job(animHideSword1)
                    )
                );

            Func<Character, bool> animHideSword2 = (ch => ch.AnimBlender.AnimSetPhase("HideSword") > 0.99f);
            DecTree.FirstSucc hideSwordWait2 = new DecTree.FirstSucc(
                new DecTree.Assert(animHideSword2),
                new DecTree.FirstFail(
                    new DecTree.Assert(ch => ch.HideSwordOrder),
                    new DecTree.Job(animHideSword2)
                    )
                );

            DecTree.FirstSucc hideSword = new DecTree.FirstSucc(
                new DecTree.Job(ch =>
                {

                    ItemSword sword = ch.Sword;
                    ch.UnequipSword();
                    ch.EquipSwordToLongswordSheath(sword);


                    return true;
                }));

            HideSwordSeq = new DecTree.FirstFail(
                resetAnimHideSword,
                hideSwordWait1,
                hideSword,
                hideSwordWait2
                );

            DecTree.Job cleanUpHideSword = new DecTree.Job(ch => { ch.HideSwordOrder = false; return true; });

            DecTree.FirstFail hideSwordNode = new DecTree.FirstFail(
                new DecTree.Assert(ch => ch.HideSwordOrder),
                new DecTree.FirstSucc(HideSwordSeq, cleanUpHideSword),
                cleanUpHideSword);

			////////////////////////////////////////////////////////////////////////////////////////////////
			// go left
			DecTree.FirstFail go_left = new DecTree.FirstFail(
			   new DecTree.Assert(ch => ch.MoveLeftOrder),
			   new DecTree.Job(ch =>
			   {
				    Quaternion orient = Quaternion.IDENTITY;
				    orient.FromAngleAxis(new Degree(90), Vector3.UNIT_Y);
				    ch.Velocity = ch.Orientation * orient * Vector3.UNIT_Z * ch.Profile.WalkSpeed;
					ch.AnimBlender.SetAnimSet("Idle");  // ### ANIMACJA CHODZENIA!

				   return true;
			   }),
			   TurnJob);

			DecTree.FirstFail go_right = new DecTree.FirstFail(
			   new DecTree.Assert(ch => ch.MoveRightOrder),
			   new DecTree.Job(ch =>
			   {
				   Quaternion orient = Quaternion.IDENTITY;
				   orient.FromAngleAxis(new Degree(-90), Vector3.UNIT_Y);
				   ch.Velocity = ch.Orientation * orient * Vector3.UNIT_Z * ch.Profile.WalkSpeed;
				   ch.AnimBlender.SetAnimSet("Idle");  // ### ANIMACJA CHODZENIA!

				   return true;
			   }),
			   TurnJob);





            Children.Add(getSwordNode);
            Children.Add(hideSwordNode);
            Children.Add(walkNodeBack);
			Children.Add(go_left);
			Children.Add(go_right);

            Children.Add(pickItemNode);
            Children.Add(followPathNode);
            Children.Add(walkNode);
            Children.Add(idleNode);

        }
    }
}
