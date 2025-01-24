using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = System.Numerics.Vector2;

namespace App1.Graphics.Textures;

public class Atlas
{
    public Texture2D Texture { get; private set; }
    private Color[] AtlasData;
    public Dictionary<string, Rectangle> TexturesRegions { get; private set; }

    private List<Rectangle> emptySpaces;
    private int topLine = 0;
    
    public Atlas(
        GraphicsDevice graphicsDevice,
        Dictionary<string, Texture2D> textures, Vector2 size)
    {
        emptySpaces = new List<Rectangle> { new Rectangle(0, 0, (int)size.X, (int)size.Y) };
        
        Texture = new Texture2D(graphicsDevice, (int)size.X, (int)size.Y);
        AtlasData = new Color[Texture.Width * Texture.Height];
        Dictionary<string, Rectangle> texturesRegions = new Dictionary<string, Rectangle>();
        
        foreach (var texture in textures)
        {
            Console.WriteLine(texture.Value.Width * texture.Value.Height);
            
            Rectangle place = new Rectangle(0, 0, 0, 0);
            
            for (int i = 0; i < emptySpaces.Count; i++)
            {
                Rectangle space = emptySpaces[i];

                if (texture.Value.Width <= space.Width &&
                    texture.Value.Height <= space.Height)
                {
                    place = space;

                    emptySpaces.Add(new Rectangle(
                        space.X + texture.Value.Width,
                        space.Y,
                        space.Width - texture.Value.Width,
                        texture.Value.Height));
                    emptySpaces.Add(new Rectangle(
                        space.X,
                        space.Y + texture.Value.Height,
                        space.Width,
                        space.Height - texture.Value.Height));
                    
                    emptySpaces.RemoveAt(i);
                    break;
                }

            }
            Console.WriteLine(place);
            PlaceTexture(texture.Value, place);
            
        }
        Texture.SetData(AtlasData);
    }

    private void PlaceTexture(Texture2D texture, Rectangle place)
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
    }

    public void Save()
    {
        try
        {
            Stream stream = File.Create("atlas.png");
            Texture.SaveAsPng(stream, Texture.Width, Texture.Height);

        }
        catch (Exception e)
        {
            Console.WriteLine("Shape processing failed: {0}", e);
            throw;
        }

        
    }
}