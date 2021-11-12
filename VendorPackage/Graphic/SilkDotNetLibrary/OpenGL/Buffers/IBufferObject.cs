namespace SilkDotNetLibrary.OpenGL.Buffers
{
    public interface IBufferObject<TDataType> where TDataType : unmanaged
    {
        void Bind();
    }
}