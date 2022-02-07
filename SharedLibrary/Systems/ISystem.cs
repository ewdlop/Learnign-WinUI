namespace SharedLibrary.Systems;

public interface ISystem
{
    void OnLoad();
    void OnUpdate(double dt);
    void OnStop();
}