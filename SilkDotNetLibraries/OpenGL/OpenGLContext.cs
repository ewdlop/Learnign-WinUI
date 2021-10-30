using SilkDotNetLibraries.OpenGL.Shader;
using Serilog;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace SilkDotNetWrapper.OpenGL
{

    public interface IReadOnlyOpenGLContext
    {
        bool IsClosed { get;}
    }
    public interface IOpenGLContext : IReadOnlyOpenGLContext
    {
        unsafe void OnLoad();
        unsafe void OnRender(double dt);
        void OnUpdate(double dt);
        void OnClose();
    }
    public class OpenGLContext : IOpenGLContext
    {
        private GL GL { get; set; }
        private readonly IWindow _window;
        public uint Vao { get; private set; }
        public uint Vbo { get; private set; }
        public uint Ebo { get; private set; }
        public uint ShaderProgram { get; private set; }
        public bool IsClosed { get; private set; }

        public OpenGLContext(IWindow Window)
        {
            Log.Information("Creating OpenGLContext...");
            _window = Window;
        }
        public unsafe void OnLoad()
        {
            GL = GL.GetApi(_window);
            Vao = GL.GenVertexArray();
            GL.BindVertexArray(Vao);

            Vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTargetARB.ArrayBuffer, Vbo);
            fixed (void* vertice =&Quad.Vertices[0])
            {
                GL.BufferData(BufferTargetARB.ArrayBuffer,
                              (nuint)(Quad.Vertices.Length * sizeof(uint)),
                              vertice,
                              BufferUsageARB.StaticDraw);
            }

            Ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTargetARB.ElementArrayBuffer, Ebo);
            fixed(void* indices =&Quad.Indices[0])
            {
                GL.BufferData(BufferTargetARB.ElementArrayBuffer,
                              (nuint)(Quad.Indices.Length * sizeof(uint)),
                              indices,
                              BufferUsageARB.StaticDraw);
            }

            uint vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, Quad.VertexShader);
            GL.CompileShader(vertexShader);

            string vertexShaderInfoLog = GL.GetShaderInfoLog(vertexShader);
            if (!string.IsNullOrWhiteSpace(vertexShaderInfoLog))
            {
                Log.Error($"Error compling fragment shader {vertexShaderInfoLog}");
            }

            uint fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, Quad.FragmentShader);
            GL.CompileShader(fragmentShader);

            string fragmentShaderInfoLog = GL.GetShaderInfoLog(vertexShader);
            if (!string.IsNullOrWhiteSpace(fragmentShaderInfoLog))
            {
                Log.Error($"Error compling fragment shader {fragmentShaderInfoLog}");
            }

            ShaderProgram = GL.CreateProgram();
            GL.AttachShader(ShaderProgram, vertexShader);
            GL.AttachShader(ShaderProgram, fragmentShader);
            GL.LinkProgram(ShaderProgram);

            GL.GetProgram(ShaderProgram, GLEnum.LinkStatus, out var linkStatus);
            if(linkStatus == 0)
            {
                Log.Error($"Error linking shader {GL.GetProgramInfoLog(ShaderProgram)}");
            }

            GL.DetachShader(ShaderProgram, vertexShader);
            GL.DetachShader(ShaderProgram, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), null);
            GL.EnableVertexAttribArray(0);
        }

        public unsafe void OnRender(double dt)
        {
            GL.Clear((uint)ClearBufferMask.ColorBufferBit);

            GL.BindVertexArray(Vao);
            GL.UseProgram(ShaderProgram);

            GL.DrawElements(PrimitiveType.Triangles, (uint)Quad.Indices.Length, DrawElementsType.UnsignedInt, null);
        }

        public void OnUpdate(double dt)
        {

        }

        public void OnClose()
        {
            if(!IsClosed)
            {
                GL.DeleteBuffer(Vbo);
                GL.DeleteBuffer(Ebo);
                GL.DeleteVertexArray(Vao);
                GL.DeleteProgram(ShaderProgram);
            }
            IsClosed = true;
        }
    }
}
