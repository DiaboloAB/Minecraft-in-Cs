using App1.Graphics.Textures;
using Microsoft.Xna.Framework;

namespace App1.Core.World;

public class BlockTextureCoord
{
    public static Vector2[][] TextureCoords { get; private set; }

    public static void SetTextureCoords(Atlas atlas)
    {
        Rectangle grassRect = atlas.TexturesRegions["grass"];
        Rectangle defaultRect = atlas.TexturesRegions["default"];
        Rectangle stoneRect = atlas.TexturesRegions["stone"];
        TextureCoords = new Vector2[4][];
        
        TextureCoords[0] = new Vector2[6];
        for (int i = 0; i < 6; i++)
            TextureCoords[0][i] = new Vector2(defaultRect.X, defaultRect.Y);
        
        TextureCoords[1] = new Vector2[6];
        TextureCoords[1][0] = new Vector2(grassRect.X + 16.0f, grassRect.Y);
        TextureCoords[1][1] = new Vector2(grassRect.X + 16.0f, grassRect.Y);
        TextureCoords[1][2] = new Vector2(grassRect.X, grassRect.Y);
        TextureCoords[1][3] = new Vector2(grassRect.X + 16.0f * 2, grassRect.Y);
        TextureCoords[1][4] = new Vector2(grassRect.X + 16.0f, grassRect.Y);
        TextureCoords[1][5] = new Vector2(grassRect.X + 16.0f, grassRect.Y);
        
        TextureCoords[2] = new Vector2[6];
        for (int i = 0; i < 6; i++)
            TextureCoords[2][i] = new Vector2(stoneRect.X, stoneRect.Y);
        
        TextureCoords[3] = new Vector2[6];
        for (int i = 0; i < 6; i++)
            TextureCoords[3][i] = new Vector2(grassRect.X + 16.0f * 2, grassRect.Y);
    }
}