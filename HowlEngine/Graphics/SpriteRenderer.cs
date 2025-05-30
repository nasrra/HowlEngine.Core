using HowlEngine.ECS;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using HowlEngine.Collections;

namespace HowlEngine.Graphics;

public class SpriteRenderer{
    private Dictionary<long, Tileset> tilesets;
    private Dictionary<string, TextureAtlas> atlases;
    private StructPool<StaticSprite> staticSprites;
    private StructPool<AnimatedSprite> animatedSprites;
    private StructPool<TileSprite> tileSprites;

    /// <summary>
    /// Creates a new SpriteRenderer instance.
    /// </summary>
    /// <param name="atlases"></param>
    /// <param name="staticSpritesAmount"></param>
    /// <param name="animatedSpritesAmount"></param>
    public SpriteRenderer(Dictionary<string, string> atlasesToLoad, int staticSpritesAmount, int animatedSpritesAmount, int tileSpritesAmount){
        staticSprites = new StructPool<StaticSprite>(staticSpritesAmount);
        animatedSprites = new StructPool<AnimatedSprite>(animatedSpritesAmount);
        tileSprites = new StructPool<TileSprite>(tileSpritesAmount);
        atlases = new Dictionary<string, TextureAtlas>();
        tilesets = new Dictionary<long, Tileset>();
        LoadTextureAtlas(atlasesToLoad);
    }

    /// <summary>
    /// Allocates A Sprite to the internal data structure.
    /// </summary>
    /// <param name="sprite">The data to assign to the newly allocated slot.</param>
    /// <returns>A Token to the data's position in the internal data structure.</returns>
    public Token AllocateStaticSprite(StaticSprite sprite){
        
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
        animatedSprites.TryGetData(ref token).Data = atlases[atlasName].CreateAnimatedSprite(animationName, atlasName);

        // return;

        return token;
    }

    public Token AllocateTileSprite(Vector2 position, long firstGid, int tileId){

        // Allocate
        Token token = tileSprites.Allocate();

        // return invalid token if it could not be allocated.

        if(!token.IsValid){
            return token;
        }

        // set data.

        tileSprites.TryGetData(ref token).Data = tilesets[firstGid].CreateTileSprite(position, tileId);

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
        RefView<StaticSprite> sprite = GetStaticSprite(ref token);
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
    public RefView<StaticSprite> GetStaticSprite(ref Token token){
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
        DrawStaticSprites(spriteBatch);
        DrawAnimatedSprites(spriteBatch);
        DrawTileSprites(spriteBatch);
    }

    public void LoadTextureAtlas(Dictionary<string, string> atlasesToLoad){
        foreach(KeyValuePair<string, string> kvp in atlasesToLoad){
            LoadTextureAtlas(kvp.Key, kvp.Value);
        }
    }

    public void LoadTextureAtlas(string atlasName, string filePath){
        atlases.Add(atlasName, TextureAtlas.FromFile(filePath));
    }

    public void LoadTilesetData(Dictionary<long, string> tilesetsToLoad){
        foreach (KeyValuePair<long, string> kvp in tilesetsToLoad){
            LoadTilesetData((int)kvp.Key, kvp.Value);
        }
    }

    public void LoadTilesetData(int firstGid, string filePath){
        string json = System.IO.File.ReadAllText(filePath);
        Tileset tileset = Tileset.FromJson(json);
        tileset.FirstGid = firstGid; 
        tilesets.Add(firstGid, tileset);
    }

    public void UnloadTilesetData(int firstGid){
        tilesets[firstGid].Dispose();
        tilesets.Remove(firstGid);
    }

    public void DrawTileSprites(SpriteBatch spriteBatch){
        for(int i = 0; i < tileSprites.Capacity; i++){
            // Skip the slot if it is not active.

            if(tileSprites.IsSlotActive(i) == false){
                continue;
            }
            
            // Get the sprite the draw.

            ref TileSprite sprite = ref tileSprites.GetData(i);
            
            // Draw if the texture atlas is currently sill in memory.

            if(tilesets.ContainsKey(sprite.TilesetFirstGid)){
                spriteBatch.Draw(
                    tilesets[sprite.TilesetFirstGid].Texture,
                    sprite.Position, 
                    sprite.TextureRegion.SourceRect,
                    sprite.Color, 
                    sprite.Rotation, 
                    sprite.Origin, 
                    sprite.Scale,
                    sprite.Effects, 
                    sprite.Layer
                ); 
            }
        }       
    }

    public void DrawAnimatedSprites(SpriteBatch spriteBatch){
        for(int i = 0; i < animatedSprites.Capacity; i++){
            // Skip the slot if it is not active.

            if(animatedSprites.IsSlotActive(i) == false){
                continue;
            }
            
            // Get the sprite the draw.

            ref AnimatedSprite sprite = ref animatedSprites.GetData(i);
            
            // Draw if the texture atlas is currently sill in memory.

            if(atlases.ContainsKey(sprite.TextureAtlasId)){
                spriteBatch.Draw(
                    atlases[sprite.TextureAtlasId].Texture,
                    sprite.Position, 
                    sprite.TextureRegion.SourceRect, 
                    sprite.Color, 
                    sprite.Rotation, 
                    sprite.Origin, 
                    sprite.Scale,
                    sprite.Effects, 
                    sprite.Layer
                ); 
            }
        }
    }

    public void DrawStaticSprites(SpriteBatch spriteBatch){
        for(int i = 0; i < staticSprites.Capacity; i++){
            // Skip the slot if it is not active.

            if(staticSprites.IsSlotActive(i) == false){
                continue;
            }
            
            // Get the sprite the draw.

            ref StaticSprite sprite = ref staticSprites.GetData(i);
            
            // Draw if the texture atlas is currently sill in memory.

            if(atlases.ContainsKey(sprite.TextureAtlasId)){
                spriteBatch.Draw(
                    atlases[sprite.TextureAtlasId].Texture,
                    sprite.Position, 
                    sprite.TextureRegion.SourceRect, 
                    sprite.Color, 
                    sprite.Rotation, 
                    sprite.Origin, 
                    sprite.Scale,
                    sprite.Effects, 
                    sprite.Layer
                ); 
            }
        }
    }

    public void FreeTextures(){
        // dispose texture atlases.
        // clear the texture atlas list.
        // GC.Collect().
    }
    
}