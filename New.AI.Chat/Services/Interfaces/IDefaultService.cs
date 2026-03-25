namespace New.AI.Chat.Services.Interfaces
{
    public interface IDefaultService<E, S>
    {
        public IList<string> Messages { get; }

        public S Data { get; }

        Task Process(E entry);

        void AddError(string message);

        bool HasErrors();
    }
}
