using Silk.NET.OpenGL;
using System;

namespace SilkDotNetLibrary.OpenGL.Buffer
{
    public class VertexArrayObjectObject<TVertexType, TIndexType> : IDisposable
       where TVertexType : unmanaged
       where TIndexType : unmanaged
    {
        public readonly uint _vertexArrayObjectObjectHandle;
        private readonly GL _gl;
        private bool disposedValue;

        public unsafe void VertexAttributePointer(uint index,
                                                  int count,
                                                  VertexAttribPointerType vertexAttribPointerType,
                                                  uint vertexSize,
                                                  int offSet)
        {
            //Setting up a vertex attribute pointer
            _gl.VertexAttribPointer(index,
                                    size:count,
                                    vertexAttribPointerType,
                                    false,
                                    vertexSize * (uint)sizeof(TVertexType),
                                    (void*)(offSet * sizeof(TVertexType)));
            _gl.EnableVertexAttribArray(index);
        }

        public void Bind()
        {
            //Binding the vertex array.
            _gl.BindVertexArray(_vertexArrayObjectObjectHandle);
        }

        protected void OnDipose()
        {
            _gl.DeleteVertexArray(_vertexArrayObjectObjectHandle);
        }

        protected void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    OnDipose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~VertexArrayObjectObject()
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
