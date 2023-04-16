using CQRS.Core.Command;

namespace CQRS.Core.Infra;
public interface ICommandDispatcher
{

    void RegisterHandler<T>(Func<T, Task> handler) where T : BaseCommand;
    Task SendAsync(BaseCommand command);

}