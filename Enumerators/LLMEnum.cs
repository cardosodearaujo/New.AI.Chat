using System.ComponentModel;

namespace New.AI.Chat.Enumerators
{
    public enum LLMEnum
    {
        [Description("gflash")]
        GeminiFlash = 0,
        
        [Description("phi3")]
        Phi3 = 1,

        [Description("qwen1.5")]
        Qwen15 = 2,

        [Description("qwen7b")]
        Qwen7b = 3,

        [Description("ner")]
        LightModel = 4
    }
}
