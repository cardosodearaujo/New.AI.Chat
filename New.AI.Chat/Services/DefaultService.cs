using New.AI.Chat.Services.Interfaces;
using New.AI.Chat.Shared;

namespace New.AI.Chat.Services
{
    public abstract class DefaultService<E, S> : IDefaultService<E, S>
    {
        public DefaultService()
        {
            _messages = new List<string>();
            Result = default;
        }

        private List<string> _messages;

        protected void AddError(string message) => _messages.Add(message);

        protected bool HasErrors() => _messages.Any();

        public Result<S> Result { get; private set; }

        protected abstract Task Validate(E entry, CancellationToken cancellationToken);

        public async Task Process(E entry, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await Validate(entry, cancellationToken);

            if (!HasErrors())
            {               
                await DoProcess(entry, cancellationToken);
                Result = Result<S>.Success(await GetResultData(entry, cancellationToken));
            }
            else
            {
                Result = Result<S>.Failure(_messages);
            }
        }

        protected abstract Task DoProcess(E entry, CancellationToken cancellationToken);

        protected virtual Task<S?> GetResultData(E entry, CancellationToken cancellationToken) => Task.FromResult(default(S));
    }
}
