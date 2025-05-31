using HowlEngine.ECS;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using HowlEngine.Collections;
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
            LoadTileset(path, 1);
        }
    }

    /// <summary>
    /// Allocates A StaticSprite to the internal data structure.
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
    /// Allocates a StaticSprite to the internal data structure and sets its texture to a specified tile within a loaded tileset.
    /// </summary>
    /// <param name="tilesetName">The name of the loaded tileset.</param>
    /// <param name="tileId">The specified tile-id on the tilesets source image.</param>
    /// <returns></returns>
    public Token AllocateStaticSprite(string tilesetName, int tileId){
        ref Tileset tileset = ref GetTileset(tilesetName);
        return AllocateStaticSprite(tilesetName, tileset.TileIdToTileName(tileId));
    }


    /// <summary>
    /// Allocates a StaticSprite to the internal data structure and sets its texture to a specified tile within a loaded tileset.
    /// </summary>
    /// <param name="tilesetName">The name of the loaded tileset.</param>
    /// <param name="spriteName">The specified name of a tile within the tilesets source image.</param>
    /// <returns></returns>
    
    public Token AllocateStaticSprite(string tilesetName, string spriteName){
        return AllocateStaticSprite(tilesetName, spriteName, Vector2.Zero);
    }


    /// <summary>
    /// Allocates a StaticSprite to the internal data structure and sets its texture to a specified tile within a loaded tileset.
    /// </summary>
    /// <param name="tilesetName">The name of the loaded tileset.</param>
    /// <param name="spriteName">The specified name of a tile within the tilesets source image.</param>
    /// <param name="position">The position where the sprite should be located.</param>
    /// <returns></returns>
    
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


    /// <summary>
    /// Loads a map of tiles by a given array of tile-ids.
    /// </summary>
    /// <param name="tileIdMap">The array that stores the tile-ids.</param>
    /// <param name="tilesetName">The name of the loaded tileset within this SpriteRenderer.</param>
    /// <param name="mapWidth">The width, in tile quantity, of the map.</param>
    /// <param name="mapHeight">The height, in tile quantity, of the map.</param>

    public void LoadTileMap(long[] tileIdMap, string tilesetName, int mapWidth, int mapHeight){

        // Get a reference to the tileset.

        ref Tileset tileset = ref GetTileset(tilesetName);
        
        // Lifting of invarience.

        int tileWidth = tileset.TileWidth;
        int tileHeight = tileset.TileHeight;
        int index = 0;
        
        // Create static sprites in accordance with the specified map.

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


    /// <summary>
    /// Allocates a AnimatedSprite to the internal data structure and sets its texture to a specified tile within a loaded tileset.
    /// </summary>
    /// <param name="tilesetName">The name of the loaded Tileset.</param>
    /// <param name="animationName">The name of an Animation within the Tileset.</param>
    /// <returns></returns>

    public Token AllocateAnimatedSprite(string tilesetName, string animationName){
        return AllocateAnimatedSprite(tilesetName, animationName, Vector2.Zero);        
    }


    /// <summary>
    /// Allocates a AnimatedSprite to the internal data structure and sets its texture to a specified tile within a loaded tileset.
    /// </summary>
    /// <param name="tilesetName">The name of the loaded Tileset.</param>
    /// <param name="animationName">The name of an Animation within the Tileset.</param>
    /// <returns></returns>

    public Token AllocateAnimatedSprite(string tilesetName, string animationName, Vector2 position){
        
        // Allocate.

        Token token = animatedSprites.Allocate();

        // return invalid token if it could not be allocated.

        if(!token.IsValid){
            return token;
        }

        // set data.
        animatedSprites.TryGetData(ref token).Data = new AnimatedSprite(tilesets[tilesetName].Animations[animationName], position, tilesetName);

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


    /// <summary>
    /// Creates a Tileset instance within this SpriteRenderer.
    /// </summary>
    /// <param name="config">The json config instance used to create the runtime Tileset instance.</param>

    public void LoadTileset(SceneManagement.Config.Tileset config){
        LoadTileset(config.Source, (int)config.Firstgid);
    }


    /// <summary>
    /// Create a Tileset instance within this SpriteRenderer.
    /// </summary>
    /// <param name="jsonPath">The file path, relative to ImagesFileDirectory, that stores the json config of a given tileset.</param>
    /// <param name="firstGid">The FirstGid offset of the newly created Tileset instance.</param>

    public void LoadTileset(string jsonPath, int firstGid = 1){
        
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


    /// <summary>
    /// Unloads a loaded Tileset within this SpriteRenderer.
    /// </summary>
    /// <param name="tilesetName">The name of the loaded tileset (the config file name without its extension).</param>

    public void UnloadTileset(string tilesetName){
        tilesets[tilesetName].Dispose();
        tilesets.Remove(tilesetName);
    }


    /// <summary>
    /// Draws all sprites, both animated and static, that is stored within this SpriteRenderer.
    /// </summary>
    /// <param name="spriteBatch"></param>

    public void DrawAll(SpriteBatch spriteBatch){
        DrawStaticSprites(spriteBatch);
        DrawAnimatedSprites(spriteBatch);
    }


    /// <summary>
    /// Draws all AnimatedSprite instances that are stored within this SpriteRenderer.
    /// </summary>
    /// <param name="spriteBatch"></param>

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


    /// <summary>
    /// Draws all StaticSprite instances that are stored within this SpriteRenderer.
    /// </summary>
    /// <param name="spriteBatch"></param>

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


    /// <summary>
    /// Gets the reference to a loaded Tileset instance within this SpriteRenderer.
    /// </summary>
    /// <param name="tilesetName">The name of the loaded tileset (the config file name without its extension).</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>

    public ref Tileset GetTileset(string tilesetName){
        ref Tileset tileset = ref System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault(tilesets, tilesetName, out bool exists);
        if(exists == false){
            throw new Exception($"[SpriteRenderer] {tilesetName} is either invalid tileset or has not been loaded.");
        }
        else{
            return ref tileset;
        }

    }

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