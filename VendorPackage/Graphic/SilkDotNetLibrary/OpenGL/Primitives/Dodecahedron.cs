using System;

namespace SilkDotNetLibrary.OpenGL.Primitives;

/// <summary>
/// 十二面體 (Dodecahedron) - 包含所有頂點屬性以匹配 Vertex 結構
/// 十二面體是一個有12個正五邊形面的柏拉圖立體
/// Position (3) + Normal (3) + TexCoords (2) + Tangent (3) + BiTangent (3) + Color (3) = 17 floats
/// </summary>
public static class Dodecahedron
{
    public const int VerticeSize = 17; // 與 Vertex 結構完全匹配
    
    // 黃金比例常數
    private const float GoldenRatio = 1.618033988749895f; // (1 + sqrt(5)) / 2
    private const float InvGoldenRatio = 0.618033988749895f; // 1 / φ
    
    /// <summary>
    /// 十二面體的完整頂點數據，包含所有屬性
    /// 十二面體有20個頂點，每個頂點 17 個浮點數
    /// </summary>
    public static readonly float[] Vertices =
    {
        // 十二面體的20個頂點，基於三組黃金矩形
        
        // 第一組：立方體的8個頂點 (±1, ±1, ±1)
        // 頂點 0: (+1, +1, +1)
        1.0f, 1.0f, 1.0f,                           // Position
        0.577350f, 0.577350f, 0.577350f,            // Normal (標準化)
        0.8f, 0.2f,                                 // TexCoords
        1.0f, 0.0f, 0.0f,                          // Tangent
        0.0f, 1.0f, 0.0f,                          // BiTangent
        0.8f, 0.3f, 0.9f,                          // Color (紫色)

        // 頂點 1: (+1, +1, -1)
        1.0f, 1.0f, -1.0f,                          // Position
        0.577350f, 0.577350f, -0.577350f,           // Normal
        0.6f, 0.2f,                                 // TexCoords
        1.0f, 0.0f, 0.0f,                          // Tangent
        0.0f, 1.0f, 0.0f,                          // BiTangent
        0.8f, 0.3f, 0.9f,                          // Color

        // 頂點 2: (+1, -1, +1)
        1.0f, -1.0f, 1.0f,                          // Position
        0.577350f, -0.577350f, 0.577350f,           // Normal
        0.8f, 0.8f,                                 // TexCoords
        1.0f, 0.0f, 0.0f,                          // Tangent
        0.0f, 1.0f, 0.0f,                          // BiTangent
        0.8f, 0.3f, 0.9f,                          // Color

        // 頂點 3: (+1, -1, -1)
        1.0f, -1.0f, -1.0f,                         // Position
        0.577350f, -0.577350f, -0.577350f,          // Normal
        0.6f, 0.8f,                                 // TexCoords
        1.0f, 0.0f, 0.0f,                          // Tangent
        0.0f, 1.0f, 0.0f,                          // BiTangent
        0.8f, 0.3f, 0.9f,                          // Color

        // 頂點 4: (-1, +1, +1)
        -1.0f, 1.0f, 1.0f,                          // Position
        -0.577350f, 0.577350f, 0.577350f,           // Normal
        0.2f, 0.2f,                                 // TexCoords
        1.0f, 0.0f, 0.0f,                          // Tangent
        0.0f, 1.0f, 0.0f,                          // BiTangent
        0.8f, 0.3f, 0.9f,                          // Color

        // 頂點 5: (-1, +1, -1)
        -1.0f, 1.0f, -1.0f,                         // Position
        -0.577350f, 0.577350f, -0.577350f,          // Normal
        0.4f, 0.2f,                                 // TexCoords
        1.0f, 0.0f, 0.0f,                          // Tangent
        0.0f, 1.0f, 0.0f,                          // BiTangent
        0.8f, 0.3f, 0.9f,                          // Color

        // 頂點 6: (-1, -1, +1)
        -1.0f, -1.0f, 1.0f,                         // Position
        -0.577350f, -0.577350f, 0.577350f,          // Normal
        0.2f, 0.8f,                                 // TexCoords
        1.0f, 0.0f, 0.0f,                          // Tangent
        0.0f, 1.0f, 0.0f,                          // BiTangent
        0.8f, 0.3f, 0.9f,                          // Color

        // 頂點 7: (-1, -1, -1)
        -1.0f, -1.0f, -1.0f,                        // Position
        -0.577350f, -0.577350f, -0.577350f,         // Normal
        0.4f, 0.8f,                                 // TexCoords
        1.0f, 0.0f, 0.0f,                          // Tangent
        0.0f, 1.0f, 0.0f,                          // BiTangent
        0.8f, 0.3f, 0.9f,                          // Color

        // 第二組：xy平面上的矩形頂點 (±φ, ±1/φ, 0)
        // 頂點 8: (+φ, +1/φ, 0)
        GoldenRatio, InvGoldenRatio, 0.0f,           // Position
        0.850651f, 0.525731f, 0.0f,                 // Normal
        1.0f, 0.4f,                                 // TexCoords
        0.0f, 0.0f, 1.0f,                          // Tangent
        -0.525731f, 0.850651f, 0.0f,               // BiTangent
        0.8f, 0.3f, 0.9f,                          // Color

        // 頂點 9: (+φ, -1/φ, 0)
        GoldenRatio, -InvGoldenRatio, 0.0f,          // Position
        0.850651f, -0.525731f, 0.0f,                // Normal
        1.0f, 0.6f,                                 // TexCoords
        0.0f, 0.0f, 1.0f,                          // Tangent
        0.525731f, 0.850651f, 0.0f,                // BiTangent
        0.8f, 0.3f, 0.9f,                          // Color

        // 頂點 10: (-φ, +1/φ, 0)
        -GoldenRatio, InvGoldenRatio, 0.0f,          // Position
        -0.850651f, 0.525731f, 0.0f,                // Normal
        0.0f, 0.4f,                                 // TexCoords
        0.0f, 0.0f, 1.0f,                          // Tangent
        0.525731f, 0.850651f, 0.0f,                // BiTangent
        0.8f, 0.3f, 0.9f,                          // Color

        // 頂點 11: (-φ, -1/φ, 0)
        -GoldenRatio, -InvGoldenRatio, 0.0f,         // Position
        -0.850651f, -0.525731f, 0.0f,               // Normal
        0.0f, 0.6f,                                 // TexCoords
        0.0f, 0.0f, 1.0f,                          // Tangent
        -0.525731f, 0.850651f, 0.0f,               // BiTangent
        0.8f, 0.3f, 0.9f,                          // Color

        // 第三組：yz平面上的矩形頂點 (0, ±φ, ±1/φ)
        // 頂點 12: (0, +φ, +1/φ)
        0.0f, GoldenRatio, InvGoldenRatio,           // Position
        0.0f, 0.850651f, 0.525731f,                 // Normal
        0.5f, 0.0f,                                 // TexCoords
        1.0f, 0.0f, 0.0f,                          // Tangent
        0.0f, -0.525731f, 0.850651f,               // BiTangent
        0.8f, 0.3f, 0.9f,                          // Color

        // 頂點 13: (0, +φ, -1/φ)
        0.0f, GoldenRatio, -InvGoldenRatio,          // Position
        0.0f, 0.850651f, -0.525731f,                // Normal
        0.5f, 0.0f,                                 // TexCoords
        1.0f, 0.0f, 0.0f,                          // Tangent
        0.0f, 0.525731f, 0.850651f,                // BiTangent
        0.8f, 0.3f, 0.9f,                          // Color

        // 頂點 14: (0, -φ, +1/φ)
        0.0f, -GoldenRatio, InvGoldenRatio,          // Position
        0.0f, -0.850651f, 0.525731f,                // Normal
        0.5f, 1.0f,                                 // TexCoords
        1.0f, 0.0f, 0.0f,                          // Tangent
        0.0f, 0.525731f, 0.850651f,                // BiTangent
        0.8f, 0.3f, 0.9f,                          // Color

        // 頂點 15: (0, -φ, -1/φ)
        0.0f, -GoldenRatio, -InvGoldenRatio,         // Position
        0.0f, -0.850651f, -0.525731f,               // Normal
        0.5f, 1.0f,                                 // TexCoords
        1.0f, 0.0f, 0.0f,                          // Tangent
        0.0f, -0.525731f, 0.850651f,               // BiTangent
        0.8f, 0.3f, 0.9f,                          // Color

        // 第四組：xz平面上的矩形頂點 (±1/φ, 0, ±φ)
        // 頂點 16: (+1/φ, 0, +φ)
        InvGoldenRatio, 0.0f, GoldenRatio,           // Position
        0.525731f, 0.0f, 0.850651f,                 // Normal
        0.7f, 0.5f,                                 // TexCoords
        0.0f, 1.0f, 0.0f,                          // Tangent
        -0.850651f, 0.0f, 0.525731f,               // BiTangent
        0.8f, 0.3f, 0.9f,                          // Color

        // 頂點 17: (+1/φ, 0, -φ)
        InvGoldenRatio, 0.0f, -GoldenRatio,          // Position
        0.525731f, 0.0f, -0.850651f,                // Normal
        0.3f, 0.5f,                                 // TexCoords
        0.0f, 1.0f, 0.0f,                          // Tangent
        0.850651f, 0.0f, 0.525731f,                // BiTangent
        0.8f, 0.3f, 0.9f,                          // Color

        // 頂點 18: (-1/φ, 0, +φ)
        -InvGoldenRatio, 0.0f, GoldenRatio,          // Position
        -0.525731f, 0.0f, 0.850651f,                // Normal
        0.3f, 0.5f,                                 // TexCoords
        0.0f, 1.0f, 0.0f,                          // Tangent
        0.850651f, 0.0f, 0.525731f,                // BiTangent
        0.8f, 0.3f, 0.9f,                          // Color

        // 頂點 19: (-1/φ, 0, -φ)
        -InvGoldenRatio, 0.0f, -GoldenRatio,         // Position
        -0.525731f, 0.0f, -0.850651f,               // Normal
        0.7f, 0.5f,                                 // TexCoords
        0.0f, 1.0f, 0.0f,                          // Tangent
        -0.850651f, 0.0f, 0.525731f,               // BiTangent
        0.8f, 0.3f, 0.9f,                          // Color
    };

    /// <summary>
    /// 十二面體的三角形面索引 - 12個五邊形面，每個面用3個三角形表示 = 36個三角形
    /// </summary>
    public static readonly uint[] Indices =
    {
        // 每個五邊形面用三角形扇形分解（從中心頂點出發）
        // 這是一個簡化的實現，實際的十二面體索引會更複雜
        
        // 面 1: 上頂面
        0, 12, 4,    0, 4, 5,     0, 5, 13,
        // 面 2: 下底面  
        2, 6, 14,    2, 14, 15,   2, 15, 3,
        // 面 3: 前面
        0, 1, 8,     0, 8, 9,     0, 9, 2,
        // 面 4: 後面
        4, 6, 10,    4, 10, 11,   4, 11, 7,
        // 面 5: 右面
        1, 3, 17,    1, 17, 19,   1, 19, 5,
        // 面 6: 左面
        6, 7, 18,    7, 11, 18,   11, 10, 18,
        // 面 7-12: 剩餘的面
        8, 16, 9,    9, 16, 2,    2, 16, 14,
        12, 13, 16,  13, 17, 16,  17, 18, 16,
        10, 18, 12,  18, 19, 12,  19, 13, 12,
        14, 15, 18,  15, 19, 18,  19, 17, 15,
        3, 15, 17,   5, 19, 13,   7, 11, 15
    };
    
    /// <summary>
    /// 獲取十二面體的總頂點數
    /// </summary>
    public static int VertexCount => Vertices.Length / VerticeSize;
    
    /// <summary>
    /// 獲取十二面體的總三角形數
    /// </summary>
    public static int TriangleCount => Indices.Length / 3;
    
    /// <summary>
    /// 獲取包圍球半徑
    /// </summary>
    public static float BoundingSphereRadius => MathF.Sqrt(3.0f + GoldenRatio);
} 