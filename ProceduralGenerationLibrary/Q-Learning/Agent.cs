using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using SharedLibrary.Transforms;

namespace ProceduralGenerationLibrary;


/// <summary>
/// Very likely not thread safe
/// Possible Parallel for 4 direction?
/// </summary>
public class Agent : IDisposable
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
    public AgentAction? PreviousAction { get; private set; }
    public float? PreviousReward { get; private set; }
    [field: Range(0f, 1f)] public float LearningRate { get; private set; } = 1.0f;
    [field: Range(0f, 1f)] public float DiscountingFactor { get; private set; } = 1.0f;
    public int MinimumStateActionPairFrequencies { get; private set; } = 0;
    public float EstimatedBestPossibleRewardValue { get; private set; } = 10;
    public bool IsPause { get; set; }
    [field: Range(1, 30_000)] public int RestTime { get; private set; } = 1;
    public object RoadBlock { get; private set; }
    public object Goodies { get; private set; }

    public (int X, int Y) StartState { get; private set; }
    public (int X,int Y) FinalState { get; private set; } = FINAL_STATE;
    public int StartX { get; private set; }
    public int StartY { get; private set; }
    public int GridSizeX { get; private set; }
    public int GridSizeY { get; private set; }


    private const int POSSIBLE_AGENT_ACTIONS_COUNT = 5;
    private const float FINAL_REWARD = 100f;
    private const int CONCURRENCY_LEVEL = 8;
    private static readonly (int, int) FINAL_STATE = (7, 9);
    private Transform _transform;
    private bool disposedValue;
    private static readonly AgentAction[] AgentActions = new AgentAction[POSSIBLE_AGENT_ACTIONS_COUNT]
    {
         AgentAction.None, AgentAction.Up, AgentAction.Down, AgentAction.Left, AgentAction.Right
    };
    private static readonly AgentAction[] NoneIdleAgentActions = new AgentAction[POSSIBLE_AGENT_ACTIONS_COUNT - 1]
    {
        AgentAction.Up, AgentAction.Down, AgentAction.Left, AgentAction.Right
    };
    private readonly ConcurrentDictionary<((int,int)?,AgentAction?),float> _stateActionPairQValue;
    private readonly ConcurrentDictionary<((int,int)?, AgentAction?),int> _stateActionPairFrequencies;
    private readonly ConcurrentDictionary<(int, int), float> _stateRewardGrid;
    private readonly ConcurrentDictionary<AgentAction,Task> _agentActionTaskDictionary;
    public IReadOnlyDictionary<AgentAction, Task> AgentActionTaskDictionary => _agentActionTaskDictionary;
    private CancellationTokenSource _stopTokenSource;

    //private readonly GUIController;
    //private readonly Grid;
    #endregion

    public Agent(int gridSizeX = 10, int gridSizeY = 10, int? startX = null, int? startY = null)
    {
        //FinalState = Grid.GoalPosition;
        GridSizeX = gridSizeX;
        GridSizeY = gridSizeY;
        _agentActionTaskDictionary = new ConcurrentDictionary<AgentAction, Task>(CONCURRENCY_LEVEL, POSSIBLE_AGENT_ACTIONS_COUNT)
        {
            [AgentAction.Left] = Left(),
            [AgentAction.Right] = Right(),
            [AgentAction.Up] = Up(),
            [AgentAction.Down] = Down(),
            [AgentAction.None] = None()
        };
        StartX = startX is not null? startX.Value : new Random().Next(0, GridSizeX);
        StartY = startY is not null? startY.Value : new Random().Next(0, GridSizeY);
        _stateActionPairQValue = new ConcurrentDictionary<((int, int)?, AgentAction?), float>(CONCURRENCY_LEVEL,GridSizeX * GridSizeY);
        _stateActionPairFrequencies = new ConcurrentDictionary<((int, int)?, AgentAction?), int>(CONCURRENCY_LEVEL,GridSizeX * GridSizeY * POSSIBLE_AGENT_ACTIONS_COUNT);
        _stateRewardGrid = new ConcurrentDictionary<(int, int), float>(CONCURRENCY_LEVEL, GridSizeX * GridSizeY);
        _stopTokenSource = new CancellationTokenSource();
        Initialized();
    }

    #region  Q_Learning_Agent
    private AgentAction Q_Learning_Agent_Action((int,int) currentState, float rewardSignal)
    {
        UpdateStep();
        if (PreviousState == FinalState)
        {
            _stateActionPairQValue[(PreviousState.Value, AgentAction.None)] = rewardSignal;
        }

        if (PreviousState.HasValue)
        {
            ((int, int), AgentAction?) stateActionPair = (PreviousState.Value, PreviousAction.Value);
            
            //StateActionPairFrequencies[stateActionPair]++;
            //StateActionPairQValue[stateActionPair] += LearningRate * (StateActionPairFrequencies[stateActionPair]) * (PreviousReward.Value +
            //    DiscountingFactor * MaxStateActionPairQValue(ref currentState) - StateActionPairQValue[stateActionPair]);

            _stateActionPairQValue[stateActionPair] += LearningRate * (PreviousReward.Value + DiscountingFactor * MaxStateActionPairQValue(currentState)) - _stateActionPairQValue[stateActionPair];
        }
        PreviousState = currentState;
        PreviousAction = ArgMaxActionExploration(currentState);
        PreviousReward = rewardSignal;
        return PreviousAction.Value;
    }

    //Page 844
    private float MaxStateActionPairQValue((int, int) currentState)
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

    private static AgentAction[] ShuffledActions()
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

    private AgentAction ArgMaxActionExploration((int, int) currentState)
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
    private float ExplorationFunction(ref (int, int) currentState, AgentAction choice)
    {
        return _stateActionPairFrequencies[(currentState,choice)] < MinimumStateActionPairFrequencies 
            ? EstimatedBestPossibleRewardValue : _stateActionPairQValue[(currentState, choice)];
    }

    private Task Left()
    {
        _transform.Position -= new Vector3(1f, 0f, 0f);
        CurrentGridX--;
        return WaitThenAction(RestTime, (CurrentGridX, CurrentGridY));
    }

    private Task Right()
    {
        _transform.Position += new Vector3(1f, 0f, 0f);
        CurrentGridX++;
        return WaitThenAction(RestTime, (CurrentGridX, CurrentGridY));
    }

    private Task Up()
    {
        _transform.Position += new Vector3(0f, 0f, 1f);
        CurrentGridY++;
        return WaitThenAction(RestTime, (CurrentGridX, CurrentGridY));
    }

    private Task Down()
    {
        _transform.Position -= new Vector3(0f, 0f, 1f);
        CurrentGridY--;
        return WaitThenAction(RestTime, (CurrentGridX, CurrentGridY));
    }

    private Task None()
    {
        ResetAgentToStart();
        UpdateIteration();
        return WaitThenAction(RestTime, (CurrentGridX, CurrentGridY));
    }

    private void ResetAgentToStart()
    {
        _transform.Position = new Vector3(StartState.X, 1f, StartState.Y);
        CurrentGridX = StartState.X;
        CurrentGridY = StartState.Y;
    }

    private async Task WaitThenAction(int waitTime, (int, int) gridCoordinate)
    {
        await foreach (Task actionTask in Wait(waitTime, gridCoordinate))
        {
            await actionTask;
        }
    }

    private async IAsyncEnumerable<Task> Wait(
        int waitTime,
        (int, int) gridCoordinate)
    {
        while (!IsPause && !_stopTokenSource.IsCancellationRequested)
        {
            await Task.Delay(waitTime, _stopTokenSource.Token);
            yield return _agentActionTaskDictionary[Q_Learning_Agent_Action(gridCoordinate, _stateRewardGrid[gridCoordinate])];
        }
    }

    #endregion

    private void Initialized()
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

    private void ResetActionGrid()
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
    private void ReInitialized()
    {
        PreviousAction = null;
        PreviousReward = null;
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
    private void Update()
    {
        //Grid.instance.UpdateColor(CurrentGridX, CurrentGridY);
    }

    public async Task StartExploringAsync()
    {
        UpdateIteration();
        await WaitThenAction(1000, StartState);
    }

    public void Stop()
    {
        _stopTokenSource.Cancel();
        _stopTokenSource.Dispose();
    }
    
    public void Reset()
    {
        _stopTokenSource = new CancellationTokenSource();
        ReInitialized();
    }

    private void UpdateStep()
    {
        Step++;
        //GUIController?.UpdateStepText(Step.ToString());
    }

    private void UpdateIteration()
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
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion
}