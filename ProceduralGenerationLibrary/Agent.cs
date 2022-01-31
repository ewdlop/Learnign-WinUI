using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Runtime.CompilerServices;
using ProceduralGenerationLibrary.Maze;
using SharedLibrary.Transforms;

public enum AgentAction
{
    Up,
    Down,
    Left,
    Right,
    None
}

public class Agent
{
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
    public int MimumumStateActionPairFrequencies { get; private set; } = 0;
    public float EstimatedBestPossibleRewardValue { get; private set; } = 10;
    public bool IsPause { get; set; }
    [field: Range(1, 30_000)] public int RestTime { get; private set; } = 1;
    public object RoadBlock { get; private set; }
    public object Goodies { get; private set; }

    public (int, int) StartState { get; private set; }
    public (int,int) FinalState { get; private set; } = (7,9);
    public int StartX { get; private set; }
    public int StartY { get; private set; }
    public int GridSizeX { get; private set; }
    public int GridSizeY { get; private set; }
    private Transform _transform;
    private readonly Dictionary<((int,int),AgentAction),float>? _stateActionPairQValue;
    private readonly Dictionary<((int,int), AgentAction),int>? _stateActionPairFrequencies;
    private readonly Dictionary<(int, int), float>? _stateRewardGrid;
    private readonly Dictionary<AgentAction,Task>? _actionDelegatesDictionary;
    private readonly CancellationTokenSource _stopTokenSource;

    //private readonly GUIController;
    //private readonly Grid;
    #endregion


    public Agent()
    {
        //FinalState = Grid.GoalPosition;
        _actionDelegatesDictionary = new Dictionary<AgentAction, Task>
        {
            [AgentAction.Left] = Left(),
            [AgentAction.Right] = Right(),
            [AgentAction.Up] = Up(),
            [AgentAction.Down] = Down(),
            [AgentAction.None] = None()
        };
        StartX = new Random().Next(0, GridSizeX);
        StartY = new Random().Next(0, GridSizeY);
        _stateActionPairQValue = new Dictionary<((int, int), AgentAction), float>();
        _stateActionPairFrequencies = new Dictionary<((int, int), AgentAction), int>();
        _stateRewardGrid = new Dictionary<(int, int), float>();
        _stopTokenSource = new CancellationTokenSource();
        Initialized();
    }

    #region  Q_Learning_Agent
    private AgentAction Q_Learning_Agent((int,int) currentState, float rewardSignal)
    {
        UpdateStep();
        if (PreviousState == FinalState)
        {
            _stateActionPairQValue[(PreviousState.Value, AgentAction.None)] = rewardSignal;
        }

        if (PreviousState.HasValue)
        {
            ((int, int), AgentAction) stateActionPair = (PreviousState.Value, PreviousAction.Value);
            
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
            return _stateActionPairQValue[(currentState, AgentAction.None)];

        float max = float.NegativeInfinity;

        foreach (AgentAction action in ShuffledActions())
        {
            max = Math.Max(_stateActionPairQValue[(currentState, action)], max);
        }
        return max;
    }

    private static AgentAction[] ShuffledActions()
    {
        AgentAction[] actions = new AgentAction[4];
        int i = 0;
        foreach (AgentAction action in Enum.GetValues(typeof(AgentAction)))
        {
            if (action == AgentAction.None) continue;
            actions[i] = action;
            i++;
        }
        Random random = new Random();
        actions = actions.OrderBy(_ => random.Next()).ToArray();
        return actions;
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

        foreach (AgentAction action in ShuffledActions())
        {
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
        return _stateActionPairFrequencies[(currentState,choice)] < MimumumStateActionPairFrequencies 
            ? EstimatedBestPossibleRewardValue : _stateActionPairQValue[(currentState, choice)];
    }

    private Task Left()
    {
        _transform.Position -= new Vector3(1f, 0f, 0f);
        CurrentGridX--;
        return WaitThenAction(RestTime, (CurrentGridX, CurrentGridY), _stopTokenSource.Token);
    }

    private Task Right()
    {
        _transform.Position += new Vector3(1f, 0f, 0f);
        CurrentGridX++;
        return WaitThenAction(RestTime, (CurrentGridX, CurrentGridY), _stopTokenSource.Token);
    }

    private Task Up()
    {
        _transform.Position += new Vector3(0f, 0f, 1f);
        CurrentGridY++;
        return WaitThenAction(RestTime, (CurrentGridX, CurrentGridY), _stopTokenSource.Token);
    }

    private Task Down()
    {
        _transform.Position -= new Vector3(0f, 0f, 1f);
        CurrentGridY--;
        return WaitThenAction(RestTime, (CurrentGridX, CurrentGridY), _stopTokenSource.Token);
    }

    private Task None()
    {
        ResetAgentToStart();
        UpdateIteration();
        return WaitThenAction(RestTime, (CurrentGridX, CurrentGridY), _stopTokenSource.Token);
    }

    private void ResetAgentToStart()
    {
        _transform.Position = new Vector3(StartState.Item1, 1f, StartState.Item2);
        CurrentGridX = StartState.Item1;
        CurrentGridY = StartState.Item2;
    }

    private async Task WaitThenAction(int waitTime, (int, int) gridCoordinate, CancellationToken cancellationToken)
    {
        await foreach (Task actionTask in Wait(waitTime, gridCoordinate).WithCancellation(cancellationToken))
        {
            await actionTask;
        }
    }

    private async IAsyncEnumerable<Task> Wait(int waitTime, (int, int) gridCoordinate)
    {
        while (!IsPause)
        {
            await Task.Delay(waitTime);
            yield return _actionDelegatesDictionary[Q_Learning_Agent(gridCoordinate, _stateRewardGrid[gridCoordinate])];
        }
    }

    #endregion

    private void Initialized()
    {
        PreviousAction = null;
        PreviousReward = null;
        PreviousState = null;
        Step = 0;
        Iteration = 0;
        _transform.Position = new Vector3(StartX, 1f, StartY);
        StartState = (StartX, StartY);
        CurrentGridX = StartState.Item1;
        CurrentGridY = StartState.Item2;
        for (int i = 0; i < GridSizeX; i++)
        {
            for (int j = 0; j < GridSizeY; j++)
            {
                foreach (AgentAction action in Enum.GetValues(typeof(AgentAction)))
                {
                    _stateActionPairQValue[((i, j), action)] = 0;
                    //StateActionPairFrequencies[((i, j), action)] = 0;
                    _stateRewardGrid[(i,j)] = 0f;
                }
            }
        }
        _stateRewardGrid[FinalState] = 100f;

        for (int i = 0; i < GridSizeX; i++)
        {
            for (int j = 0; j < GridSizeY; j++)
            {
                if (i != StartState.Item1 && i != FinalState.Item1 && j != StartState.Item2 && j != FinalState.Item2)
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
                if(i == 0)
                {
                    _stateActionPairQValue[((i, j), AgentAction.Left)] = float.NegativeInfinity;
                }
                if(j == 0)
                {
                    _stateActionPairQValue[((i, j), AgentAction.Down)] = float.NegativeInfinity;
                }
                if(i == GridSizeX-1)
                {
                    _stateActionPairQValue[((i, j), AgentAction.Right)] = float.NegativeInfinity;
                }
                if(j == GridSizeY-1)
                {
                    _stateActionPairQValue[((i, j), AgentAction.Up)] = float.NegativeInfinity;
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
        CurrentGridX = StartState.Item1;
        CurrentGridY = StartState.Item2;

        for (int i = 0; i < GridSizeX; i++)
        {
            for (int j = 0; j < GridSizeY; j++)
            {
                foreach (AgentAction action in Enum.GetValues(typeof(AgentAction)))
                {
                    if(!(_stateActionPairQValue.ContainsKey(((i, j), action)) && _stateActionPairQValue[((i, j), action)] == float.NegativeInfinity))
                    {
                        _stateActionPairQValue[((i, j), action)] = 0;
                        //StateActionPairFrequencies[((i, j), action)] = 0;
                    }
                }
            }
        }
    }
    private void Update()
    {
        //Grid.instance.UpdateColor(CurrentGridX, CurrentGridY);
    }

    public async Task StartExploring()
    {
        UpdateIteration();
        await WaitThenAction(1000, StartState, _stopTokenSource.Token);
    }

    public void Stop()
    {
        ReInitialized();
        _stopTokenSource.Cancel();
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
    #endregion
}