using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace LlamaCppLib
{
    public class LlmPrompt
    {
        public LlmPrompt(string prompt, bool preprendBosToken = false, bool processSpecialTokens = false)
        {
            this.Prompt = prompt;
            this.PrependBosToken = preprendBosToken;
            this.ProcessSpecialTokens = processSpecialTokens;

            this.Tokens = Channel.CreateUnbounded<byte[]>();
        }

        public LlmPrompt(string prompt, SamplingOptions samplingOptions, bool preprendBosToken = false, bool processSpecialTokens = false) :
            this(prompt, preprendBosToken, processSpecialTokens)
        {
            this.SamplingOptions = samplingOptions;
        }

        public bool Cancelled { get; private set; }

        public SamplingOptions SamplingOptions { get; private set; } = new();
        public string Prompt { get; private set; }
        public bool PrependBosToken { get; private set; }
        public bool ProcessSpecialTokens { get; private set; }
        public int[]? ExtraStopTokens { get; set; }

        public Channel<byte[]> Tokens { get; private set; }

        public async IAsyncEnumerable<byte[]> NextToken([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var result = default(byte[]?);

            while (true)
            {
                try
                {
                    if ((result = await this.Tokens.Reader.ReadAsync(cancellationToken)).Length == 0)
                        break;
                }
                catch (Exception ex) when (ex is OperationCanceledException || ex is ChannelClosedException)
                {
                    this.Cancelled = true;
                    break;
                }

                yield return result;
            }
        }

        public double PromptingSpeed { get; set; }
        public double SamplingSpeed { get; set; }
    }

    public class TokenEnumerator : IAsyncEnumerable<string>
    {
        private MultibyteCharAssembler _assembler = new();
        private LlmPrompt _prompt;

        public TokenEnumerator(LlmPrompt prompt) => _prompt = prompt;

        public async IAsyncEnumerator<string> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            await foreach (var token in _prompt.NextToken(cancellationToken))
                yield return _assembler.Consume(token);

            yield return _assembler.Consume();
        }
    }
}