using System;
using System.Buffers;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace App1.Graphics;

public class CubeRenderer
{
    Texture2D cubeTexture;
    private readonly GraphicsDevice graphicsDevice;
    private readonly Effect effect;
    private VertexBuffer vertexBuffer;
    private IndexBuffer indexBuffer;
    private VertexBuffer instanceBuffer;

    private Matrix view;
    private Matrix projection;
    private Dictionary<Texture2D, (InstanceData[] instances, int count)> texturedInstances;
    
    private InstanceData[] instances;
    private BlockModel chestModel;
    
    public struct InstanceData
    {
        public Matrix World;
        
        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 1),
            new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 2),
            new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 3),
            new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 4)
        );
    }
    
    public CubeRenderer(GraphicsDevice graphicsDevice, Effect effect, Camera camera, int maxInstancesPerBatch = 1000)
    {
        this.graphicsDevice = graphicsDevice;
        this.texturedInstances = new Dictionary<Texture2D, (InstanceData[] instances, int count)>();
        this.effect = effect;
        
        UpdateViewProjection(camera);
        CreateBuffers(maxInstancesPerBatch);
    }

    public void UpdateViewProjection(Camera camera)
    {
        view = camera.View;
        projection = camera.Projection;
    }
    
    private void CreateBuffers(int maxInstancesPerBatch)
    {
        VertexPositionTexture[] vertices = Cube.CreateVertices();
        short[] indices = CreateIndices();
        
        vertexBuffer = new VertexBuffer(graphicsDevice, VertexPositionTexture.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
        vertexBuffer.SetData(vertices);
        
        indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, indices.Length, BufferUsage.WriteOnly);
        indexBuffer.SetData(indices);
        
        instanceBuffer = new VertexBuffer(graphicsDevice, InstanceData.VertexDeclaration, maxInstancesPerBatch, BufferUsage.WriteOnly);
    }
    
    private short[] CreateIndices()
    {
        return new short[]
        {
            0, 1, 2, 0, 2, 3, // Front
            4, 5, 6, 4, 6, 7, // Back
            8, 9, 10, 8, 10, 11, // Top
            12, 13, 14, 12, 14, 15, // Bottom
            16, 17, 18, 16, 18, 19, // Left
            20, 21, 22, 20, 22, 23 // Right
        };
    }

    public void UpdateInstances(IEnumerable<CubeData> cubes)
    {
        texturedInstances.Clear();

        foreach (var cube in cubes)
        {
            if (!texturedInstances.ContainsKey(cube.Texture))
            {
                texturedInstances[cube.Texture] = (new InstanceData[1000], 0);
            }
            var textureBatch = texturedInstances[cube.Texture];
            ref var instances = ref textureBatch.instances;
            ref var count = ref textureBatch.count;

            instances[count++] = new InstanceData
            {
                World = Matrix.CreateTranslation(cube.Position) * Matrix.CreateScale(cube.Scale)
            };

            texturedInstances[cube.Texture] = (instances, count);
        }
    }

    public void Draw()
    {
        graphicsDevice.DepthStencilState = DepthStencilState.Default;
        
        effect.Parameters["View"].SetValue(view);
        effect.Parameters["Projection"].SetValue(projection);
        
        graphicsDevice.SetVertexBuffer(vertexBuffer);
        graphicsDevice.Indices = indexBuffer;
        
        foreach (var textureBatch in texturedInstances)
        {
            var texture = textureBatch.Key;
            var (instances, instanceCount) = textureBatch.Value;
            
            if (instanceCount == 0) continue;
            
            effect.Parameters["Texture"].SetValue(texture);
            
            instanceBuffer.SetData(instances, 0, instanceCount);
            
            instanceBuffer.SetData(instances);
            graphicsDevice.SetVertexBuffers(
                    new VertexBufferBinding(vertexBuffer, 0, 0),
                    new VertexBufferBinding(instanceBuffer, 0, 1)
                    );
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, 24, 0, 12, instanceCount);
            }
        }
    }
}