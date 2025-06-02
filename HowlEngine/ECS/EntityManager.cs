using System;
using HowlEngine.Collections;
using HowlEngine.ECS;
using Microsoft.Xna.Framework;

public class EntityManager{
    private ClassPool<Entity> entities;

    public EntityManager(int entityAmount){
        entities = new ClassPool<Entity>(entityAmount);
    }

    public Token AllocateEntity(Entity entity){

        // Allocate.

        Token token = entities.Allocate();
        
        // return invalid token if it could not be allocated.

        if(!token.IsValid){
            return token;
        }

        // set data.

        RefView<Entity> entityRef = entities.TryGetData(ref token);
        entityRef.Data = entity;

        // return.

        return token;
    }

    public void FreeEntity(ref Token token){
        entities.Free(ref token);
    }

    public void DisposeAt(ref Token token){
        entities.DisposeAt(ref token);
    }

    public void Update(GameTime gameTime){
        for(int i = 0; i < entities.Capacity; i++){
            if(entities.IsSlotActive(i) == false){
                continue;
            }
            entities.GetData(i).Update(gameTime);
        }
    }

    public void FixedUpdate(GameTime gameTime){
        for(int i = 0; i < entities.Capacity; i++){
            if(entities.IsSlotActive(i) == false){
                continue;
            }
            entities.GetData(i).FixedUpdate(gameTime);
        }
    }
}