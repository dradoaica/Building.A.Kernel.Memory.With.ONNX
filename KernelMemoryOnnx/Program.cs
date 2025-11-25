using KernelMemoryOnnx;
using KernelMemoryOnnx.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.Configuration;
using Microsoft.KernelMemory.SemanticKernel;
using Microsoft.ML.Tokenizers;
using Microsoft.SemanticKernel.Connectors.Onnx;

/* This example shows how to use Kernel Memory with ONNX local models.
 *
 * 1. Download Phi-4 model using: git-lfs clone https://huggingface.co/microsoft/phi-4-onnx
 * 2. Download BGE-Micro-v2 model using: git-lfs clone https://huggingface.co/TaylorAI/bge-micro-v2
 * 3. Place the models in the Models folder under the project root
 * 4. Run the code
 *
 */
try
{
    // 1. Configure ONNX Text Generation (Phi-4)
    ConsoleHelper.ConsoleWriterSection("Configuring ONNX Text Generation (Phi-4)...");
    var textModelDir = GetFullPathToModelDir("phi-4-onnx/gpu/gpu-int4-rtn-block-32");
    var onnxConfig = new OnnxConfig
    {
        TextModelDir = textModelDir,
        MaxTokens = 16384,
    };
    Console.WriteLine($"Text Model Directory: {onnxConfig.TextModelDir}");
    // 2. Configure ONNX Text Embedding Generation (BGE-Micro-v2)
    ConsoleHelper.ConsoleWriterSection("Configuring ONNX Text Embedding Generation (BGE-Micro-v2)...");
    var bertOnnxOptions = new BertOnnxOptions
    {
        CaseSensitive = false,
        MaximumTokens = 512,
    };
    var onnxModelPath = GetFullPathToModelFilePath("bge-micro-v2/onnx", "model.onnx");
    var vocabPath = GetFullPathToModelFilePath("bge-micro-v2", "vocab.txt");
    var bgeEmbeddingGenerator = BertOnnxTextEmbeddingGenerationService.Create(
        onnxModelPath,
        vocabPath,
        bertOnnxOptions
    );
    Console.WriteLine($"ONNX Model Path: {onnxModelPath}");
    Console.WriteLine($"Vocab Path: {vocabPath}");
    // 3. Build Kernel Memory
    ConsoleHelper.ConsoleWriterSection("Building Kernel Memory...");
    var memoryBuilder = new KernelMemoryBuilder().WithOnnxTextGeneration(onnxConfig)
        .WithCustomEmbeddingGenerator(
            new SemanticKernelTextEmbeddingGenerator(
                bgeEmbeddingGenerator,
                new SemanticKernelConfig
                {
                    MaxTokenTotal = 512,
                },
#pragma warning disable KMEXP00
                new BertTextTokenizer(BertTokenizer.Create(vocabPath))
#pragma warning restore KMEXP00
            )
        )
        .WithCustomTextPartitioningOptions(
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
        );
    var memory = memoryBuilder.Build();
    Console.WriteLine("Kernel Memory built successfully");
    // 4. Import Data
    ConsoleHelper.ConsoleWriterSection("Importing Data...");
    var textToImport = "Today is October 22nd, 2476";
    await memory.ImportTextAsync(textToImport);
    Console.WriteLine($"Imported: '{textToImport}'");
    textToImport = "Tomorrow will be October 23rd, 2476";
    await memory.ImportTextAsync(textToImport);
    Console.WriteLine($"Imported: '{textToImport}'");
    // 5. Ask a Question
    ConsoleHelper.ConsoleWriterSection("Asking Question...");
    const string question = "What's the current date?";
    Console.WriteLine($"Question: {question}");
    var answer = await memory.AskAsync(question);
    Console.WriteLine($"Answer: {answer.Result}");
}
catch (Exception ex)
{
    ConsoleHelper.ConsoleWriteException("An error occurred:", ex.Message);
    if (ex is DirectoryNotFoundException or InvalidOperationException)
    {
        ConsoleHelper.ConsoleWriteWarning(
            "Please ensure you have downloaded the required ONNX models and placed them in the 'Models' directory."
        );
    }
}

ConsoleHelper.ConsolePressAnyKey();

return 0;

static string GetFullPathToModelDir(string directoryName)
{
    var path = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Models", directoryName);
    return Directory.Exists(path) ? path
        : throw new DirectoryNotFoundException($"Required model directory {path} not found.");
}

static string GetFullPathToModelFilePath(string directoryName, string fileName)
{
    var path = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Models", directoryName, fileName);
    return File.Exists(path) ? path
        : throw new InvalidOperationException("Required model file " + path + " not found.");
}
