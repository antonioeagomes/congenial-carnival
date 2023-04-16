using CQRS.Core.Command;
using CQRS.Core.Infra;

namespace Post.Cmd.Infra.Dispatchers;

public class CommandDispatcher : ICommandDispatcher
{
    private readonly Dictionary<Type, Func<BaseCommand, Task>> _handlers = new();

    public void RegisterHandler<T>(Func<T, Task> handler) where T : BaseCommand
    {
        if (_handlers.ContainsKey(typeof(T)))
        {
            throw new IndexOutOfRangeException("Command already registered");
        }

        _handlers.Add(typeof(T), x => handler((T)x));
    }

    public async Task SendAsync(BaseCommand command)
    {
        if (_handlers.TryGetValue(command.GetType(), out Func<BaseCommand, Task>? handler))
        {
            await handler(command);
        }
        else
        {
            throw new ArgumentNullException(nameof(handler), "Handler not registered");
        }
    }

}