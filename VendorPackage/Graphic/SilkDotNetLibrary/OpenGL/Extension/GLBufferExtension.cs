using Silk.NET.OpenGL;
using System;

namespace SilkDotNetLibrary.OpenGL.Extension
{
    public static class GLBufferExtension
    {
        public static unsafe uint GenBuffer(this GL gl, Span<float> span, in BufferTargetARB bufferTargetARB)
        {
            //Setting the gl instance and storing our buffer type.
            uint bufferHandle = gl.GenBuffer();
            gl.BindBuffer(bufferTargetARB, bufferHandle);
            fixed (void* data = span)
            {
                gl.BufferData(bufferTargetARB,
                               (nuint)(span.Length * sizeof(float)),
                               data,
                               BufferUsageARB.StaticDraw);
            }
            return bufferHandle;
        }
    }
}
