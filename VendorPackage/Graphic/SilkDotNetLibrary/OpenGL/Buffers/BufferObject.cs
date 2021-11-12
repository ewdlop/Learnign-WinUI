using Silk.NET.OpenGL;
using System;

namespace SilkDotNetLibrary.OpenGL.Buffers
{
    public class BufferObject<TDataType> : IBufferObject<TDataType>, IDisposable
        where TDataType : unmanaged
    {
        public readonly uint _bufferHandle;
        private readonly BufferTargetARB _bufferTargetARB;
        private readonly GL _gl;
        protected bool disposedValue;

        public unsafe BufferObject(GL gl, Span<TDataType> span, BufferTargetARB bufferTargetARB)
        {
            //Setting the gl instance and storing our buffer type.
            _gl = gl;
            _bufferTargetARB = bufferTargetARB;

            //Getting the handle, and then uploading the data to said handle.
            _bufferHandle = _gl.GenBuffer();
            Bind();
            fixed (void* data = span)
            {
                _gl.BufferData(_bufferTargetARB,
                               (nuint)(span.Length * sizeof(TDataType)),
                               data,
                               BufferUsageARB.StaticDraw);
            }
        }

        public void Bind()
        {
            //Binding the buffer object, with the correct buffer type.
            _gl.BindBuffer(_bufferTargetARB, _bufferHandle);
        }

        protected virtual void OnDipose()
        {
            _gl.DeleteBuffer(_bufferHandle);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    OnDipose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~BufferObject()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
