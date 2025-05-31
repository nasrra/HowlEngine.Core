using HowlEngine.ECS;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using HowlEngine.Collections;
using HowlEngine.SceneManagement;
using HowlEngine.Graphics.Config;
using System;

namespace HowlEngine.Graphics;

public class SpriteRenderer : IDisposable{
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
            LoadTilesetData(path, 1);
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

    public Token AllocateStaticSprite(string tilesetName, int tileId){
        ref Tileset tileset = ref GetTileset(tilesetName);
        return AllocateStaticSprite(tilesetName, tileset.TileIdToTileName(tileId));
    }

    public Token AllocateStaticSprite(string tilesetName, string spriteName){
        return AllocateStaticSprite(tilesetName, spriteName, Vector2.Zero);
    }

    public Token AllocateStaticSprite(string tilesetName, string spriteName, Vector2 position){
        
        // Allocate.

        Token token = staticSprites.Allocate();

        // return invalid token if it could not be allocated.

        if(!token.IsValid){
            return token;
        }

        // set data.
        staticSprites.TryGetData(ref token).Data = new StaticSprite(tilesets[tilesetName].TextureRegions[spriteName], position, tilesetName);

        // return;

        return token;
    }


    public void LoadTileMap(long[] tileIdMap, string tilesetName, int mapWidth, int mapHeight){
        ref Tileset tileset = ref GetTileset(tilesetName);
        int tileWidth = tileset.TileWidth;
        int tileHeight = tileset.TileHeight;
        int index = 0;
        for(int y = 0; y < mapHeight; y++){
            for(int x = 0; x < mapWidth; x++){
                if(tileIdMap[index] > 0){
                    AllocateStaticSprite(
                        tilesetName, 
                        tileset.TileIdToTileName((int)tileIdMap[index]), 
                        new Vector2(
                            (x % mapWidth) * tileWidth,
                            y * tileHeight
                        )
                    );
                }
                index++;
            }
        }
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
        animatedSprites.TryGetData(ref token).Data = new AnimatedSprite(tilesets[atlasName].Animations[animationName], Vector2.Zero, atlasName);

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

    public void LoadTilesetData(SceneManagement.Config.Tileset token){
        LoadTilesetData(token.Source, (int)token.Firstgid);
    }

    public void LoadTilesetData(string jsonPath, int firstGid){
        
        // get the name of the tileset.

        string name = System.IO.Path.GetFileNameWithoutExtension(jsonPath);
       
        // Allocate a new tileset.
        
        tilesets.Add(name, new Tileset());
        
        //  get a reference to the struct.
        
        ref Tileset tileset = ref System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault(tilesets, name, out bool exists);
        if(exists == true){
            tileset.FirstGid = firstGid;
            tileset.InitialiseFromJson(jsonPath);
        }
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

    public ref Tileset GetTileset(string tilesetName){
        ref Tileset tileset = ref System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault(tilesets, tilesetName, out bool exists);
        if(exists == false){
            throw new Exception($"[SpriteRenderer] {tilesetName} is either invalid tileset or has not been loaded.");
        }
        else{
            return ref tileset;
        }

    }

    // public void FreeTextures(){
    //     // dispose texture atlases.
    //     // clear the texture atlas list.
    //     // GC.Collect().
    // }

    public void Dispose(){

        // dispose all tilesets.
        
        foreach(string key in tilesets.Keys){
            ref Tileset tileset = ref System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault(tilesets, key, out bool exists);
            if(exists == true){
                tileset.Dispose();
            }
        }

        tilesets.Clear();

        // dispose of sprites.
    }
}