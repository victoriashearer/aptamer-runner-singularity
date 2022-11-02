using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AptamerRunner;

public static class Helpers
{
    public static ImageInfo FullDockerImageName()
    {
        var vars = Environment.GetEnvironmentVariables();

        string image = vars.GetStringOrDefault(EnvVar.ImageOverride, Consts.DefaultImage);
        string tag = vars.GetStringOrDefault(EnvVar.TagOverride, Consts.DefaultTag);
        string? repo = vars.Contains(EnvVar.RepositoryOverride)
            ? vars[EnvVar.RepositoryOverride] as string
            : Consts.DefaultRepository;


        if (string.IsNullOrWhiteSpace(image))
            throw new Exception($"{EnvVar.ImageOverride} cannot be an empty string");

        if (string.IsNullOrWhiteSpace(tag))
        {
            // Might specifying an empty string to default to ":latest"
            tag = Consts.DefaultTag;
        }

        return new ImageInfo(repo: repo, image: image, tag: tag);
    }

    public static async Task<bool> DockerExistsAsync()
    {
        try
        {
            return await ExecuteCmdAsync(args: new List<string>() { "--version" });
        }
        catch (Exception)
        {
            // Almost certainly we've just filed to execute the docker command because it doesn't exist
            return false;
        }
    }

    /// <summary>
    /// Executes a docker command (adding sudo if necessary) and returns whether the command
    /// was successful or not.
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    internal static async Task<bool> ExecuteCmdAsync(List<string> args)
    {
        string fileName = "docker";
        List<string> arguments = new List<string>(args);

        if (OsRequiresSudo())
        {
            fileName = "sudo";
            arguments.Insert(0, "docker");
        }

        var startInfo = new ProcessStartInfo()
        {
            FileName = fileName
        };

        foreach (var argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        if (Debug.IsEnabled)
        {
            Console.WriteLine("RUNNING COMMAND:");
            var allCommandParts = new List<string>() { startInfo.FileName };
            allCommandParts.AddRange(startInfo.ArgumentList);
            Console.WriteLine(string.Join(' ', allCommandParts));
            Console.WriteLine();
        }

        var process = Process.Start(startInfo);

        if (process == null)
        {
            throw new Exception("Failed to start subprocess");
        }

        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            return false;
        }

        return true;
    }

    internal static bool OsRequiresSudo()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Throw an exception if the current platform is not supported
    /// </summary>
    /// <returns></returns>
    /// <exception cref="PlatformNotSupportedException"></exception>
    internal static bool IsPlatformSupported()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ||
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return true;
        }

        return false;
    }

    public static async Task<string> NixUserIdAsync()
    {
        var startInfo = new ProcessStartInfo()
        {
            FileName = "id",
            Arguments = "-u",
            RedirectStandardOutput = true
        };

        var p = Process.Start(startInfo)!;
        await p.WaitForExitAsync();
        var id = await p.StandardOutput.ReadToEndAsync();
        return id;
    }

    public static async Task<string> NixUserGroupAsync()
    {
        var startInfo = new ProcessStartInfo()
        {
            FileName = "id",
            Arguments = "-g",
            RedirectStandardOutput = true
        };

        var p = Process.Start(startInfo)!;
        await p.WaitForExitAsync();
        var id = await p.StandardOutput.ReadToEndAsync();
        return id;
    }
}