using KernelMemoryOnnx;
using KernelMemoryOnnx.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.Configuration;
using Microsoft.KernelMemory.SemanticKernel;
using Microsoft.ML.Tokenizers;
using Microsoft.SemanticKernel.Connectors.Onnx;

/* This example shows how to use Kernel Memory with Ollama and BERT
 *
 * 1. Download phi-4-onnx model from git-lfs clone https://huggingface.co/microsoft/phi-4-onnx
 *
 * 2. Download bge-micro-v2 model from git-lfs clone https://huggingface.co/TaylorAI/bge-micro-v2
 *
 * 3. Place the models in the Models folder under the project root
 *
 * 4. Run the code
 */

var memoryBuilder = new KernelMemoryBuilder();
var onnxConfig = new OnnxConfig
{
    TextModelDir = GetFullPathToModelDirectory("phi-4-onnx/gpu/gpu-int4-rtn-block-32"),
    MaxTokens = 16384,
};
memoryBuilder.WithOnnxTextGeneration(onnxConfig);
var bertOnnxOptions = new BertOnnxOptions
{
    CaseSensitive = false,
    MaximumTokens = 512,
};
var bgeVocabPath = GetFullPathToModelFile("bge-micro-v2", "vocab.txt");
var bgeEmbeddingGenerator = BertOnnxTextEmbeddingGenerationService.Create(
    GetFullPathToModelFile("bge-micro-v2/onnx", "model.onnx"),
    bgeVocabPath,
    bertOnnxOptions
);
memoryBuilder.WithCustomEmbeddingGenerator(
    new SemanticKernelTextEmbeddingGenerator(
        bgeEmbeddingGenerator,
        new SemanticKernelConfig
        {
            MaxTokenTotal = 512,
        },
#pragma warning disable KMEXP00
        new BertTextTokenizer(BertTokenizer.Create(bgeVocabPath))
#pragma warning restore KMEXP00
    )
);
var memory = memoryBuilder.WithCustomTextPartitioningOptions(
        new TextPartitioningOptions
        {
            MaxTokensPerParagraph = 256,
            OverlappingTokens = 50,
        }
    )
    .WithSimpleVectorDb()
    .Configure(b => b.Services.AddLogging(l =>
            {
                l.SetMinimumLevel(LogLevel.Warning);
                l.AddSimpleConsole(c => c.SingleLine = true);
            }
        )
    )
    .Build();
await memory.ImportTextAsync("Yesterday was October 21st, 2476");
await memory.ImportTextAsync("Tomorrow will be October 23rd, 2476");
var answer = await memory.AskAsync("What's the current date?");
Console.WriteLine(answer.Result);

ConsoleHelper.ConsolePressAnyKey();

return 0;

static string GetFullPathToModelDirectory(string directoryName)
{
    var path = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Models", directoryName);
    return Directory.Exists(path) ? path
        : throw new DirectoryNotFoundException($"Required model folder {path} not found.");
}

static string GetFullPathToModelFile(string directoryName, string fileName)
{
    var path = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Models", directoryName, fileName);
    return File.Exists(path) ? path
        : throw new InvalidOperationException("Required model file " + path + " not found.");
}
