using System;
using System.Buffers;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using App1.Graphics.Textures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace App1.Graphics;

public class FaceRenderer
{
    private int maxInstancesPerBatch;

    public float textureSize = 1.0f;
    
    Texture2D cubeTexture;
    private readonly GraphicsDevice graphicsDevice;
    private readonly Effect effect;

    private Matrix view;
    private Matrix projection;

    private IndexBuffer indexBuffer;
    private VertexBuffer instanceBuffer;
    private VertexBuffer[] vertexBuffer;
    private List<InstanceData>[] facesInstances;
    private Texture2D textureAtlas;
    
    public struct InstanceData
    {
        public Matrix World;
        public Vector2 TexturePosition;
        
        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 1),
            new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 2),
            new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 3),
            new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 4),
            new VertexElement(64, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 5)
        );
    }
    
    public FaceRenderer(GraphicsDevice graphicsDevice, Effect effect, Camera camera, float textureSize, int maxInstancesPerBatch = 50000)
    {
        this.textureSize = textureSize;
        this.graphicsDevice = graphicsDevice;
        this.facesInstances = new List<InstanceData>[6];
        
        
        this.effect = effect;
        
        UpdateViewProjection(camera);
        CreateBuffers(maxInstancesPerBatch);
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
    
    private void CreateBuffers(int maxInstancesPerBatch)
    {
        this.maxInstancesPerBatch = maxInstancesPerBatch;
        // Console.WriteLine($"Vertices: {vertices.Length}, Indices: {indices.Length}");
        
        vertexBuffer = new VertexBuffer[6];
        for (int i = 0; i < 6; i++)
        {
            vertexBuffer[i] = new VertexBuffer(graphicsDevice, VertexPositionTexture.VertexDeclaration, 4, BufferUsage.WriteOnly);
            VertexPositionTexture[] vertices = Cube.CreateFaceVertices(i, textureSize);
            facesInstances[i] = new List<InstanceData>();
            vertexBuffer[i].SetData(vertices); 
        }
        Console.WriteLine("Vertex buffer created.");
        
        
        short[] indices = new short[] { 0, 1, 2, 0, 2, 3 };
        indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, indices.Length, BufferUsage.WriteOnly);
        indexBuffer.SetData(indices);
        Console.WriteLine("Index buffer created.");
        
        instanceBuffer = new VertexBuffer(graphicsDevice, InstanceData.VertexDeclaration, maxInstancesPerBatch, BufferUsage.WriteOnly);
        Console.WriteLine("Instance buffer created.");
    }

    public void UpdateInstances(IEnumerable<FaceData> faces)
    {
        for (int i = 0; i < 6; i++)
            facesInstances[i].Clear();
        
        foreach (var face in faces)
        {
            facesInstances[face.Orientation].Add(new InstanceData
            {
                World = Matrix.CreateTranslation(face.Position),
                TexturePosition = face.TexCoord / 2048.0f
            });
        }
    }

    public void Draw()
    {

        graphicsDevice.DepthStencilState = DepthStencilState.Default;
        effect.Parameters["View"].SetValue(view);
        effect.Parameters["Projection"].SetValue(projection);
        
        graphicsDevice.Indices = indexBuffer; 
 
        for (int i = 0; i < 6; i++)
        {
            if (facesInstances[i].Count == 0)
                continue;
            
            graphicsDevice.SetVertexBuffer(vertexBuffer[i]);

            int instance = 0;
            int instanceCount = facesInstances[i].Count;
            while (instance < instanceCount)
            {
                int instancesToDraw = Math.Min(maxInstancesPerBatch, instanceCount - instance);
                instanceBuffer.SetData(facesInstances[i].GetRange(instance, instancesToDraw).ToArray());
                graphicsDevice.SetVertexBuffers(
                    new VertexBufferBinding(vertexBuffer[i], 0, 0),
                    new VertexBufferBinding(instanceBuffer, 0, 1)
                );
                
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    graphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, 2, facesInstances[i].Count);
                }
                
                instance += maxInstancesPerBatch;
            }
            
            
        }
        
    }
}