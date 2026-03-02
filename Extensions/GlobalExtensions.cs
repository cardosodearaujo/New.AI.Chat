namespace New.AI.Chat.Extensions
{
    public static class GlobalExtensions
    {
        public static bool IsBase64String(this string texto)
        {
            if (string.IsNullOrWhiteSpace(texto) || texto.Length % 4 != 0)
                return false;

            Span<byte> buffer = new byte[(texto.Length * 3) / 4];

            return Convert.TryFromBase64String(texto, buffer, out _);
        }
    }
}
