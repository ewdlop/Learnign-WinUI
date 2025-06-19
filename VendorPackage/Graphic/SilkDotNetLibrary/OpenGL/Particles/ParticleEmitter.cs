using System;
using System.Numerics;

namespace SilkDotNetLibrary.OpenGL.Particles;

/// <summary>
/// 粒子發射器類，負責定義粒子的生成規則和屬性
/// </summary>
public class ParticleEmitter
{
    /// <summary>
    /// 發射器的位置
    /// </summary>
    public Vector3 Position { get; set; }
    
    /// <summary>
    /// 發射器的方向
    /// </summary>
    public Vector3 Direction { get; set; } = Vector3.UnitY;
    
    /// <summary>
    /// 發射錐角度（弧度）
    /// </summary>
    public float SpreadAngle { get; set; } = MathF.PI / 6; // 30度
    
    /// <summary>
    /// 每秒發射的粒子數量
    /// </summary>
    public float EmissionRate { get; set; } = 50.0f;
    
    /// <summary>
    /// 粒子初始速度的最小值
    /// </summary>
    public float MinSpeed { get; set; } = 1.0f;
    
    /// <summary>
    /// 粒子初始速度的最大值
    /// </summary>
    public float MaxSpeed { get; set; } = 3.0f;
    
    /// <summary>
    /// 粒子生命週期的最小值
    /// </summary>
    public float MinLifetime { get; set; } = 1.0f;
    
    /// <summary>
    /// 粒子生命週期的最大值
    /// </summary>
    public float MaxLifetime { get; set; } = 3.0f;
    
    /// <summary>
    /// 粒子大小的最小值
    /// </summary>
    public float MinSize { get; set; } = 0.1f;
    
    /// <summary>
    /// 粒子大小的最大值
    /// </summary>
    public float MaxSize { get; set; } = 0.3f;
    
    /// <summary>
    /// 粒子顏色（起始顏色）
    /// </summary>
    public Vector4 StartColor { get; set; } = Vector4.One;
    
    /// <summary>
    /// 粒子顏色（結束顏色）
    /// </summary>
    public Vector4 EndColor { get; set; } = new Vector4(1, 1, 1, 0);
    
    /// <summary>
    /// 是否啟用發射器
    /// </summary>
    public bool IsEnabled { get; set; } = true;
    
    /// <summary>
    /// 發射器是否循環發射
    /// </summary>
    public bool Loop { get; set; } = true;
    
    /// <summary>
    /// 發射持續時間（如果不循環）
    /// </summary>
    public float Duration { get; set; } = 5.0f;
    
    private float _emissionTimer = 0.0f;
    private float _durationTimer = 0.0f;
    private Random _random = new Random();
    
    /// <summary>
    /// 更新發射器
    /// </summary>
    /// <param name="deltaTime">時間增量</param>
    /// <returns>本幀需要發射的粒子數量</returns>
    public int Update(float deltaTime)
    {
        if (!IsEnabled)
            return 0;
            
        if (!Loop)
        {
            _durationTimer += deltaTime;
            if (_durationTimer >= Duration)
                return 0;
        }
        
        _emissionTimer += deltaTime;
        float emissionInterval = 1.0f / EmissionRate;
        
        int particlesToEmit = 0;
        while (_emissionTimer >= emissionInterval)
        {
            _emissionTimer -= emissionInterval;
            particlesToEmit++;
        }
        
        return particlesToEmit;
    }
    
    /// <summary>
    /// 生成一個新粒子
    /// </summary>
    /// <returns>新創建的粒子</returns>
    public Particle EmitParticle()
    {
        // 隨機方向（在錐形範圍內）
        Vector3 direction = GetRandomDirection();
        
        // 隨機速度
        float speed = Lerp(MinSpeed, MaxSpeed, (float)_random.NextDouble());
        Vector3 velocity = direction * speed;
        
        // 隨機生命週期
        float lifetime = Lerp(MinLifetime, MaxLifetime, (float)_random.NextDouble());
        
        // 隨機大小
        float size = Lerp(MinSize, MaxSize, (float)_random.NextDouble());
        
        // 創建粒子
        return Particle.Create(Position, velocity, StartColor, size, lifetime);
    }
    
    /// <summary>
    /// 獲取隨機方向（在發射錐形範圍內）
    /// </summary>
    private Vector3 GetRandomDirection()
    {
        // 在錐形範圍內隨機生成方向
        float theta = (float)_random.NextDouble() * 2.0f * MathF.PI; // 方位角
        float phi = (float)_random.NextDouble() * SpreadAngle; // 極角
        
        // 將球坐標轉換為笛卡爾坐標
        float x = MathF.Sin(phi) * MathF.Cos(theta);
        float y = MathF.Cos(phi);
        float z = MathF.Sin(phi) * MathF.Sin(theta);
        
        Vector3 randomDir = new Vector3(x, y, z);
        
        // 將隨機方向旋轉到發射器方向
        return RotateVectorToDirection(randomDir, Direction);
    }
    
    /// <summary>
    /// 將向量旋轉到指定方向
    /// </summary>
    private Vector3 RotateVectorToDirection(Vector3 vector, Vector3 targetDirection)
    {
        Vector3 up = Vector3.UnitY;
        Vector3 right = Vector3.Cross(targetDirection, up);
        if (right.LengthSquared() < 0.001f)
        {
            right = Vector3.UnitX;
        }
        right = Vector3.Normalize(right);
        up = Vector3.Cross(right, targetDirection);
        
        return vector.X * right + vector.Y * targetDirection + vector.Z * up;
    }
    
    /// <summary>
    /// 線性插值
    /// </summary>
    private float Lerp(float a, float b, float t)
    {
        return a + (b - a) * t;
    }
    
    /// <summary>
    /// 重置發射器
    /// </summary>
    public void Reset()
    {
        _emissionTimer = 0.0f;
        _durationTimer = 0.0f;
    }
} 