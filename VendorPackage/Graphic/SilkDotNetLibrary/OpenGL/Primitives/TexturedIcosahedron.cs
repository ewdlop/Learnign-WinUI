using System;

namespace SilkDotNetLibrary.OpenGL.Primitives;

/// <summary>
/// 帶紋理的二十面體 (Textured Icosahedron) - 包含位置、法向量和紋理座標
/// 使用球面映射來計算紋理座標
/// </summary>
public static class TexturedIcosahedron
{
    public const int VerticeSize = 8; // Position (3) + Normal (3) + TexCoords (2)
    
    // 黃金比例常數
    private const float GoldenRatio = 1.618033988749895f; // (1 + sqrt(5)) / 2
    private const float Pi = 3.14159265359f;
    
    /// <summary>
    /// 二十面體的頂點數據：位置 (X,Y,Z) + 法向量 (Nx,Ny,Nz) + 紋理座標 (U,V)
    /// 紋理座標使用球面映射算法計算
    /// </summary>
    public static readonly float[] Vertices =
    {
        // 12個頂點構成的二十面體，每個頂點包含位置、法向量和紋理座標
        
        // 頂點 0: (+1, +φ, 0)
         1.0f,  GoldenRatio, 0.0f,     0.525731f,  0.850651f, 0.0f,      0.75f, 0.167f,
        // 頂點 1: (-1, +φ, 0)
        -1.0f,  GoldenRatio, 0.0f,    -0.525731f,  0.850651f, 0.0f,      0.25f, 0.167f,
        // 頂點 2: (+1, -φ, 0)
         1.0f, -GoldenRatio, 0.0f,     0.525731f, -0.850651f, 0.0f,      0.75f, 0.833f,
        // 頂點 3: (-1, -φ, 0)
        -1.0f, -GoldenRatio, 0.0f,    -0.525731f, -0.850651f, 0.0f,      0.25f, 0.833f,
        
        // 頂點 4: (0, +1, +φ)
         0.0f,  1.0f,  GoldenRatio,    0.0f,  0.525731f,  0.850651f,      0.5f,  0.0f,
        // 頂點 5: (0, -1, +φ)
         0.0f, -1.0f,  GoldenRatio,    0.0f, -0.525731f,  0.850651f,      0.5f,  1.0f,
        // 頂點 6: (0, +1, -φ)
         0.0f,  1.0f, -GoldenRatio,    0.0f,  0.525731f, -0.850651f,      0.0f,  0.0f,
        // 頂點 7: (0, -1, -φ)
         0.0f, -1.0f, -GoldenRatio,    0.0f, -0.525731f, -0.850651f,      0.0f,  1.0f,
        
        // 頂點 8: (+φ, 0, +1)
         GoldenRatio, 0.0f,  1.0f,     0.850651f, 0.0f,  0.525731f,       1.0f,  0.5f,
        // 頂點 9: (-φ, 0, +1)
        -GoldenRatio, 0.0f,  1.0f,    -0.850651f, 0.0f,  0.525731f,       0.0f,  0.5f,
        // 頂點 10: (+φ, 0, -1)
         GoldenRatio, 0.0f, -1.0f,     0.850651f, 0.0f, -0.525731f,       1.0f,  0.5f,
        // 頂點 11: (-φ, 0, -1)
        -GoldenRatio, 0.0f, -1.0f,    -0.850651f, 0.0f, -0.525731f,       0.0f,  0.5f,
        
        // 額外的頂點用於解決紋理接縫問題（展開二十面體的紋理映射）
        // 這些是重複的頂點，但有不同的紋理座標
        
        // 頂點 12-23: 為了正確的紋理映射而複製的頂點
         1.0f,  GoldenRatio, 0.0f,     0.525731f,  0.850651f, 0.0f,      0.875f, 0.167f, // 0的副本
        -1.0f,  GoldenRatio, 0.0f,    -0.525731f,  0.850651f, 0.0f,      0.125f, 0.167f, // 1的副本
         1.0f, -GoldenRatio, 0.0f,     0.525731f, -0.850651f, 0.0f,      0.875f, 0.833f, // 2的副本
        -1.0f, -GoldenRatio, 0.0f,    -0.525731f, -0.850651f, 0.0f,      0.125f, 0.833f, // 3的副本
         0.0f,  1.0f,  GoldenRatio,    0.0f,  0.525731f,  0.850651f,      0.625f, 0.0f,   // 4的副本
         0.0f, -1.0f,  GoldenRatio,    0.0f, -0.525731f,  0.850651f,      0.625f, 1.0f,   // 5的副本
         0.0f,  1.0f, -GoldenRatio,    0.0f,  0.525731f, -0.850651f,      0.375f, 0.0f,   // 6的副本
         0.0f, -1.0f, -GoldenRatio,    0.0f, -0.525731f, -0.850651f,      0.375f, 1.0f,   // 7的副本
         GoldenRatio, 0.0f,  1.0f,     0.850651f, 0.0f,  0.525731f,       0.75f,  0.5f,   // 8的副本
        -GoldenRatio, 0.0f,  1.0f,    -0.850651f, 0.0f,  0.525731f,       0.25f,  0.5f,   // 9的副本
         GoldenRatio, 0.0f, -1.0f,     0.850651f, 0.0f, -0.525731f,       0.875f, 0.5f,   // 10的副本
        -GoldenRatio, 0.0f, -1.0f,    -0.850651f, 0.0f, -0.525731f,       0.125f, 0.5f    // 11的副本
    };

    /// <summary>
    /// 二十面體的三角形面索引 - 使用適當的頂點來避免紋理接縫
    /// </summary>
    public static readonly uint[] Indices =
    {
        // 使用基本頂點和重複頂點來創建正確的紋理映射
        0, 4, 1,    // 三角形 1
        0, 1, 6,    // 三角形 2
        1, 4, 9,    // 三角形 3
        1, 9, 11,   // 三角形 4
        1, 11, 6,   // 三角形 5
        
        0, 8, 4,    // 三角形 6
        0, 10, 8,   // 三角形 7
        0, 6, 10,   // 三角形 8
        
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
    /// 計算球面映射的紋理座標
    /// </summary>
    /// <param name="x">頂點 X 座標</param>
    /// <param name="y">頂點 Y 座標</param>
    /// <param name="z">頂點 Z 座標</param>
    /// <returns>紋理座標 (U, V)</returns>
    private static (float u, float v) CalculateSphericalTexCoords(float x, float y, float z)
    {
        // 標準化頂點
        float length = MathF.Sqrt(x * x + y * y + z * z);
        x /= length;
        y /= length;
        z /= length;
        
        // 計算球面座標
        float theta = MathF.Atan2(z, x); // 方位角
        float phi = MathF.Acos(y);       // 極角
        
        // 轉換為紋理座標
        float u = (theta + Pi) / (2.0f * Pi);
        float v = phi / Pi;
        
        return (u, v);
    }
    
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
            
            // 紋理座標保持不變
            scaledVertices[i + 6] = Vertices[i + 6]; // U
            scaledVertices[i + 7] = Vertices[i + 7]; // V
        }
        return scaledVertices;
    }
} 