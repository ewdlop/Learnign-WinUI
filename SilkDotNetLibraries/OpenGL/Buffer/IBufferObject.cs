namespace _3rdPartyGraphicLibrary.OpenGL.Buffer
{
    public interface IBufferObject<TDataType> where TDataType : unmanaged
    {
        void Bind();
    }
}