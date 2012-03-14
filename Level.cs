using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using MogreNewt;

namespace Gra
{
    class Level
    {
        Entity GraphicsEntity;
        SceneNode GraphicsNode;

        Entity CollisionEntity;
        SceneNode CollisionNode;

        Body Body;

        public String Name;

        public NavMesh navMesh = new NavMesh();

        public bool LoadNewMap = false;
        public String NewMapName;
        public String NewMapNav;

        void SetGraphicsMesh(String meshFile)
        {
            GraphicsNode = Engine.Singleton.SceneManager.RootSceneNode.CreateChildSceneNode();
            GraphicsEntity = Engine.Singleton.SceneManager.CreateEntity(meshFile);
            GraphicsNode.AttachObject(GraphicsEntity);
            GraphicsEntity.CastShadows = false;
        }

        void SetCollisionMesh(String meshFile)
        {
            CollisionNode = Engine.Singleton.SceneManager.RootSceneNode.CreateChildSceneNode();
            CollisionEntity = Engine.Singleton.SceneManager.CreateEntity(meshFile);
            CollisionNode.AttachObject(CollisionEntity);

            CollisionNode.SetVisible(false);

            MogreNewt.CollisionPrimitives.TreeCollisionSceneParser collision =
                new MogreNewt.CollisionPrimitives.TreeCollisionSceneParser(
               Engine.Singleton.NewtonWorld);
            collision.ParseScene(CollisionNode, true, 1);
            Body = new Body(Engine.Singleton.NewtonWorld, collision);
            collision.Dispose();
            Body.AttachNode(CollisionNode);
            Body.UserData = this;
            Body.MaterialGroupID = Engine.Singleton.MaterialManager.LevelMaterialID;
        }

        public void LoadLevel(String LevelName, String NavMeshName, bool isTheSame = false) 
        {			
            this.Name = LevelName;
            String Name = LevelName + ".mesh";
            NavMeshName = "Media/nav/" + NavMeshName + ".obj";
            SetGraphicsMesh(Name);

            navMesh.LoadFromOBJ(NavMeshName);

            if (isTheSame)
            {
                SetCollisionMesh(Name);
            }
            else
            {
                LevelName += "Col.mesh";
                SetCollisionMesh(LevelName);
            }
        }

        public void DeleteLevel()
        {
            Engine.Singleton.AutoSave(null);
            GraphicsNode.DetachAllObjects();
            CollisionNode.DetachAllObjects();
            Body.Dispose();
            Engine.Singleton.SceneManager.RootSceneNode.RemoveChild(GraphicsNode);
            Engine.Singleton.SceneManager.RootSceneNode.RemoveChild(CollisionNode);
        }
    }
}
