using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace HowlEngine.Graphics;

public class TextureAtlas{
    private Dictionary<string, TextureRegion> regions;

    /// <summary>
    /// Gets or Sets the source texture represented by this texture atlas.
    /// </summary>
    public Texture2D texture {get; set;}

    /// <summary>
    /// Creates a new texture atlas.
    /// </summary>
    public TextureAtlas(){
        regions = new Dictionary<string, TextureRegion>();
    }

    /// <summary>
    /// Creates a new texture atlas instance using the given texture.
    /// </summary>
    /// <param name="_texture">The source texture represented by this texture atlas.</param>
    public TextureAtlas(Texture2D texture){
        this.texture = texture;
        regions = new Dictionary<string, TextureRegion>();
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
        TextureRegion region = new TextureRegion(texture, x, y, width, height);
        regions.Add(name, region);
    }

    /// <summary>
    /// Gets the region from this texture atlas with the specified name.
    /// </summary>
    /// <param name="name">The name of the region to retrieve.</param>
    /// <returns>The TextureRegion with the specified name.</returns>
    public TextureRegion GetRegion(string name){
        return regions[name];
    }

    /// <summary>
    /// Removes the region from this texture atlas with the specified name.
    /// </summary>
    /// <param name="name">The name of the region to remove.</param>
    /// <returns></returns>
    public bool RemoveRegion(string name){
        return regions.Remove(name);
    }

    /// <summary>
    /// Removes all regions from this texture atlas.
    /// </summary>
    public void Clear(){
        regions.Clear();
    }

    public static TextureAtlas FromFile(ContentManager content, string fileName){
        TextureAtlas atlas = new TextureAtlas();
        string file_path = Path.Combine(content.RootDirectory, fileName);

        try{
            using (Stream stream = TitleContainer.OpenStream(file_path)){
                using(XmlReader reader = XmlReader.Create(stream)){
                    XDocument doc = XDocument.Load(reader);
                    XElement root = doc.Root;

                    // The <Texture> element contains the content path for the Texture2D to load.
                    // It is retrieved to then use the content manager to load the texture.
                    string texture_path = root.Element("Texture").Value;
                    atlas.texture = content.Load<Texture2D>(texture_path);

                    // Read all region data and create classes from said data.
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

                    return atlas;
                }
            }
        }
        catch(Exception e){
            // Console.WriteLine(e.ToString());
            throw new Exception(e.ToString());
        }
    }

    /// <summary>
    /// Creates a new sprite using the region from this texture atlas with the specified name.
    /// </summary>
    /// <param name="regionName">The name of the region to create the sprite with.</param>
    /// <returns>A new Sprite using the texture region witht the specified name.</returns>
    public Sprite CreateSprite(string regionName){
        TextureRegion region = GetRegion(regionName);
        return new Sprite(region);
    }
}