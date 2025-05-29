using HowlEngine.ECS;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using HowlEngine.Collections;

namespace HowlEngine.Graphics;

public class SpriteRenderer{
    private Dictionary<string, TextureAtlas> atlases;
    private StructPool<Sprite> staticSprites;
    private StructPool<AnimatedSprite> animatedSprites;

    /// <summary>
    /// Creates a new SpriteRenderer instance.
    /// </summary>
    /// <param name="atlases"></param>
    /// <param name="staticSpritesAmount"></param>
    /// <param name="animatedSpritesAmount"></param>
    public SpriteRenderer(Dictionary<string, string> atlasesToLoad, int staticSpritesAmount, int animatedSpritesAmount){
        staticSprites = new StructPool<Sprite>(staticSpritesAmount);
        animatedSprites = new StructPool<AnimatedSprite>(animatedSpritesAmount);
        atlases = new Dictionary<string, TextureAtlas>();
        LoadTextureAtlas(atlasesToLoad);
    }

    /// <summary>
    /// Gets a WeakReference to a TextureAtlas stored within this sprite renderer.
    /// </summary>
    /// <param name="atlasName">The sprecified name of the TextureAtlas to retrieve from the internal data structure.</param>
    /// <returns>A WeakReference to the TextureAtlas.</returns>
    public WeakReference<TextureAtlas> GetTextureAtlas(string atlasName){
        return new WeakReference<TextureAtlas>(atlases[atlasName]);
    }

    /// <summary>
    /// Allocates A Sprite to the internal data structure.
    /// </summary>
    /// <param name="sprite">The data to assign to the newly allocated slot.</param>
    /// <returns>A Token to the data's position in the internal data structure.</returns>
    public Token AllocateStaticSprite(Sprite sprite){
        
        // Allocate

        Token token = staticSprites.Allocate();
        
        // return invalid token if it could not be allocated.

        if(!token.IsValid){
            return token;
        }

        // set data.

        staticSprites.TryGetData(ref token).Data = sprite;
        
        // return.

        return token;
    }

    /// <summary>
    /// Allocates a AnimatedSprite to the internal data structure.
    /// </summary>
    /// <param name="sprite">The data to assign to the newly allocated slot.</param>
    /// <returns>A Token to the data's position in the internal data structure.</returns>
    public Token AllocateAnimatedSprite(AnimatedSprite sprite){
        
        // Allocate.

        Token token = animatedSprites.Allocate();

        // return invalid token if it could not be allocated.

        if(!token.IsValid){
            return token;
        }

        // set data.

        animatedSprites.TryGetData(ref token).Data = sprite;

        // return;

        return token;
    }

    public Token AllocateAnimatedSprite(string atlasName, string animationName){
        
        // Allocate.

        Token token = animatedSprites.Allocate();

        // return invalid token if it could not be allocated.

        if(!token.IsValid){
            return token;
        }

        // set data.
        animatedSprites.TryGetData(ref token).Data = atlases[atlasName].CreateAnimatedSprite(animationName);

        // return;

        return token;
    }


    /// <summary>
    /// Frees a Sprite from the internal data structure at a given index.
    /// </summary>
    /// <param name="index">The specified index to free at.</param>
    public void FreeStaticSprite(int index){
        staticSprites.Free(index);
    }

    /// <summary>
    /// Frees a AnimatedSprite from the internal data structure at a given index.
    /// </summary>
    /// <param name="index">The specified index to free at.</param>
    public void FreeAnimatedSprite(int index){
        animatedSprites.Free(index);
    }

    /// <summary>
    /// Sets the Position of a Sprite within this SpriteRenderer.
    /// </summary>
    /// <param name="token">The specified Token to reference a Sprite within the internal data structure.</param>
    /// <param name="position">The position to assign to the specified Sprite.</param>
    public void SetStaticSpritePosition(ref Token token, Vector2 position){
        RefView<Sprite> sprite = GetStaticSprite(ref token);
        if(sprite.IsValid){
            sprite.Data.Position = position;
        }
    }

    /// <summary>
    /// Sets the Position of a AnimatedSprite within this SpriteRenderer.
    /// </summary>
    /// <param name="token">The specified Token to reference a Sprite within the internal data structure.</param>
    /// <param name="position">The position to assign to the specified AnimatedSprite.</param>
    public void SetAnimatedSpritePosition(ref Token token, Vector2 position){
        RefView<AnimatedSprite> sprite = GetAnimatedSprite(ref token);
        if(sprite.IsValid){
            sprite.Data.Position = position;
        }
    }

    /// <summary>
    /// Gets a RefView to directly access a Sprite within this SpriteRenderer.
    /// </summary>
    /// <param name="token">The specified Token to reference a Sprite within the internal data structure.</param>
    /// <returns>A RefView of the retrieved data by the specified Token.</returns>
    public RefView<Sprite> GetStaticSprite(ref Token token){
        return staticSprites.TryGetData(ref token);
    }

    /// <summary>
    /// Gets a RefView to directly access a AnimatedSprite within this SpriteRenderer.
    /// </summary>
    /// <param name="token">The specified Token to reference a Animated Sprite within the internal data structure.</param>
    /// <returns>A RefView of the retrieved data by the specified Token.</returns>
    public RefView<AnimatedSprite> GetAnimatedSprite(ref Token token){
        return animatedSprites.TryGetData(ref token);
    }

    /// <summary>
    /// Updates all AnimatedSprites stored within the internal data structure.
    /// </summary>
    /// <param name="gameTime"></param>
    public void Update(GameTime gameTime){

        // Update animated sprites.

        for(int i = 0; i < animatedSprites.Capacity; i++){

            // Skip the slot if it is not active.

            if(animatedSprites.IsSlotActive(i) == false){
                continue;
            }

            // Get the animated sprite to update.

            ref AnimatedSprite sprite = ref animatedSprites.GetData(i);
            
            // sprite.ElapsedTime += TimeSpan.FromSeconds(HowlApp.DeltaTime);
            sprite.ElapsedTime += gameTime.ElapsedGameTime;
            if(sprite.ElapsedTime >= sprite.Animation.Interval){
                sprite.ElapsedTime -= sprite.Animation.Interval;
                sprite.CurrentFame = sprite.CurrentFame >= sprite.Animation.Frames.Length-1? 0 : sprite.CurrentFame += 1;
                sprite.TextureRegion = sprite.Animation.Frames[sprite.CurrentFame];
            } 
        }
    }

    public void DrawAll(SpriteBatch spriteBatch, SpriteSortMode sortMode = SpriteSortMode.Deferred, BlendState blendState = null, SamplerState samplerState = null, DepthStencilState depthStencilState = null, RasterizerState rasterizerState = null, Effect effect = null, Matrix? transformMatrix = null){
        for(int i = 0; i < staticSprites.Capacity; i++){
            // Skip the slot if it is not active.

            if(staticSprites.IsSlotActive(i) == false){
                continue;
            }
            
            // Get the sprite the draw.

            ref Sprite sprite = ref staticSprites.GetData(i);
            
            // Draw if the texture atlas is currently sill in memory.

            if(sprite.Texture.TryGetTarget(out Texture2D texture)){
                spriteBatch.Draw(
                    texture,
                    sprite.Position, 
                    sprite.TextureRegion.SourceRect, 
                    sprite.Color, 
                    sprite.Rotation, 
                    sprite.Origin, 
                    sprite.Scale,
                    sprite.Effects, 
                    sprite.Layer
                ); 
            }else{
                sprite.Texture = null;
            }
        }

        for(int i = 0; i < animatedSprites.Capacity; i++){
            // Skip the slot if it is not active.

            if(animatedSprites.IsSlotActive(i) == false){
                continue;
            }
            
            // Get the sprite the draw.

            ref AnimatedSprite sprite = ref animatedSprites.GetData(i);
            
            // Draw if the texture atlas is currently sill in memory.

            if(sprite.TextureAtlas.TryGetTarget(out TextureAtlas atlas)){
                spriteBatch.Draw(
                    atlas.Texture,
                    sprite.Position, 
                    sprite.TextureRegion.SourceRect, 
                    sprite.Color, 
                    sprite.Rotation, 
                    sprite.Origin, 
                    sprite.Scale,
                    sprite.Effects, 
                    sprite.Layer
                ); 
            }else{
                sprite.TextureAtlas = null;
            }
        }

    }

    public void LoadTextureAtlas(string atlasName, string filePath){
        atlases.Add(atlasName, TextureAtlas.FromFile(filePath));
    }

    public void LoadTextureAtlas(Dictionary<string, string> atlasesToLoad){
        foreach(KeyValuePair<string, string> kvp in atlasesToLoad){
            atlases.Add(kvp.Key, TextureAtlas.FromFile(kvp.Value));
        }
    }

    public void DrawStaticSprites(){

    }

    public void FreeTextures(){
        // dispose texture atlases.
        // clear the texture atlas list.
        // GC.Collect().
    }
    
}