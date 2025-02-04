using System;
using System.Reflection.Metadata;
using App1.Core.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace App1.Graphics.Renderer;

public class Renderer
{
    private readonly GraphicsDevice graphicsDevice;
    private readonly Effect effect;
    private Texture2D textureAtlas;
    private Matrix view;
    private Matrix projection;

    public Renderer(GraphicsDevice graphicsDevice, Effect effect)
    {
        this.graphicsDevice = graphicsDevice;
        this.effect = effect;
    }
    
    public void SetAtlasTexture(Texture2D texture)
    {
        Console.WriteLine(texture.Width);
        effect.Parameters["Texture"].SetValue(texture);
        Console.WriteLine("Texture set.");
    }

    public void UpdateViewProjection(Camera camera)
    {
        view = camera.View;
        projection = camera.Projection;
    }
    
    public void DrawWorld(World world, Vector3 position, int radius)
    {
        graphicsDevice.DepthStencilState = DepthStencilState.Default;
        effect.Parameters["View"].SetValue(view);
        effect.Parameters["Projection"].SetValue(projection);
        
        
        for (int i = (int)position.X - radius; i < position.Z + radius; i++)
        {
            for (int j = (int)position.Z - radius; j < position.Z + radius; j++)
            {
                if (world.chunks.ContainsKey((i, j)))
                {
                    var chunk = world.GetChunk(i, j);
                    graphicsDevice.SetVertexBuffer(chunk.VertexBuffer);
                    graphicsDevice.Indices = chunk.IndexBuffer;
                    
                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        graphicsDevice.DrawIndexedPrimitives(
                            PrimitiveType.TriangleList,
                            0,
                            0,
                            chunk.IndexCount
                        );
                    }
                }
            }
        }
        
        // foreach (var chunk in world.Chunks)
        // {
        //     graphicsDevice.SetVertexBuffer(chunk.VertexBuffer);
        //     graphicsDevice.Indices = chunk.IndexBuffer;
        //     
        //     foreach (EffectPass pass in effect.CurrentTechnique.Passes)
        //     {
        //         pass.Apply();
        //         graphicsDevice.DrawIndexedPrimitives(
        //             PrimitiveType.TriangleList,
        //             0,
        //             0,
        //             chunk.IndexCount
        //         );
        //     }
        // }
    }
}