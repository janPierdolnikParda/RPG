using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using MogreNewt;

namespace Gra
{
    public class Character : SelectableObject
    {
        public Entity Entity;
        public SceneNode Node;
        public Body Body;

        public CharacterProfile Profile;

        public Vector3 Velocity;

        public Vector3 Displacement;

        public Body ObjectSensor;
        public Node SensorNode;

        Quaternion _Orientation;

        public List<GameObject> Contacts;

        public Described PickingTarget;

        public List<DescribedProfile> Inventory;
        ItemSword _Sword;
        Entity SwordEntity;
        public CharacterAnimBlender AnimBlender;

        public QuestManager ActiveQuests;
        public List<Quest> Quests;
        public List<Quest> QuestsDone;

        // Rozkazy
        public bool PickItemOrder;
        public bool MoveOrder;
        public bool MoveOrderBack;

        public bool GetSwordOrder;
        public bool HideSwordOrder;

        bool _RunOrder;

        public bool RunOrder
        {
            get
            {
                return _RunOrder;
            }
            set
            {
                if (_RunOrder == true && value == false)
                {
                    _RunOrder = false;
                    Profile.WalkSpeed -= 2.0f;
                }
                if (_RunOrder == false && value == true)
                {
                    _RunOrder = true;
                    Profile.WalkSpeed += 2.0f;
                }
            }
        }

        public bool FollowPathOrder;
        public List<Vector3> WalkPath;  
        // Pozwolenia
        public bool TalkPerm;
        public bool InventoryPerm;
        // Inne
        public float TurnDelta;

        public static DecTree.Node Tree = new CharacterDecTree();

        public Statistics Statistics;

        public Character(CharacterProfile profile)
        {
            Profile = profile.Clone();

            _Orientation = Quaternion.IDENTITY;

            Entity = Engine.Singleton.SceneManager.CreateEntity(Profile.MeshName);
            Node = Engine.Singleton.SceneManager.RootSceneNode.CreateChildSceneNode();
            Node.AttachObject(Entity);

            // Bierzemy połowę rozmiaru BoundingBoxa i mnożymy go przez czynnik
            Vector3 scaledSize = Entity.BoundingBox.HalfSize * Profile.BodyScaleFactor;

            ConvexCollision collision = new MogreNewt.CollisionPrimitives.Capsule(
                Engine.Singleton.NewtonWorld,
                // Wybieramy szerokość bądź długość
                System.Math.Min(scaledSize.x, scaledSize.z),
                // Z uwagi na to, że wzięliśmy połowę rozmiaru,
                // musimy wysokość pomnożyć przez 2
                scaledSize.y * 2,
                // Kapsuła stworzona przez Newtona będzie leżeć wzdłóż osi X,
                // więc ustalamy pion wzdłóż osi Y
                Vector3.UNIT_X.GetRotationTo(Vector3.UNIT_Y),
                Engine.Singleton.GetUniqueBodyId()); // Unikalny identyfikator ciała




            Vector3 inertia, offset;
            collision.CalculateInertialMatrix(out inertia, out offset);

            inertia *= Profile.BodyMass;

            Body = new Body(Engine.Singleton.NewtonWorld, collision, true);
            Body.AttachNode(Node);
            Body.SetMassMatrix(Profile.BodyMass, inertia);
            Body.AutoSleep = false;

            Body.Transformed += BodyTransformCallback;
            Body.ForceCallback += BodyForceCallback;

            Body.UserData = this;
            Body.MaterialGroupID = Engine.Singleton.MaterialManager.CharacterMaterialID;

            Joint upVector = new MogreNewt.BasicJoints.UpVector(
            Engine.Singleton.NewtonWorld, Body, Vector3.UNIT_Y);

            collision.Dispose();

            SensorNode = Node.CreateChildSceneNode(new Vector3(0, 0, System.Math.Min(scaledSize.x, scaledSize.z) * 2));

            collision = new MogreNewt.CollisionPrimitives.Cylinder(
                Engine.Singleton.NewtonWorld,
                System.Math.Min(scaledSize.x, scaledSize.z) * 2,
                scaledSize.y * 2,
                Vector3.UNIT_X.GetRotationTo(Vector3.UNIT_Y),
                Engine.Singleton.GetUniqueBodyId());
            ObjectSensor = new Body(Engine.Singleton.NewtonWorld, collision, true);
            ObjectSensor.SetMassMatrix(1, new Vector3(1, 1, 1));

            ObjectSensor.UserData = this;
            ObjectSensor.MaterialGroupID = Engine.Singleton.MaterialManager.CharacterSensorMaterialID;

            Contacts = new List<GameObject>();

            Inventory = new List<DescribedProfile>();

            AnimBlender = new CharacterAnimBlender();
            AnimBlender.SetEntity(Entity);

            ActiveQuests = new QuestManager();
            Quests = new List<Quest>();
            QuestsDone = new List<Quest>();

            Statistics = new Statistics();
        }

        void BodyTransformCallback(Body sender, Quaternion orientation,
            Vector3 position, int threadIndex)
        {
            Node.Position = position;
            Node.Orientation = Orientation;
        }

        public void BodyForceCallback(Body body, float timeStep, int threadIndex)
        {
            Vector3 force = (Velocity - Body.Velocity * new Vector3(1, 0, 1))
                * Profile.BodyMass * Engine.FixedFPS;
            Body.Velocity = Velocity * new Vector3(1, 0, 1) + Body.Velocity * Vector3.UNIT_Y;
        }

        public override void Update()
        {
            ObjectSensor.SetPositionOrientation(SensorNode._getDerivedPosition(), Node.Orientation);
            AnimBlender.Update();

            InventoryPerm = false;
            TalkPerm = false;
            Tree.Visit(this);

            Contacts.Clear();

            ActiveQuests.Update();
        }

        public void TurnTo(Vector3 point)
        {
            Orientation = Vector3.UNIT_Z.GetRotationTo((point - Position) * new Vector3(1, 0, 1));
        }

        public void TryPick(Described target)
        {
            PickingTarget = target;
            PickItemOrder = true;
        }

        public bool DropItem(int itemIndex)
        {
            if (!Inventory[itemIndex].IsEquipment)
            {
                Described item = new Described(Inventory[itemIndex]);
                item.Position = Position + Orientation * Vector3.UNIT_Z * 0.2f;
                Engine.Singleton.ObjectManager.Add(item);
                Inventory.RemoveAt(itemIndex);
                return true;
            }
            return false;
        }

        public void UnequipSword()
        {
            Entity.DetachObjectFromBone(SwordEntity);
            Engine.Singleton.SceneManager.DestroyEntity(SwordEntity);
            _Sword.IsEquipment = false;
            SwordEntity = null;

        }

        public void EquipSwordToLongswordSheath(ItemSword value)
        {
            SwordEntity = Engine.Singleton.SceneManager.CreateEntity(value.MeshName);
            Entity.AttachObjectToBone(
                "LongswordSheath",
                SwordEntity, Vector3.UNIT_Z.GetRotationTo(-Vector3.UNIT_Y),
                value.HandleOffset);
            value.IsEquipment = true;
            value.InUse = false;
        }

        public void EquipSwordToSword(ItemSword value)
        {
            SwordEntity = Engine.Singleton.SceneManager.CreateEntity(value.MeshName);
            Entity.AttachObjectToBone(
                "Sword",
                SwordEntity, Vector3.UNIT_Z.GetRotationTo(-Vector3.UNIT_Y),
                value.HandleOffset);
            value.IsEquipment = true;
            value.InUse = true;
        }

        public ItemSword Sword
        {
            get
            {
                return _Sword;
            }
            set
            {
                if (SwordEntity != null)
                {
                    UnequipSword();
                }
                if (value != null)
                {
                    EquipSwordToLongswordSheath(value);

                }
                _Sword = value;
            }
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
                return Profile.DisplayName;
            }
            set
            {
                Profile.DisplayName = value;
            }
        }

        public override string Description
        {
            get
            {
                return Profile.Description;
            }
            set
            {
                Profile.Description = value;
            }
        }

        public override Vector3 DisplayNameOffset
        {
            get
            {
                return Profile.DisplayNameOffset;
            }
            set
            {
                Profile.DisplayNameOffset = value;
            }
        }

        public override void Destroy()
        {
            base.Destroy();
        }
    }
}
