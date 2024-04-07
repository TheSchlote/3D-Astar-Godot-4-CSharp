using Godot;
using System;

public partial class KeyPressQTE : QTE
{
    private Label keyPromptLabel;
    private string[] keys = { "A", "S", "D", "F" };
    private string currentKey;
    private Timer timer;
    public override void _Ready()
    {
        keyPromptLabel = GetNode<Label>("KeyPress");
        timer = GetNode<Timer>("Timer");
        timer.Timeout += OnTimerTimeout;
    }

    public override void StartQTE()
    {
        Show();
        currentKey = keys[new Random().Next(keys.Length)];
        keyPromptLabel.Text = $"Press {currentKey}!";
        timer.Start(3.0f); // 3 seconds to press the key
        Status = QTEStatus.Active;
    }

    public override void EndQTE()
    {
        Hide();
        timer.Stop();
        keyPromptLabel.Text = Status == QTEStatus.Success ? "Success!" : "Failed!";
    }

    public override void UpdateQTE(double delta)
    {
        if (Status != QTEStatus.Active)
            return;

        if (Input.IsActionJustPressed(currentKey))
        {
            Status = QTEStatus.Success;
            EndQTE();
        }
    }
    public override void Show()
    {
        keyPromptLabel.Visible = true;
    }

    public override void Hide()
    {
        keyPromptLabel.Visible = false;
    }
    private void OnTimerTimeout()
    {
        if (Status == QTEStatus.Active)
        {
            Status = QTEStatus.Failed;
            EndQTE();
        }
    }
}
