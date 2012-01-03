using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using MogreNewt;

namespace Gra
{
    public class TriggerVolume : GameObject
    {
        enum ObjectState
        {
            New,
            Present,
            Unknown
        }

        class ObjectInfo
        {
            public GameObject GameObject;
            public ObjectState State;

            public ObjectInfo(GameObject gameObject, ObjectState state)
            {
                GameObject = gameObject;
                State = state;
            }
        }

        List<ObjectInfo> ObjectsInside;


        Body Body;
        List<Collision> CompoundParts;

        public delegate void CharacterEnteredHandler(TriggerVolume sender, Character character);
        public delegate void CharacterLeftHandler(TriggerVolume sender, Character character);
        public event CharacterEnteredHandler OnCharacterEntered;
        public event CharacterLeftHandler OnCharacterLeft;

        public void BeginShapeBuild()
        {
            CompoundParts = new List<Collision>();
            ObjectsInside = new List<ObjectInfo>();
        }

        public void EndShapeBuild()
        {
            Collision collision = new MogreNewt.CollisionPrimitives.CompoundCollision(
              Engine.Singleton.NewtonWorld,
              CompoundParts.ToArray(),
              Engine.Singleton.GetUniqueBodyId());

            Body = new Body(Engine.Singleton.NewtonWorld, collision, true);
            Body.SetMassMatrix(0, Vector3.ZERO);
            Body.UserData = this;
            Body.MaterialGroupID = Engine.Singleton.MaterialManager.TriggerVolumeMaterialID;

            // Usuwamy z pamięci już użyte części geometrii
            foreach (ConvexCollision c in CompoundParts)
                c.Dispose();
            CompoundParts = null;
        }

        public void AddBoxPart(Vector3 offset, Quaternion _orientation, Vector3 size)
        {
            ConvexCollision collision = new MogreNewt.CollisionPrimitives.Box(
              Engine.Singleton.NewtonWorld,
              size,
              _orientation,
              offset,
              Engine.Singleton.GetUniqueBodyId());
            CompoundParts.Add(collision);
        }

        public override Vector3 Position
        {
            get { return Body.Position; }
            set
            {
                Body.SetPositionOrientation(value, Body.Orientation);
            }
        }

        public override Quaternion Orientation
        {
            get { return Body.Orientation; }
            set
            {
                Body.SetPositionOrientation(Body.Position, value);
            }
        }

        public void HandleGameObject(GameObject gameObject)
        {
            // Próbujemy znaleźć wpis zgłoszonego obiektu
            ObjectInfo entry = ObjectsInside.Find(
                delegate(ObjectInfo info)
                {
                    return info.GameObject == gameObject;
                }
            );

            if (entry == null)
                ObjectsInside.Add(new ObjectInfo(gameObject, ObjectState.New));
            else
                entry.State = ObjectState.Present;
        }

        public override void Update()
        {
            for (int i = ObjectsInside.Count - 1; i >= 0; i--)
            {
                ObjectInfo info = ObjectsInside[i];
                switch (info.State)
                {
                    case ObjectState.Unknown:
                        if (info.GameObject is Character && info.GameObject.Exists && OnCharacterLeft != null)
                            OnCharacterLeft(this, info.GameObject as Character);
                        ObjectsInside.RemoveAt(i);
                        break;

                    case ObjectState.New:
                        if (info.GameObject is Character && info.GameObject.Exists && OnCharacterEntered != null)
                            OnCharacterEntered(this, info.GameObject as Character);
                        info.State = ObjectState.Unknown;
                        break;


                    case ObjectState.Present:
                        info.State = ObjectState.Unknown;
                        break;
                }
            }
        }
    }
}
