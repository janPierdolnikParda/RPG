using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using MogreNewt;

namespace Gra
{
    public class EnemyDecTree : DecTree.Enemies.e_FirstSucc
    {
        DecTree.Enemies.e_Node PickItemDownSeq;
        DecTree.Enemies.e_Node GetSwordSeq;
        DecTree.Enemies.e_Node HideSwordSeq;
        DecTree.Enemies.e_Job TurnJob;

        public EnemyDecTree()
        {
            // #1
            DecTree.Enemies.e_FirstSucc resetAnim = new DecTree.Enemies.e_FirstSucc(
              new DecTree.Enemies.e_Assert(en => en.AnimBlender.CurrentAnimSet == "PickItemDown"),
              new DecTree.Enemies.e_Job(en =>
              {
                  en.AnimBlender.ResetAnimSet("PickItemDown");
                  en.AnimBlender.SetAnimSet("PickItemDown");
                  en.Velocity = Vector3.ZERO;
                  en.TurnTo(en.PickingTarget.Position);
                  return true;
              }));

            // #2
            Func<Enemy, bool> animBendDown = (
              en => en.AnimBlender.AnimSetPhase("PickItemDown") > 0.5f
              );
            DecTree.Enemies.e_FirstSucc bendDown = new DecTree.Enemies.e_FirstSucc(
              new DecTree.Enemies.e_Assert(animBendDown),
              new DecTree.Enemies.e_FirstFail(
                new DecTree.Enemies.e_Assert(en => en.PickingTarget.Exists), // #2.1
                new DecTree.Enemies.e_Job(animBendDown)
                )
              );

            // #3
            DecTree.Enemies.e_FirstSucc pickItem = new DecTree.Enemies.e_FirstSucc(
              new DecTree.Enemies.e_Assert(en => en.PickingTarget == null || !en.PickingTarget.Exists),
              new DecTree.Enemies.e_Job(en =>
              {
                  en.Inventory.Add(en.PickingTarget.Profile);
                  Engine.Singleton.ObjectManager.Destroy(en.PickingTarget);
                  en.PickingTarget = null;
                  return true; // #3.1
              }));
            // #4
            DecTree.Enemies.e_Job bendUp = new DecTree.Enemies.e_Job(
              en => en.AnimBlender.AnimSetPhase("PickItemDown") > 0.99f
              );

            PickItemDownSeq = new DecTree.Enemies.e_FirstFail(
                resetAnim,
                bendDown,
                pickItem,
                bendUp);

            DecTree.Enemies.e_Job cleanUp = new DecTree.Enemies.e_Job(en => { en.PickItemOrder = false; return true; });
            DecTree.Enemies.e_FirstFail pickItemNode = new DecTree.Enemies.e_FirstFail(
              new DecTree.Enemies.e_Assert(en => en.PickItemOrder),
              new DecTree.Enemies.e_FirstSucc(PickItemDownSeq, cleanUp), // #1
              cleanUp); // #2

            TurnJob = new DecTree.Enemies.e_Job(en =>
            {
                if (en.TurnDelta != 0)
                {
                    Quaternion rotation = Quaternion.IDENTITY;
                    rotation.FromAngleAxis(new Degree(en.TurnDelta), Vector3.UNIT_Y);
                    en.Orientation *= rotation;
                    en.TurnDelta = 0;
                }
                return true;
            });

            DecTree.Enemies.e_FirstFail walkNode = new DecTree.Enemies.e_FirstFail(
                new DecTree.Enemies.e_Assert(en => en.MoveOrder),
                new DecTree.Enemies.e_Job(en =>
                {
                    en.Velocity = en.Orientation * Vector3.UNIT_Z * en.Profile.WalkSpeed;


                    if (en.RunOrder)
                        en.AnimBlender.SetAnimSet("Run");
                    else if (en.Sword != null)
                        if (en.Sword.InUse)
                            en.AnimBlender.SetAnimSet("WalkSword");
                        else
                            en.AnimBlender.SetAnimSet("Walk");
                    else
                        en.AnimBlender.SetAnimSet("Walk");

                    return true;
                }),
                TurnJob);

            DecTree.Enemies.e_FirstFail walkNodeBack = new DecTree.Enemies.e_FirstFail(
               new DecTree.Enemies.e_Assert(en => en.MoveOrderBack),
               new DecTree.Enemies.e_Job(en =>
               {
                   en.Velocity = -(en.Orientation * Vector3.UNIT_Z * en.Profile.WalkSpeed);
                   en.AnimBlender.SetAnimSet("WalkBack");

                   return true;
               }),
               TurnJob);

            DecTree.Enemies.e_FirstFail idleNode = new DecTree.Enemies.e_FirstFail(
              new DecTree.Enemies.e_Job(en =>
              {
                  en.TalkPerm = true;
                  en.InventoryPerm = true;
                  en.Velocity = Vector3.ZERO;
                  en.AnimBlender.SetAnimSet("Idle");
                  return true;
              }),
              TurnJob);

            DecTree.Enemies.e_FirstFail followPathNode = new DecTree.Enemies.e_FirstFail(
              new DecTree.Enemies.e_Assert(en => en.FollowPathOrder),
              new DecTree.Enemies.e_Job(en =>
              {
                  if (Op2D.Dist(en.Position, en.WalkPath[0]) < 0.5f)
                  {
                      en.WalkPath.RemoveAt(0);
                  }
                  if (en.WalkPath.Count == 0)
                  {
                      en.FollowPathOrder = false;
                      return true;
                  }
                  else
                  {
                      en.Orientation = Quaternion.Slerp(
                        0.2f,
                        en.Orientation,
                        Vector3.UNIT_Z.GetRotationTo(
                          (Op2D.XZ * (en.WalkPath[0] - en.Position)).NormalisedCopy),
                        true
                      );
                      en.Velocity = en.Orientation * Vector3.UNIT_Z * en.Profile.WalkSpeed;
                      en.AnimBlender.SetAnimSet("Walk");
                  }
                  return false;
              }
            ));

            // get sword

            DecTree.Enemies.e_FirstSucc resetAnimGetSword = new DecTree.Enemies.e_FirstSucc(
                new DecTree.Enemies.e_Assert(en => en.AnimBlender.CurrentAnimSet == "GetSword"),
                new DecTree.Enemies.e_Job(en =>
                {
                    en.Velocity = Vector3.ZERO;
                    en.AnimBlender.ResetAnimSet("GetSword");
                    en.AnimBlender.SetAnimSet("GetSword");

                    return true;
                }));

            Func<Enemy, bool> animGetSword1 = (en => en.AnimBlender.AnimSetPhase("GetSword") > 0.5f);
            DecTree.Enemies.e_FirstSucc getSwordWait1 = new DecTree.Enemies.e_FirstSucc(
                new DecTree.Enemies.e_Assert(animGetSword1),
                new DecTree.Enemies.e_FirstFail(
                    new DecTree.Enemies.e_Assert(en => en.GetSwordOrder),
                    new DecTree.Enemies.e_Job(animGetSword1)
                    )
                );

            Func<Enemy, bool> animGetSword2 = (en => en.AnimBlender.AnimSetPhase("GetSword") > 0.99f);
            DecTree.Enemies.e_FirstSucc getSwordWait2 = new DecTree.Enemies.e_FirstSucc(
                new DecTree.Enemies.e_Assert(animGetSword2),
                new DecTree.Enemies.e_FirstFail(
                    new DecTree.Enemies.e_Assert(en => en.GetSwordOrder),
                    new DecTree.Enemies.e_Job(animGetSword2)
                    )
                );

            DecTree.Enemies.e_FirstSucc getSword = new DecTree.Enemies.e_FirstSucc(
                new DecTree.Enemies.e_Job(en =>
                {
                    ItemSword sword = en.Sword;
                    en.UnequipSword();
                    en.EquipSwordToSword(sword);

                    return true;
                }));

            GetSwordSeq = new DecTree.Enemies.e_FirstFail(
                resetAnimGetSword,
                getSwordWait1,
                getSword,
                getSwordWait2
                );

            DecTree.Enemies.e_Job cleanUpGetSword = new DecTree.Enemies.e_Job(en => { en.GetSwordOrder = false; return true; });

            DecTree.Enemies.e_FirstFail getSwordNode = new DecTree.Enemies.e_FirstFail(
                new DecTree.Enemies.e_Assert(en => en.GetSwordOrder),
                new DecTree.Enemies.e_FirstSucc(GetSwordSeq, cleanUpGetSword),
                cleanUpGetSword);


            // hide sword

            DecTree.Enemies.e_FirstSucc resetAnimHideSword = new DecTree.Enemies.e_FirstSucc(
                new DecTree.Enemies.e_Assert(en => en.AnimBlender.CurrentAnimSet == "HideSword"),
                new DecTree.Enemies.e_Job(en =>
                {
                    en.Velocity = Vector3.ZERO;
                    en.AnimBlender.ResetAnimSet("HideSword");
                    en.AnimBlender.SetAnimSet("HideSword");
                    return true;
                }));

            Func<Enemy, bool> animHideSword1 = (en => en.AnimBlender.AnimSetPhase("HideSword") > 0.5f);
            DecTree.Enemies.e_FirstSucc hideSwordWait1 = new DecTree.Enemies.e_FirstSucc(
                new DecTree.Enemies.e_Assert(animHideSword1),
                new DecTree.Enemies.e_FirstFail(
                    new DecTree.Enemies.e_Assert(en => en.HideSwordOrder),
                    new DecTree.Enemies.e_Job(animHideSword1)
                    )
                );

            Func<Enemy, bool> animHideSword2 = (en => en.AnimBlender.AnimSetPhase("HideSword") > 0.99f);
            DecTree.Enemies.e_FirstSucc hideSwordWait2 = new DecTree.Enemies.e_FirstSucc(
                new DecTree.Enemies.e_Assert(animHideSword2),
                new DecTree.Enemies.e_FirstFail(
                    new DecTree.Enemies.e_Assert(en => en.HideSwordOrder),
                    new DecTree.Enemies.e_Job(animHideSword2)
                    )
                );

            DecTree.Enemies.e_FirstSucc hideSword = new DecTree.Enemies.e_FirstSucc(
                new DecTree.Enemies.e_Job(en =>
                {

                    ItemSword sword = en.Sword;
                    en.UnequipSword();
                    en.EquipSwordToLongswordSheath(sword);


                    return true;
                }));

            HideSwordSeq = new DecTree.Enemies.e_FirstFail(
                resetAnimHideSword,
                hideSwordWait1,
                hideSword,
                hideSwordWait2
                );

            DecTree.Enemies.e_Job cleanUpHideSword = new DecTree.Enemies.e_Job(en => { en.HideSwordOrder = false; return true; });

            DecTree.Enemies.e_FirstFail hideSwordNode = new DecTree.Enemies.e_FirstFail(
                new DecTree.Enemies.e_Assert(en => en.HideSwordOrder),
                new DecTree.Enemies.e_FirstSucc(HideSwordSeq, cleanUpHideSword),
                cleanUpHideSword);

            e_Children.Add(getSwordNode);
            e_Children.Add(hideSwordNode);
            e_Children.Add(walkNodeBack);

            e_Children.Add(pickItemNode);
            e_Children.Add(followPathNode);
            e_Children.Add(walkNode);
            e_Children.Add(idleNode);
       }
    }
}
