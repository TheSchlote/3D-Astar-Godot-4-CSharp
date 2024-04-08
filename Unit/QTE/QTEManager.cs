using Godot;
public enum QTEType
{
    KeyPress,
    TimedPowerBar,
    Sequence
}
public partial class QTEManager : Control
{
    [Export]
    PackedScene keyPressQTEScene;
    [Export]
    PackedScene timedPowerBarQTEScene;
    [Export]
    PackedScene sequenceQTEScene;

    private QTE currentQTE = null;

    public override void _Ready()
    {
        //This is for testing, eventually this gets called when an attack is selected
        StartQTE(QTEType.Sequence);
    }

    public void StartQTE(QTEType type)
    {
        currentQTE?.QueueFree();
        currentQTE = null;

        switch (type)
        {
            case QTEType.KeyPress:
                currentQTE = (QTE)keyPressQTEScene.Instantiate();
                break;
            case QTEType.TimedPowerBar:
                currentQTE = (QTE)timedPowerBarQTEScene.Instantiate();
                break;
            case QTEType.Sequence:
                currentQTE = (QTE)(sequenceQTEScene.Instantiate());
                break;
        }

        if (currentQTE != null)
        {
            AddChild(currentQTE);
            currentQTE.StartQTE();
        }
    }

    public override void _Process(double delta)
    {
        currentQTE?.UpdateQTE(delta);
    }
}