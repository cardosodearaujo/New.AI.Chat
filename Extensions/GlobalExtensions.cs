using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;

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

        private static readonly ConcurrentDictionary<Enum, string> _descriptionCache = new();

        public static string GetDescription(this Enum value)
        {
            if (value == null) return string.Empty;

            // Busca no cache; se não existir, executa a função de Reflection e salva
            return _descriptionCache.GetOrAdd(value, enumValue =>
            {
                FieldInfo? field = enumValue.GetType().GetField(enumValue.ToString());

                if (field == null) return enumValue.ToString();

                // Extrai o atributo Description
                var attribute = field.GetCustomAttribute<DescriptionAttribute>();

                return attribute == null ? enumValue.ToString() : attribute.Description;
            });
        }
    }
}
