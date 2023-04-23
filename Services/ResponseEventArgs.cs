namespace OpenAIService
{
    public class ResponseEventArgs : EventArgs
    {
        public int PromptTokens { get; }
        public int CompletionTokens { get; }
        public int TotalTokens { get; }

        public ResponseEventArgs(int promptTokens, int completionTokens, int totalTokens)
        {
            PromptTokens = promptTokens;
            CompletionTokens = completionTokens;
            TotalTokens = totalTokens;
        }
    }

}