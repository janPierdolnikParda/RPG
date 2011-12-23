using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using MogreNewt;

namespace Gra
{
    public class Anim
    {
        public string StateName;
        public AnimationState State;
        public bool Loop;
        Entity Entity;
        public AnimBlendGroup AffectedGroup;

        public Anim(string stateName, bool loop)
        {
            StateName = stateName;
            Loop = loop;
        }

        public void GetAnimationState(Entity entity)
        {
            Entity = entity;
            State = Entity.GetAnimationState(StateName);
            State.Loop = Loop;
            State.Weight = 0;
        }

        public void AddBlendMask()
        {
            if (State.HasBlendMask == false)
                State.CreateBlendMask(Entity.Skeleton.NumBones, 0);

            foreach (string maskEntry in AffectedGroup.BlendMask)
                State.SetBlendMaskEntry(Entity.Skeleton.GetBone(maskEntry).Handle, 1);
        }
    }
}
