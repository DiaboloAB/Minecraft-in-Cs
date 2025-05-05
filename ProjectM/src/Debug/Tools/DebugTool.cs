using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectM.Core.Player;
using ProjectM.Core.World;
using ProjectM.Utils;

namespace ProjectM.Debug.Tools;

public class DebugTool
{
    public bool isActive { get; private set; } = false;
    
    private World world;
    private Player player;
    private ContentManager content;

    private SpriteFont debugFont;

    public float FPS = 0;

    public float MS = 0;
    
    public DebugTool(World world, Player player, ContentManager content)
    {
        this.content = content;
        this.world = world;
        this.player = player;
        this.debugFont = this.content.Load<SpriteFont>("Fonts/File");
    }

    public void Update(float deltaTime)
    {
        if (Input.IsKeyPressed(Keys.F3))
        {
            isActive = !isActive;
        }
        
        if (isActive)
        {
            // HandleDebugTool();
        }
        
        // Update FPS and MS
        FPS = 1 / deltaTime;
        MS = deltaTime * 1000;
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        if (isActive)
        {
            spriteBatch.DrawString(debugFont, $"FPS: {FPS}", new Vector2(10, 10), Color.White);
            spriteBatch.DrawString(debugFont, $"Facing: {player.Camera.Facing}", new Vector2(10, 30), Color.White);
            spriteBatch.DrawString(debugFont, $"Position:  {player.Position.X}, {player.Position.Y}, {player.Position.Z}", new Vector2(10, 50), Color.White);
            spriteBatch.DrawString(debugFont, "Rotation: " + player.Rotation, new Vector2(10, 70), Color.White);
            spriteBatch.DrawString(debugFont, $"bloc:  {MathUtilities.RoundVector3(player.Position)}", new Vector2(10, 90), Color.White);
            spriteBatch.DrawString(debugFont, $"chunkbloc:  {MathUtilities.RoundVector3(player.Position) - world.GetChunk(player.Position).WorldPosition}", new Vector2(10, 110), Color.White);
            spriteBatch.DrawString(debugFont, $"Chunk: {world.GetChunkPosition(player.Position)}", new Vector2(10, 130), Color.White);
        }
    }
    
}