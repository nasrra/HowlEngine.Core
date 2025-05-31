using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HowlEngine.Graphics;

public struct Tileset{

    /// <summary>
    /// Gets the TextureRegions stored within this tileset.
    /// </summary>
    public Dictionary<string,TextureRegion> TextureRegions {get; private set;}

    /// <summary>
    /// Gets the Animations stored within this tileset.
    /// </summary>
    public Dictionary<string,Animation> Animations {get; private set;}

    /// <summary>
    /// Stores a key mapping for TileId to Name when drawing a tile by its id.
    /// </summary>
    private Dictionary<int, string> _idToNameMap;

    /// <summary>
    /// The source file directory, relative to the ImagesFileDirectory, that is used as the image for this texture (png, jpg, etc.).
    /// </summary>
    public string Image {get; private set;}

    /// <summary>
    /// The height, in pixels, of the image stored in this texture.
    /// </summary>
    public int ImageHeight {get; private set;}

    /// <summary>
    /// The width, in pixels, of the image stored in this texture.
    /// </summary>
    public int ImageWidth {get; private set;}

    /// <summary>
    /// The height, in pixels, of an individual tile within this tileset.
    /// </summary>
    public int TileHeight {get; private set;}

    /// <summary>
    /// The width, in pixels, of an individual tile within this tileset.
    /// </summary>
    public int TileWidth {get; private set;}

    /// <summary>
    /// The amount of tiles that fit into the ImageWidth.
    /// </summary>
    public int TilesPerRow {get; private set;}

    /// <summary>
    /// The offset of indexing TileIds.
    /// </summary>
    public int FirstGid {get; set;} = 1;


    /// <summary>
    /// Gets or Sets the source texture represented by this texture atlas.
    /// </summary>
    public Texture2D Texture {get; private set;}

    /// <summary>
    /// Creates a new Tileset instance.
    /// </summary>
    public Tileset(){
        TextureRegions  = new();
        Animations      = new();
        _idToNameMap    = new();
    }

    /// <summary>
    /// Initialises a Tileset via a Json config.
    /// </summary>
    /// <param name="filePath">The file path, relative to ImagesFileDirectory, that stores the json config of a given tileset.</param>
    public void InitialiseFromJson(string filePath){
        string fullPath     = System.IO.Path.Combine(HowlApp.ImagesFileDirectory,filePath);
        string directory    = System.IO.Path.GetDirectoryName(fullPath);
        string json         = System.IO.File.ReadAllText(fullPath);

        // Convert the Json config into a config class to parse into this runtime struct.

        Config.Tileset config = JsonSerializer.Deserialize<Config.Tileset>(json, Json.Converter.Settings);

        ImageHeight     = (int)config.Imageheight;
        ImageWidth      = (int)config.Imagewidth;
        TileHeight      = (int)config.Tileheight;
        TileWidth       = (int)config.Tilewidth;
        Image           = config.Image;
        TilesPerRow     = ImageWidth / TileWidth;

        // Create texture regions with their assigned indexes.

        TextureRegions  = new Dictionary<string, TextureRegion>();
        Animations      = new Dictionary<string, Animation>();

        for(int i = 0; i < config.Tiles.Length; i++){
            
            // Get the current tile we are working with.

            Config.Tile tile = config.Tiles[i];
            string animationName = null; // <-- will remain null unless the tile has an AnimationName property.

            // Loop through properties to find TileName.
            // A Tilename signifies that the tile has data in it (png, jpg, image, etc).

            for(int j = 0; j < tile.Properties.Length; j++){
                
                // Get the property.

                Config.Property property = tile.Properties[j];
                string propertyName = property.Name;


                if(propertyName == "TileName"){
                    
                    //  Map the tiles id to its name.

                    _idToNameMap.Add((int)tile.Id+FirstGid, property.Value); // <-- add FirstGid as tiled stores tileId in "tiles" indexed at 0 and not 1 :).

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

                // Create an array of texture regions that represent the frames of the animation.

                TextureRegion[] frames = new TextureRegion[tile.Animation.Length];

                for(int j = 0; j < frames.Length; j++){
                    
                    //  Convert TileId to UV coords of the image stored by this tileset.

                    int[] uv = TiledIdToUVCoord((int)tile.Animation[j].Tileid);

                    // Add a new TextureRegion to the assigned frame of the animation.

                    frames[j] = new TextureRegion(uv[0], uv[1], TileWidth, TileHeight);
                }

                // Create and add the new Animation to this tileset.

                Animation animation = new Animation(frames, System.TimeSpan.FromMilliseconds(tile.Animation[0].Duration));
                Animations.Add(animationName, animation);
            }
        }

        // load the texture for the tileset, Image is always with the config file.

        Texture = Texture2D.FromFile(HowlApp.GraphicsDevice, System.IO.Path.Combine(HowlApp.ImagesFileDirectory, System.IO.Path.Combine(directory, Image)));
    }

    /// <summary>
    /// Gets the name of a tile from a given tile-id.
    /// </summary>
    /// <param name="tileId">The specified tile-id.</param>
    /// <returns>The name of a tile.</returns>
    public string TileIdToTileName(int tileId){
        return _idToNameMap[tileId];
    }

    /// <summary>
    /// Disposes all stored data within the internal data structures of this Texture.
    /// </summary>
    public void Dispose(){
        
        Texture.Dispose();
        Texture = null;

        TextureRegions.Clear();
        TextureRegions = null;
        
        Animations.Clear();
        Animations = null;

        _idToNameMap.Clear();
        _idToNameMap = null;
    }

    /// <summary>
    /// Gets the uv-coordinate on an image file of a given tile-id.
    /// </summary>
    /// <param name="tileId">The specified tile-id.</param>
    /// <returns>The uv-coordinate of a tile.</returns>
    public int[] TiledIdToUVCoord(int tileId){
        return new int[]{
            (tileId % TilesPerRow) * (int)TileWidth,
            (tileId / TilesPerRow) * (int)TileHeight

        };
    }
}