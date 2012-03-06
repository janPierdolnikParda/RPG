using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using MogreNewt;

namespace Gra
{
    public class Enemy : SelectableObject, ISomethingMoving
    {
        public enum StateTypes
        {
            IDLE,
            WALK,
            ATTACK,
            DEAD
        }
  
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

        public AnimationState walkAnim;
        public AnimationState idleAnim;
        public AnimationState attackAnim;

        public bool TalkPerm;
        public bool InventoryPerm;

        public bool PickItemOrder;
        public bool MoveOrder;
        public bool MoveOrderBack;

        public bool GetSwordOrder;
        public bool HideSwordOrder;
        bool _RunOrder;

		public AnimationState Animation(string anim)
		{
			return Entity.GetAnimationState(anim);
		}

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

        public float TurnDelta;

        public bool FollowPathOrder;
        public List<Vector3> WalkPath;
        public static DecTree.Enemies.e_Node Tree = new EnemyDecTree();


        //////////////////////////////////////////////
        //              Moje zmienne:
        //////////////////////////////////////////////

        Container Container;
        public bool isContainer;
        bool isSeen;
        bool isReachable;
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

        public Prize DropPrize;
		
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

        Character.FriendType _FriendlyType;
        public Character.FriendType FriendlyType
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

        StateTypes _State;
        public StateTypes State
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

        int _DropExp;
        public int DropExp
        {
            get
            {
                return _DropExp;
            }

            set
            {
                _DropExp = value;
            }
        }

        String _ProfName;
        public String ProfName
        {
            get
            {
                return _ProfName;
            }

            set
            {
                _ProfName = value;
            }
        }

        public Enemy(CharacterProfile profile, bool czyPojemnik, float zasiegWzr, float zasiegOgl)
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
            Body.MaterialGroupID = Engine.Singleton.MaterialManager.EnemyMaterialID;

            Joint upVector = new MogreNewt.BasicJoints.UpVector(
            Engine.Singleton.NewtonWorld, Body, Vector3.UNIT_Y);

            collision.Dispose();

            isContainer = czyPojemnik;
            isSeen = false;
            isReachable = false;
            _ZasiegWzroku = zasiegWzr;
            _ZasiegOgolny = zasiegOgl;
            _Statistics = Profile.Statistics.statistics_Clone();
            State = StateTypes.IDLE;

            //DROPPRIZE KUFAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA

            if (Profile.DropPrizeID == "")
                Profile.DropPrizeID = "pPusty";

            DropPrize = PrizeManager.P[Profile.DropPrizeID].prize_Clone();
            List<DescribedProfile> lista_tym = new List<DescribedProfile>();
            List<DescribedProfile> lista_tym2 = new List<DescribedProfile>(DropPrize.ItemsList);

            if (DropPrize.ItemsList.Count > 2)
            {
                for (int i = 0; i < 2; i++)
                {
                    int Los = Engine.Singleton.Random.Next(lista_tym2.Count);
                    lista_tym.Add(lista_tym2[Los]);
                    lista_tym2.RemoveAt(Los);
                    DropPrize.ItemsList = new List<DescribedProfile>(lista_tym);
                }
            }

            else
                DropPrize.ItemsList = new List<DescribedProfile>(DropPrize.ItemsList);

            DropPrize.AmountGold = Engine.Singleton.Random.Next(DropPrize.AmountGold / 2, DropPrize.AmountGold + 1);
            DropExp = DropPrize.AmountExp;

            //PO DROPPRIZIE KUFAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA

            walkAnim = Entity.GetAnimationState("WalkLegs");
            idleAnim = Entity.GetAnimationState("IdleLegs");
            attackAnim = Entity.GetAnimationState("DrawSwordTorso");
			
			//Animation("IdleLegs").Enabled = true;
			//Animation("IdleLegs").Loop = true;
			FriendlyType = Profile.FriendlyType;
            ProfName = Profile.ProfileName;
            
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

        public void SetPrize(Prize NewPrize)
        {
            DropPrize = NewPrize.prize_Clone();
        }

        public void TurnTo(Vector3 Vect)
        {
            Orientation = Vector3.UNIT_Z.GetRotationTo((Vect - Position) * new Vector3(1, 0, 1));
        }

        public void Attack()
        {
            bool Blok = false;

            for (int i = 0; i < Engine.Singleton.HumanController.Character.Statistics.Ataki; i++)
            {
                bool Kryt = false;
                bool BlokNieUdany = true;
                bool Unikniety = false;

                //if (Engine.Singleton.HumanController.Character.FocusedEnemy != null)
                //{
                    if (Engine.Singleton.Procenty(Statistics.WalkaWrecz))
                    {
                        if (Engine.Singleton.Procenty(50))  // REAKCJA WROGA
                        {
                            if (!Blok)
                            {
                                if (Engine.Singleton.Procenty(50)) // BLOK!
                                {
                                    Blok = true;

                                    if (Engine.Singleton.Random.Next(101) <= Engine.Singleton.HumanController.Character.Statistics.WalkaWrecz)
                                    {
                                        BlokNieUdany = false;
                                    }
                                }

                                else
                                {
                                    //    UNIK !!

                                    if (Engine.Singleton.Random.Next(101) <= Engine.Singleton.HumanController.Character.Statistics.Zrecznosc)
                                        Unikniety = true;
                                }
                            }

                            else
                            {
                                //    UNIK !!

                                if (Engine.Singleton.Random.Next(101) <= Engine.Singleton.HumanController.Character.Statistics.Zrecznosc)
                                    Unikniety = true;
                            }
                        }

                        if (BlokNieUdany && !Unikniety)
                        {
                            int Obrazenia = Engine.Singleton.Kostka(1, 6);

                            if (Engine.Singleton.Procenty(5))
                            {
                                Kryt = true;
                                Obrazenia *= 2;
                            }

                            Obrazenia = Obrazenia + Statistics.Sila
                                - Engine.Singleton.HumanController.Character.Statistics.Wytrzymalosc;

                            if (Obrazenia < 0)
                                Obrazenia = 0;

                            Engine.Singleton.HumanController.Character.Statistics.aktualnaZywotnosc -= Obrazenia;

                            if (Kryt)
                                Engine.Singleton.HumanController.HUD.LogAdd(Profile.DisplayName + " trafia cie za " + Obrazenia.ToString() + " (KRYT!)", new ColourValue(0.7f, 0.4f, 0));
                            else
                                Engine.Singleton.HumanController.HUD.LogAdd(Profile.DisplayName + " trafia cie za " + Obrazenia.ToString(), new ColourValue(0.4f, 0.6f, 0.8f));
                        }
                    }

                    else
                        Engine.Singleton.HumanController.HUD.LogAdd(Profile.DisplayName + " cie nie trafia", new ColourValue(0.4f, 0.5f, 0.9f));
                //}

            }
        }

        public override void Update()
        {
            walkAnim = Entity.GetAnimationState("WalkLegs");
            idleAnim = Entity.GetAnimationState("IdleLegs");
            attackAnim = Entity.GetAnimationState("DrawSwordTorso");
            Tree.e_Visit(this);

            if (Statistics.aktualnaZywotnosc <= 0)
                State = StateTypes.DEAD;

            double Distance = Engine.Distance(Position, Engine.Singleton.HumanController.Character.Position);
            Distance = System.Math.Abs(Distance);

            if (Distance <= _ZasiegWzroku && State != StateTypes.DEAD)
            {
                PredicateRaycast raycast = new PredicateRaycast((b => !(b.UserData is TriggerVolume)));
                raycast.Go(Engine.Singleton.NewtonWorld, Position + Profile.HeadOffset, Engine.Singleton.HumanController.Character.Position);
                
                if (raycast.Contacts.Count <= 1)
                {
                    isSeen = true;

                    TurnTo(Engine.Singleton.HumanController.Character.Position);
                }

                else
                    isSeen = false;
            }

            if (Distance <= _ZasiegOgolny)
            {
                isReachable = true;
            }

            else
                isReachable = false;

            if ((isReachable == true) && Distance > 1 && State != StateTypes.DEAD && FriendlyType == Character.FriendType.ENEMY)
            {
                Engine.Singleton.CurrentLevel.navMesh.AStar(Position, Engine.Singleton.HumanController.Character.Position);
                if (Engine.Singleton.CurrentLevel.navMesh.TriPath.Count > 1)
                {
                    Engine.Singleton.CurrentLevel.navMesh.GetPortals();
                    WalkPath = Engine.Singleton.CurrentLevel.navMesh.Funnel();

                    idleAnim.Enabled = false;
                    walkAnim.Enabled = true;
                    attackAnim.Enabled = false;
                    walkAnim.Loop = true;
                    walkAnim.AddTime(1.0f / 90.0f);

                    FollowPathOrder = true;
					//MoveOrder = true;
                    State = StateTypes.WALK;
                }
            }

            else
            {
                FollowPathOrder = false;

                if (State == StateTypes.WALK)
                    State = StateTypes.IDLE;

                switch (State)
                {
                    case StateTypes.IDLE:
                        walkAnim.Enabled = false;
                        idleAnim.Enabled = true;
                        attackAnim.Enabled = false;
                        idleAnim.Loop = true;
                        idleAnim.AddTime(1.0f / 90.0f);
                        break;
                    case StateTypes.DEAD:
                        //Animacja deda.
                        break;
                    //Animacja ataku bedzie tam nizej w case StateTypes.Attack, nie tutaj!!

                }
                if (isReachable)
                {
                    switch (State)
                    {
                        case StateTypes.IDLE:
                            //Console.WriteLine(FriendlyType.ToString());
                            if (FriendlyType == Character.FriendType.ENEMY)
                            {
                                State = StateTypes.ATTACK;
                            }
                            break;
                        
                        case StateTypes.ATTACK:
                            walkAnim.Enabled = false;
                            idleAnim.Enabled = false;
                            attackAnim.Enabled = true;
                            attackAnim.Loop = false;
                            attackAnim.AddTime(1.0f / 90.0f);
                            
                            if (Engine.Singleton.HumanController.Character.Statistics.aktualnaZywotnosc > 0 && attackAnim.HasEnded)
                            {
                                attackAnim.TimePosition = 0;
                                Attack();
                                //Engine.Singleton.HumanController.Character.Statistics.aktualnaZywotnosc -= Statistics.Sila;
                            }
                            break;

                        case StateTypes.DEAD:
                            IsContainer = true;
                            break;
                    }
                }
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
