using ProceduralGenerationLibrary.Noise;
using SharedLibrary.Transforms;
using static ProceduralGenerationLibrary.Noise.Noise;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Numerics;

namespace ProceduralGenerationLibrary.Texture;

public class TextureFactory
{
    [Range(2, 512)]
    public int resolution = 256;

    public float frequency = 1f;

    [Range(1, 8)]
    public int octaves = 1;

    [Range(1f, 4f)]
    public float lacunarity = 2f;

    [Range(0f, 1f)]
    public float persistence = 0.5f;

    [Range(1, 3)]
    public int dimensions = 3;

    public NoiseMethodType type;

    //public Gradient coloring;


    //private Texture2D CreateTexture()
    //{
    //    Texture2D Texture = new Texture2D(resolution, resolution, TextureFormat.RGB24, true);
    //    Texture.name = "Procedural Texture";
    //    Texture.wrapMode = TextureWrapMode.Clamp;
    //    Texture.filterMode = FilterMode.Trilinear;
    //    Texture.anisoLevel = 9;
    //    //GetComponent<MeshRenderer>().material.mainTexture = Texture;
    //    FillTexture(Texture);
    //    return Texture;
    //}


    //public void FillTexture(ref Texture2D texture2D)
    //{
    //    if (Texture.width != resolution)
    //    {
    //        Texture.Reinitialize(resolution, resolution);
    //    }

    //    Vector3 point00 = transform.TransformPoint(new Vector3(-0.5f, -0.5f));
    //    Vector3 point10 = transform.TransformPoint(new Vector3(0.5f, -0.5f));
    //    Vector3 point01 = transform.TransformPoint(new Vector3(-0.5f, 0.5f));
    //    Vector3 point11 = transform.TransformPoint(new Vector3(0.5f, 0.5f));

    //    NoiseMethod method = Noise.methods[(int)type][dimensions - 1];
    //    float stepSize = 1f / resolution;
    //    for (int y = 0; y < resolution; y++)
    //    {
    //        Vector3 point0 = Vector3.Lerp(point00, point01, (y + 0.5f) * stepSize);
    //        Vector3 point1 = Vector3.Lerp(point10, point11, (y + 0.5f) * stepSize);
    //        for (int x = 0; x < resolution; x++)
    //        {
    //            Vector3 point = Vector3.Lerp(point0, point1, (x + 0.5f) * stepSize);
    //            float sample = Noise.Sum(method, point, frequency, octaves, lacunarity, persistence);
    //            if (type != NoiseMethodType.Value)
    //            {
    //                sample = sample * 0.5f + 0.5f;
    //            }
    //            Texture.SetPixel(x, y, coloring.Evaluate(sample));
    //        }
    //    }
    //    Texture.Apply();
    //}
}