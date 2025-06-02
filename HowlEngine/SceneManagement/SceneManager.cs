
using System.IO;
using HowlEngine.Physics;
using System.Collections.Generic;
using HowlEngine.SceneManagement.Config;
using System;
using Microsoft.Xna.Framework;

namespace HowlEngine.SceneManagement;

public class SceneManager{
    /// <summary>
    /// Gets the name of the currently loaded scene.
    /// </summary>
    public string CurrentScene { get; private set; }
    
    /// <summary>
    /// Creates a new SceneManager instance.
    /// </summary>
    public SceneManager(){
        CurrentScene = "";
    }

    /// <summary>
    /// Loads a Scene from a given file path, relative to the ScenesFileDirectory.
    /// </summary>
    /// <param name="filePath">The specified path to the scene json file.</param>
    public void LoadScene(string filePath){
        string sceneDataJson = File.ReadAllText(Path.Combine(HowlApp.ScenesFileDirectory,filePath));
        Config.Scene config = Config.Scene.FromJson(sceneDataJson);

        // cache.
        Dictionary<string, Config.Template> chachedTemplates = new Dictionary<string, Config.Template>();
        Dictionary<string, string> chachedTemplateTypes = new Dictionary<string, string>();

        // load tilesets.
        foreach(Config.Tileset token in config.Tilesets){
            HowlApp.SpriteRenderer.LoadTileset(token);
        }

        for(int i = 0; i < config.LayerGroup.Length; i++){
            Config.LayerGroup layerGroup = config.LayerGroup[i];

            // Draw all tile layers.
            if(layerGroup.Name == "TileLayers"){
                for(int j = 0; j < layerGroup.Layers.Length; j++){
                    Config.Layer layer = layerGroup.Layers[j];
                    if(layer.Visible == true){
                        HowlApp.SpriteRenderer.LoadTileMap(layer.Data, layer.Name, (int)config.Width, (int)config.Height);
                    }
                }
            }

            if(layerGroup.Name == "ObjectLayers"){
                
                for(int j = 0; j < layerGroup.Layers.Length; j++){
                    Config.Layer layer = layerGroup.Layers[j];

                    if(layer.Visible == false){
                        continue;
                    }

                    // load all colliders.
                    if(layer.Name == "Collisions"){
                        foreach(Config.Object col in layer.Objects){
                            HowlApp.PhysicsSystem.AllocatePyhsicsBody(new PhysicsBodyAABB(new RectangleCollider((int)col.X,(int)col.Y,(int)col.Width,(int)col.Height)));
                        }
                    }

                    // load all entities.
                    if(layer.Name == "Entities"){
                        for(int k = 0; k < layer.Objects.Length; k++){
                            
                            Config.Object obj = layer.Objects[k];
                            string templateFilePath = obj.Template;

                            // cache the entity template for future usage (if there are multiple of the entity to load).

                            if(!chachedTemplates.ContainsKey(templateFilePath)){
                                string entityTemplateJson = File.ReadAllText(Path.Combine(HowlApp.TemplatesFileDirectory, templateFilePath)); 
                                Config.Template entityTemplate = Template.FromJson(entityTemplateJson);
                                for(int l = 0; l < entityTemplate.Object.Property.Length; l++){
                                    if(entityTemplate.Object.Property[l].Name == "Type"){
                                        chachedTemplateTypes.Add(templateFilePath, entityTemplate.Object.Property[l].Value.ToString());
                                    }
                                }
                                chachedTemplates.Add(templateFilePath, entityTemplate);
                            }

                            object[] args = new object[(obj.Property != null? obj.Property.Length : 0) + 1]; // <-- add two for positional data.
                            args[0] = new Vector2(obj.X, obj.Y);

                            // create an instance of the new entity.

                            HowlApp.TypeFactory.CreateInstance(chachedTemplateTypes[templateFilePath], args);
                        }
                    }
                }

            }

        }

        // clear for memory management.

        chachedTemplates.Clear();
        chachedTemplateTypes.Clear();
    }
}