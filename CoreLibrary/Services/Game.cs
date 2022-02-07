using Leopotam.EcsLite;
using Microsoft.Extensions.Logging;
using Serilog;
using SharedLibrary.Components;

namespace CoreLibrary.Services;

public class Game : IGame
{
    private readonly EntitySystem _entitySystem;
    private readonly ComponentSystem<TransformComponent> _transformComponentSystem;
    private readonly ILogger<Game> _logger;
    public Game(EntitySystem entitySystem, ComponentSystem<TransformComponent> transformComponent, ILogger<Game> logger)
    {
        _entitySystem = entitySystem;
        _transformComponentSystem = transformComponent;
        _logger = logger;
    }
    public void OnLoad()
    {
        EcsPackedEntityWithWorld entity = _entitySystem.CreateEntity();
        TransformComponent transformComponent = new();
        _transformComponentSystem.TryAddComponent(entity, ref transformComponent);
    }

    public void OnUpdate(double dt)
    {

    }

    public void OnStop()
    {
        throw new System.NotImplementedException();
    }
}