using Godot;
public enum QTEStatus { Inactive, Active, Success, Failed }
public abstract partial class QTE : Node
{
    public QTEStatus Status { get; protected set; } = QTEStatus.Inactive;

    public abstract void StartQTE();
    public abstract void EndQTE();
    public virtual void UpdateQTE(double delta) { }
    public virtual void Show() { }
    public virtual void Hide() { }
}