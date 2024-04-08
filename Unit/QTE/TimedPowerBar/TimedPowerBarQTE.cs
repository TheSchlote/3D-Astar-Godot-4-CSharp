using Godot;

public partial class TimedPowerBarQTE : QTE
{
    [Export]
    public const float startOfHitRange = 40.0f;
    [Export]
    public const float endOfHitRange = 60.0f;
    public ProgressBar PowerBar;
    private float barChangeSpeed = 40.0f;
    private float maxPowerBarValue = 100.0f;
    private float currentPowerBarValue = 0.0f;

    public override void _Ready()
    {
        PowerBar = GetNode<ProgressBar>("TimedPowerBar");
    }

    public override void StartQTE()
    {
        Show();
        currentPowerBarValue = 0.0f;
        PowerBar.Value = 0;
        Status = QTEStatus.Active;
    }

    public override void EndQTE()
    {
        Hide();
        // Check if the power bar value is within the success range
        if (currentPowerBarValue >= startOfHitRange && currentPowerBarValue <= endOfHitRange)
        {
            Status = QTEStatus.Success;
        }
        else
        {
            Status = QTEStatus.Failed;
        }
    }

    public override void UpdateQTE(double delta)
    {
        if (Status != QTEStatus.Active)
            return;

        currentPowerBarValue += barChangeSpeed * (float)delta;
        PowerBar.Value = currentPowerBarValue;

        if (currentPowerBarValue >= maxPowerBarValue)
        {
            // You could reset the value or handle it as a failed QTE.
            currentPowerBarValue = 0.0f;
        }

        if (Input.IsActionJustPressed("ui_accept")) // Replace with your action for stopping the bar
        {
            EndQTE();
        }
    }
    public override void Show()
    {
        PowerBar.Visible = true;
    }

    public override void Hide()
    {
        PowerBar.Visible = false;
    }
}
