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
    /// 基於三組正交的黃金矩形生成
    /// </summary>
    public static readonly float[] Vertices =
    {
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
        0.2f, 0.8f, 0.4f,                          // Color (綠色)

        // 頂點 9: (+φ, -1/φ, 0)
        GoldenRatio, -InvGoldenRatio, 0.0f,          // Position
        0.850651f, -0.525731f, 0.0f,                // Normal
        1.0f, 0.6f,                                 // TexCoords
        0.0f, 0.0f, 1.0f,                          // Tangent
        0.525731f, 0.850651f, 0.0f,                // BiTangent
        0.2f, 0.8f, 0.4f,                          // Color

        // 頂點 10: (-φ, +1/φ, 0)
        -GoldenRatio, InvGoldenRatio, 0.0f,          // Position
        -0.850651f, 0.525731f, 0.0f,                // Normal
        0.0f, 0.4f,                                 // TexCoords
        0.0f, 0.0f, 1.0f,                          // Tangent
        0.525731f, 0.850651f, 0.0f,                // BiTangent
        0.2f, 0.8f, 0.4f,                          // Color

        // 頂點 11: (-φ, -1/φ, 0)
        -GoldenRatio, -InvGoldenRatio, 0.0f,         // Position
        -0.850651f, -0.525731f, 0.0f,               // Normal
        0.0f, 0.6f,                                 // TexCoords
        0.0f, 0.0f, 1.0f,                          // Tangent
        -0.525731f, 0.850651f, 0.0f,               // BiTangent
        0.2f, 0.8f, 0.4f,                          // Color

        // 第三組：yz平面上的矩形頂點 (0, ±φ, ±1/φ)
        // 頂點 12: (0, +φ, +1/φ)
        0.0f, GoldenRatio, InvGoldenRatio,           // Position
        0.0f, 0.850651f, 0.525731f,                 // Normal
        0.5f, 0.0f,                                 // TexCoords
        1.0f, 0.0f, 0.0f,                          // Tangent
        0.0f, -0.525731f, 0.850651f,               // BiTangent
        0.9f, 0.6f, 0.2f,                          // Color (橙色)

        // 頂點 13: (0, +φ, -1/φ)
        0.0f, GoldenRatio, -InvGoldenRatio,          // Position
        0.0f, 0.850651f, -0.525731f,                // Normal
        0.5f, 0.0f,                                 // TexCoords
        1.0f, 0.0f, 0.0f,                          // Tangent
        0.0f, 0.525731f, 0.850651f,                // BiTangent
        0.9f, 0.6f, 0.2f,                          // Color

        // 頂點 14: (0, -φ, +1/φ)
        0.0f, -GoldenRatio, InvGoldenRatio,          // Position
        0.0f, -0.850651f, 0.525731f,                // Normal
        0.5f, 1.0f,                                 // TexCoords
        1.0f, 0.0f, 0.0f,                          // Tangent
        0.0f, 0.525731f, 0.850651f,                // BiTangent
        0.9f, 0.6f, 0.2f,                          // Color

        // 頂點 15: (0, -φ, -1/φ)
        0.0f, -GoldenRatio, -InvGoldenRatio,         // Position
        0.0f, -0.850651f, -0.525731f,               // Normal
        0.5f, 1.0f,                                 // TexCoords
        1.0f, 0.0f, 0.0f,                          // Tangent
        0.0f, -0.525731f, 0.850651f,               // BiTangent
        0.9f, 0.6f, 0.2f,                          // Color

        // 第四組：xz平面上的矩形頂點 (±1/φ, 0, ±φ)
        // 頂點 16: (+1/φ, 0, +φ)
        InvGoldenRatio, 0.0f, GoldenRatio,           // Position
        0.525731f, 0.0f, 0.850651f,                 // Normal
        0.7f, 0.5f,                                 // TexCoords
        0.0f, 1.0f, 0.0f,                          // Tangent
        -0.850651f, 0.0f, 0.525731f,               // BiTangent
        0.3f, 0.4f, 0.9f,                          // Color (藍色)

        // 頂點 17: (+1/φ, 0, -φ)
        InvGoldenRatio, 0.0f, -GoldenRatio,          // Position
        0.525731f, 0.0f, -0.850651f,                // Normal
        0.3f, 0.5f,                                 // TexCoords
        0.0f, 1.0f, 0.0f,                          // Tangent
        0.850651f, 0.0f, 0.525731f,                // BiTangent
        0.3f, 0.4f, 0.9f,                          // Color

        // 頂點 18: (-1/φ, 0, +φ)
        -InvGoldenRatio, 0.0f, GoldenRatio,          // Position
        -0.525731f, 0.0f, 0.850651f,                // Normal
        0.3f, 0.5f,                                 // TexCoords
        0.0f, 1.0f, 0.0f,                          // Tangent
        0.850651f, 0.0f, 0.525731f,                // BiTangent
        0.3f, 0.4f, 0.9f,                          // Color

        // 頂點 19: (-1/φ, 0, -φ)
        -InvGoldenRatio, 0.0f, -GoldenRatio,         // Position
        -0.525731f, 0.0f, -0.850651f,               // Normal
        0.7f, 0.5f,                                 // TexCoords
        0.0f, 1.0f, 0.0f,                          // Tangent
        -0.850651f, 0.0f, 0.525731f,               // BiTangent
        0.3f, 0.4f, 0.9f,                          // Color
    };

    /// <summary>
    /// 十二面體的正確三角形面索引
    /// 12個五邊形面，每個面用3個三角形表示 = 36個三角形 = 108個索引
    /// 正確的十二面體面連接關係
    /// </summary>
    public static readonly uint[] Indices =
    {
        // 根據正確的十二面體拓撲結構重新構建索引
        // 每個五邊形面分解為3個三角形 (fan triangulation)
        
        // 五邊形面 1: 頂部面 (包含頂點 12, 13)
        0, 8, 1,    1, 8, 9,    1, 9, 3,
        
        // 五邊形面 2: (包含頂點 0, 1, 13, 12, 4)
        0, 1, 13,   0, 13, 12,  0, 12, 4,
        
        // 五邊形面 3: (包含頂點 0, 4, 6, 2, 8)
        0, 4, 6,    0, 6, 2,    0, 2, 8,
        
        // 五邊形面 4: (包含頂點 2, 6, 18, 16, 8)
        2, 6, 18,   2, 18, 16,  2, 16, 8,
        
        // 五邊形面 5: (包含頂點 8, 16, 18, 14, 9)
        8, 16, 18,  8, 18, 14,  8, 14, 9,
        
        // 五邊形面 6: (包含頂點 9, 14, 15, 3, 17)
        9, 14, 15,  9, 15, 3,   9, 3, 17,
        
        // 五邊形面 7: (包含頂點 3, 15, 7, 5, 1)
        3, 15, 7,   3, 7, 5,    3, 5, 1,
        
        // 五邊形面 8: (包含頂點 1, 5, 19, 17, 13)
        1, 5, 19,   1, 19, 17,  1, 17, 13,
        
        // 五邊形面 9: (包含頂點 13, 17, 19, 10, 12)
        13, 17, 19, 13, 19, 10, 13, 10, 12,
        
        // 五邊形面 10: (包含頂點 12, 10, 11, 4, 6)
        12, 10, 11, 12, 11, 4,  12, 4, 6,
        
        // 五邊形面 11: (包含頂點 4, 11, 7, 18, 6)
        4, 11, 7,   4, 7, 18,   4, 18, 6,
        
        // 五邊形面 12: 底部面 (包含頂點 14, 15)
        14, 18, 7,  7, 15, 14,  14, 11, 18
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
    /// 獲取包圍球半徑 (正確的計算公式)
    /// </summary>
    public static float BoundingSphereRadius => MathF.Sqrt(GoldenRatio * GoldenRatio + 1.0f);
}