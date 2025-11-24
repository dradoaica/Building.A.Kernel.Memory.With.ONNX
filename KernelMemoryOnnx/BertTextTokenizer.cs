using Microsoft.KernelMemory.AI;
using Microsoft.ML.Tokenizers;
using System.Diagnostics.CodeAnalysis;

namespace KernelMemoryOnnx;

[Experimental("KMEXP00")]
internal sealed class BertTextTokenizer(BertTokenizer tokenizer) : ITextTokenizer
{
    public int CountTokens(string text) => tokenizer.EncodeToIds(text).Count;

    public IReadOnlyList<string> GetTokens(string text) =>
        tokenizer.EncodeToTokens(text, out _).Select(t => t.Value).ToArray();
}
