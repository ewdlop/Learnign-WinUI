# 粒子系統使用指南

## 概述

這個粒子系統專為基於 Silk.NET 的 OpenGL 專案設計，提供高效能的粒子模擬和渲染功能。系統使用實例化渲染（Instanced Rendering）技術，能夠同時處理數千個粒子而保持良好的性能。

## 主要特色

- **高效能渲染**：使用 OpenGL 實例化渲染技術
- **物理模擬**：支援重力、風力、阻尼等物理效果
- **靈活的發射器系統**：可配置的粒子發射器
- **多種粒子效果**：火焰、煙霧、火花、雨滴等
- **即時控制**：動態調整粒子參數和發射器狀態
- **記憶體友好**：固定記憶體分配，避免垃圾回收

## 核心組件

### 1. Particle 結構

```csharp
public struct Particle
{
    public Vector3 Position;      // 位置
    public Vector3 Velocity;      // 速度
    public Vector4 Color;         // 顏色 (RGBA)
    public float Size;            // 大小
    public float Life;            // 當前生命值
    public float MaxLife;         // 最大生命值
    public float Mass;            // 質量
    public float Rotation;        // 旋轉角度
    public float RotationSpeed;   // 旋轉速度
}
```

### 2. ParticleEmitter 發射器

發射器負責定義粒子的生成規則：

```csharp
var emitter = new ParticleEmitter
{
    Position = new Vector3(0, 0, 0),           // 發射位置
    Direction = Vector3.UnitY,                 // 發射方向
    SpreadAngle = MathF.PI / 4,               // 擴散角度（45度）
    EmissionRate = 100.0f,                    // 每秒發射粒子數
    MinSpeed = 1.0f,                          // 最小速度
    MaxSpeed = 3.0f,                          // 最大速度
    MinLifetime = 1.0f,                       // 最小生命週期
    MaxLifetime = 3.0f,                       // 最大生命週期
    MinSize = 0.1f,                           // 最小大小
    MaxSize = 0.3f,                           // 最大大小
    StartColor = new Vector4(1, 0.8f, 0.2f, 1), // 起始顏色
    EndColor = new Vector4(1, 0.2f, 0, 0),      // 結束顏色
    IsEnabled = true,                         // 是否啟用
    Loop = true                               // 是否循環
};
```

### 3. ParticleSystem 管理器

負責管理所有粒子和發射器：

```csharp
// 創建粒子系統
var particleSystem = new ParticleSystem(gl, maxParticles: 2000);

// 設置全局物理參數
particleSystem.Gravity = new Vector3(0, -9.81f, 0);  // 重力
particleSystem.Wind = new Vector3(1.0f, 0, 0);       // 風力
particleSystem.Damping = 0.98f;                      // 阻尼係數

// 添加發射器
particleSystem.AddEmitter(emitter);

// 更新和渲染
particleSystem.Update(deltaTime);
particleSystem.Render(viewMatrix, projectionMatrix);
```

## 預設粒子效果

### 火焰效果

```csharp
var fireEmitter = new ParticleEmitter
{
    Position = new Vector3(0, 0, 0),
    Direction = Vector3.UnitY,
    SpreadAngle = MathF.PI / 4,
    EmissionRate = 80.0f,
    MinSpeed = 1.0f,
    MaxSpeed = 3.0f,
    MinLifetime = 0.8f,
    MaxLifetime = 2.0f,
    MinSize = 0.1f,
    MaxSize = 0.4f,
    StartColor = new Vector4(1.0f, 0.8f, 0.2f, 1.0f), // 黃橙色
    EndColor = new Vector4(1.0f, 0.2f, 0.0f, 0.0f)    // 紅色透明
};
```

### 煙霧效果

```csharp
var smokeEmitter = new ParticleEmitter
{
    Position = new Vector3(0, 1.0f, 0),
    Direction = Vector3.UnitY,
    SpreadAngle = MathF.PI / 3,
    EmissionRate = 30.0f,
    MinSpeed = 0.5f,
    MaxSpeed = 1.5f,
    MinLifetime = 3.0f,
    MaxLifetime = 6.0f,
    MinSize = 0.2f,
    MaxSize = 0.8f,
    StartColor = new Vector4(0.5f, 0.5f, 0.5f, 0.7f), // 灰色
    EndColor = new Vector4(0.3f, 0.3f, 0.3f, 0.0f)    // 深灰透明
};
```

### 火花效果

```csharp
var sparkEmitter = new ParticleEmitter
{
    Position = new Vector3(2.0f, 0, 0),
    Direction = new Vector3(0, 1, 0.3f),
    SpreadAngle = MathF.PI / 2,
    EmissionRate = 120.0f,
    MinSpeed = 3.0f,
    MaxSpeed = 6.0f,
    MinLifetime = 0.3f,
    MaxLifetime = 1.0f,
    MinSize = 0.05f,
    MaxSize = 0.15f,
    StartColor = new Vector4(1.0f, 1.0f, 0.8f, 1.0f), // 亮黃色
    EndColor = new Vector4(1.0f, 0.5f, 0.0f, 0.0f)    // 橙色透明
};
```

### 雨滴效果

```csharp
var rainEmitter = new ParticleEmitter
{
    Position = new Vector3(-2.0f, 5.0f, 0),
    Direction = new Vector3(0, -1, 0.1f),
    SpreadAngle = MathF.PI / 12,
    EmissionRate = 200.0f,
    MinSpeed = 8.0f,
    MaxSpeed = 12.0f,
    MinLifetime = 2.0f,
    MaxLifetime = 4.0f,
    MinSize = 0.05f,
    MaxSize = 0.1f,
    StartColor = new Vector4(0.7f, 0.8f, 1.0f, 0.8f), // 淡藍色
    EndColor = new Vector4(0.5f, 0.6f, 0.9f, 0.3f)    // 深藍半透明
};
```

## 使用範例

### 基本使用

```csharp
// 創建和初始化
var particleExample = new ParticleSystemExample(gl, camera, logger);

// 在遊戲循環中更新
particleExample.Update(deltaTime);

// 在渲染循環中繪製
particleExample.Render();
```

### 動態控制

```csharp
// 切換效果
particleExample.ToggleFire();    // 切換火焰
particleExample.ToggleSmoke();   // 切換煙霧
particleExample.ToggleSparks();  // 切換火花
particleExample.ToggleRain();    // 切換雨滴

// 創建爆炸效果
particleExample.CreateExplosion(new Vector3(0, 0, 0));

// 清除所有粒子
particleExample.ClearAllParticles();

// 查看統計信息
particleExample.LogParticleStats();
```

## 自定義粒子效果

### 創建自定義發射器

```csharp
var customEmitter = new ParticleEmitter
{
    Position = new Vector3(0, 0, 0),
    Direction = new Vector3(0, 1, 0),
    SpreadAngle = MathF.PI / 8,              // 22.5度擴散
    EmissionRate = 50.0f,                    // 每秒50個粒子
    MinSpeed = 2.0f,
    MaxSpeed = 5.0f,
    MinLifetime = 1.5f,
    MaxLifetime = 3.0f,
    MinSize = 0.08f,
    MaxSize = 0.25f,
    StartColor = new Vector4(0.8f, 0.2f, 1.0f, 1.0f), // 紫色
    EndColor = new Vector4(0.2f, 0.8f, 1.0f, 0.0f),   // 青色透明
    IsEnabled = true,
    Loop = true
};

particleSystem.AddEmitter(customEmitter);
```

### 動態調整發射器

```csharp
// 在更新循環中動態調整
float time = (float)DateTime.Now.TimeOfDay.TotalSeconds;

// 讓發射器位置動畫
emitter.Position = new Vector3(
    MathF.Sin(time) * 2.0f,
    MathF.Cos(time * 0.5f) * 1.0f,
    0
);

// 調整發射率
emitter.EmissionRate = 50.0f + MathF.Sin(time * 2.0f) * 25.0f;

// 調整顏色
float colorIntensity = (MathF.Sin(time * 3.0f) + 1.0f) * 0.5f;
emitter.StartColor = new Vector4(colorIntensity, 0.8f, 0.2f, 1.0f);
```

## 性能優化建議

### 1. 粒子數量管理
- 根據目標平台調整最大粒子數量
- 行動裝置：500-1000 個粒子
- 桌面電腦：1000-5000 個粒子
- 高端系統：5000+ 個粒子

### 2. 發射率控制
- 避免同時使用過多高發射率的發射器
- 根據距離調整發射率（遠距離減少粒子）
- 在不可見時停用發射器

### 3. 生命週期優化
- 使用適當的生命週期（不要太長）
- 及時清理不需要的粒子
- 避免創建太多短壽命粒子

### 4. 渲染優化
- 粒子系統使用 Alpha Blending，可能影響性能
- 考慮使用 Depth Sorting 以獲得更好的視覺效果
- 在合適的時機渲染粒子（通常在透明物體之前）

## 故障排除

### 常見問題

1. **粒子不顯示**
   - 檢查發射器是否啟用（`IsEnabled = true`）
   - 確認粒子生命週期設置合理
   - 檢查相機位置和粒子位置
   - 確認 Alpha Blending 已啟用

2. **性能問題**
   - 減少最大粒子數量
   - 降低發射率
   - 縮短粒子生命週期
   - 檢查是否有過多的發射器同時運行

3. **渲染順序問題**
   - 確保粒子在透明物體渲染階段繪製
   - 檢查深度測試設置
   - 考慮禁用深度寫入

### 除錯技巧

```csharp
// 獲取粒子系統狀態
int activeParticles = particleSystem.GetActiveParticleCount();
Console.WriteLine($"活躍粒子數: {activeParticles}");

// 檢查發射器狀態
Console.WriteLine($"火焰發射器: {fireEmitter.IsEnabled ? "啟用" : "停用"}");
Console.WriteLine($"發射率: {fireEmitter.EmissionRate}");
```

## 進階功能

### 1. 粒子碰撞
可以擴展系統以支援粒子與場景的碰撞檢測。

### 2. 紋理支援
可以為粒子添加紋理支援，創建更逼真的效果。

### 3. GPU 粒子
對於需要大量粒子的場景，可以考慮將計算移至 GPU。

### 4. 物理整合
可以與物理引擎整合，實現更複雜的粒子行為。

---

這個粒子系統提供了創建各種視覺效果的強大基礎。通過調整不同的參數，您可以創建從簡單的火花到複雜的天氣效果等各種粒子系統。 