# EntityEventSystem
A simple event system for use with Unity's DOTS framework

# Overview
EntityEventSystem is a simple system for publishing and subscribing to *events as entities*. I'm steadily transitioning most of my projects from [my own ECS framework](https://github.com/tbriley/AlphaECS), where I included a simple wrapper around UniRx's MessageBroker, to Unity's. In plainspeak, previously if you wanted to arbitrarily abstract and communicate data between systems, for example, when something was damaged, you could do something like:

```
public class DamageEvent
{
  public Entity Attacker;
  public Entity Target;
  public int Amount;
  
  public DamageEvent(Entity attacker, Entity target, int amount)
  {
    Attacker = attacker;
    Target = target;
    Amount = amount;
  }
}

public class DamageSystem
{
  void Update()
  {
    if(...some logic which tests and checks whether somethings been damaged...)
    {
        EventSystem.Publish(new DamageEvent(attacker, target, 30));
    }
  }
}
```

There could then be any number of systems that want to know when something has been damaged and deal with it in some way, like a `DamageFXSystem` that maybe spawns some particles or plays some sound effects. To subscribe to the event was something like:

```
public class DamageFXSystem
{
  [SerializeField] AudioSource audio;
  
  void Start()
  {
    EventSystem.OnEvent<DamageEvent>(evt =>
    {
      //some logic can go here, like playing different sound effects based on the amount of damage dealt
      if(evt.Amount > 0 && evt.Amount < 10)
        audio.Play("MinDamage:);
      else
        audio.Play("MaxDamage");
    });
  }
}
```

I won't go too much into Unity's ECS approach, but we generally try to keep things in systems and communicate data as entities. Most of the ideas for this were pulled and poked and prodded from sources like https://forum.unity.com/threads/how-to-animate-in-ecs.524904/ and https://forum.unity.com/threads/batch-entitycommandbuffer.593569/, but I fit them to my own style + needs and tried to keeps things simple, even at the cost of some performance, as a large amount of what I use the system for is communicating data between the `GameObject` world and `ECS` world.

# Example
The only example included at the moment is `PointerPublisherBehaviour`, which I use to communicate click events in the GameObject world to systems in the ECS world. Put `GameObjectEntity` and `PointerClickPublisher` on a button, and in a system you can then do something like:

```
using Unity.Entities;

public class AnimatedButtonSystem : ComponentSystem
{
  protected override void OnUpdate()
  {
    ForEach((ref PointerEvent pointerEvent) =>
    {
      if (pointerEvent.EventType != PointerEventType.Click)
        return;

      var pointerEntity = pointerEvent.Entity;
      ForEach((Entity entity, Button button, Animator animator) =>
      {
        if(pointerEntity == entity)
        {
          animator.Play("OnClick");
        }
      });
    });
  }
}
```

Here we `listen` for pointer click events, check if the thing we clicked on was one of our animated buttons, and if so, play an animation.

# What's Next
Keep-it-simple for now. Will continue adding glue bits like `PointerPublisherBehaviour` as needed. Hopefully eventually there is a better built-in-way of handling these things and I can deprecate a lot of this.
