using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MinecraftGeometry
{
    public class GeometryModel
    {
        [JsonPropertyName("minecraft:geometry")]
        public List<Geometry> Geometry { get; set; }
    }

    public class Geometry
    {
        [JsonPropertyName("bones")]
        public List<Bone> Bones { get; set; }
        
        [JsonPropertyName("texture_width")]
        public int TextureWidth { get; set; }
        
        [JsonPropertyName("texture_height")]
        public int TextureHeight { get; set; }
        
    }
    

    public class Bone
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("pivot")]
        public List<float> Pivot { get; set; }

        [JsonPropertyName("cubes")]
        public List<Cube> Cubes { get; set; }
    }

    public class Cube
    {
        [JsonPropertyName("origin")]
        public List<float> Origin { get; set; }

        [JsonPropertyName("size")]
        public List<float> Size { get; set; }

        [JsonPropertyName("uv")]
        public UvMapping UV { get; set; }
    }

    public class UvMapping
    {
        [JsonPropertyName("north")]
        public FaceUV North { get; set; }

        [JsonPropertyName("east")]
        public FaceUV East { get; set; }

        [JsonPropertyName("south")]
        public FaceUV South { get; set; }

        [JsonPropertyName("west")]
        public FaceUV West { get; set; }

        [JsonPropertyName("up")]
        public FaceUV Up { get; set; }

        [JsonPropertyName("down")]
        public FaceUV Down { get; set; }
    }

    public class FaceUV
    {
        [JsonPropertyName("uv")]
        public List<float> UV { get; set; }

        [JsonPropertyName("uv_size")]
        public List<float> UVSize { get; set; }
    }

}