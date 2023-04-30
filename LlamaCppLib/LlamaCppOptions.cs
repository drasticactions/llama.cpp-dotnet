﻿namespace LlamaCppLib
{
    using LlamaToken = System.Int32;

    public class LlamaCppOptions
    {
        public int? ThreadCount { get; set; }
        public int? TopK { get; set; }
        public float? TopP { get; set; }
        public float? Temperature { get; set; }
        public float? RepeatPenalty { get; set; }

        // New sampling options
        public Dictionary<LlamaToken, float> LogitBias { get; set; } = new();
        public float? TfsZ { get; set; }
        public float? TypicalP { get; set; }
        public float? FrequencyPenalty { get; set; }
        public float? PresencePenalty { get; set; }
        public int? Mirostat { get; set; }
        public float? MirostatTAU { get; set; }
        public float? MirostatETA { get; set; }
        public bool? PenalizeNewLine { get; set; }

        public bool IsConfigured => ThreadCount != null && TopK != null && TopP != null && Temperature != null && RepeatPenalty != null;
    }
}
