using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using MogreNewt;

namespace Gra
{
    public class AnimBlendGroup
    {
        List<Anim> Anims;
        AnimBlender Blender;
        public Anim Target;
        public string[] BlendMask;

        public AnimBlendGroup(AnimBlender blender)
        {
            Anims = new List<Anim>();
            Blender = blender;
        }

        public void AddAnim(string stateName)
        {
            Anim anim = Blender.AllAnims[stateName];
            Anims.Add(anim);
            anim.AffectedGroup = this;
        }

        public void Update()
        {
          float blendOut = 0;
 
          foreach (Anim anim in Anims)
          {
            if (anim != Target)
            {
              if (anim.State.Enabled)
              {
                float weightOut = System.Math.Min(Blender.BlendSpeed, anim.State.Weight);
                blendOut += weightOut;
                anim.State.Weight -= weightOut;
                if (anim.State.Weight <= 0.0f)                        
                  anim.State.Enabled = false;   
              }
            }                
            if (anim.State.Enabled)
              anim.State.AddTime(Engine.FixedTimeStep * Blender.TimeScale);
          }

          if (Target != null && Target.State.Weight < 1.0f)
          {
              Target.State.Weight += blendOut;
              if (Target.State.Weight > 1.0f)
                  Target.State.Weight = 1.0f;
          }
        }
    }
}
