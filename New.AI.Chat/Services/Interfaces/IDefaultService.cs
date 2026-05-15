using System.Threading;
using New.AI.Chat.Shared;

namespace New.AI.Chat.Services.Interfaces
{
    public interface IDefaultService<E, S>
    {
        public Result<S> Result { get; }

        Task Process(E entry, CancellationToken cancellationToken = default);
    }
}
