using Godot;
using System;
using System.Collections.Generic;

public partial class BattleStates : Node
{
    [Export]
    public NodePath initialState;

    private Dictionary<string, State> _states;
    private State _currentState;

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

        _currentState = GetNode<State>(initialState);
        _currentState.Enter();
    }

    public override void _Process(double delta)
    {
        _currentState.Update((float) delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        _currentState._PhysicsProcess((float)delta);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        _currentState.HandleInput(@event);
    }

    public void TransitionTo(string key)
    {
        if (!_states.ContainsKey(key) || _currentState == _states[key])
        {
            return;
        }

        _currentState.Exit();
        _currentState = _states[key];
        _currentState.Enter();
    }
}