using System;

namespace SilkDotNetLibrary.OpenGL.Primitives;

/// <summary>
/// 二十面體 (Icosahedron) - 正二十面體是一個有20個正三角形面的柏拉圖立體
/// 基於黃金比例構建，具有完美的幾何對稱性
/// </summary>
public static class Icosahedron
{
    public const int VerticeSize = 6; // Position (3) + Normal (3)
    
    // 黃金比例常數
    private const float GoldenRatio = 1.618033988749895f; // (1 + sqrt(5)) / 2
    
    /// <summary>
    /// 二十面體的頂點數據：位置 (X,Y,Z) + 法向量 (Nx,Ny,Nz)
    /// 每個頂點的法向量就是其標準化的位置向量（因為是從中心發出的）
    /// </summary>
    public static readonly float[] Vertices =
    {
        // 12個頂點構成的二十面體，基於黃金比例的矩形
        
        // 前面5個頂點（上半部分）
        // 頂點 0: (+1, +φ, 0)
         1.0f,  GoldenRatio, 0.0f,     0.525731f,  0.850651f, 0.0f,
        // 頂點 1: (-1, +φ, 0)
        -1.0f,  GoldenRatio, 0.0f,    -0.525731f,  0.850651f, 0.0f,
        // 頂點 2: (+1, -φ, 0)
         1.0f, -GoldenRatio, 0.0f,     0.525731f, -0.850651f, 0.0f,
        // 頂點 3: (-1, -φ, 0)
        -1.0f, -GoldenRatio, 0.0f,    -0.525731f, -0.850651f, 0.0f,
        
        // 中間4個頂點
        // 頂點 4: (0, +1, +φ)
         0.0f,  1.0f,  GoldenRatio,    0.0f,  0.525731f,  0.850651f,
        // 頂點 5: (0, -1, +φ)
         0.0f, -1.0f,  GoldenRatio,    0.0f, -0.525731f,  0.850651f,
        // 頂點 6: (0, +1, -φ)
         0.0f,  1.0f, -GoldenRatio,    0.0f,  0.525731f, -0.850651f,
        // 頂點 7: (0, -1, -φ)
         0.0f, -1.0f, -GoldenRatio,    0.0f, -0.525731f, -0.850651f,
        
        // 後面4個頂點
        // 頂點 8: (+φ, 0, +1)
         GoldenRatio, 0.0f,  1.0f,     0.850651f, 0.0f,  0.525731f,
        // 頂點 9: (-φ, 0, +1)
        -GoldenRatio, 0.0f,  1.0f,    -0.850651f, 0.0f,  0.525731f,
        // 頂點 10: (+φ, 0, -1)
         GoldenRatio, 0.0f, -1.0f,     0.850651f, 0.0f, -0.525731f,
        // 頂點 11: (-φ, 0, -1)
        -GoldenRatio, 0.0f, -1.0f,    -0.850651f, 0.0f, -0.525731f
    };

    /// <summary>
    /// 二十面體的三角形面索引 - 20個三角形面，每個面3個頂點
    /// 索引按逆時針順序排列以確保正確的面法向量
    /// </summary>
    public static readonly uint[] Indices =
    {
        // 上半部分的三角形（圍繞頂點 0 和 1）
        0, 4, 1,    // 三角形 1
        0, 1, 6,    // 三角形 2
        1, 4, 9,    // 三角形 3
        1, 9, 11,   // 三角形 4
        1, 11, 6,   // 三角形 5
        
        // 上半部分的三角形（圍繞頂點 0）
        0, 8, 4,    // 三角形 6
        0, 10, 8,   // 三角形 7
        0, 6, 10,   // 三角形 8
        
        // 中間部分的三角形
        4, 8, 5,    // 三角形 9
        8, 2, 5,    // 三角形 10
        8, 10, 2,   // 三角形 11
        10, 7, 2,   // 三角形 12
        10, 6, 7,   // 三角形 13
        6, 11, 7,   // 三角形 14
        11, 3, 7,   // 三角形 15
        11, 9, 3,   // 三角形 16
        9, 5, 3,    // 三角形 17
        9, 4, 5,    // 三角形 18
        
        // 下半部分的三角形（圍繞頂點 2 和 3）
        2, 7, 3,    // 三角形 19
        3, 5, 2     // 三角形 20
    };
    
    /// <summary>
    /// 獲取二十面體的總頂點數
    /// </summary>
    public static int VertexCount => Vertices.Length / VerticeSize;
    
    /// <summary>
    /// 獲取二十面體的總三角形數
    /// </summary>
    public static int TriangleCount => Indices.Length / 3;
    
    /// <summary>
    /// 獲取包圍球半徑（從中心到任意頂點的距離）
    /// </summary>
    public static float BoundingSphereRadius => MathF.Sqrt(1.0f + GoldenRatio * GoldenRatio);
    
    /// <summary>
    /// 計算縮放後的頂點數據
    /// </summary>
    /// <param name="scale">縮放因子</param>
    /// <returns>縮放後的頂點數組</returns>
    public static float[] GetScaledVertices(float scale)
    {
        var scaledVertices = new float[Vertices.Length];
        for (int i = 0; i < Vertices.Length; i += VerticeSize)
        {
            // 縮放位置
            scaledVertices[i] = Vertices[i] * scale;         // X
            scaledVertices[i + 1] = Vertices[i + 1] * scale; // Y
            scaledVertices[i + 2] = Vertices[i + 2] * scale; // Z
            
            // 法向量保持不變（已經是標準化的）
            scaledVertices[i + 3] = Vertices[i + 3]; // Nx
            scaledVertices[i + 4] = Vertices[i + 4]; // Ny
            scaledVertices[i + 5] = Vertices[i + 5]; // Nz
        }
        return scaledVertices;
    }
} 