namespace BusinessLogicLayer.Helpers;
public static class LoggingService
{
    public static void LogError(string message)
    {
        // Log error to file
        WriteToFile(message, "ERROR");
    }

    public static void LogInfo(string message)
    {
        // Log info to file
        WriteToFile(message, "INFO");
    }

    public static void LogWarning(string message)
    {
        // Log warning to file
        WriteToFile(message, "WARNING");
    }

    private static void WriteToFile(string message, string type)
    {
        //set month name to directory
        string directory = DateTime.Now.ToString("MMMM");
        string path = $"logs\\{directory}";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string filePath = $"{path}\\{DateTime.Now.ToString()
                                                 .Split(" ")[0]
                                                 .Replace("/", "_")}.txt";
        FileInfo fileInfo = new FileInfo(filePath);
        using (StreamWriter streamWriter = fileInfo.AppendText())
        {
            streamWriter.Write($"[{type}] : ");
            streamWriter.WriteLine(message);
            streamWriter.Close();
        }
    }
}
