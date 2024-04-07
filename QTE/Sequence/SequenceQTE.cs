using Godot;
using System;
using System.Collections.Generic;

public partial class SequenceQTE : QTE
{
    private const int MaxMatchingNumber = 4;
    private Label promptLabel;
    private Timer inputTimer;
    private Queue<string> sequenceToMatch = new Queue<string>();
    private Queue<string> playerInputs = new Queue<string>();
    private string[] possibleKeys = { "A", "S", "D", "F" };

    public override void _Ready()
    {
        promptLabel = GetNode<Label>("Label");
        inputTimer = GetNode<Timer>("Timer");
    }

    public override void StartQTE()
    {
        Show();
        sequenceToMatch.Clear();
        playerInputs.Clear();
        GenerateSequence();
        DisplaySequence();
        inputTimer.Start();
        Status = QTEStatus.Active;
    }

    private void GenerateSequence()
    {
        var random = new Random();
        for (int i = 0; i < MaxMatchingNumber; i++)
        {
            sequenceToMatch.Enqueue(possibleKeys[random.Next(possibleKeys.Length)]);
        }
    }

    private void DisplaySequence()
    {
        promptLabel.Text = "Match this sequence: " + string.Join(" ", sequenceToMatch.ToArray());
    }

    public override void UpdateQTE(double delta)
    {
        if (Status != QTEStatus.Active)
            return;

        foreach (var key in possibleKeys)
        {
            if (Input.IsActionJustPressed(key))
            {
                playerInputs.Enqueue(key);
                CheckSequence();
            }
        }
    }

    private void CheckSequence()
    {
        if (playerInputs.Count == sequenceToMatch.Count)
        {
            while (playerInputs.Count > 0)
            {
                if (playerInputs.Dequeue() != sequenceToMatch.Dequeue())
                {
                    Status = QTEStatus.Failed;
                    EndQTE();
                    return;
                }
            }
            Status = QTEStatus.Success;
            EndQTE();
        }
    }

    public override void EndQTE()
    {
        Hide();
        inputTimer.Stop();
        if (Status == QTEStatus.Success)
        {
            promptLabel.Text = "Success!";
        }
        else
        {
            promptLabel.Text = "Failed!";
        }
    }

    public override void Show()
    {
        promptLabel.Visible = true;
    }

    public override void Hide()
    {
        promptLabel.Visible = false;
    }

    private void OnInputTimerTimeout()
    {
        if (Status == QTEStatus.Active)
        {
            Status = QTEStatus.Failed;
            EndQTE();
        }
    }
}
