using System.ComponentModel;

namespace New.AI.Chat.Enumerators
{
    public enum LLMEnum
    {
        [Description("phi3")]
        Phi3,

        [Description("qwen1.5")]
        Qwen15,

        [Description("qwen7b")]
        Qwen7b,

        [Description("ner")]
        LightModel
    }
}
