namespace New.AI.Chat.DTOs
{
    public class DefaultDTO<R>
    {
        public DefaultDTO()
        {
            Success = false;
            ErrorMessages = new List<string>();
        }

        public bool Success { get; set; }
        public R Data { get; set; }
        public IList<string> ErrorMessages { get; }

        public void AddError(string error)
        {
            Success = false;
            ErrorMessages.Add(error);
        }

        public bool HasErrors()
        {
            return ErrorMessages.Any();
        }
    }
}
