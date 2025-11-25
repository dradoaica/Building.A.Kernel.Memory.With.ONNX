# Kernel Memory with ONNX

## Overview

The application `KernelMemoryOnnx` uses:

- **Phi-4 (ONNX)**: For text generation (answering questions).
- **BGE-Micro-v2 (ONNX)**: For text embedding generation (converting text to vectors).
- **Kernel Memory**: To orchestrate the ingestion of text and the retrieval of relevant information to answer
  questions.

## Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later.
- Visual Studio 2026 or VS Code.
- Download Phi-4 model using: git-lfs clone https://huggingface.co/microsoft/phi-4-onnx and place the model files in
  `Models` folder.
- Download BGE-Micro-v2 model using: git-lfs clone https://huggingface.co/TaylorAI/bge-micro-v2 and place the model
  files
  in `Models` folder.

## Problem

This project demonstrates how to use [Microsoft Kernel Memory](https://github.com/microsoft/kernel-memory) with local
ONNX models for text generation and text embedding generation. It serves as a hands-on example for building a RAG (
Retrieval-Augmented Generation) pipeline without relying on external cloud APIs.

## Project Structure

- **KernelMemoryOnnx**: The main console application.
    - **Common/ConsoleHelper.cs**: Helper methods for formatting console output.
    - **BertTextTokenizer.cs**: A custom tokenizer implementation using `BertTokenizer` for the embedding model.
    - **Program.cs**: The main entry point. Configures Kernel Memory, imports data, and asks a question.

## How to Run

1. **Open the Solution**: Open `KernelMemoryOnnx.slnx` or the project folder in your IDE.
2. **Restore Dependencies**: Run `dotnet restore` to install the required NuGet packages.
3. **Run the Application**: Run the application using your IDE or `dotnet run`.

## How it Works

1. **Configuration**: The application configures Kernel Memory to use the local ONNX models for both generation and
   embedding.
2. **Ingestion**: It imports two simple text facts into the memory ("Yesterday was..." and "Tomorrow will be...").
3. **Retrieval & Generation**: It asks a question ("What's the current date?"). Kernel Memory retrieves the relevant
   fact and uses the Phi-4 model to generate an answer.

