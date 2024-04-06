using Godot;
using System;

public partial class QTE : Control
{
    private Label keyPromptLabel;
    private Timer timer;
    private String[] keys = { "A", "S", "D", "F" };
    private String currentKey;
    private bool isQTEActive = false;

    public override void _Ready()
    {
        keyPromptLabel = GetNode<Label>("Label");
        StartQTE();
    }

    private void StartQTE()
    {
        currentKey = keys[new Random().Next(keys.Length)];
        keyPromptLabel.Text = $"Press {currentKey}!";
        isQTEActive = true;
    }

    public override void _Process(double delta)
    {
        if (isQTEActive && Input.IsActionJustPressed(currentKey))
        {
            isQTEActive = false;
            keyPromptLabel.Text = "Success!";
            // Handle success (e.g., reward the player)
        }
    }

    private void OnTimerTimeout()
    {
        if (isQTEActive)
        {
            isQTEActive = false;
            keyPromptLabel.Text = "Failed!";
            // Handle failure (e.g., penalize the player)
        }
    }
}
