using System;

namespace SilkDotNetLibrary.OpenGL.Primitives;

/// <summary>
/// 四面體 (Tetrahedron) - 包含所有頂點屬性以匹配 Vertex 結構
/// 四面體是最簡單的柏拉圖立體，有4個等邊三角形面和4個頂點
/// Position (3) + Normal (3) + TexCoords (2) + Tangent (3) + BiTangent (3) + Color (3) = 17 floats
/// </summary>
public static class Tetrahedron
{
    public const int VerticeSize = 17; // 與 Vertex 結構完全匹配
    
    /// <summary>
    /// 四面體的完整頂點數據，包含所有屬性
    /// 四面體有4個頂點，每個頂點 17 個浮點數
    /// 基於正四面體的幾何結構，放置在原點周圍
    /// </summary>
    public static readonly float[] Vertices =
    {
        // 正四面體的4個頂點，基於立方體的對角線
        // 使用標準化座標確保等邊三角形面
        
        // 頂點 0: 第一個頂點 (前上右)
        1.0f, 1.0f, 1.0f,                           // Position
        0.577350f, 0.577350f, 0.577350f,            // Normal (標準化)
        0.0f, 0.0f,                                 // TexCoords
        -0.816497f, 0.408248f, 0.408248f,          // Tangent
        0.0f, -0.707107f, 0.707107f,               // BiTangent
        0.0f, 1.0f, 0.5f,                          // Color (青綠色)

        // 頂點 1: 第二個頂點 (後上左)
        -1.0f, 1.0f, -1.0f,                         // Position
        -0.577350f, 0.577350f, -0.577350f,          // Normal
        1.0f, 0.0f,                                 // TexCoords
        0.816497f, 0.408248f, 0.408248f,           // Tangent
        0.0f, -0.707107f, 0.707107f,               // BiTangent
        0.0f, 1.0f, 0.5f,                          // Color

        // 頂點 2: 第三個頂點 (前下左)
        -1.0f, -1.0f, 1.0f,                         // Position
        -0.577350f, -0.577350f, 0.577350f,          // Normal
        0.0f, 1.0f,                                 // TexCoords
        0.816497f, -0.408248f, 0.408248f,          // Tangent
        0.0f, 0.707107f, 0.707107f,                // BiTangent
        0.0f, 1.0f, 0.5f,                          // Color

        // 頂點 3: 第四個頂點 (後下右)  
        1.0f, -1.0f, -1.0f,                         // Position
        0.577350f, -0.577350f, -0.577350f,          // Normal
        1.0f, 1.0f,                                 // TexCoords
        -0.816497f, -0.408248f, 0.408248f,         // Tangent
        0.0f, 0.707107f, 0.707107f,                // BiTangent
        0.0f, 1.0f, 0.5f,                          // Color
    };

    /// <summary>
    /// 四面體的三角形面索引
    /// 4個三角形面，每個面3個頂點 = 12個索引
    /// 所有面使用逆時針頂點順序（從外部看）
    /// </summary>
    public static readonly uint[] Indices =
    {
        // 面 1: 底面（包含頂點 0, 1, 2）
        0, 2, 1,    // 逆時針順序
        
        // 面 2: 右側面（包含頂點 0, 1, 3）
        0, 1, 3,    // 逆時針順序
        
        // 面 3: 左側面（包含頂點 0, 2, 3）
        0, 3, 2,    // 逆時針順序
        
        // 面 4: 後面（包含頂點 1, 2, 3）
        1, 2, 3     // 逆時針順序
    };
    
    /// <summary>
    /// 獲取四面體的總頂點數
    /// </summary>
    public static int VertexCount => Vertices.Length / VerticeSize;
    
    /// <summary>
    /// 獲取四面體的總三角形數
    /// </summary>
    public static int TriangleCount => Indices.Length / 3;
    
    /// <summary>
    /// 獲取四面體的總面數
    /// </summary>
    public static int FaceCount => 4;
    
    /// <summary>
    /// 獲取包圍球半徑
    /// 對於邊長為 2√2 的正四面體
    /// </summary>
    public static float BoundingSphereRadius => MathF.Sqrt(1.5f);
    
    /// <summary>
    /// 獲取四面體的邊長
    /// </summary>
    public static float EdgeLength => 2.0f * MathF.Sqrt(2.0f);
    
    /// <summary>
    /// 獲取四面體的高度
    /// </summary>
    public static float Height => MathF.Sqrt(8.0f / 3.0f);
} 