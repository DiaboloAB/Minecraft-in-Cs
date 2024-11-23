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
    public List<SubMesh> SubMeshes { get; set; }
    public int TextureIndex { get; set; }

    private List<Vector3> createVertices(Vector3 origin, Vector3 size)
    {
        List<Vector3> vertices = new List<Vector3>();
        
        // Front face
        vertices.Add(new Vector3(origin.X, origin.Y, origin.Z));
        vertices.Add(new Vector3(origin.X + size.X, origin.Y, origin.Z));
        vertices.Add(new Vector3(origin.X + size.X, origin.Y + size.Y, origin.Z));
        vertices.Add(new Vector3(origin.X, origin.Y + size.Y, origin.Z));
        
        // Back face
        vertices.Add(new Vector3(origin.X, origin.Y, origin.Z + size.Z));
        vertices.Add(new Vector3(origin.X + size.X, origin.Y, origin.Z + size.Z));
        vertices.Add(new Vector3(origin.X + size.X, origin.Y + size.Y, origin.Z + size.Z));
        vertices.Add(new Vector3(origin.X, origin.Y + size.Y, origin.Z + size.Z));
        
        // Top face
        vertices.Add(new Vector3(origin.X, origin.Y + size.Y, origin.Z));
        vertices.Add(new Vector3(origin.X + size.X, origin.Y + size.Y, origin.Z));
        vertices.Add(new Vector3(origin.X + size.X, origin.Y + size.Y, origin.Z + size.Z));
        vertices.Add(new Vector3(origin.X, origin.Y + size.Y, origin.Z + size.Z));
        
        // Bottom face
        vertices.Add(new Vector3(origin.X, origin.Y, origin.Z));
        vertices.Add(new Vector3(origin.X + size.X, origin.Y, origin.Z));
        vertices.Add(new Vector3(origin.X + size.X, origin.Y, origin.Z + size.Z));
        vertices.Add(new Vector3(origin.X, origin.Y, origin.Z + size.Z));
        
        // Right face
        vertices.Add(new Vector3(origin.X + size.X, origin.Y, origin.Z));
        vertices.Add(new Vector3(origin.X + size.X, origin.Y, origin.Z + size.Z));
        vertices.Add(new Vector3(origin.X + size.X, origin.Y + size.Y, origin.Z + size.Z));
        vertices.Add(new Vector3(origin.X + size.X, origin.Y + size.Y, origin.Z));
        
        // Left face
        vertices.Add(new Vector3(origin.X, origin.Y, origin.Z));
        vertices.Add(new Vector3(origin.X, origin.Y, origin.Z + size.Z));
        vertices.Add(new Vector3(origin.X, origin.Y + size.Y, origin.Z + size.Z));
        vertices.Add(new Vector3(origin.X, origin.Y + size.Y, origin.Z));
        
        return vertices;
    }
    
    private List<Vector2> createUVCords(MinecraftGeometry.Cube cube)
    {
        List<Vector2> uvs = new List<Vector2>();
        
        uvs.Add(new Vector2(cube.UV.North.UV[0], cube.UV.North.UV[1]));
        uvs.Add(new Vector2(cube.UV.North.UV[0] + cube.UV.North.UVSize[0], cube.UV.North.UV[1]));
        uvs.Add(new Vector2(cube.UV.North.UV[0] + cube.UV.North.UVSize[0], cube.UV.North.UV[1] + cube.UV.North.UVSize[1]));
        uvs.Add(new Vector2(cube.UV.North.UV[0], cube.UV.North.UV[1] + cube.UV.North.UVSize[1]));
        
        uvs.Add(new Vector2(cube.UV.East.UV[0], cube.UV.East.UV[1]));
        uvs.Add(new Vector2(cube.UV.East.UV[0] + cube.UV.East.UVSize[0], cube.UV.East.UV[1]));
        uvs.Add(new Vector2(cube.UV.East.UV[0] + cube.UV.East.UVSize[0], cube.UV.East.UV[1] + cube.UV.East.UVSize[1]));
        uvs.Add(new Vector2(cube.UV.East.UV[0], cube.UV.East.UV[1] + cube.UV.East.UVSize[1]));
        
        uvs.Add(new Vector2(cube.UV.South.UV[0], cube.UV.South.UV[1]));
        uvs.Add(new Vector2(cube.UV.South.UV[0] + cube.UV.South.UVSize[0], cube.UV.South.UV[1]));
        uvs.Add(new Vector2(cube.UV.South.UV[0] + cube.UV.South.UVSize[0], cube.UV.South.UV[1] + cube.UV.South.UVSize[1]));
        uvs.Add(new Vector2(cube.UV.South.UV[0], cube.UV.South.UV[1] + cube.UV.South.UVSize[1]));
        
        uvs.Add(new Vector2(cube.UV.West.UV[0], cube.UV.West.UV[1]));
        uvs.Add(new Vector2(cube.UV.West.UV[0] + cube.UV.West.UVSize[0], cube.UV.West.UV[1]));
        uvs.Add(new Vector2(cube.UV.West.UV[0] + cube.UV.West.UVSize[0], cube.UV.West.UV[1] + cube.UV.West.UVSize[1]));
        uvs.Add(new Vector2(cube.UV.West.UV[0], cube.UV.West.UV[1] + cube.UV.West.UVSize[1]));
        
        uvs.Add(new Vector2(cube.UV.Up.UV[0], cube.UV.Up.UV[1]));
        uvs.Add(new Vector2(cube.UV.Up.UV[0] + cube.UV.Up.UVSize[0], cube.UV.Up.UV[1]));
        uvs.Add(new Vector2(cube.UV.Up.UV[0] + cube.UV.Up.UVSize[0], cube.UV.Up.UV[1] + cube.UV.Up.UVSize[1]));
        uvs.Add(new Vector2(cube.UV.Up.UV[0], cube.UV.Up.UV[1] + cube.UV.Up.UVSize[1]));
        
        uvs.Add(new Vector2(cube.UV.Down.UV[0], cube.UV.Down.UV[1]));
        uvs.Add(new Vector2(cube.UV.Down.UV[0] + cube.UV.Down.UVSize[0], cube.UV.Down.UV[1]));
        uvs.Add(new Vector2(cube.UV.Down.UV[0] + cube.UV.Down.UVSize[0], cube.UV.Down.UV[1] + cube.UV.Down.UVSize[1]));
        uvs.Add(new Vector2(cube.UV.Down.UV[0], cube.UV.Down.UV[1] + cube.UV.Down.UVSize[1]));
        
        return uvs;
    }
    
    public static BlockModel LoadModel(ContentManager content, string modelName)
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
            foreach (var bone in geometry.Bones)
            {
                foreach (var cube in bone.Cubes)
                {
                    SubMesh subMesh = new SubMesh();
                    
                    subMesh.Origin = new Vector3(cube.Origin[0] - 8, cube.Origin[1] - 8, cube.Origin[2] - 8);
                    subMesh.Size = new Vector3(cube.Size[0], cube.Size[1], cube.Size[2]);
                    
                    subMesh.UVCords = model.createUVCords(cube);
                    subMesh.Vertices = model.createVertices(subMesh.Origin, subMesh.Size);
                    
                    model.SubMeshes.Add(subMesh);
                }
            }
        }
        
        
        return model;
    }

    public void PrintModel()
    {
        Console.WriteLine("Model Details:");
        foreach (var subMesh in SubMeshes)
        {
            Console.WriteLine("SubMesh:");
         
            Console.WriteLine("Origin: " + subMesh.Origin);
            Console.WriteLine("Size: " + subMesh.Size);
            
            foreach (var uv in subMesh.UVCords)
            {
                Console.WriteLine("UV: " + uv);
            }
        }
        
    }
    
    public short[] createIndices()
    {
        List<short> indices = new List<short>();
        short index = 0;
        foreach (var subMesh in SubMeshes)
        {
            for (int i = 0; i < subMesh.UVCords.Count; i += 4)
            {
                indices.Add(index);
                indices.Add((short)(index + 1));
                indices.Add((short)(index + 2));
                indices.Add(index);
                indices.Add((short)(index + 2));
                indices.Add((short)(index + 3));
                index += 4;
            }
        }
        return indices.ToArray();
    }
}



public class SubMesh
{
    public Vector3 Origin { get; set; }
    public Vector3 Size { get; set; }
    
    public List<Vector3> Vertices { get; set; }
    public List<Vector2> UVCords { get; set; }
}

