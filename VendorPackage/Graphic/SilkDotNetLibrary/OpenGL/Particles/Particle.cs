using System.Numerics;
using System.Runtime.InteropServices;

namespace SilkDotNetLibrary.OpenGL.Particles;

/// <summary>
/// 粒子結構，包含所有必要的屬性用於粒子模擬和渲染
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Particle
{
    /// <summary>
    /// 粒子在世界空間中的位置
    /// </summary>
    public Vector3 Position;
    
    /// <summary>
    /// 粒子的速度向量
    /// </summary>
    public Vector3 Velocity;
    
    /// <summary>
    /// 粒子的顏色（RGBA）
    /// </summary>
    public Vector4 Color;
    
    /// <summary>
    /// 粒子的大小
    /// </summary>
    public float Size;
    
    /// <summary>
    /// 粒子的當前生命值（0表示死亡）
    /// </summary>
    public float Life;
    
    /// <summary>
    /// 粒子的最大生命值
    /// </summary>
    public float MaxLife;
    
    /// <summary>
    /// 粒子的質量（影響重力和其他力的作用）
    /// </summary>
    public float Mass;
    
    /// <summary>
    /// 粒子的旋轉角度（弧度）
    /// </summary>
    public float Rotation;
    
    /// <summary>
    /// 粒子的旋轉速度（弧度/秒）
    /// </summary>
    public float RotationSpeed;
    
    /// <summary>
    /// 檢查粒子是否存活
    /// </summary>
    public readonly bool IsAlive => Life > 0.0f;
    
    /// <summary>
    /// 獲取粒子的生命比例（0到1之間）
    /// </summary>
    public readonly float LifeRatio => MaxLife > 0 ? Life / MaxLife : 0.0f;
    
    /// <summary>
    /// 創建一個新的粒子
    /// </summary>
    public static Particle Create(Vector3 position, Vector3 velocity, Vector4 color, float size, float life, float mass = 1.0f)
    {
        return new Particle
        {
            Position = position,
            Velocity = velocity,
            Color = color,
            Size = size,
            Life = life,
            MaxLife = life,
            Mass = mass,
            Rotation = 0.0f,
            RotationSpeed = 0.0f
        };
    }
    
    /// <summary>
    /// 重置粒子為死亡狀態
    /// </summary>
    public void Kill()
    {
        Life = 0.0f;
    }
    
    /// <summary>
    /// 復活粒子（設置新的生命值）
    /// </summary>
    public void Respawn(float newLife)
    {
        Life = newLife;
        MaxLife = newLife;
    }
} 