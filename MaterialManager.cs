using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using MogreNewt;

namespace Gra
{
    class MaterialManager
    {
        public MaterialID LevelMaterialID;
        public MaterialID CharacterMaterialID;
        public MaterialID TriggerVolumeMaterialID;
        public MaterialID DescribedMaterialID;
        public MaterialID CharacterSensorMaterialID;
        public MaterialID EnemyMaterialID;

        MaterialPair TriggerVolumeCharacterPair;
        MaterialPair CharacterSensorPair;
        MaterialPair SensorLevelPair;
        MaterialPair SensorDescribedObjectPair;
        MaterialPair SensorTriggerVolumePair;
        MaterialPair DescribedTriggerVolumePair;
        MaterialPair EnemyTriggerVolumePair;


        public void Initialise()
        {
            LevelMaterialID = new MaterialID(Engine.Singleton.NewtonWorld);
            CharacterMaterialID = new MaterialID(Engine.Singleton.NewtonWorld);
            TriggerVolumeMaterialID = new MaterialID(Engine.Singleton.NewtonWorld);
            DescribedMaterialID = new MaterialID(Engine.Singleton.NewtonWorld);
            CharacterSensorMaterialID = new MaterialID(Engine.Singleton.NewtonWorld);
            EnemyMaterialID = new MaterialID(Engine.Singleton.NewtonWorld);

            TriggerVolumeCharacterPair = new MaterialPair(
                Engine.Singleton.NewtonWorld,
                CharacterMaterialID, TriggerVolumeMaterialID);
            TriggerVolumeCharacterPair.SetContactCallback(new TriggerVolumeGameObjectCallback());

            CharacterSensorPair = new MaterialPair(
                Engine.Singleton.NewtonWorld,
                CharacterMaterialID, CharacterSensorMaterialID);
            CharacterSensorPair.SetContactCallback(new SensorGameObjectCallback());

            SensorLevelPair = new MaterialPair(
                Engine.Singleton.NewtonWorld,
                LevelMaterialID, CharacterSensorMaterialID);
            SensorLevelPair.SetContactCallback(new IgnoreCollisionCallback());

            SensorTriggerVolumePair = new MaterialPair(
                Engine.Singleton.NewtonWorld,
                TriggerVolumeMaterialID, CharacterSensorMaterialID);
            SensorTriggerVolumePair.SetContactCallback(new IgnoreCollisionCallback());

            SensorDescribedObjectPair = new MaterialPair(
                Engine.Singleton.NewtonWorld,
                DescribedMaterialID, CharacterSensorMaterialID);
            SensorDescribedObjectPair.SetContactCallback(new SensorGameObjectCallback());

            DescribedTriggerVolumePair = new MaterialPair(
                Engine.Singleton.NewtonWorld,
                DescribedMaterialID, TriggerVolumeMaterialID);
            DescribedTriggerVolumePair.SetContactCallback(new IgnoreCollisionCallback());

            EnemyTriggerVolumePair = new MaterialPair(
                Engine.Singleton.NewtonWorld,
                EnemyMaterialID, TriggerVolumeMaterialID);
            EnemyTriggerVolumePair.SetContactCallback(new IgnoreCollisionCallback());

        }

        class SensorGameObjectCallback : ContactCallback
        {
            public override int UserAABBOverlap(ContactMaterial material, Body body0, Body body1, int threadIndex)
            {
                Vector3[] contactPoints;
                Vector3[] contactNormals;
                float[] contactPenetration;

                if (MogreNewt.CollisionTool.CollisionCollide(
                    Engine.Singleton.NewtonWorld,
                    2,
                    body0.Collision,
                    body0.Orientation,
                    body0.Position,
                    body1.Collision,
                    body1.Orientation,
                    body1.Position,
                    out contactPoints,
                    out contactNormals,
                    out contactPenetration,
                    threadIndex) != 0)
                {
                    if (body0.UserData != body1.UserData)
                        if (body0.UserData is Character && body0.MaterialGroupID == Engine.Singleton.MaterialManager.CharacterSensorMaterialID)
                        {
                            Character character = body0.UserData as Character;
                            character.Contacts.Add(body1.UserData as GameObject);
                        }
                        else
                        {
                            Character character = body1.UserData as Character;
                            character.Contacts.Add(body0.UserData as GameObject);
                        }
                }
                return 0;
            }
        }

        class SensorActivatorCallback : ContactCallback
        {
            public override int UserAABBOverlap(ContactMaterial material, Body body0, Body body1, int threadIndex)
            {
                Vector3[] contactPoints;
                Vector3[] contactNormals;
                float[] contactPenetration;

                if (MogreNewt.CollisionTool.CollisionCollide(
                    Engine.Singleton.NewtonWorld,
                    2,
                    body0.Collision,
                    body0.Orientation,
                    body0.Position,
                    body1.Collision,
                    body1.Orientation,
                    body1.Position,
                    out contactPoints,
                    out contactNormals,
                    out contactPenetration,
                    threadIndex) != 0)
                {
                    if (body0.UserData != body1.UserData)
                        if (body0.UserData is Character && body0.MaterialGroupID == Engine.Singleton.MaterialManager.CharacterSensorMaterialID)
                        {
                            Character character = body0.UserData as Character;
                            character.Contacts.Add(body1.UserData as GameObject);
                        }
                        else
                        {
                            Character character = body1.UserData as Character;
                            character.Contacts.Add(body0.UserData as GameObject);
                        }
                }
                return 0;
            }
        }

        class IgnoreCollisionCallback : ContactCallback
        {
            public override int UserAABBOverlap(ContactMaterial material, Body body0, Body body1, int threadIndex)
            {
                return 0;
            }
        }


        class TriggerVolumeGameObjectCallback : ContactCallback
        {
            public override int UserAABBOverlap(ContactMaterial material, Body body0, Body body1, int threadIndex)
            {
                Vector3[] contactPoints;
                Vector3[] contactNormals;
                float[] contactPenetration;

                if (MogreNewt.CollisionTool.CollisionCollide(
                    Engine.Singleton.NewtonWorld,
                    2,
                    body0.Collision,
                    body0.Orientation,
                    body0.Position,
                    body1.Collision,
                    body1.Orientation,
                    body1.Position,
                    out contactPoints,
                    out contactNormals,
                    out contactPenetration,
                    threadIndex) != 0)
                {
                    if (body0.UserData is TriggerVolume)
                    {
                        TriggerVolume triggerVolume = body0.UserData as TriggerVolume;
                        triggerVolume.HandleGameObject((Character)body1.UserData);
                    }
                    else
                    {
                        TriggerVolume triggerVolume = body1.UserData as TriggerVolume;
                        triggerVolume.HandleGameObject((Character)body0.UserData);
                    }
                }
                return 0;
            }
        }
    }

}
