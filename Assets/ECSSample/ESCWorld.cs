using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Rendering;
using Hsinpa.ECS.JobSystem;
using Hsinpa.ECS.System;

namespace Hsinpa.ECS.Sample {
    public class ESCWorld : MonoBehaviour
    {

        [SerializeField] private Mesh mesh;
        [SerializeField] private Material material;

        private SpriteSystem spriteSystem;
        private CustomInputSystem customInputSystem;

        void Start()
        {
            this.customInputSystem = new CustomInputSystem();
            World world = World.DefaultGameObjectInjectionWorld;
            EntityManager entityManager = world.EntityManager;

            this.spriteSystem = world.GetOrCreateSystem<SpriteSystem>();
            this.spriteSystem.SetUp(this.customInputSystem);

            EntityArchetype entityArchetype = entityManager.CreateArchetype(
                typeof(Translation),
                typeof(LocalToWorld)
            );

            Entity entity = entityManager.CreateEntity(entityArchetype);
            var meshDescription = new RenderMeshDescription(mesh: mesh, material: material, ShadowCastingMode.Off, receiveShadows: false);

            RenderMeshUtility.AddComponents(entity, entityManager, meshDescription);
        }


        private void Update()
        {
            ProcessSystem();
        }

        private void ProcessSystem() {
            this.spriteSystem.Update();
        }

    }
}
