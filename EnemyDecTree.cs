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
        DecTree.Enemies.e_Job TurnJob;

        public EnemyDecTree()
        {
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
                        
						//en.Animation("IdleLegs").Enabled = false;
						//en.Animation("WalkLegs").Enabled = true;
						//en.Animation("WalkLegs").Loop = true;
                    //en.Animation("WalkLegs").AddTime(1.0f / 90.0f);
                    return true;
                }),
                TurnJob);

            DecTree.Enemies.e_FirstFail idleNode = new DecTree.Enemies.e_FirstFail(
              new DecTree.Enemies.e_Job(en =>
              {
                  en.TalkPerm = true;
                  en.InventoryPerm = true;
                  en.Velocity = Vector3.ZERO;
				  
				  
				  
				  //en.Animation("WalkLegs").Enabled = false;
                 //en.Animation("IdleLegs").Enabled = true;
				  //en.Animation("IdleLegs").Loop = true;
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
					  
					  
					  //en.Animation("IdleLegs").Enabled = false;

					  //en.Animation("WalkLegs").Enabled = true;
					//en.Animation("WalkLegs").Loop = true;
                  }
                  return false;
              }
            ));


            e_Children.Add(followPathNode);
            e_Children.Add(walkNode);
            e_Children.Add(idleNode);
       }
    }
}
