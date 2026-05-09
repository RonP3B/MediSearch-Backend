using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using MediSearch.Shared.Constants;
using Microsoft.EntityFrameworkCore.Design;

namespace MediSearch.Infrastructure.Persistence.Shared.Contexts;

public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    private const string PresentationDirectoryName = "Presentation";
    private const string WebApiProjectDirectoryName = "MediSearch.Presentation.WebApi";
    private const string HostingDirectoryName = "Hosting";
    private const string AppHostProjectDirectoryName = "MediSearch.Hosting.AppHost";
    private const int PostgresContainerPort = 5432;

    public AppDbContext CreateDbContext(string[] args)
    {
        string environment =
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
            ?? "Development";

        string webApiPath = ResolveProjectDirectory(
            PresentationDirectoryName,
            WebApiProjectDirectoryName
        );
        string appHostPath = ResolveProjectDirectory(
            HostingDirectoryName,
            AppHostProjectDirectoryName
        );
        string? connectionString = GetConnectionString(webApiPath, appHostPath, environment);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"Connection string '{ServiceNames.Database}' was not found for design-time AppDbContext creation."
            );
        }

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        optionsBuilder.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();

        return new AppDbContext(optionsBuilder.Options);
    }

    private static string ResolveProjectDirectory(
        string layerDirectoryName,
        string projectDirectoryName
    )
    {
        string currentDirectory = Directory.GetCurrentDirectory();

        string[] candidates =
        [
            Path.GetFullPath(
                Path.Combine(currentDirectory, "..", "..", layerDirectoryName, projectDirectoryName)
            ),
            Path.GetFullPath(
                Path.Combine(currentDirectory, "..", layerDirectoryName, projectDirectoryName)
            ),
            Path.GetFullPath(
                Path.Combine(currentDirectory, layerDirectoryName, projectDirectoryName)
            ),
            Path.GetFullPath(
                Path.Combine(
                    currentDirectory,
                    "..",
                    "..",
                    "src",
                    layerDirectoryName,
                    projectDirectoryName
                )
            ),
            Path.GetFullPath(
                Path.Combine(
                    AppContext.BaseDirectory,
                    "..",
                    "..",
                    "..",
                    "..",
                    layerDirectoryName,
                    projectDirectoryName
                )
            ),
            Path.GetFullPath(
                Path.Combine(
                    AppContext.BaseDirectory,
                    "..",
                    "..",
                    "..",
                    "..",
                    "..",
                    "src",
                    layerDirectoryName,
                    projectDirectoryName
                )
            ),
        ];

        return candidates.FirstOrDefault(Directory.Exists)
            ?? throw new DirectoryNotFoundException(
                $"Could not locate the {projectDirectoryName} directory for design-time configuration."
            );
    }

    private static string? GetConnectionString(
        string webApiPath,
        string appHostPath,
        string environment
    )
    {
        string? environmentConnectionString = Environment.GetEnvironmentVariable(
            $"ConnectionStrings__{ServiceNames.Database}"
        );

        if (!string.IsNullOrWhiteSpace(environmentConnectionString))
        {
            return environmentConnectionString;
        }

        string? aspireLocalConnectionString = TryBuildAspireLocalConnectionString(appHostPath);

        if (!string.IsNullOrWhiteSpace(aspireLocalConnectionString))
        {
            return aspireLocalConnectionString;
        }

        string? connectionString = ReadConnectionString(
            Path.Combine(webApiPath, "appsettings.json")
        );

        string environmentSettingsPath = Path.Combine(
            webApiPath,
            $"appsettings.{environment}.json"
        );
        string? environmentFileConnectionString = ReadConnectionString(environmentSettingsPath);
        string? baseConnectionString = string.IsNullOrWhiteSpace(environmentFileConnectionString)
            ? connectionString
            : environmentFileConnectionString;

        if (string.IsNullOrWhiteSpace(baseConnectionString))
        {
            return null;
        }

        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(baseConnectionString);
        var aspireCredentials = ReadAspireDatabaseCredentials(appHostPath);

        if (!string.IsNullOrWhiteSpace(aspireCredentials.Username))
        {
            connectionStringBuilder.Username = aspireCredentials.Username;
        }

        if (!string.IsNullOrWhiteSpace(aspireCredentials.Password))
        {
            connectionStringBuilder.Password = aspireCredentials.Password;
        }

        return connectionStringBuilder.ConnectionString;
    }

    private static string? TryBuildAspireLocalConnectionString(string appHostPath)
    {
        var aspireCredentials = ReadAspireDatabaseCredentials(appHostPath);

        if (
            string.IsNullOrWhiteSpace(aspireCredentials.Username)
            || string.IsNullOrWhiteSpace(aspireCredentials.Password)
        )
        {
            return null;
        }

        var aspireEndpoint = ReadAspireDatabaseEndpoint();

        if (aspireEndpoint is null)
        {
            return null;
        }

        var connectionStringBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = aspireEndpoint.Value.Host,
            Port = aspireEndpoint.Value.Port,
            Database = ServiceNames.Database,
            Username = aspireCredentials.Username,
            Password = aspireCredentials.Password,
        };

        return connectionStringBuilder.ConnectionString;
    }

    private static (string? Username, string? Password) ReadAspireDatabaseCredentials(
        string appHostPath
    )
    {
        string? username =
            Environment.GetEnvironmentVariable(
                $"Parameters__{ServiceNames.DatabaseServer}-username"
            )
            ?? Environment.GetEnvironmentVariable(
                $"Parameters:{ServiceNames.DatabaseServer}-username"
            );
        string? password =
            Environment.GetEnvironmentVariable(
                $"Parameters__{ServiceNames.DatabaseServer}-password"
            )
            ?? Environment.GetEnvironmentVariable(
                $"Parameters:{ServiceNames.DatabaseServer}-password"
            );

        if (!string.IsNullOrWhiteSpace(username) || !string.IsNullOrWhiteSpace(password))
        {
            return (username, password);
        }

        string appHostProjectFilePath = Path.Combine(
            appHostPath,
            $"{AppHostProjectDirectoryName}.csproj"
        );
        string? userSecretsId = ReadUserSecretsId(appHostProjectFilePath);

        if (string.IsNullOrWhiteSpace(userSecretsId))
        {
            return (null, null);
        }

        string secretsFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Microsoft",
            "UserSecrets",
            userSecretsId,
            "secrets.json"
        );

        if (!File.Exists(secretsFilePath))
        {
            return (null, null);
        }

        using var stream = File.OpenRead(secretsFilePath);
        using var document = JsonDocument.Parse(
            stream,
            new JsonDocumentOptions
            {
                CommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
            }
        );

        string usernameKey = $"Parameters:{ServiceNames.DatabaseServer}-username";
        string passwordKey = $"Parameters:{ServiceNames.DatabaseServer}-password";

        username = document.RootElement.TryGetProperty(usernameKey, out var usernameElement)
            ? usernameElement.GetString()
            : null;
        password = document.RootElement.TryGetProperty(passwordKey, out var passwordElement)
            ? passwordElement.GetString()
            : null;

        return (username, password);
    }

    private static string? ReadUserSecretsId(string appHostProjectFilePath)
    {
        if (!File.Exists(appHostProjectFilePath))
        {
            return null;
        }

        var document = XDocument.Load(appHostProjectFilePath);

        return document
            .Descendants()
            .FirstOrDefault(e => e.Name.LocalName == "UserSecretsId")
            ?.Value;
    }

    private static string? ReadConnectionString(string path)
    {
        if (!File.Exists(path))
        {
            return null;
        }

        using var stream = File.OpenRead(path);
        using var document = JsonDocument.Parse(
            stream,
            new JsonDocumentOptions
            {
                CommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
            }
        );

        if (
            !document.RootElement.TryGetProperty("ConnectionStrings", out var connectionStrings)
            || !connectionStrings.TryGetProperty(ServiceNames.Database, out var connectionString)
        )
        {
            return null;
        }

        return connectionString.GetString();
    }

    private static (string Host, int Port)? ReadAspireDatabaseEndpoint()
    {
        string? environmentHost =
            Environment.GetEnvironmentVariable(
                $"{ServiceNames.DatabaseServer.ToUpperInvariant()}_HOST"
            )
            ?? Environment.GetEnvironmentVariable(
                $"{ServiceNames.Database.ToUpperInvariant()}_HOST"
            );
        string? environmentPortValue =
            Environment.GetEnvironmentVariable(
                $"{ServiceNames.DatabaseServer.ToUpperInvariant()}_PORT"
            )
            ?? Environment.GetEnvironmentVariable(
                $"{ServiceNames.Database.ToUpperInvariant()}_PORT"
            );

        if (
            !string.IsNullOrWhiteSpace(environmentHost)
            && int.TryParse(environmentPortValue, out int environmentPort)
        )
        {
            return (environmentHost, environmentPort);
        }

        string? dockerOutput = TryExecuteProcess(
            "docker",
            "ps --format \"{{.Names}}\t{{.Ports}}\""
        );

        if (string.IsNullOrWhiteSpace(dockerOutput))
        {
            return null;
        }

        foreach (
            string line in dockerOutput.Split(
                Environment.NewLine,
                StringSplitOptions.RemoveEmptyEntries
            )
        )
        {
            string[] parts = line.Split('\t', 2, StringSplitOptions.TrimEntries);

            if (
                parts.Length != 2
                || (
                    !parts[0]
                        .Equals(ServiceNames.DatabaseServer, StringComparison.OrdinalIgnoreCase)
                    && !parts[0]
                        .StartsWith(
                            $"{ServiceNames.DatabaseServer}-",
                            StringComparison.OrdinalIgnoreCase
                        )
                )
            )
            {
                continue;
            }

            Match portMatch = Regex.Match(
                parts[1],
                $@"(?<host>\[[^\]]+\]|[^:,\s]+):(?<port>\d+)->{PostgresContainerPort}/tcp",
                RegexOptions.CultureInvariant
            );

            if (!portMatch.Success)
            {
                continue;
            }

            string host = portMatch.Groups["host"].Value.Trim('[', ']');

            if (int.TryParse(portMatch.Groups["port"].Value, out int port))
            {
                return (host, port);
            }
        }

        return null;
    }

    private static string? TryExecuteProcess(string fileName, string arguments)
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using var process = Process.Start(startInfo);

            if (process is null)
            {
                return null;
            }

            string standardOutput = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return process.ExitCode == 0 ? standardOutput : null;
        }
        catch
        {
            return null;
        }
    }
}
