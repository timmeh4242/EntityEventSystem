//using AlphaECS;
//using AlphaECS.Unity;
//using System;
//using System.Collections.Generic;
//using UnityEngine.UI;
//using UniRx;
//using UniRx.Triggers;
using Unity.Entities;

public class EventPublishingSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        //throw new System.NotImplementedException();
    }
    //IGroup eventButtons;

    //public override void Initialize(IEventSystem eventSystem, IPoolManager poolManager, GroupFactory groupFactory)
    //{
    //    base.Initialize(eventSystem, poolManager, groupFactory);

    //    eventButtons = this.CreateGroup(new HashSet<Type> { typeof(Button), typeof(EventDataWrapper) });
    //}

    //public override void OnEnable()
    //{
    //    base.OnEnable();

    //    eventButtons.OnAdd().Subscribe(entity =>
    //    {
    //        var button = entity.GetComponent<Button>();
    //        var eventDataWrapper = entity.GetComponent<EventDataWrapper>();

    //        button.OnPointerClickAsObservable().Subscribe(_ =>
    //        {
    //            EventSystem.Publish(eventDataWrapper.EventData);
    //        }).AddTo(this.Disposer).AddTo(eventDataWrapper.Disposer);
    //    }).AddTo(this.Disposer);
    //}
}