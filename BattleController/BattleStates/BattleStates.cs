using Godot;
using System.Collections.Generic;

public partial class BattleStates : Node
{
    [Export]
    public NodePath initialState;

    private Dictionary<string, State> _states;
    public State CurrentState;

    public override void _Ready()
    {
        _states = new Dictionary<string, State>();
        foreach (Node node in GetChildren())
        {
            if(node is State s)
            {
                _states[node.Name] = s;
                s.battleStates = this;
                s.Ready();
                s.Exit(); //resets all states
            }
        }

        CurrentState = GetNode<State>(initialState);
        CurrentState.Enter();
    }

    public override void _Process(double delta)
    {
        CurrentState.Update((float) delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        CurrentState._PhysicsProcess((float)delta);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        CurrentState.HandleInput(@event);
    }

    public void TransitionTo(string key)
    {
        if (!_states.ContainsKey(key))
        {
            return;
        }

        if (CurrentState != _states[key])
        {
            CurrentState.Exit();
            CurrentState = _states[key];
            CurrentState.Enter();
        }
    }

}