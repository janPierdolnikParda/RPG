using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

namespace Gra
{
    public class AnimBlender
    {
        public Dictionary<string, AnimBlendGroup> Groups;
        public Dictionary<string, Anim> AllAnims;
        public Dictionary<string, string[]> AnimSets;
        public List<Anim[]> Links;
        public float BlendSpeed = 0.05f;
        public float TimeScale = 1;
        public string DefaultAnimSetName;
        public string CurrentAnimSet;

        public AnimBlender()
        {
            Groups = new Dictionary<string, AnimBlendGroup>();
            AllAnims = new Dictionary<string, Anim>();
            AnimSets = new Dictionary<string, string[]>();
            Links = new List<Anim[]>();
        }

        public void AddGroup(string name, params string[] mask)
        {
            Groups.Add(name, new AnimBlendGroup(this));
            Groups[name].BlendMask = mask;
        }

        public void RegisterAnim(string stateName, bool loop = true)
        {
            AllAnims.Add(stateName, new Anim(stateName, loop));
        }

        public void RegisterSet(string setName, params string[] setParts)
        {
            AnimSets.Add(setName, setParts);
        }

        public void RegisterLink(params string[] links)
        {
            Anim[] anims = new Anim[links.Length];
            for (int i = 0; i < links.Length; i++)
                anims[i] = AllAnims[links[i]];
            Links.Add(anims);
        }

        public void SetEntity(Entity entity)
        {
            entity.Skeleton.BlendMode = SkeletonAnimationBlendMode.ANIMBLEND_CUMULATIVE;
            foreach (Anim anim in AllAnims.Values)
            {
                anim.GetAnimationState(entity);
                anim.AddBlendMask();
            }
            foreach (string anim in AnimSets[DefaultAnimSetName])
                AllAnims[anim].State.Weight = 1.0f;
        }

        public void SetTarget(string targetName)
        {
            Anim target = AllAnims[targetName];
            target.State.Enabled = true;
            target.AffectedGroup.Target = target;
        }

        public void SetAnimSet(string animSetName)
        {
            CurrentAnimSet = animSetName;
            foreach (string animName in AnimSets[animSetName])
                SetTarget(animName);
        }

        public float AnimPhase(string name)
        {
            return AllAnims[name].State.TimePosition / AllAnims[name].State.Length;
        }

        public float AnimSetPhase(string name)
        {
            return AnimPhase(AnimSets[name][0]);
        }
        public void ResetAnimSet(string name)
        {
            foreach (string animName in AnimSets[name])
                AllAnims[animName].State.TimePosition = 0;
        }

        public void Update()
        {
            foreach (AnimBlendGroup group in Groups.Values)
                group.Update();

            foreach (Anim[] link in Links)
                foreach (Anim anim in link)
                    anim.State.TimePosition = link[0].State.TimePosition;
        }
    }
}
