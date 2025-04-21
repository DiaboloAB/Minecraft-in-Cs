using ProjectM.Graphics.Textures;
using Microsoft.Xna.Framework;

namespace ProjectM.Core.World;

public class BlockTextureCoord
{
    public static Vector2[][] TextureCoords { get; private set; }

    public static void SetTextureCoords(Atlas atlas)
    {
        Rectangle grassRect = atlas.TexturesRegions["grass"];
        Rectangle defaultRect = atlas.TexturesRegions["default"];
        Rectangle stoneRect = atlas.TexturesRegions["stone"];
        Rectangle leavesRect = atlas.TexturesRegions["leaves"];
        Rectangle woodRect = atlas.TexturesRegions["wood"];
        TextureCoords = new Vector2[6][];
        
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
        
        TextureCoords[4] = new Vector2[6];
        TextureCoords[4][0] = new Vector2(woodRect.X, woodRect.Y);
        TextureCoords[4][1] = new Vector2(woodRect.X, woodRect.Y);
        TextureCoords[4][2] = new Vector2(woodRect.X + 16.0f, woodRect.Y);
        TextureCoords[4][3] = new Vector2(woodRect.X + 16.0f, woodRect.Y);
        TextureCoords[4][4] = new Vector2(woodRect.X, woodRect.Y);
        TextureCoords[4][5] = new Vector2(woodRect.X, woodRect.Y);
        
        TextureCoords[5] = new Vector2[6];
        for (int i = 0; i < 6; i++)
            TextureCoords[5][i] = new Vector2(leavesRect.X, leavesRect.Y);
    }
}