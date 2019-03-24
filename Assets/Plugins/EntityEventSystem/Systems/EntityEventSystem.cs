using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

#if UNITY_2019_1_OR_NEWER
[UpdateBefore(typeof(EndSimulationEntityCommandBufferSystem))]
#else
[UpdateBefore(typeof(EndFrameBarrier))]
#endif
public class EntityEventSystem : ComponentSystem
{
    Dictionary<Type, IEventBatch> dataEvents;
    Dictionary<Type, IEventBatch> objectEvents;

    protected override void OnCreateManager()
    {
        base.OnCreateManager();

        dataEvents = new Dictionary<Type, IEventBatch>();
        objectEvents = new Dictionary<Type, IEventBatch>();
    }

    protected override void OnDestroyManager()
    {
        base.OnDestroyManager();

        dataEvents?.Clear();
        objectEvents?.Clear();
    }

    protected override void OnUpdate()
    {
        //HACK + TODO -> force the system to update
        Entities.ForEach((Entity entity) => { });

        Entities.ForEach((Entity entity, ref EventComponentData eventData, Transform transform) =>
        {
            GameObjectEntity.Destroy(transform.gameObject);
        });

        Entities.ForEach((Entity entity, ref EventComponentData eventData) =>
        {
            PostUpdateCommands.DestroyEntity(entity);
        });

        foreach(var kvp in dataEvents)
        {
            kvp.Value.Update(EntityManager);
        }
        dataEvents?.Clear();

        foreach (var kvp in objectEvents)
        {
            kvp.Value.Update(EntityManager);
        }
        objectEvents?.Clear();
    }

    public void PublishData<T>(T evt)
        where T : struct, IComponentData
    {
        var type = evt.GetType();
        if (!dataEvents.TryGetValue(type, out var batch))
        {
            batch = dataEvents[type] = new EventDataBatch<T>();
        }
        ((EventDataBatch<T>)batch).Queue.Add(evt);
    }

    public void PublishObject(Component evt)
    {
        var type = evt.GetType();
        if(!objectEvents.TryGetValue(type, out var batch))
        {
            batch = objectEvents[type] = new EventObjectBatch();
        }
        ((EventObjectBatch)batch).Queue.Add(evt);
    }

    class EventDataBatch<T> : IEventBatch
        where T : struct, IComponentData
    {
        public List<T> Queue = new List<T>();

        public void Update(EntityManager entityManager)
        {
            foreach (var evt in Queue)
            {
                var entity = entityManager.CreateEntity();
                entityManager.AddComponentData(entity, evt);
                entityManager.AddComponentData(entity, new EventComponentData());
            }
        }
    }

    class EventObjectBatch : IEventBatch
    {
        public List<Component> Queue = new List<Component>();

        public void Update(EntityManager entityManager)
        {
            foreach (var evt in Queue)
            {
                var entity = GameObjectEntity.AddToEntityManager(entityManager, evt.gameObject);
                entityManager.AddComponentData(entity, new EventComponentData());
            }
        }
    }

    interface IEventBatch
    {
        void Update(EntityManager entityManager);
    }
}


