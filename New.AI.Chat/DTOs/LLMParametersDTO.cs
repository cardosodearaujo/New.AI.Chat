namespace New.AI.Chat.DTOs
{
    public class LLMParametersDTO
    {
        public int TakeLowGranularitySemanticIDs { get; set; }
        public int TakeLowGranularityWithHighGranularitySemanticIDs { get; set; }
        public int TakeLowGranularityWithHighGranularityLexicalIDs { get; set; }
    }
}
