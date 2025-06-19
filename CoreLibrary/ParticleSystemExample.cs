using System.Numerics;
using Microsoft.Extensions.Logging;
using SilkDotNetLibrary.OpenGL.Particles;
using Silk.NET.OpenGL;
using SharedLibrary.Cameras;
using System;

namespace CoreLibrary;

/// <summary>
/// 粒子系統使用範例
/// 展示如何創建和配置不同類型的粒子效果
/// </summary>
public class ParticleSystemExample
{
    private readonly GL _gl;
    private readonly ICamera _camera;
    private readonly ILogger<ParticleSystemExample> _logger;
    
    private ParticleSystem _particleSystem;
    private ParticleEmitter _fireEmitter;
    private ParticleEmitter _smokeEmitter;
    private ParticleEmitter _sparkEmitter;
    private ParticleEmitter _rainEmitter;
    
    public ParticleSystemExample(GL gl, ICamera camera, ILogger<ParticleSystemExample> logger)
    {
        _gl = gl;
        _camera = camera;
        _logger = logger;
        
        _logger.LogInformation("正在初始化粒子系統範例...");
        
        InitializeParticleSystem();
        CreateParticleEmitters();
    }
    
    /// <summary>
    /// 初始化粒子系統
    /// </summary>
    private void InitializeParticleSystem()
    {
        // 創建可容納 2000 個粒子的系統
        _particleSystem = new ParticleSystem(_gl, maxParticles: 2000);
        
        // 設置全局物理參數
        _particleSystem.Gravity = new Vector3(0, -5.0f, 0); // 較輕的重力
        _particleSystem.Wind = new Vector3(0.5f, 0, 0); // 輕微的風力
        _particleSystem.Damping = 0.99f; // 很少的阻尼
        
        _logger.LogInformation("粒子系統初始化完成，最大粒子數：2000");
    }
    
    /// <summary>
    /// 創建不同類型的粒子發射器
    /// </summary>
    private void CreateParticleEmitters()
    {
        CreateFireEmitter();
        CreateSmokeEmitter();
        CreateSparkEmitter();
        CreateRainEmitter();
        
        // 將發射器添加到粒子系統
        _particleSystem.AddEmitter(_fireEmitter);
        _particleSystem.AddEmitter(_smokeEmitter);
        _particleSystem.AddEmitter(_sparkEmitter);
        _particleSystem.AddEmitter(_rainEmitter);
        
        _logger.LogInformation("已創建 4 個粒子發射器：火焰、煙霧、火花、雨滴");
    }
    
    /// <summary>
    /// 創建火焰效果發射器
    /// </summary>
    private void CreateFireEmitter()
    {
        _fireEmitter = new ParticleEmitter
        {
            Position = new Vector3(0, 0, 0),
            Direction = Vector3.UnitY,
            SpreadAngle = MathF.PI / 4, // 45度擴散
            EmissionRate = 80.0f,
            MinSpeed = 1.0f,
            MaxSpeed = 3.0f,
            MinLifetime = 0.8f,
            MaxLifetime = 2.0f,
            MinSize = 0.1f,
            MaxSize = 0.4f,
            StartColor = new Vector4(1.0f, 0.8f, 0.2f, 1.0f), // 黃橙色
            EndColor = new Vector4(1.0f, 0.2f, 0.0f, 0.0f),   // 紅色，透明結束
            IsEnabled = true,
            Loop = true
        };
    }
    
    /// <summary>
    /// 創建煙霧效果發射器
    /// </summary>
    private void CreateSmokeEmitter()
    {
        _smokeEmitter = new ParticleEmitter
        {
            Position = new Vector3(0, 1.0f, 0),
            Direction = Vector3.UnitY,
            SpreadAngle = MathF.PI / 3, // 60度擴散
            EmissionRate = 30.0f,
            MinSpeed = 0.5f,
            MaxSpeed = 1.5f,
            MinLifetime = 3.0f,
            MaxLifetime = 6.0f,
            MinSize = 0.2f,
            MaxSize = 0.8f,
            StartColor = new Vector4(0.5f, 0.5f, 0.5f, 0.7f), // 灰色
            EndColor = new Vector4(0.3f, 0.3f, 0.3f, 0.0f),   // 深灰色，透明結束
            IsEnabled = true,
            Loop = true
        };
    }
    
    /// <summary>
    /// 創建火花效果發射器
    /// </summary>
    private void CreateSparkEmitter()
    {
        _sparkEmitter = new ParticleEmitter
        {
            Position = new Vector3(2.0f, 0, 0),
            Direction = new Vector3(0, 1, 0.3f),
            SpreadAngle = MathF.PI / 2, // 90度擴散
            EmissionRate = 120.0f,
            MinSpeed = 3.0f,
            MaxSpeed = 6.0f,
            MinLifetime = 0.3f,
            MaxLifetime = 1.0f,
            MinSize = 0.05f,
            MaxSize = 0.15f,
            StartColor = new Vector4(1.0f, 1.0f, 0.8f, 1.0f), // 亮黃色
            EndColor = new Vector4(1.0f, 0.5f, 0.0f, 0.0f),   // 橙色，透明結束
            IsEnabled = true,
            Loop = true
        };
    }
    
    /// <summary>
    /// 創建雨滴效果發射器
    /// </summary>
    private void CreateRainEmitter()
    {
        _rainEmitter = new ParticleEmitter
        {
            Position = new Vector3(-2.0f, 5.0f, 0),
            Direction = new Vector3(0, -1, 0.1f),
            SpreadAngle = MathF.PI / 12, // 15度擴散
            EmissionRate = 200.0f,
            MinSpeed = 8.0f,
            MaxSpeed = 12.0f,
            MinLifetime = 2.0f,
            MaxLifetime = 4.0f,
            MinSize = 0.05f,
            MaxSize = 0.1f,
            StartColor = new Vector4(0.7f, 0.8f, 1.0f, 0.8f), // 淡藍色
            EndColor = new Vector4(0.5f, 0.6f, 0.9f, 0.3f),   // 深藍色，半透明結束
            IsEnabled = false, // 預設關閉雨滴效果
            Loop = true
        };
    }
    
    /// <summary>
    /// 更新粒子系統
    /// </summary>
    public void Update(float deltaTime)
    {
        _particleSystem.Update(deltaTime);
        
        // 可以根據需要動態調整發射器位置
        UpdateEmitterPositions(deltaTime);
    }
    
    /// <summary>
    /// 動態更新發射器位置（創建動畫效果）
    /// </summary>
    private void UpdateEmitterPositions(float deltaTime)
    {
        float time = (float)DateTime.Now.TimeOfDay.TotalSeconds;
        
        // 讓火焰發射器左右搖擺
        _fireEmitter.Position = new Vector3(
            MathF.Sin(time * 0.5f) * 0.5f,
            0,
            MathF.Cos(time * 0.3f) * 0.3f
        );
        
        // 讓火花發射器繞圈移動
        _sparkEmitter.Position = new Vector3(
            2.0f + MathF.Cos(time) * 1.0f,
            MathF.Sin(time * 0.5f) * 0.5f,
            MathF.Sin(time) * 1.0f
        );
    }
    
    /// <summary>
    /// 渲染粒子系統
    /// </summary>
    public void Render()
    {
        var viewMatrix = _camera.GetViewMatrix();
        var projectionMatrix = _camera.GetProjectionMatrix();
        
        _particleSystem.Render(viewMatrix, projectionMatrix);
    }
    
    /// <summary>
    /// 切換雨滴效果
    /// </summary>
    public void ToggleRain()
    {
        _rainEmitter.IsEnabled = !_rainEmitter.IsEnabled;
        _logger.LogInformation($"雨滴效果 {(_rainEmitter.IsEnabled ? "開啟" : "關閉")}");
    }
    
    /// <summary>
    /// 切換火焰效果
    /// </summary>
    public void ToggleFire()
    {
        _fireEmitter.IsEnabled = !_fireEmitter.IsEnabled;
        _logger.LogInformation($"火焰效果 {(_fireEmitter.IsEnabled ? "開啟" : "關閉")}");
    }
    
    /// <summary>
    /// 切換煙霧效果
    /// </summary>
    public void ToggleSmoke()
    {
        _smokeEmitter.IsEnabled = !_smokeEmitter.IsEnabled;
        _logger.LogInformation($"煙霧效果 {(_smokeEmitter.IsEnabled ? "開啟" : "關閉")}");
    }
    
    /// <summary>
    /// 切換火花效果
    /// </summary>
    public void ToggleSparks()
    {
        _sparkEmitter.IsEnabled = !_sparkEmitter.IsEnabled;
        _logger.LogInformation($"火花效果 {(_sparkEmitter.IsEnabled ? "開啟" : "關閉")}");
    }
    
    /// <summary>
    /// 獲取粒子系統統計信息
    /// </summary>
    public void LogParticleStats()
    {
        int activeParticles = _particleSystem.GetActiveParticleCount();
        _logger.LogInformation($"目前活躍粒子數：{activeParticles}");
        _logger.LogInformation($"火焰發射器：{(_fireEmitter.IsEnabled ? "啟用" : "停用")}");
        _logger.LogInformation($"煙霧發射器：{(_smokeEmitter.IsEnabled ? "啟用" : "停用")}");
        _logger.LogInformation($"火花發射器：{(_sparkEmitter.IsEnabled ? "啟用" : "停用")}");
        _logger.LogInformation($"雨滴發射器：{(_rainEmitter.IsEnabled ? "啟用" : "停用")}");
    }
    
    /// <summary>
    /// 清除所有粒子
    /// </summary>
    public void ClearAllParticles()
    {
        _particleSystem.Clear();
        _logger.LogInformation("已清除所有粒子");
    }
    
    /// <summary>
    /// 創建爆炸效果
    /// </summary>
    public void CreateExplosion(Vector3 position)
    {
        var explosionEmitter = new ParticleEmitter
        {
            Position = position,
            Direction = Vector3.UnitY,
            SpreadAngle = MathF.PI, // 180度擴散（半球）
            EmissionRate = 1000.0f, // 高發射率
            MinSpeed = 5.0f,
            MaxSpeed = 15.0f,
            MinLifetime = 0.5f,
            MaxLifetime = 2.0f,
            MinSize = 0.1f,
            MaxSize = 0.5f,
            StartColor = new Vector4(1.0f, 1.0f, 0.8f, 1.0f), // 亮白色
            EndColor = new Vector4(1.0f, 0.3f, 0.0f, 0.0f),   // 紅色，透明結束
            IsEnabled = true,
            Loop = false, // 不循環
            Duration = 0.2f // 只持續 0.2 秒
        };
        
        _particleSystem.AddEmitter(explosionEmitter);
        _logger.LogInformation($"在位置 {position} 創建爆炸效果");
    }
    
    public void Dispose()
    {
        _particleSystem.Dispose();
        _logger.LogInformation("粒子系統範例已釋放資源");
    }
} 