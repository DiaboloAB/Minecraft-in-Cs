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
    private InstanceData[] instances;

    
    public struct VertexPositionTexture
    {
        public Vector3 Position;
        public Vector2 Texturecoordinate;

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
        );

        public VertexPositionTexture(Vector3 position, Vector2 texturecoordinate)
        {
            Position = position;
            Texturecoordinate = texturecoordinate;
        }
    }
    
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
    
    public CubeRenderer(GraphicsDevice graphicsDevice, Effect effect, Camera camera, Texture2D cubeTexture)
    {
        this.graphicsDevice = graphicsDevice;
        this.cubeTexture = cubeTexture;
        this.effect = effect;
        
        UpdateViewProjection(camera);
        CreateBuffers();
    }

    public void UpdateViewProjection(Camera camera)
    {
        view = camera.View;
        projection = camera.Projection;
    }
    
    private void CreateBuffers()
    {
        VertexPositionTexture[] vertices = CreateVertices();
        short[] indices = CreateIndices();
        
        vertexBuffer = new VertexBuffer(graphicsDevice, VertexPositionTexture.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
        vertexBuffer.SetData(vertices);
        
        indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, indices.Length, BufferUsage.WriteOnly);
        indexBuffer.SetData(indices);
        
        instanceBuffer = new VertexBuffer(graphicsDevice, InstanceData.VertexDeclaration, 1000, BufferUsage.WriteOnly);
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
    
    private VertexPositionTexture[] CreateVertices()
    {
        return new VertexPositionTexture[]
        {
        // Front face
            new VertexPositionTexture(new Vector3(-0.5f,  0.5f,  0.5f), new Vector2(0.25f, 0.66f)),
            new VertexPositionTexture(new Vector3( 0.5f,  0.5f,  0.5f), new Vector2(0.5f, 0.66f)),
            new VertexPositionTexture(new Vector3( 0.5f, -0.5f,  0.5f), new Vector2(0.5f, 1.0f)),
            new VertexPositionTexture(new Vector3(-0.5f, -0.5f,  0.5f), new Vector2(0.25f, 1.0f)),
            
            // Back face
            new VertexPositionTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector2(0.25f, 0.0f)),
            new VertexPositionTexture(new Vector3( 0.5f, -0.5f, -0.5f), new Vector2(0.5f, 0.0f)),
            new VertexPositionTexture(new Vector3( 0.5f,  0.5f, -0.5f), new Vector2(0.5f, 0.33f)),
            new VertexPositionTexture(new Vector3(-0.5f,  0.5f, -0.5f), new Vector2(0.25f, 0.33f)),
            
            // Top face
            new VertexPositionTexture(new Vector3(-0.5f,  0.5f, -0.5f), new Vector2(0.25f, 0.33f)),
            new VertexPositionTexture(new Vector3( 0.5f,  0.5f, -0.5f), new Vector2(0.5f, 0.33f)),
            new VertexPositionTexture(new Vector3( 0.5f,  0.5f,  0.5f), new Vector2(0.5f, 0.66f)),
            new VertexPositionTexture(new Vector3(-0.5f,  0.5f,  0.5f), new Vector2(0.25f, 0.66f)),
            
            // // Bottom face
            new VertexPositionTexture(new Vector3(-0.5f, -0.5f,  0.5f), new Vector2(0.25f, 0.66f)),
            new VertexPositionTexture(new Vector3( 0.5f, -0.5f,  0.5f), new Vector2(0.5f, 0.66f)),
            new VertexPositionTexture(new Vector3( 0.5f, -0.5f, -0.5f), new Vector2(0.5f, 1.0f)),
            new VertexPositionTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector2(0.25f, 1.0f)),
            //
            // // Left face
            // new VertexPositionTexture(new Vector3(-0.5f,  0.5f, -0.5f), new Vector2(0.0f, 0.0f)),
            // new VertexPositionTexture(new Vector3(-0.5f,  0.5f,  0.5f), new Vector2(1.0f, 0.0f)),
            // new VertexPositionTexture(new Vector3(-0.5f, -0.5f,  0.5f), new Vector2(1.0f, 1.0f)),
            // new VertexPositionTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector2(0.0f, 1.0f)),
            //
            // // Right face
            // new VertexPositionTexture(new Vector3(0.5f,  0.5f,  0.5f), new Vector2(0.0f, 0.0f)),
            // new VertexPositionTexture(new Vector3(0.5f,  0.5f, -0.5f), new Vector2(1.0f, 0.0f)),
            // new VertexPositionTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector2(1.0f, 1.0f)),
            // new VertexPositionTexture(new Vector3(0.5f, -0.5f,  0.5f), new Vector2(0.0f, 1.0f))
        };
    }

    public void UpdateInstances(IEnumerable<CubeData> cubes)
    {
        instances = cubes.Select(cube => new InstanceData
        {
            World = Matrix.CreateTranslation(cube.Position) * Matrix.CreateScale(cube.Scale),
        }).ToArray();
        
        instanceBuffer.SetData(instances);
    }

    public void Draw()
    {
        effect.Parameters["View"].SetValue(view);
        effect.Parameters["Projection"].SetValue(projection);
        effect.Parameters["Texture"].SetValue(cubeTexture);
        
        graphicsDevice.SetVertexBuffers(
            new VertexBufferBinding(vertexBuffer, 0, 0),
            new VertexBufferBinding(instanceBuffer, 0, 1)
        );
        graphicsDevice.Indices = indexBuffer;

        foreach (EffectPass pass in effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            graphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, 24, 0, 12, instances.Length);
        }
    }
}