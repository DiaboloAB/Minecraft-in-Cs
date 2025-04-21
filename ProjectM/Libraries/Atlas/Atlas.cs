using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectM.Graphics.Textures;

public class Atlas
{
    public Texture2D Texture { get; private set; }
    private Color[] AtlasData;
    public Dictionary<string, Rectangle> TexturesRegions { get; private set; }

    private List<Rectangle> emptySpaces;
    private int topLine = 0;

    public Atlas(GraphicsDevice graphicsDevice, string path, string jsonPath)
    {
        FileStream stream = File.OpenRead(path);
        Texture = Texture2D.FromStream(graphicsDevice, stream);
        
        AtlasData = new Color[Texture.Width * Texture.Height];
        
        emptySpaces = new List<Rectangle> { new Rectangle(0, 0, (int)Texture.Width, (int)Texture.Height) };
        
        Texture = new Texture2D(graphicsDevice, (int)Texture.Width, (int)Texture.Height);
        TexturesRegions = new Dictionary<string, Rectangle>();
        
        string json = File.ReadAllText(jsonPath);
        TexturesRegions = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, Rectangle>>(json);
        
    }
    
    public Atlas(
        GraphicsDevice graphicsDevice, Vector2 size,
        Dictionary<string, Texture2D> textures = null)
    {
        emptySpaces = new List<Rectangle> { new Rectangle(0, 0, (int)size.X, (int)size.Y) };
        
        Texture = new Texture2D(graphicsDevice, (int)size.X, (int)size.Y);
        TexturesRegions = new Dictionary<string, Rectangle>();
        AtlasData = new Color[Texture.Width * Texture.Height];
        if (textures != null)
        {
            foreach (var texture in textures)
            {
                try
                {
                    Rectangle place = FindPlace(texture.Value);
                    PlaceTexture(texture.Value, texture.Key, place);
                } catch (Exception e)
                {
                    Console.WriteLine("Texture processing failed: {0}", e);
                    throw;
                }
                
            }
            Texture.SetData(AtlasData);
        }
    }

    public void AddTexture(string name, Texture2D texture)
    {
        try
        {
            Rectangle place = FindPlace(texture);
            PlaceTexture(texture, name, place);
        } catch (Exception e)
        {
            Console.WriteLine("Texture processing failed: {0}", e);
            throw;
        }
    }

    private Rectangle FindPlace(Texture2D texture)
    {
        Rectangle place = new Rectangle(0, 0, 0, 0);
            
        for (int i = 0; i < emptySpaces.Count; i++)
        {
            Rectangle space = emptySpaces[i];

            if (texture.Width <= space.Width &&
                texture.Height <= space.Height)
            {
                place = space;

                emptySpaces.Add(new Rectangle(
                    space.X + texture.Width,
                    space.Y,
                    space.Width - texture.Width,
                    texture.Height));
                emptySpaces.Add(new Rectangle(
                    space.X,
                    space.Y + texture.Height,
                    space.Width,
                    space.Height - texture.Height));
                    
                emptySpaces.RemoveAt(i);
                break;
            }

        }
        return place;
    }

    private void PlaceTexture(Texture2D texture, string name, Rectangle place)
    {
        Color[] textureData = new Color[texture.Width * texture.Height];
        texture.GetData(textureData);
        for (int i = 0; i < texture.Width; i++)
        {
            for (int j = 0; j < texture.Height; j++)
            {
                float atlasIndex = (place.X + i) + (place.Y + j) * Texture.Width;
                float textureIndex = i + j * texture.Width;
                AtlasData[(int)atlasIndex] = textureData[(int)textureIndex];
            }
        }
        TexturesRegions.Add(name, new Rectangle(place.X, place.Y, texture.Width, texture.Height));
    }

    public void Save()
    {
        try
        {
            // save texture
            Stream stream = File.Create("atlas.png");
            Texture.SaveAsPng(stream, Texture.Width, Texture.Height);

            // save textureRegion as json
            string json = System.Text.Json.JsonSerializer.Serialize(TexturesRegions);
            File.WriteAllText("atlas.json", json);
        }
        catch (Exception e)
        {
            Console.WriteLine("Shape processing failed: {0}", e);
            throw;
        }
    }

    public void load(GraphicsDevice graphicsDevice)
    {
        try
        {
            Stream stream = File.OpenRead("atlas.png");
            Texture = Texture2D.FromStream(graphicsDevice, stream);
            
            AtlasData = new Color[Texture.Width * Texture.Height];
            
            emptySpaces = new List<Rectangle> { new Rectangle(0, 0, (int)Texture.Width, (int)Texture.Height) };
            
            string json = File.ReadAllText("atlas.json");
            
            TexturesRegions = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, Rectangle>>(json);
            
        } catch (Exception e)
        {
            Console.WriteLine("Shape processing failed: {0}", e);
            throw;
        }
    }
}