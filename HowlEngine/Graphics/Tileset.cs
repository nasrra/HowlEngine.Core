using System;
using System.Collections.Generic;
using System.Text.Json;
using HowlEngine.Graphics.Config;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HowlEngine.Graphics;

public class Tileset{

    public Dictionary<string,TextureRegion> TextureRegions {get; private set;}
    public Dictionary<string,Animation> Animations {get; private set;}
    private Dictionary<int, string> _idToNameMap;
    public string Image {get; private set;}
    public int ImageHeight {get; private set;}
    public int ImageWidth {get; private set;}
    public int TileHeight {get; private set;}
    public int TileWidth {get; private set;}
    public int TilesPerRow {get; private set;}
    public int FirstGid {get; set;} = 1;


    /// <summary>
    /// Gets or Sets the source texture represented by this texture atlas.
    /// </summary>
    public Texture2D Texture {get; private set;}

    public Tileset(){
        TextureRegions = new();
        Animations = new();
        _idToNameMap = new();
    }

    public void InitialiseFromJson(string filePath){
        string fullPath = System.IO.Path.Combine(HowlApp.ImagesFileDirectory,filePath);
        string directory = System.IO.Path.GetDirectoryName(fullPath);
        string json = System.IO.File.ReadAllText(fullPath);

        Config.Tileset config = JsonSerializer.Deserialize<Config.Tileset>(json, Json.Converter.Settings);

        ImageHeight     = (int)config.Imageheight;
        ImageWidth      = (int)config.Imagewidth;
        TileHeight      = (int)config.Tileheight;
        TileWidth       = (int)config.Tilewidth;
        Image           = config.Image;
        TilesPerRow     = ImageWidth / TileWidth;

        // Create texture regions with their assigned indexes.

        TextureRegions = new Dictionary<string, TextureRegion>();
        Animations  = new Dictionary<string, Animation>();

        for(int i = 0; i < config.Tiles.Length; i++){
            
            // Loop through properties to find TileName.
            // A Tilename signifies that the tile has data in it (png, jpg, image, etc).
            Config.Tile tile = config.Tiles[i];
            string animationName = null;

            for(int j = 0; j < tile.Properties.Length; j++){
                Config.Property property = tile.Properties[j];
                
                string propertyName = property.Name;
                if(propertyName == "TileName"){
                    
                    _idToNameMap.Add((int)tile.Id, property.Value);

                    // Convert TileId into UV coords of the image stored by this tileset.
                    int[] uv = TiledIdToUVCoord((int)tile.Id);

                    // Add a new TextureRegion to draw from.

                    TextureRegions.Add(property.Value, new TextureRegion(uv[0], uv[1], TileWidth, TileHeight));
                }
                if(propertyName == "AnimationName"){
                    animationName = property.Value;
                }
            }

            // add animation.
            if(animationName != null && tile.Animation != null){
                TextureRegion[] frames = new TextureRegion[tile.Animation.Length];

                for(int j = 0; j < frames.Length; j++){
                    
                    int[] uv = TiledIdToUVCoord((int)tile.Animation[j].Tileid);

                    frames[j] = new TextureRegion(uv[0], uv[1], TileWidth, TileHeight);
                }

                Animation animation = new Animation(frames, System.TimeSpan.FromMilliseconds(tile.Animation[0].Duration));
                Animations.Add(animationName, animation);
                Console.WriteLine("!"+animationName+"!");
            }
        }

        Console.WriteLine(Animations.ContainsKey("SlimeIdle"));
        // load the texture for the tileset, Image is always with the config file.

        Texture = Texture2D.FromFile(HowlApp.GraphicsDevice, System.IO.Path.Combine(HowlApp.ImagesFileDirectory, System.IO.Path.Combine(directory, Image)));
    }


    public StaticSprite CreateStaticSprite(Vector2 position, string spriteName, string textureId){
        return new StaticSprite(TextureRegions[spriteName],  position, textureId);
    }

    public AnimatedSprite CreateAnimatedSprite(Vector2 position, string spriteName, string textureId){
        return new AnimatedSprite(Animations[spriteName], position, textureId);
    }


    public void Dispose(){
        Texture.Dispose();
        Texture = null;
    }


    public int[] TiledIdToUVCoord(int tileId){
        return new int[]{
            (tileId % TilesPerRow) * (int)TileWidth,
            (tileId / TilesPerRow) * (int)TileHeight

        };
    }
}