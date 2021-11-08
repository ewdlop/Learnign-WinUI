namespace SilkDotNetLibrary.OpenGL.Buffer
{
    public interface IBufferObject<TDataType> where TDataType : unmanaged
    {
        void Bind();
    }
}