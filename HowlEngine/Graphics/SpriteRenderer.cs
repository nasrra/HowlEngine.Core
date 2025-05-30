using HowlEngine.ECS;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using HowlEngine.Collections;
using HowlEngine.SceneManagement;
using HowlEngine.Graphics.Config;

namespace HowlEngine.Graphics;

public class SpriteRenderer{
    private Dictionary<string, Tileset> tilesets;
    private StructPool<StaticSprite> staticSprites;
    private StructPool<AnimatedSprite> animatedSprites;

    /// <summary>
    /// Creates a new SpriteRenderer instance.
    /// </summary>
    /// <param name="atlases"></param>
    /// <param name="staticSpritesAmount"></param>
    /// <param name="animatedSpritesAmount"></param>
    public SpriteRenderer(List<string> tilesetsToLoad, int staticSpritesAmount, int animatedSpritesAmount){
        staticSprites = new StructPool<StaticSprite>(staticSpritesAmount);
        animatedSprites = new StructPool<AnimatedSprite>(animatedSpritesAmount);
        tilesets = new Dictionary<string, Tileset>();
        foreach(string path in tilesetsToLoad){
            LoadTilesetData(path);
        }
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

    public Token AllocateStaticSprite(string atlasName, string spriteName){
        
        // Allocate.

        Token token = staticSprites.Allocate();

        // return invalid token if it could not be allocated.

        if(!token.IsValid){
            return token;
        }

        // set data.
        staticSprites.TryGetData(ref token).Data = tilesets[atlasName].CreateStaticSprite(Vector2.Zero, spriteName, atlasName);

        // return;

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
        animatedSprites.TryGetData(ref token).Data = tilesets[atlasName].CreateAnimatedSprite(Vector2.Zero, animationName, atlasName);

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
    }

    // public void LoadTilesetData(Dictionary<long, string> tilesetsToLoad){
    //     foreach (KeyValuePair<long, string> kvp in tilesetsToLoad){
    //         LoadTilesetData((int)kvp.Key, kvp.Value);
    //     }
    // }

    public void LoadTilesetData(TilesetToken token){
        LoadTilesetData(token.Source);
    }

    public void LoadTilesetData(string filePath){
        string name = System.IO.Path.GetFileNameWithoutExtension(filePath);
        tilesets.Add(name, new Tileset());
        Tileset tileset = tilesets[name];
        tileset.InitialiseFromJson(filePath);
    }

    public void UnloadTilesetData(string tilesetName){
        tilesets[tilesetName].Dispose();
        tilesets.Remove(tilesetName);
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

            if(tilesets.ContainsKey(sprite.TilesetId)){
                spriteBatch.Draw(
                    tilesets[sprite.TilesetId].Texture,
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

            if(tilesets.ContainsKey(sprite.TilesetId)){
                spriteBatch.Draw(
                    tilesets[sprite.TilesetId].Texture,
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