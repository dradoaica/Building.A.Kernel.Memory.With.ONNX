using Microsoft.KernelMemory.AI;
using Microsoft.ML.Tokenizers;
using System.Diagnostics.CodeAnalysis;

namespace KernelMemoryOnnx;

/// <summary>A text tokenizer that uses the BERT tokenizer.</summary>
/// <param name="tokenizer">The BERT tokenizer instance.</param>
[Experimental("KMEXP00")]
internal sealed class BertTextTokenizer(BertTokenizer tokenizer) : ITextTokenizer
{
    /// <inheritdoc />
    public int CountTokens(string text) => tokenizer.EncodeToIds(text).Count;

    /// <inheritdoc />
    public IReadOnlyList<string> GetTokens(string text) =>
        tokenizer.EncodeToTokens(text, out _).Select(t => t.Value).ToArray();
}
