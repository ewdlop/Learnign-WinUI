using System;

namespace SilkDotNetLibrary.OpenGL.Primitives;

/// <summary>
/// 完整的二十面體 (Full Icosahedron) - 包含所有頂點屬性以匹配 Vertex 結構
/// Position (3) + Normal (3) + TexCoords (2) + Tangent (3) + BiTangent (3) + Color (3) = 17 floats
/// </summary>
public static class FullIcosahedron
{
    public const int VerticeSize = 17; // 與 Vertex 結構完全匹配
    
    // 黃金比例常數
    private const float GoldenRatio = 1.618033988749895f; // (1 + sqrt(5)) / 2
    
    /// <summary>
    /// 二十面體的完整頂點數據，包含所有屬性
    /// 每個頂點 17 個浮點數：Position(3) + Normal(3) + TexCoords(2) + Tangent(3) + BiTangent(3) + Color(3)
    /// </summary>
    public static readonly float[] Vertices =
    {
        // 頂點 0: (+1, +φ, 0) - 標準化後的位置、法向量、紋理座標、切線、雙切線、顏色
        1.0f,  GoldenRatio, 0.0f,                    // Position
        0.525731f,  0.850651f, 0.0f,                 // Normal
        0.75f, 0.167f,                               // TexCoords
        0.0f, 0.0f, 1.0f,                           // Tangent (計算得出)
        -0.850651f, 0.525731f, 0.0f,                // BiTangent (法向量 × 切線)
        1.0f, 0.8f, 0.2f,                           // Color (金色)
        
        // 頂點 1: (-1, +φ, 0)
        -1.0f,  GoldenRatio, 0.0f,                   // Position
        -0.525731f,  0.850651f, 0.0f,                // Normal
        0.25f, 0.167f,                               // TexCoords
        0.0f, 0.0f, 1.0f,                           // Tangent
        0.850651f, 0.525731f, 0.0f,                 // BiTangent
        1.0f, 0.8f, 0.2f,                           // Color
        
        // 頂點 2: (+1, -φ, 0)
        1.0f, -GoldenRatio, 0.0f,                    // Position
        0.525731f, -0.850651f, 0.0f,                 // Normal
        0.75f, 0.833f,                               // TexCoords
        0.0f, 0.0f, 1.0f,                           // Tangent
        0.850651f, 0.525731f, 0.0f,                 // BiTangent
        1.0f, 0.8f, 0.2f,                           // Color
        
        // 頂點 3: (-1, -φ, 0)
        -1.0f, -GoldenRatio, 0.0f,                   // Position
        -0.525731f, -0.850651f, 0.0f,                // Normal
        0.25f, 0.833f,                               // TexCoords
        0.0f, 0.0f, 1.0f,                           // Tangent
        -0.850651f, 0.525731f, 0.0f,                // BiTangent
        1.0f, 0.8f, 0.2f,                           // Color
        
        // 頂點 4: (0, +1, +φ)
        0.0f,  1.0f,  GoldenRatio,                   // Position
        0.0f,  0.525731f,  0.850651f,               // Normal
        0.5f,  0.0f,                                 // TexCoords
        1.0f, 0.0f, 0.0f,                           // Tangent
        0.0f, 0.850651f, -0.525731f,                // BiTangent
        1.0f, 0.8f, 0.2f,                           // Color
        
        // 頂點 5: (0, -1, +φ)
        0.0f, -1.0f,  GoldenRatio,                   // Position
        0.0f, -0.525731f,  0.850651f,               // Normal
        0.5f,  1.0f,                                 // TexCoords
        1.0f, 0.0f, 0.0f,                           // Tangent
        0.0f, -0.850651f, -0.525731f,               // BiTangent
        1.0f, 0.8f, 0.2f,                           // Color
        
        // 頂點 6: (0, +1, -φ)
        0.0f,  1.0f, -GoldenRatio,                   // Position
        0.0f,  0.525731f, -0.850651f,               // Normal
        0.0f,  0.0f,                                 // TexCoords
        1.0f, 0.0f, 0.0f,                           // Tangent
        0.0f, 0.850651f, 0.525731f,                 // BiTangent
        1.0f, 0.8f, 0.2f,                           // Color
        
        // 頂點 7: (0, -1, -φ)
        0.0f, -1.0f, -GoldenRatio,                   // Position
        0.0f, -0.525731f, -0.850651f,               // Normal
        0.0f,  1.0f,                                 // TexCoords
        1.0f, 0.0f, 0.0f,                           // Tangent
        0.0f, -0.850651f, 0.525731f,                // BiTangent
        1.0f, 0.8f, 0.2f,                           // Color
        
        // 頂點 8: (+φ, 0, +1)
        GoldenRatio, 0.0f,  1.0f,                    // Position
        0.850651f, 0.0f,  0.525731f,                // Normal
        1.0f,  0.5f,                                 // TexCoords
        0.0f, 1.0f, 0.0f,                           // Tangent
        -0.525731f, 0.0f, 0.850651f,                // BiTangent
        1.0f, 0.8f, 0.2f,                           // Color
        
        // 頂點 9: (-φ, 0, +1)
        -GoldenRatio, 0.0f,  1.0f,                   // Position
        -0.850651f, 0.0f,  0.525731f,               // Normal
        0.0f,  0.5f,                                 // TexCoords
        0.0f, 1.0f, 0.0f,                           // Tangent
        0.525731f, 0.0f, 0.850651f,                 // BiTangent
        1.0f, 0.8f, 0.2f,                           // Color
        
        // 頂點 10: (+φ, 0, -1)
        GoldenRatio, 0.0f, -1.0f,                    // Position
        0.850651f, 0.0f, -0.525731f,                // Normal
        1.0f,  0.5f,                                 // TexCoords
        0.0f, 1.0f, 0.0f,                           // Tangent
        0.525731f, 0.0f, 0.850651f,                 // BiTangent
        1.0f, 0.8f, 0.2f,                           // Color
        
        // 頂點 11: (-φ, 0, -1)
        -GoldenRatio, 0.0f, -1.0f,                   // Position
        -0.850651f, 0.0f, -0.525731f,               // Normal
        0.0f,  0.5f,                                 // TexCoords
        0.0f, 1.0f, 0.0f,                           // Tangent
        -0.525731f, 0.0f, 0.850651f,                // BiTangent
        1.0f, 0.8f, 0.2f,                           // Color
    };

    /// <summary>
    /// 二十面體的三角形面索引 - 20個三角形面
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
} 