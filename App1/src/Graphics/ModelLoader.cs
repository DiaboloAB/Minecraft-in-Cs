using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace App1.Graphics;

public class BlockModel
{
    public VertexBuffer VertexBuffer { get; set; }
    public IndexBuffer IndexBuffer { get; set; }
    public List<SubMesh> SubMeshes { get; set; }
    
    public Vector2 TextureSize { get; set; }
    
    private List<Vector3> createVertices(Vector3 origin, Vector3 size)
    {
        
        
        float third = 1.0f / 3.0f;
        Vector3[] vertices =
        {
            // Front face
            new Vector3(0, size.Y, size.Z),
            new Vector3(size.X,  size.Y, size.Z),
            new Vector3(size.X, 0, size.Z),
            new Vector3(0, 0, size.Z),

            // Back face
            new Vector3(size.X, size.Y, 0),
            new Vector3(0, size.Y, 0),
            new Vector3(0, 0, 0),
            new Vector3(size.X, 0, 0),
            //
            // // Top face
            new Vector3(0, size.Y, 0),
            new Vector3(size.X, size.Y, 0),
            new Vector3(size.X, size.Y, size.Z),
            new Vector3(0, size.Y, size.Z),
            //
            // // Bottom face
            new Vector3(0, 0, size.Z),
            new Vector3(size.X, 0, size.Z),
            new Vector3(size.X, 0, 0),
            new Vector3(0, 0, 0),
            // //
            // // Left face
            new Vector3(0, size.Y, 0),
            new Vector3(0, size.Y, size.Z),
            new Vector3(0, 0, size.Z),
            new Vector3(0, 0, 0),
            //
            // Right face
            new Vector3(size.X, size.Y, size.Z),
            new Vector3(size.X, size.Y, 0),
            new Vector3(size.X, 0, 0),
            new Vector3(size.X, 0, size.Z)



        };
        
        List<Vector3> verticesList = new List<Vector3>();
        
        for (int i = 0; i < vertices.Length; i++)
        {
            verticesList.Add(vertices[i] + origin);
        }
        
        return verticesList;
        
    }
    
    private List<Vector2> createUVCords(MinecraftGeometry.Cube cube, Vector2 textureSize)
    {
        List<Vector2> uvs = new List<Vector2>();
        
        uvs.Add(new Vector2(cube.UV.South.UV[0] / textureSize.X, cube.UV.South.UV[1] / textureSize.Y));
        uvs.Add(new Vector2((cube.UV.South.UV[0] + cube.UV.South.UVSize[0]) / textureSize.X, cube.UV.South.UV[1] / textureSize.Y));
        uvs.Add(new Vector2((cube.UV.South.UV[0] + cube.UV.South.UVSize[0]) / textureSize.X, (cube.UV.South.UV[1] + cube.UV.South.UVSize[1]) / textureSize.Y));
        uvs.Add(new Vector2(cube.UV.South.UV[0] / textureSize.X, (cube.UV.South.UV[1] + cube.UV.South.UVSize[1]) / textureSize.Y));
        
        uvs.Add(new Vector2(cube.UV.North.UV[0] / textureSize.X, cube.UV.North.UV[1] / textureSize.Y));
        uvs.Add(new Vector2((cube.UV.North.UV[0] + cube.UV.North.UVSize[0]) / textureSize.X, cube.UV.North.UV[1] / textureSize.Y));
        uvs.Add(new Vector2((cube.UV.North.UV[0] + cube.UV.North.UVSize[0]) / textureSize.X, (cube.UV.North.UV[1] + cube.UV.North.UVSize[1]) / textureSize.Y));
        uvs.Add(new Vector2(cube.UV.North.UV[0] / textureSize.X, (cube.UV.North.UV[1] + cube.UV.North.UVSize[1]) / textureSize.Y));
        
        uvs.Add(new Vector2(cube.UV.Up.UV[0] / textureSize.X, cube.UV.Up.UV[1] / textureSize.Y));
        uvs.Add(new Vector2((cube.UV.Up.UV[0] + cube.UV.Up.UVSize[0]) / textureSize.X, cube.UV.Up.UV[1] / textureSize.Y));
        uvs.Add(new Vector2((cube.UV.Up.UV[0] + cube.UV.Up.UVSize[0]) / textureSize.X, (cube.UV.Up.UV[1] + cube.UV.Up.UVSize[1]) / textureSize.Y));
        uvs.Add(new Vector2(cube.UV.Up.UV[0] / textureSize.X, (cube.UV.Up.UV[1] + cube.UV.Up.UVSize[1]) / textureSize.Y));
        
        uvs.Add(new Vector2(cube.UV.Down.UV[0] / textureSize.X, cube.UV.Down.UV[1] / textureSize.Y));
        uvs.Add(new Vector2((cube.UV.Down.UV[0] + cube.UV.Down.UVSize[0]) / textureSize.X, cube.UV.Down.UV[1] / textureSize.Y));
        uvs.Add(new Vector2((cube.UV.Down.UV[0] + cube.UV.Down.UVSize[0]) / textureSize.X, (cube.UV.Down.UV[1] + cube.UV.Down.UVSize[1]) / textureSize.Y));
        uvs.Add(new Vector2(cube.UV.Down.UV[0] / textureSize.X, (cube.UV.Down.UV[1] + cube.UV.Down.UVSize[1]) / textureSize.Y));
        
        
        uvs.Add(new Vector2(cube.UV.East.UV[0] / textureSize.X, cube.UV.East.UV[1] / textureSize.Y));
        uvs.Add(new Vector2((cube.UV.East.UV[0] + cube.UV.East.UVSize[0]) / textureSize.X, cube.UV.East.UV[1] / textureSize.Y));
        uvs.Add(new Vector2((cube.UV.East.UV[0] + cube.UV.East.UVSize[0]) / textureSize.X, (cube.UV.East.UV[1] + cube.UV.East.UVSize[1]) / textureSize.Y));
        uvs.Add(new Vector2(cube.UV.East.UV[0] / textureSize.X, (cube.UV.East.UV[1] + cube.UV.East.UVSize[1]) / textureSize.Y));
        
        uvs.Add(new Vector2(cube.UV.West.UV[0] / textureSize.X, cube.UV.West.UV[1] / textureSize.Y));
        uvs.Add(new Vector2((cube.UV.West.UV[0] + cube.UV.West.UVSize[0]) / textureSize.X, cube.UV.West.UV[1] / textureSize.Y));
        uvs.Add(new Vector2((cube.UV.West.UV[0] + cube.UV.West.UVSize[0]) / textureSize.X, (cube.UV.West.UV[1] + cube.UV.West.UVSize[1]) / textureSize.Y));
        uvs.Add(new Vector2(cube.UV.West.UV[0] / textureSize.X, (cube.UV.West.UV[1] + cube.UV.West.UVSize[1]) / textureSize.Y));

        return uvs;
    }
    
    private void CreateBuffers(GraphicsDevice graphicsDevice)
    {
        List<VertexPositionTexture> vertexList = new List<VertexPositionTexture>();
        List<short> indexList = new List<short>();
        short index = 0;

        foreach (var subMesh in SubMeshes)
        {
            for (int i = 0; i < subMesh.Vertices.Count; i++)
            {
                vertexList.Add(new VertexPositionTexture(subMesh.Vertices[i], subMesh.UVCords[i]));
            }

            for (int i = 0; i < subMesh.Vertices.Count; i += 4)
            {
                indexList.Add(index);
                indexList.Add((short)(index + 1));
                indexList.Add((short)(index + 2));
                indexList.Add(index);
                indexList.Add((short)(index + 2));
                indexList.Add((short)(index + 3));
                index += 4;
            }
        }

        VertexBuffer = new VertexBuffer(graphicsDevice, VertexPositionTexture.VertexDeclaration, vertexList.Count, BufferUsage.WriteOnly);
        VertexBuffer.SetData(vertexList.ToArray());

        IndexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, indexList.Count, BufferUsage.WriteOnly);
        IndexBuffer.SetData(indexList.ToArray());
    }
    
    public static BlockModel LoadModel(ContentManager content, string modelName, GraphicsDevice graphicsDevice)
    {
        BlockModel model = new BlockModel();
        string filePath = Path.Combine(content.RootDirectory, modelName + ".json");
        
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Model file not found: " + filePath);
        }
        
        string json = File.ReadAllText(filePath);
        
        var rawModel = JsonSerializer.Deserialize<MinecraftGeometry.GeometryModel>(json);
        
        model.SubMeshes = new List<SubMesh>();
        
        foreach (var geometry in rawModel.Geometry)
        {
            model.TextureSize = new Vector2(geometry.TextureWidth, geometry.TextureHeight);
            
            foreach (var bone in geometry.Bones)
            {
                foreach (var cube in bone.Cubes)
                {
                    SubMesh subMesh = new SubMesh();
                    
                    subMesh.Origin = new Vector3(cube.Origin[0] / 16, (cube.Origin[1] - 8) / 16, cube.Origin[2] / 16);
                    Console.WriteLine("Origin: " + subMesh.Origin);
                    subMesh.Size = new Vector3(cube.Size[0] / 16, cube.Size[1] / 16, cube.Size[2] / 16);
                    Console.WriteLine("Size: " + subMesh.Size);
                    
                    subMesh.UVCords = model.createUVCords(cube, model.TextureSize);
                    subMesh.Vertices = model.createVertices(subMesh.Origin, subMesh.Size);
                    
                    model.SubMeshes.Add(subMesh);
                }
            }
        }
        
        model.CreateBuffers(graphicsDevice);
        return model;
    }

    public void PrintModel()
    {
        // Console.WriteLine("Model Details:");
        // foreach (var subMesh in SubMeshes)
        // {
        //     Console.WriteLine("SubMesh:");
        //  
        //     Console.WriteLine("Origin: " + subMesh.Origin);
        //     Console.WriteLine("Size: " + subMesh.Size);
        //     
        //     foreach (var uv in subMesh.UVCords)
        //     {
        //         Console.WriteLine("UV: " + uv);
        //     }
        // }
        
    }
}



public class SubMesh
{
    public Vector3 Origin { get; set; }
    public Vector3 Size { get; set; }
    
    public List<Vector3> Vertices { get; set; }
    public List<Vector2> UVCords { get; set; }
}

