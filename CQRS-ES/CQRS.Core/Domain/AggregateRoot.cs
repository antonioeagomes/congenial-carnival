using CQRS.Core.Events;

namespace CQRS.Core.Domain;

public abstract class AggregateRoot
{
    protected Guid _id;
    private readonly List<BaseEvent> _changes = new();

    public Guid Id { get => _id; }

    public int Version { get; set; } = -1;

    public IEnumerable<BaseEvent> GetUncommittedChanges() => _changes;

    public void MarkChangesAsCommitted()
    {
        _changes.Clear();
    }

    private void ApplyChange(BaseEvent e, bool isNewEvent)
    {
        var method = this.GetType().GetMethod("Apply", new Type[] { e.GetType() });

        if (method == null)
        {
            throw new ArgumentException(nameof(method), $"The Apply method was not found in the aggregate for {e.GetType().Name}");
        }

        method.Invoke(this, new object[] { e });

        if (isNewEvent)
        {
            _changes.Add(e);
        }
    }

    protected void RaiseEvent(BaseEvent e)
    {
        ApplyChange(e, true);
    }

    public void ReplayEvents(IEnumerable<BaseEvent> events)
    {
        foreach (var e in events)
        {
            ApplyChange(e, false);
        }
    }

}