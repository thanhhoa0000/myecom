namespace MyEcom.Services.Identity.API;

public static class EnvReader
{
    public static void Load(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"The file {filePath} was not found!");

        foreach (var line in File.ReadAllLines(filePath))
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                continue;
            
            var parts = line.Split("=");
            if (parts.Length != 2)
                continue;

            var key = parts[0].Trim();
            var value = parts[1].Trim();
            Environment.SetEnvironmentVariable(key, value);
        }
    }
}