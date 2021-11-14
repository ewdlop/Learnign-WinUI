namespace SharedLibrary.Cameras
{
    public interface ICameraMoveable
    {
        float Speed { get; }
        void Move();
        void Reset();
    }
}
