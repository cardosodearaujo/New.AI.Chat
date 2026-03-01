using New.AI.Chat.Services.Interfaces;

namespace New.AI.Chat.Services
{
    public abstract class DefaultService<E,S> : IDefaultService<E, S>
    {
        public DefaultService()
        {
            Messages = new List<string>();
            Data = default;
        }

        public IList<string> Messages { get; }

        public S Data { get; set; }

        public void AddError(string message)
        {
            Messages.Add(message);
        }

        public bool HasErrors()
        {
            return Messages.Any();
        }

        protected abstract Task Validate(E entry);

        public async Task Process(E entry)
        {
            await Validate(entry);

            if (!HasErrors())
            {
                await DoProcess(entry);
            }
        }

        protected abstract Task DoProcess(E entry);
    }
}
