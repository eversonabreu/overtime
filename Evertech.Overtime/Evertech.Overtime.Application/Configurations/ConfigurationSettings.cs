using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Evertech.Overtime.Application.Configurations;

[ExcludeFromCodeCoverage]
public static class ConfigurationSettings
{
    public static void ResolveSecrets(IConfiguration configuration, string environmentName)
    {
        try
        {
            var secretsFilePath = FindSecretsFile(environmentName.ToLower());
            var secrets = LoadSecrets(secretsFilePath);
            var requiredSecrets = configuration
                .AsEnumerable()
                .Where(x => x.Value == "${SECRET}")
                .ToList();

            foreach (var requiredSecret in requiredSecrets)
            {
                if (!secrets.TryGetValue(requiredSecret.Key, out var value))
                    throw new InvalidOperationException($"Secret not found for key '{requiredSecret.Key}'.");

                configuration[requiredSecret.Key] = value;
            }

            var unresolvedSecrets = configuration
                .AsEnumerable()
                .Where(x => x.Value == "${SECRET}")
                .Select(x => x.Key)
                .ToList();

            if (unresolvedSecrets.Count > 0)
                throw new InvalidOperationException($"Unresolved secrets: {string.Join(", ", unresolvedSecrets)}");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to resolve application secrets.", ex);
        }
    }


    public static string FindSecretsFile(string environmentName)
    {
        try
        {
            string fileName = $"overtime-secrets.{environmentName}.config";
            const int maxLevels = 10;
            var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
            var currentLevel = 0;

            while (directory != null && currentLevel <= maxLevels)
            {
                var filePath = Path.Combine(directory.FullName, fileName);

                if (File.Exists(filePath))
                    return filePath;

                directory = directory.Parent;
                currentLevel++;
            }

            throw new FileNotFoundException($"'{fileName}' was not found after searching {maxLevels} directory levels.");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to locate secrets file.", ex);
        }
    }

    public static Dictionary<string, string> LoadSecrets(string filePath)
    {
        try
        {
            var lines = File.ReadAllLines(filePath, Encoding.UTF8);
            var secrets = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var trimmedLine = line.Trim();

                if (trimmedLine.StartsWith("#"))
                    continue;

                var separatorIndex = trimmedLine.IndexOf('=');

                if (separatorIndex <= 0)
                    throw new InvalidOperationException($"Invalid configuration line: '{line}'.");

                var key = trimmedLine[..separatorIndex].Trim();
                var value = trimmedLine[(separatorIndex + 1)..].Trim();
                secrets[key] = value;
            }

            return secrets;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to load secrets file '{filePath}'.", ex);
        }
    }

    public static string GetEnvironmentName(string[] args)
    {
        try
        {
            var environmentArgument = args
                .Select((value, index) => new { value, index })
                .FirstOrDefault(x =>
                    x.value.Equals("--environment",
                        StringComparison.OrdinalIgnoreCase) || x.value.StartsWith("--environment=", StringComparison.OrdinalIgnoreCase));

            if (environmentArgument != null)
            {
                string environmentName;

                if (environmentArgument.value.Contains('='))
                    environmentName = environmentArgument.value.Split('=', 2)[1];
                else
                    environmentName = args[environmentArgument.index + 1];

                return environmentName;
            }

            var isReleaseCommand = args.Any(x => x.Equals("-c", StringComparison.OrdinalIgnoreCase));

            if (isReleaseCommand)
               return "Development";

#if DEBUG
            return "Development";
#else
        return "Production";
#endif
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to resolve environment name.", ex);
        }
    }
}