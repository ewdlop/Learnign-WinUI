using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using SharedLibrary.Transforms;

namespace ProceduralGenerationLibrary;


/// <summary>
/// Very likely not thread safe
/// Possible Parallel for 4 direction?
/// </summary>
public class Agent : IDisposable, IAsyncDisposable
{
    public enum AgentAction
    {
        Up,
        Down,
        Left,
        Right,
        None
    }

    #region PropertyAndFields
    public int Step { get; set; }
    public int Iteration { get; private set; }
    public int CurrentGridX { get; private set; }
    public int CurrentGridY { get; private set; }
    public (int, int)? PreviousState { get; private set; } 
    public AgentAction PreviousAction { get; private set; } = AgentAction.None;
    public float PreviousReward { get; private set; } = 0;
    [field: Range(0f, 1f)] public float LearningRate { get; private set; } = 1.0f;
    [field: Range(0f, 1f)] public float DiscountingFactor { get; private set; } = 1.0f;
    public int MinimumStateActionPairFrequencies { get; private set; } = 0;
    public float EstimatedBestPossibleRewardValue { get; private set; } = 10;
    public bool IsPause { get; set; }
    [field: Range(1, 30_000)] public int RestTime { get; private set; } = 1;
    //public object RoadBlock { get; private set; }
    //public object Goodies { get; private set; }

    public (int X, int Y) StartState { get; private set; }
    public (int X,int Y) FinalState { get; private set; } = FINAL_STATE;
    public int StartX { get; private set; }
    public int StartY { get; private set; }
    public int GridSizeX { get; private set; }
    public int GridSizeY { get; private set; }


    protected const int POSSIBLE_AGENT_ACTIONS_COUNT = 5;
    protected const float FINAL_REWARD = 100f;
    protected const int CONCURRENCY_LEVEL = 8;
    protected static readonly (int, int) FINAL_STATE = (7, 9);
    protected Transform _transform;
    protected bool disposedValue;
    protected static readonly AgentAction[] AgentActions = new AgentAction[POSSIBLE_AGENT_ACTIONS_COUNT]
    {
         AgentAction.None, AgentAction.Up, AgentAction.Down, AgentAction.Left, AgentAction.Right
    };
    protected static readonly AgentAction[] NoneIdleAgentActions = new AgentAction[POSSIBLE_AGENT_ACTIONS_COUNT - 1]
    {
        AgentAction.Up, AgentAction.Down, AgentAction.Left, AgentAction.Right
    };
    protected readonly Dictionary<((int,int)?,AgentAction?),float> _stateActionPairQValue;
    protected readonly Dictionary<((int,int)?, AgentAction?),int> _stateActionPairFrequencies;
    protected readonly Dictionary<(int, int), float> _stateRewardGrid;
    protected readonly Dictionary<AgentAction,Action> _agentActionDictionary;
    public IReadOnlyDictionary<AgentAction, Action> AgentActionDictionary => _agentActionDictionary;
    protected CancellationTokenSource _stopTokenSource;

    //private readonly GUIController;
    //private readonly Grid;
    #endregion

    public Agent(int gridSizeX = 10, int gridSizeY = 10, int? startX = null, int? startY = null)
    {
        //FinalState = Grid.GoalPosition;
        GridSizeX = gridSizeX;
        GridSizeY = gridSizeY;
        //_agentActionTaskDictionary = new ConcurrentDictionary<AgentAction, Task>(CONCURRENCY_LEVEL, POSSIBLE_AGENT_ACTIONS_COUNT)
        //{
        //    [AgentAction.Left] = Left(),
        //    [AgentAction.Right] = Right(),
        //    [AgentAction.Up] = Up(),
        //    [AgentAction.Down] = Down(),
        //    [AgentAction.None] = None()
        //};
        _agentActionDictionary = new Dictionary<AgentAction, Action>(POSSIBLE_AGENT_ACTIONS_COUNT)
        {
            [AgentAction.Left] = Left,
            [AgentAction.Right] = Right,
            [AgentAction.Up] = Up,
            [AgentAction.Down] = Down,
            [AgentAction.None] = None
        };
        StartX = startX is not null? startX.Value : new Random().Next(0, GridSizeX);
        StartY = startY is not null? startY.Value : new Random().Next(0, GridSizeY);
        _stateActionPairQValue = new Dictionary<((int, int)?, AgentAction?), float>(GridSizeX * GridSizeY);
        _stateActionPairFrequencies = new Dictionary<((int, int)?, AgentAction?), int>(GridSizeX * GridSizeY * POSSIBLE_AGENT_ACTIONS_COUNT);
        _stateRewardGrid = new Dictionary<(int, int), float>(GridSizeX * GridSizeY);
        _stopTokenSource = new CancellationTokenSource();
        Initialized();
    }

    #region  Q_Learning_Agent
    protected virtual AgentAction Q_Learning_Agent_Action((int,int) currentState, float rewardSignal)
    {
        UpdateStep();
        if (PreviousState == FinalState && PreviousState is not null)
        {
            _stateActionPairQValue[(PreviousState.Value, AgentAction.None)] = rewardSignal;
        }

        if (PreviousState.HasValue)
        {
            ((int, int), AgentAction?) stateActionPair = (PreviousState.Value, PreviousAction);
            
            //StateActionPairFrequencies[stateActionPair]++;
            //StateActionPairQValue[stateActionPair] += LearningRate * (StateActionPairFrequencies[stateActionPair]) * (PreviousReward.Value +
            //    DiscountingFactor * MaxStateActionPairQValue(ref currentState) - StateActionPairQValue[stateActionPair]);

            _stateActionPairQValue[stateActionPair] += LearningRate * (PreviousReward + DiscountingFactor * MaxStateActionPairQValue(currentState)) - _stateActionPairQValue[stateActionPair];
        }
        PreviousState = currentState;
        PreviousAction = ArgMaxActionExploration(currentState);
        PreviousReward = rewardSignal;
        return PreviousAction;
    }

    //Page 844
    protected virtual float MaxStateActionPairQValue((int, int) currentState)
    {
        if (currentState == FinalState)
        {
            return _stateActionPairQValue[(currentState, AgentAction.None)];
        }
        
        float max = float.NegativeInfinity;

        AgentAction[] actionsArray = ShuffledActions();
        for (int i = 0; i < actionsArray.Length; i++)
        {
            AgentAction action = actionsArray[i];
            max = Math.Max(_stateActionPairQValue[(currentState, action)], max);
        }
        return max;
    }

    protected static AgentAction[] ShuffledActions()
    {
        Random random = new Random();
        return NoneIdleAgentActions.OrderBy(_ => random.Next()).ToArray();
    }

    #region not working
    //private AgentAction ArgMaxActionExploration(ref (int, int) currentState)
    //{
    //    if (currentState == FinalState)
    //        return AgentAction.None;

    //    AgentAction argMaxAction = AgentAction.None;
    //    float max = float.NegativeInfinity;

    //    foreach (AgentAction action in SuffledActions())
    //    {
    //        if (action == AgentAction.None)
    //            continue;

    //        if (CurrentGridX - 1 < 0 && action == AgentAction.Left)
    //        {
    //            continue;
    //        }
    //        else if (CurrentGridX + 1 >= GridSizeX && action == AgentAction.Right)
    //        {
    //            continue;
    //        }
    //        else if (CurrentGridY + 1 >= GridSizeY && action == AgentAction.Up)
    //        {
    //            continue;
    //        }
    //        else if (CurrentGridY - 1 < 0 && action == AgentAction.Down)
    //        {
    //            continue;
    //        }
    //        else
    //        {
    //            float value = ExplorationFunction(ref currentState, action);
    //            if (value >= max)
    //            {
    //                max = value;
    //                argMaxAction = action;
    //            }
    //        }
    //    }
    //    return argMaxAction;
    //}
    #endregion

    protected virtual AgentAction ArgMaxActionExploration((int, int) currentState)
    {
        if (currentState == FinalState) return AgentAction.None;

        AgentAction argMaxAgentAction = AgentAction.None;
        float max = float.NegativeInfinity;

        AgentAction[] actionsArray = ShuffledActions();
        for (int i = 0; i < actionsArray.Length; i++)
        {
            AgentAction action = actionsArray[i];
            float value = _stateActionPairQValue[(currentState, action)];
            if (!(value >= max)) continue;
            max = value;
            argMaxAgentAction = action;
        }
        return argMaxAgentAction;
    }

    //Give the agent the option to have the incentives to explore more?
    //Page 842, this function is not well defined apparently 
    protected virtual float ExplorationFunction(ref (int, int) currentState, AgentAction choice)
    {
        return _stateActionPairFrequencies[(currentState,choice)] < MinimumStateActionPairFrequencies 
            ? EstimatedBestPossibleRewardValue : _stateActionPairQValue[(currentState, choice)];
    }

    protected virtual void Left()
    {
        _transform.Position -= new Vector3(1f, 0f, 0f);
        CurrentGridX--;
        //return WaitThenAction(RestTime, (CurrentGridX, CurrentGridY));
    }

    protected virtual void Right()
    {
        _transform.Position += new Vector3(1f, 0f, 0f);
        CurrentGridX++;
        //return WaitThenAction(RestTime, (CurrentGridX, CurrentGridY));
    }

    protected virtual void Up()
    {
        _transform.Position += new Vector3(0f, 0f, 1f);
        CurrentGridY++;
        //return WaitThenAction(RestTime, (CurrentGridX, CurrentGridY));
    }

    protected virtual void Down()
    {
        _transform.Position -= new Vector3(0f, 0f, 1f);
        CurrentGridY--;
        //return WaitThenAction(RestTime, (CurrentGridX, CurrentGridY));
    }

    protected virtual void None()
    {
        ResetAgentToStart();
        UpdateIteration();
        //return WaitThenAction(RestTime, (CurrentGridX, CurrentGridY));
    }

    protected virtual void ResetAgentToStart()
    {
        _transform.Position = new Vector3(StartState.X, 1f, StartState.Y);
        CurrentGridX = StartState.X;
        CurrentGridY = StartState.Y;
    }

    protected virtual void StartActions()
    {
        foreach (Action action in NoWaitThenActions())
        {
            action();
        }
    }
    protected virtual async Task StartActionsAsync(TimeSpan waitTime)
    {
        await foreach (Action action in WaitThenActionsAsync(waitTime))
        {
            action();
        }
    }

    protected virtual IEnumerable<Action> NoWaitThenActions()
    /*        (int, int) gridCoordinate*/
    {
        while (!IsPause && !_stopTokenSource.IsCancellationRequested)
        {
            yield return _agentActionDictionary[Q_Learning_Agent_Action((CurrentGridX, CurrentGridY), _stateRewardGrid[(CurrentGridX, CurrentGridY)])];
        }
    }

    protected virtual async IAsyncEnumerable<Action> WaitThenActionsAsync(
        TimeSpan waitTime)
/*        (int, int) gridCoordinate*/
    {
        while (!IsPause && !_stopTokenSource.IsCancellationRequested)
        {
            await Task.Delay(waitTime, _stopTokenSource.Token);
            yield return _agentActionDictionary[Q_Learning_Agent_Action((CurrentGridX,CurrentGridY), _stateRewardGrid[(CurrentGridX, CurrentGridY)])];
        }
    }

    #endregion

    protected virtual void Initialized()
    {
        _transform.Position = new Vector3(StartX, 1f, StartY);
        StartState = (StartX, StartY);
        CurrentGridX = StartState.X;
        CurrentGridY = StartState.Y;
        _stateRewardGrid[FinalState] = FINAL_REWARD;
        ResetActionGrid();

        for (int i = 0; i < GridSizeX; i++)
        {
            for (int j = 0; j < GridSizeY; j++)
            {
                if (i != StartState.X && i != FinalState.X && j != StartState.Y && j != FinalState.Y)
                {
                    double random = new Random().NextDouble();
                    if (random is <= 0.3f and <= 0.2f)
                    {
                        //CreateRoadBlock()
                        //Instantiate(RoadBlock, new Vector3(i, 0.5f, j), Quaternion.identity);
                        if (i + 1 < GridSizeX)
                        {
                            _stateActionPairQValue[((i + 1, j), AgentAction.Left)] = float.NegativeInfinity;
                        }
                        if (i - 1 >= 0)
                        {
                            _stateActionPairQValue[((i - 1, j), AgentAction.Right)] = float.NegativeInfinity;
                        }
                        if (j + 1 < GridSizeY)
                        {
                            _stateActionPairQValue[((i, j + 1), AgentAction.Down)] = float.NegativeInfinity;
                        }
                        if (j - 1 >= 0)
                        {
                            _stateActionPairQValue[((i, j - 1), AgentAction.Up)] = float.NegativeInfinity;
                        }
                    }
                    //else
                    //{
                    //    Instantiate(Goodies, new Vector3(i, 0.5f, j), Quaternion.identity);
                    //    StateRewardGrid[(i, j)] = 5f;
                    //}
                }

                if (i != 0 && j != 0 && i != GridSizeX - 1 && j != GridSizeY - 1) continue;
                _stateRewardGrid[(i, j)] = 0f;
                //Prevent the agent go out of bound
                if (i == 0)
                {
                    _stateActionPairQValue[((i, j), AgentAction.Left)] = float.NegativeInfinity;
                }
                if (j == 0)
                {
                    _stateActionPairQValue[((i, j), AgentAction.Down)] = float.NegativeInfinity;
                }
                if (i == GridSizeX - 1)
                {
                    _stateActionPairQValue[((i, j), AgentAction.Right)] = float.NegativeInfinity;
                }
                if (j == GridSizeY - 1)
                {
                    _stateActionPairQValue[((i, j), AgentAction.Up)] = float.NegativeInfinity;
                }
            }
        }
    }

    protected virtual void ResetActionGrid()
    {
        for (int i = 0; i < GridSizeX; i++)
        {
            for (int j = 0; j < GridSizeY; j++)
            {
                for (int k = 0; k < AgentActions.Length; k++)
                {
                    _stateActionPairQValue[((i, j), AgentActions[k])] = 0;
                }
            }
        }
    }

    #region Run
    protected virtual void ReInitialized()
    {
        PreviousState = null;
        Step = 0;
        Iteration = 0;
        _transform.Position = new Vector3(StartX, 1f, StartY);
        StartState = (StartX, StartY);
        CurrentGridX = StartState.X;
        CurrentGridY = StartState.Y;

        for (int i = 0; i < GridSizeX; i++)
        {
            for (int j = 0; j < GridSizeY; j++)
            {
                foreach (AgentAction action in Enum.GetValues(typeof(AgentAction)))
                {
                    for (int k = 0; k < AgentActions.Length; k++)
                    {
                        if (!(_stateActionPairQValue.ContainsKey(((i, j), action)) && float.IsNegativeInfinity(_stateActionPairQValue[((i, j), action)])))
                        {
                            _stateActionPairQValue[((i, j), AgentActions[k])] = 0;
                            //StateActionPairFrequencies[((i, j), action)] = 0;
                        }
                    }
                }
            }
        }
    }
    protected virtual void Update()
    {
        //Grid.instance.UpdateColor(CurrentGridX, CurrentGridY);
    }

    protected virtual void StartExploring()
    {
        UpdateIteration();
        StartActions();
    }
    
    public async Task StartExploringAsync(TimeSpan waitTime)
    {
        UpdateIteration();
        await StartActionsAsync(waitTime);
    }

    public virtual void Stop()
    {
        _stopTokenSource.Cancel();
        _stopTokenSource.Dispose();
    }
    
    public virtual void Reset()
    {
        _stopTokenSource = new CancellationTokenSource();
        ReInitialized();
    }

    protected virtual void UpdateStep()
    {
        Step++;
        //GUIController?.UpdateStepText(Step.ToString());
    }

    protected void UpdateIteration()
    {
        Iteration++;
        //GUIController?.UpdateInterationText(Iteration.ToString());
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _stopTokenSource.Cancel();
                _stopTokenSource.Dispose();
            }
            disposedValue = true;
        }
    }

    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                await _stopTokenSource.CancelAsync();
                _stopTokenSource.Dispose();
            }
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion
}