using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using MogreNewt;

namespace Gra
{
    public class Character : SelectableObject, ISomethingMoving
    {
        public enum FriendType
        {
            NEUTRAL,
            FRIENDLY,
            ENEMY
        };

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
        public GameObject Contact;

        public Described PickingTarget;

        public List<DescribedProfile> Inventory;
        ItemSword _Sword;
        Entity SwordEntity;
        public CharacterAnimBlender AnimBlender;

        public QuestManager ActiveQuests;
        public List<Quest> Quests;
		public ISomethingMoving FocusedEnemy;

        // Rozkazy
        public bool PickItemOrder;
        public bool MoveOrder;
        public bool MoveOrderBack;
		public bool MoveLeftOrder;
		public bool MoveRightOrder;

		public bool AttackOrder;

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

		Statistics _Statistics;
		public Statistics Statistics
		{
			get
			{
				return _Statistics;
			}

			set
			{
				_Statistics = value;
			}
		}

        float _ZasiegWzroku;
        public float ZasiegWzroku
        {
            get
            {
                return _ZasiegWzroku;
            }

            set
            {
                _ZasiegWzroku = value;
            }
        }

        float _ZasiegOgolny;
        public float ZasiegOgolny
        {
            get
            {
                return _ZasiegOgolny;
            }

            set
            {
                _ZasiegOgolny = value;
            }
        }

        FriendType _FriendlyType;
        public FriendType FriendlyType
        {
            get
            {
                return _FriendlyType;
            }

            set
            {
                _FriendlyType = value;
            }
        }

        Enemy.StateTypes _State;
        public Enemy.StateTypes State
        {
            get
            {
                return _State;
            }

            set
            {
                _State = value;
            }
        }

        public Character(CharacterProfile profile)
        {
            Profile = profile.Clone();

            _Orientation = Quaternion.IDENTITY;

            Entity = Engine.Singleton.SceneManager.CreateEntity(Profile.MeshName);
            Node = Engine.Singleton.SceneManager.RootSceneNode.CreateChildSceneNode();
            Node.AttachObject(Entity);

            Vector3 scaledSize = Entity.BoundingBox.HalfSize * Profile.BodyScaleFactor;

			ConvexCollision collision = new MogreNewt.CollisionPrimitives.Capsule(
				Engine.Singleton.NewtonWorld,
				System.Math.Min(scaledSize.x, scaledSize.z),
				scaledSize.y * 2,
				Vector3.UNIT_X.GetRotationTo(Vector3.UNIT_Y),
				Engine.Singleton.GetUniqueBodyId());



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
            Inventory = Profile.Inventory;

            AnimBlender = new CharacterAnimBlender();
            AnimBlender.SetEntity(Entity);

            ActiveQuests = new QuestManager();
            Quests = new List<Quest>();
            //QuestsDone = new List<Quest>();

            _Statistics = Profile.Statistics.statistics_Clone();

			FriendlyType = Profile.FriendlyType;

            State = Enemy.StateTypes.IDLE;

            if (Profile.DialogRoot != null && Profile.DialogRoot != "")
            {
                TalkRoot = Conversations.D[Profile.DialogRoot].Reactions.Values.ElementAt(0);
            }
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
            if (Statistics.aktualnaZywotnosc <= 0)
            {
                //Console.WriteLine("NIE ZYJESZ");
                State = Enemy.StateTypes.DEAD;
            }

            ObjectSensor.SetPositionOrientation(SensorNode._getDerivedPosition(), Node.Orientation);
            SetContact();
            AnimBlender.Update();

            InventoryPerm = false;
            TalkPerm = false;
            Tree.Visit(this);

            Contacts.Clear();

            ActiveQuests.Update();
        }

        public Radian getY()
        {
            Matrix3 orientMatrix;
            orientMatrix = Engine.Singleton.Camera.Orientation.ToRotationMatrix();

            Radian yRad, pRad, rRad;
            orientMatrix.ToEulerAnglesYXZ(out yRad, out pRad, out rRad);

            return yRad;
        }

        public Radian getX()
        {
            Matrix3 orientMatrix;
            orientMatrix = Engine.Singleton.Camera.Orientation.ToRotationMatrix();

            Radian yRad, pRad, rRad;
            orientMatrix.ToEulerAnglesYXZ(out yRad, out pRad, out rRad);

            return pRad;
        }

        public void SetContact()
        {

            float length = 5.0f;

            Vector3 AimPosition = new Vector3();
            AimPosition.x = (float)System.Math.Sin((double)-getY().ValueRadians) * length;
            AimPosition.x = (float)System.Math.Cos((double)getX().ValueRadians) * AimPosition.x + Engine.Singleton.Camera.Position.x;
            AimPosition.y = (float)System.Math.Sin((double)getX().ValueRadians) * length + Engine.Singleton.Camera.Position.y;
            AimPosition.z = (float)System.Math.Cos((double)getY().ValueRadians) * -length;
            AimPosition.z = (float)System.Math.Cos((double)getX().ValueRadians) * AimPosition.z + Engine.Singleton.Camera.Position.z;

            Contact = null;

            PredicateRaycast raycast = new PredicateRaycast((b => !(b.UserData is TriggerVolume) && (b.MaterialGroupID != Engine.Singleton.MaterialManager.CharacterSensorMaterialID) && (b.MaterialGroupID != Engine.Singleton.MaterialManager.LevelMaterialID)));
            raycast.Go(Engine.Singleton.NewtonWorld, Engine.Singleton.Camera.Position, AimPosition);

            if (raycast.Contacts.Count != 0)
            {
                raycast.SortContacts();
                AimPosition = Position
                  + (AimPosition - Position) * raycast.Contacts[0].Distance
                  + raycast.Contacts[0].Normal * 0.01f * length;

                if (raycast.Contacts[0].Body.UserData is GameObject)
                  Contact = raycast.Contacts[0].Body.UserData as GameObject;
            }         
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
            Node.DetachAllObjects();
            Engine.Singleton.SceneManager.DestroySceneNode(Node);
            Engine.Singleton.SceneManager.DestroyEntity(Entity);
            Body.Dispose();
            Body = null;

            base.Destroy();
        }
    }
}
