using System;
using HowlEngine.Collections;
using Microsoft.Xna.Framework;

namespace HowlEngine.ECS;

public abstract class Entity : IDisposable{
    public string Name;
    
    protected Token _id;
    public ref Token Id{
        get => ref _id;
    }
    public Vector2 Position {get; set;} = Vector2.Zero;

    public Entity(){
        
        // allocate this entity into the entity manager.
        
        _id = HowlApp.EntityManager.AllocateEntity(this);
    }

    public abstract void Start();

    public abstract void Update(GameTime gameTime);

    public abstract void FixedUpdate(GameTime gameTime);

    public virtual void Dispose(){

        // free this entity from the entity manager.
        HowlApp.EntityManager.FreeEntity(ref _id);
    }
}