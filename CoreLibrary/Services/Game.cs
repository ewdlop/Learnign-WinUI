using System;
using Leopotam.EcsLite;
using Microsoft.Extensions.Logging;
using SharedLibrary.Components;
using SharedLibrary.Event.Handler;

namespace CoreLibrary.Services;

public class Game : IGame, IDisposable
{
    private readonly EntitySystem _entitySystem;
    private readonly ComponentSystem<TransformComponent> _transformComponentSystem;
    private readonly ILogger<Game> _logger;
    private readonly IEventHandler _eventHandler;
    public Game(EntitySystem entitySystem, 
                ComponentSystem<TransformComponent> transformComponent, 
                ILogger<Game> logger,
                IEventHandler eventHandler)
    {
        _entitySystem = entitySystem;
        _transformComponentSystem = transformComponent;
        _logger = logger;
        _eventHandler = eventHandler;
        _eventHandler.OnWindowUpdate += OnUpdate;
    }
    public void OnLoad()
    {
        EcsPackedEntityWithWorld entity = _entitySystem.CreateEntity();
        TransformComponent transformComponent = new();
        _transformComponentSystem.TryAddComponent(entity, ref transformComponent);
    }

    public void OnUpdate(object sender, double dt)
    {
        
    }

    public void OnUpdate(double dt)
    {

    }

    public void OnStop()
    {
        throw new System.NotImplementedException();
    }

    public void Dispose()
    {
        _entitySystem?.Dispose();
    }
}