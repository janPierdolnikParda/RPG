using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using MogreNewt;

namespace Gra
{
    public class WayPoint : SelectableObject
    {
        public Entity Entity;
        public SceneNode Node;
        public Body Body;

        public Vector3 Velocity;
        public Vector3 Displacement;
        public Vector3 Inertia;
        Quaternion _Orientation;
        Vector3 _DisplayNameOffset;
        String _DisplayName;

        public WayPoint()
        {
            _Orientation = Quaternion.IDENTITY;
            _DisplayNameOffset = new Vector3(0, 0.2f, 0);

            Entity = Engine.Singleton.SceneManager.CreateEntity("Spawn.mesh");
            Node = Engine.Singleton.SceneManager.RootSceneNode.CreateChildSceneNode();
            Node.AttachObject(Entity);

            ConvexCollision collision = new MogreNewt.CollisionPrimitives.ConvexHull(Engine.Singleton.NewtonWorld,
                Node,
                Quaternion.IDENTITY,
                0.1f,
                Engine.Singleton.GetUniqueBodyId());

            Vector3 inertia, offset;
            collision.CalculateInertialMatrix(out inertia, out offset);

            Inertia = inertia;

            Body = new Body(Engine.Singleton.NewtonWorld, collision, true);
            Body.AttachNode(Node);
            Body.SetMassMatrix(0, inertia * 0);

            Body.ForceCallback += BodyForceCallback;

            Body.UserData = this;
            Body.MaterialGroupID = Engine.Singleton.MaterialManager.WaypointMaterialID;
            //Body.MaterialGroupID = Engine.Singleton.MaterialManager.CharacterMaterialID;

            collision.Dispose();
        }

        public void BodyForceCallback(Body body, float timeStep, int threadIndex)
        {
            Vector3 force = (Velocity - Body.Velocity * new Vector3(1, 0, 1))
                * 0 * Engine.FixedFPS;
            Body.Velocity = Velocity * new Vector3(1, 0, 1) + Body.Velocity * Vector3.UNIT_Y;
        }

        public override void Update()
        {
        }

        public override Vector3 Position
        {
            get
            {
                return Body.Position;
            }
            set
            {
                Body.SetPositionOrientation(value, Orientation);
            }
        }

        public override Quaternion Orientation
        {
            get
            {
                return _Orientation;
            }
            set
            {
                _Orientation = value;
            }
        }

        public override string DisplayName
        {
            get
            {
                return _DisplayName;
            }
            set
            {
                _DisplayName = value;
            }
        }

        public override string Description
        {
            get
            {
                return "";
            }
            set
            {
            
            }
        }

        public override Vector3 DisplayNameOffset
        {
            get
            {
                return _DisplayNameOffset;
            }
            set
            {
                _DisplayNameOffset = value;
            }
        }

        public override void Destroy()
        {
            Node.DetachAllObjects();
            Engine.Singleton.SceneManager.DestroySceneNode(Node);
            Engine.Singleton.SceneManager.DestroyEntity(Entity);
            Body.Dispose();
            Body = null;

            base.Destroy();
        }


    }
}
