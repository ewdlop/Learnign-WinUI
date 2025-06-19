using System;
using System.Collections.Generic;
using System.Numerics;
using Silk.NET.OpenGL;
using SilkDotNetLibrary.OpenGL.Buffers;
using SilkDotNetLibrary.OpenGL.Shaders;

namespace SilkDotNetLibrary.OpenGL.Particles;

/// <summary>
/// 粒子系統管理器，負責更新和渲染粒子
/// </summary>
public class ParticleSystem : IDisposable
{
    private readonly GL _gl;
    private readonly int _maxParticles;
    private readonly Particle[] _particles;
    private readonly ParticleEmitter[] _emitters;
    private readonly List<ParticleEmitter> _emitterList;
    
    // OpenGL 緩衝區
    private VertexArrayBufferObject<float, uint> _vao;
    private BufferObject<float> _vbo;
    private BufferObject<float> _instanceVbo;
    private BufferObject<uint> _ebo;
    
    // 著色器
    private Shaders.Shader _particleShader;
    
    // 重力和其他全局力
    public Vector3 Gravity { get; set; } = new Vector3(0, -9.81f, 0);
    public Vector3 Wind { get; set; } = Vector3.Zero;
    public float Damping { get; set; } = 0.98f; // 阻尼係數
    
    // 實例數據（用於實例化渲染）
    private float[] _instanceData;
    
    // 簡單的四邊形頂點（粒子的基本形狀）
    private static readonly float[] QuadVertices = {
        // 位置      // 紋理座標
        -0.5f, -0.5f, 0.0f,  0.0f, 0.0f,
         0.5f, -0.5f, 0.0f,  1.0f, 0.0f,
         0.5f,  0.5f, 0.0f,  1.0f, 1.0f,
        -0.5f,  0.5f, 0.0f,  0.0f, 1.0f
    };
    
    private static readonly uint[] QuadIndices = {
        0, 1, 2,
        2, 3, 0
    };
    
    public ParticleSystem(GL gl, int maxParticles = 1000)
    {
        _gl = gl;
        _maxParticles = maxParticles;
        _particles = new Particle[maxParticles];
        _emitterList = new List<ParticleEmitter>();
        
        // 創建實例數據數組（每個粒子需要：位置(3) + 顏色(4) + 大小(1) + 旋轉(1) = 9個浮點數）
        _instanceData = new float[maxParticles * 9];
        
        InitializeOpenGLResources();
        InitializeShaders();
    }
    
    /// <summary>
    /// 初始化 OpenGL 資源
    /// </summary>
    private unsafe void InitializeOpenGLResources()
    {
        // 創建 VAO 和 VBO
        _vbo = new BufferObject<float>(_gl, QuadVertices, BufferTargetARB.ArrayBuffer);
        _ebo = new BufferObject<uint>(_gl, QuadIndices, BufferTargetARB.ElementArrayBuffer);
        _vao = new VertexArrayBufferObject<float, uint>(_gl, _vbo, _ebo);
        
        // 設置頂點屬性（位置和紋理座標）
        _vao.VertexAttributePointer(_gl, 0, 3, VertexAttribPointerType.Float, 5, 0); // 位置
        _vao.VertexAttributePointer(_gl, 1, 2, VertexAttribPointerType.Float, 5, 3); // 紋理座標
        
        // 創建實例 VBO
        _instanceVbo = new BufferObject<float>(_gl, _instanceData, BufferTargetARB.ArrayBuffer, BufferUsageARB.DynamicDraw);

        // 設置實例屬性
        _vao.BindBy(_gl);
        _instanceVbo.BindBy(_gl);
        
        // 實例位置（屬性 2）
        _gl.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 9 * sizeof(float), (void*)0);
        _gl.EnableVertexAttribArray(2);
        _gl.VertexAttribDivisor(2, 1);
        
        // 實例顏色（屬性 3）
        _gl.VertexAttribPointer(3, 4, VertexAttribPointerType.Float, false, 9 * sizeof(float), (void*)(3 * sizeof(float)));
        _gl.EnableVertexAttribArray(3);
        _gl.VertexAttribDivisor(3, 1);
        
        // 實例大小（屬性 4）
        _gl.VertexAttribPointer(4, 1, VertexAttribPointerType.Float, false, 9 * sizeof(float), (void*)(7 * sizeof(float)));
        _gl.EnableVertexAttribArray(4);
        _gl.VertexAttribDivisor(4, 1);
        
        // 實例旋轉（屬性 5）
        _gl.VertexAttribPointer(5, 1, VertexAttribPointerType.Float, false, 9 * sizeof(float), (void*)(8 * sizeof(float)));
        _gl.EnableVertexAttribArray(5);
        _gl.VertexAttribDivisor(5, 1);
        
        _gl.BindVertexArray(0);
    }
    
    /// <summary>
    /// 初始化著色器
    /// </summary>
    private void InitializeShaders()
    {
        _particleShader = new Shaders.Shader(_gl);
        
        string vertexShaderSource = @"
            #version 330 core
            
            layout (location = 0) in vec3 aPosition;
            layout (location = 1) in vec2 aTexCoord;
            layout (location = 2) in vec3 aInstancePos;
            layout (location = 3) in vec4 aInstanceColor;
            layout (location = 4) in float aInstanceSize;
            layout (location = 5) in float aInstanceRotation;
            
            uniform mat4 uView;
            uniform mat4 uProjection;
            
            out vec2 vTexCoord;
            out vec4 vColor;
            
            void main()
            {
                vTexCoord = aTexCoord;
                vColor = aInstanceColor;
                
                float cos_rot = cos(aInstanceRotation);
                float sin_rot = sin(aInstanceRotation);
                
                vec3 rotatedPos = vec3(
                    aPosition.x * cos_rot - aPosition.y * sin_rot,
                    aPosition.x * sin_rot + aPosition.y * cos_rot,
                    aPosition.z
                ) * aInstanceSize;
                
                vec3 worldPos = aInstancePos + rotatedPos;
                
                gl_Position = uProjection * uView * vec4(worldPos, 1.0);
            }
        ";
        
        string fragmentShaderSource = @"
            #version 330 core
            
            in vec2 vTexCoord;
            in vec4 vColor;
            
            out vec4 FragColor;
            
            void main()
            {
                vec2 circleCoord = vTexCoord * 2.0 - 1.0;
                float distance = length(circleCoord);
                
                if (distance > 1.0)
                    discard;
                
                float alpha = 1.0 - smoothstep(0.7, 1.0, distance);
                
                FragColor = vec4(vColor.rgb, vColor.a * alpha);
            }
        ";
        
        _particleShader.LoadBy(_gl, vertexShaderSource, fragmentShaderSource);
    }
    
    /// <summary>
    /// 添加粒子發射器
    /// </summary>
    public void AddEmitter(ParticleEmitter emitter)
    {
        _emitterList.Add(emitter);
    }
    
    /// <summary>
    /// 移除粒子發射器
    /// </summary>
    public void RemoveEmitter(ParticleEmitter emitter)
    {
        _emitterList.Remove(emitter);
    }
    
    /// <summary>
    /// 更新粒子系統
    /// </summary>
    public void Update(float deltaTime)
    {
        // 更新發射器並發射新粒子
        foreach (var emitter in _emitterList)
        {
            int particlesToEmit = emitter.Update(deltaTime);
            for (int i = 0; i < particlesToEmit; i++)
            {
                EmitParticle(emitter);
            }
        }
        
        // 更新所有粒子
        for (int i = 0; i < _maxParticles; i++)
        {
            if (_particles[i].IsAlive)
            {
                UpdateParticle(ref _particles[i], deltaTime);
            }
        }
        
        // 更新實例數據
        UpdateInstanceData();
    }
    
    /// <summary>
    /// 更新單個粒子
    /// </summary>
    private void UpdateParticle(ref Particle particle, float deltaTime)
    {
        // 更新生命值
        particle.Life -= deltaTime;
        
        if (particle.Life <= 0)
        {
            particle.Kill();
            return;
        }
        
        // 應用力（重力、風等）
        Vector3 acceleration = (Gravity + Wind) / particle.Mass;
        
        // 更新速度和位置
        particle.Velocity += acceleration * deltaTime;
        particle.Velocity *= MathF.Pow(Damping, deltaTime); // 應用阻尼
        particle.Position += particle.Velocity * deltaTime;
        
        // 更新旋轉
        particle.Rotation += particle.RotationSpeed * deltaTime;
        
        // 更新顏色（隨著生命值變化）
        float lifeRatio = particle.LifeRatio;
        // 這裡可以添加顏色漸變邏輯
        particle.Color = Vector4.Lerp(new Vector4(1, 1, 1, 0), particle.Color, lifeRatio);
    }
    
    /// <summary>
    /// 發射新粒子
    /// </summary>
    private void EmitParticle(ParticleEmitter emitter)
    {
        // 找到第一個死亡的粒子位置
        for (int i = 0; i < _maxParticles; i++)
        {
            if (!_particles[i].IsAlive)
            {
                _particles[i] = emitter.EmitParticle();
                return;
            }
        }
    }
    
    /// <summary>
    /// 更新實例數據
    /// </summary>
    private void UpdateInstanceData()
    {
        int activeParticles = 0;
        
        for (int i = 0; i < _maxParticles; i++)
        {
            if (_particles[i].IsAlive)
            {
                int index = activeParticles * 9;
                
                // 位置
                _instanceData[index + 0] = _particles[i].Position.X;
                _instanceData[index + 1] = _particles[i].Position.Y;
                _instanceData[index + 2] = _particles[i].Position.Z;
                
                // 顏色
                _instanceData[index + 3] = _particles[i].Color.X;
                _instanceData[index + 4] = _particles[i].Color.Y;
                _instanceData[index + 5] = _particles[i].Color.Z;
                _instanceData[index + 6] = _particles[i].Color.W;
                
                // 大小
                _instanceData[index + 7] = _particles[i].Size;
                
                // 旋轉
                _instanceData[index + 8] = _particles[i].Rotation;
                
                activeParticles++;
            }
        }
        
        // 更新 VBO
        if (activeParticles > 0)
        {
            _instanceVbo.BindBy(_gl);
            unsafe
            {
                fixed (float* data = _instanceData)
                {
                    _gl.BufferSubData(BufferTargetARB.ArrayBuffer, 0, (nuint)(activeParticles * 9 * sizeof(float)), data);
                }
            }
        }
    }
    
    /// <summary>
    /// 渲染粒子系統
    /// </summary>
    public unsafe void Render(Matrix4x4 view, Matrix4x4 projection)
    {
        // 計算存活的粒子數量
        int activeParticles = 0;
        for (int i = 0; i < _maxParticles; i++)
        {
            if (_particles[i].IsAlive)
                activeParticles++;
        }
        
        if (activeParticles == 0)
            return;
        
        // 啟用混合
        _gl.Enable(EnableCap.Blend);
        _gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        
        // 使用粒子著色器
        _particleShader.UseBy(_gl);
        _particleShader.SetUniformBy(_gl, "uView", view);
        _particleShader.SetUniformBy(_gl, "uProjection", projection);

        // 綁定 VAO 並渲染
        _vao.BindBy(_gl);
        _gl.DrawElementsInstanced(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, (void*)0, (uint)activeParticles);
        _gl.BindVertexArray(0);
        
        // 禁用混合
        _gl.Disable(EnableCap.Blend);
    }
    
    /// <summary>
    /// 清除所有粒子
    /// </summary>
    public void Clear()
    {
        for (int i = 0; i < _maxParticles; i++)
        {
            _particles[i].Kill();
        }
    }
    
    /// <summary>
    /// 獲取存活粒子數量
    /// </summary>
    public int GetActiveParticleCount()
    {
        int count = 0;
        for (int i = 0; i < _maxParticles; i++)
        {
            if (_particles[i].IsAlive)
                count++;
        }
        return count;
    }
    
    public void Dispose()
    {
        _vao.DisposeBy(_gl);
        _vbo.DisposeBy(_gl);
        _instanceVbo.DisposeBy(_gl);
        _ebo.DisposeBy(_gl);
        _particleShader.DisposeBy(_gl);
    }
} 