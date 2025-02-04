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
    private int maxInstancesPerBatch;
    
    Texture2D cubeTexture;
    private readonly GraphicsDevice graphicsDevice;
    private readonly Effect effect;
    private VertexBuffer vertexBuffer;
    private IndexBuffer indexBuffer;
    private VertexBuffer instanceBuffer;

    private Matrix view;
    private Matrix projection;
    private Dictionary<Texture2D, (InstanceData[] instances, int count)> texturedInstances;
    private Dictionary<(Texture2D, BlockModel), (InstanceData[] instances, int count)> texturedComplexInstances;
    
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
    
    public CubeRenderer(GraphicsDevice graphicsDevice, Effect effect, Camera camera, int maxInstancesPerBatch = 50000)
    {
        this.graphicsDevice = graphicsDevice;
        this.texturedInstances = new Dictionary<Texture2D, (InstanceData[] instances, int count)>();
        this.texturedComplexInstances = new Dictionary<(Texture2D, BlockModel), (InstanceData[] instances, int count)>();
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
        this.maxInstancesPerBatch = maxInstancesPerBatch;
        VertexPositionTexture[] vertices = Cube.CreateVertices();
        short[] indices = Cube.CreateIndices();
        Console.WriteLine($"Vertices: {vertices.Length}, Indices: {indices.Length}");
        
        vertexBuffer = new VertexBuffer(graphicsDevice, VertexPositionTexture.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
        vertexBuffer.SetData(vertices);
        Console.WriteLine("Vertex buffer created.");
        
        indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, indices.Length, BufferUsage.WriteOnly);
        indexBuffer.SetData(indices);
        
        instanceBuffer = new VertexBuffer(graphicsDevice, InstanceData.VertexDeclaration, maxInstancesPerBatch, BufferUsage.WriteOnly);
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

            if (count >= instances.Length)
            {
                Array.Resize(ref instances, instances.Length * 2);
            }
            
            instances[count++] = new InstanceData
            {
                World = Matrix.CreateTranslation(cube.Position) * Matrix.CreateScale(cube.Scale)
            };

            texturedInstances[cube.Texture] = (instances, count);
        }
    }
    
    public void UpdateComplexInstances(IEnumerable<CubeData> cubes, BlockModel model)
    {
        texturedComplexInstances.Clear();
        
        foreach (var cube in cubes)
        {
            if (!texturedComplexInstances.ContainsKey((cube.Texture, model)))
            {
                texturedComplexInstances[(cube.Texture, model)] = (new InstanceData[1000], 0);
            }
            var textureBatch = texturedComplexInstances[(cube.Texture, model)];
            ref var instances = ref textureBatch.instances;
            ref var count = ref textureBatch.count;

            instances[count++] = new InstanceData
            {
                World = Matrix.CreateTranslation(cube.Position) * Matrix.CreateScale(cube.Scale)
            };

            texturedComplexInstances[(cube.Texture, model)] = (instances, count);
        }
        
        Console.WriteLine("Complex instances updated.");
        Console.WriteLine($"Textured complex instances: {texturedComplexInstances.Count}");
        Console.WriteLine($"Textured instances: {texturedInstances.Count}");
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
            
            int instance = 0;

            effect.Parameters["Texture"].SetValue(texture);
            while (instance < instanceCount)
            {
            
                int instancesToDraw = Math.Min(maxInstancesPerBatch, instanceCount - instance);
                InstanceData[] tmpInstances = new InstanceData[instancesToDraw];
                Array.Copy(instances, instance, tmpInstances, 0, instancesToDraw);
                
                instanceBuffer.SetData(tmpInstances);
                graphicsDevice.SetVertexBuffers(
                    new VertexBufferBinding(vertexBuffer, 0, 0),
                    new VertexBufferBinding(instanceBuffer, 0, 1)
                );
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    graphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, 24, 0, 12, instancesToDraw);
                }
                
                instance += maxInstancesPerBatch;
            }

        }
        
        foreach(var textureBatch in texturedComplexInstances)
        {
            var (texture, model) = textureBatch.Key;
            var (instances, instanceCount) = textureBatch.Value;
            
            graphicsDevice.SetVertexBuffer(model.VertexBuffer);
            graphicsDevice.Indices = model.IndexBuffer;
            
            effect.Parameters["Texture"].SetValue(texture);
            
            instanceBuffer.SetData(instances);
            graphicsDevice.SetVertexBuffers(
                new VertexBufferBinding(model.VertexBuffer, 0, 0),
                new VertexBufferBinding(instanceBuffer, 0, 1)
            );
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, 24 * model.SubMeshes.Count, 0, 12 * model.SubMeshes.Count, instanceCount);
            }
        }
    }
}