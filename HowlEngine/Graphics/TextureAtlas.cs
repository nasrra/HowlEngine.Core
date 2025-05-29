using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace HowlEngine.Graphics;

public class TextureAtlas{
    private Dictionary<string, TextureRegion> _regions;
    private Dictionary<string, Animation> _animations;

    // NOTE:
    // Do NOT store Textures outside of this class.
    // Use a WeakReference to it to avoid memory leaks.

    /// <summary>
    /// Gets or Sets the source texture represented by this texture atlas.
    /// </summary>
    public Texture2D Texture {get; private set;}

    /// <summary>
    /// Creates a new texture atlas.
    /// </summary>
    public TextureAtlas(){
        _regions = new Dictionary<string, TextureRegion>();
        _animations = new Dictionary<string, Animation>();
    }

    /// <summary>
    /// Creates a new texture atlas instance using the given texture.
    /// </summary>
    /// <param name="texture">The source texture represented by this texture atlas.</param>
    public TextureAtlas(Texture2D texture){
        Texture = texture;
        _regions = new Dictionary<string, TextureRegion>();
        _animations = new Dictionary<string, Animation>();
    }

    /// <summary>
    /// Creates a new region and adds it to this texture atlas.
    /// </summary>
    /// <param name="name">The name to give the texture region.</param>
    /// <param name="x">The top-left x-coordinate position of the region boundary, relative to the top-left corner of the source texture boundary.</param>
    /// <param name="y">The top-left y-coordinate position of the region boundary, relative to the top-left corner of the source texture boundary.</param>
    /// <param name="width">The width, in pixels, of the region.</param>
    /// <param name="height">The height, in pixels, of the region.</param>
    public void AddRegion(string name, int x, int y, int width, int height){
        TextureRegion region = new TextureRegion(x, y, width, height);
        _regions.Add(name, region);
    }

    /// <summary>
    /// Gets the region from this texture atlas with the specified name.
    /// </summary>
    /// <param name="name">The name of the region to retrieve.</param>
    /// <returns>The TextureRegion with the specified name.</returns>
    public TextureRegion GetRegion(string name){
        return _regions[name];
    }

    /// <summary>
    /// Removes the region from this texture atlas with the specified name.
    /// </summary>
    /// <param name="name">The name of the region to remove.</param>
    /// <returns></returns>
    public bool RemoveRegion(string name){
        return _regions.Remove(name);
    }

    /// <summary>
    /// Removes all regions from this texture atlas.
    /// </summary>
    public void Clear(){
        _regions.Clear();
    }

    /// <summary>
    /// Adds the given animation to this texture atlas witht the specified name.
    /// </summary>
    /// <param name="animationName">The name of the animation.</param>
    /// <param name="animation">The animation to add.</param>
    public void AddAnimation(string animationName, Animation animation){
        _animations.Add(animationName, animation);
    }

    /// <summary>
    /// Gets the animation from this texture atlas with a specified name.
    /// </summary>
    /// <param name="animationName">The name of the animation.</param>
    /// <returns></returns>
    public Animation GetAnimation(string animationName){
        return _animations[animationName];
    }

    /// <summary>
    /// Remoes the animation from this texture atlas with a specified name.
    /// </summary>
    /// <param name="animationName">The name of the animation.</param>
    /// <returns>true if the animation was successfully removed.</returns>
    public bool RemoveAnimation(string animationName){
        return _animations.Remove(animationName);
    }

    public static TextureAtlas FromFile(string fileName){
        TextureAtlas atlas = new TextureAtlas();
        string file_path = Path.Combine(HowlApp.ImagesFileDirectory, fileName);
        Console.WriteLine(file_path);
        try{
            using (StreamReader stream = new StreamReader(file_path)){
                using(XmlReader reader = XmlReader.Create(stream)){
                    XDocument doc = XDocument.Load(reader);
                    XElement root = doc.Root;

                    // The <Texture> element contains the content path for the Texture2D to load.
                    // It is retrieved to then use the content manager to load the texture.
                    string texturePath = root.Element("FilePath").Value;
                    atlas.Texture = Texture2D.FromFile(HowlApp.GraphicsDevice, System.IO.Path.Combine(HowlApp.ImagesFileDirectory, texturePath));

                    // =================================================================================
                    // The <Regions> element contains individual <Region> elements, each one describing
                    // a different texture region within the atlas.  
                    //
                    // Example:
                    // <Regions>
                    //      <Region name="spriteOne" x="0" y="0" width="32" height="32" />
                    //      <Region name="spriteTwo" x="32" y="0" width="32" height="32" />
                    // </Regions>
                    //
                    // So we retrieve all of the <Region> elements then loop through each one
                    // and generate a new TextureRegion instance from it and add it to this atlas.
                    // =================================================================================

                    IEnumerable<XElement> regions = root.Element("Regions")?.Elements("Region");
                    if(regions != null){
                        foreach(XElement region in regions){
                            string name = region.Attribute("name")?.Value;
                            int x = int.Parse(region.Attribute("x")?.Value ?? "0");
                            int y = int.Parse(region.Attribute("y")?.Value ?? "0");
                            int width = int.Parse(region.Attribute("width")?.Value ?? "0");
                            int height = int.Parse(region.Attribute("height")?.Value ?? "0");

                            if(string.IsNullOrEmpty(name) == false){
                                atlas.AddRegion(name, x, y, width, height);
                            }
                        }
                    }

                    // =======================================================================================
                    // The <Animations> element contains individual <Animation> elements, each one describing
                    // a different animation within the atlas.
                    //
                    // Example:
                    // <Animations>
                    //      <Animation name="animation" interval="100">
                    //          <Frame region="spriteOne" />
                    //          <Frame region="spriteTwo" />
                    //      </Animation>
                    // </Animations>
                    //
                    // So we retrieve all of the <Animation> elements then loop through each one
                    // and generate a new Animation instance from it and add it to this atlas.
                    // =======================================================================================

                    IEnumerable<XElement> animations = root.Element("Animations")?.Elements("Animation");
                    if(animations != null){
                        foreach(XElement animation in animations){
                            string name = animation.Attribute("name")?.Value;
                            TimeSpan interval = TimeSpan.FromMilliseconds(float.Parse(animation.Attribute("interval")?.Value ?? "0"));
                            var frames = animation.Elements("Frame").ToList();
                            TextureRegion[] textureRegions = new TextureRegion[frames.Count];
                            if(frames.Count>0){
                                for(int i = 0; i < frames.Count; i++){
                                    textureRegions[i] = atlas.GetRegion(frames[i].Attribute("region")?.Value);
                                }
                            }
                            atlas.AddAnimation(
                                name,
                                new Animation(textureRegions, interval)
                            );
                        }
                    }

                    return atlas;
                }
            }
        }
        catch(Exception e){
            // #if DEBUG
            // Console.WriteLine(e.ToString());
            // #endif
            throw new Exception(e.ToString());
        }
    }

    /// <summary>
    /// Creates a new sprite using the region from this texture atlas with the specified name.
    /// </summary>
    /// <param name="regionName">The name of the region to create the sprite with.</param>
    /// <returns>A new Sprite using the texture region witht the specified name.</returns>
    public Sprite CreateSprite(string regionName){
        return new Sprite(GetRegion(regionName), new WeakReference<Texture2D>(Texture));
    }

    /// <summary>
    /// Creates a new sprite using the animation from this texture atlas with the specified name.
    /// </summary>
    /// <param name="animationName">the name of the animation to creat the sprite with.</param>
    /// <returns></returns>
    public AnimatedSprite CreateAnimatedSprite(string animationName){
        return new AnimatedSprite(GetAnimation(animationName), new WeakReference<TextureAtlas>(this));
    }
}