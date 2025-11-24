namespace KernelMemoryOnnx.Common;

/// <summary>Provides helper methods for console output and metrics display.</summary>
public static class ConsoleHelper
{
    /// <summary>Writes a header to the console.</summary>
    /// <param name="lines">The lines to write.</param>
    public static void ConsoleWriteHeader(params string[] lines)
    {
        var defaultColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(" ");
        foreach (var line in lines)
        {
            Console.WriteLine(line);
        }

        var maxLength = lines.Select(x => x.Length).Max();
        Console.WriteLine(new string('#', maxLength));
        Console.ForegroundColor = defaultColor;
    }

    /// <summary>Writes a section header to the console.</summary>
    /// <param name="lines">The lines to write.</param>
    public static void ConsoleWriterSection(params string[] lines)
    {
        var defaultColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(" ");
        foreach (var line in lines)
        {
            Console.WriteLine(line);
        }

        var maxLength = lines.Select(x => x.Length).Max();
        Console.WriteLine(new string('-', maxLength));
        Console.ForegroundColor = defaultColor;
    }

    /// <summary>Waits for the user to press any key.</summary>
    public static void ConsolePressAnyKey()
    {
        var defaultColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(" ");
        Console.WriteLine("Press any key to finish.");
        Console.ReadKey();
    }

    /// <summary>Writes an exception to the console.</summary>
    /// <param name="lines">The lines to write.</param>
    public static void ConsoleWriteException(params string[] lines)
    {
        var defaultColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        const string exceptionTitle = "EXCEPTION";
        Console.WriteLine(" ");
        Console.WriteLine(exceptionTitle);
        Console.WriteLine(new string('#', exceptionTitle.Length));
        Console.ForegroundColor = defaultColor;
        foreach (var line in lines)
        {
            Console.WriteLine(line);
        }
    }

    /// <summary>Writes a warning to the console.</summary>
    /// <param name="lines">The lines to write.</param>
    public static void ConsoleWriteWarning(params string[] lines)
    {
        var defaultColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.DarkMagenta;
        const string warningTitle = "WARNING";
        Console.WriteLine(" ");
        Console.WriteLine(warningTitle);
        Console.WriteLine(new string('#', warningTitle.Length));
        Console.ForegroundColor = defaultColor;
        foreach (var line in lines)
        {
            Console.WriteLine(line);
        }
    }
}
