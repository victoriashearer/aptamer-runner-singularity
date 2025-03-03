using AptamerRunner;

const int exitFailure = 1;

if (!Helpers.IsPlatformSupported())
{
    Console.WriteLine("This operating system is not supported by this application");
    Environment.Exit(exitFailure);
    return; // Silence uninitialized warning
}

var arguments = Environment.GetCommandLineArgs().ToList();

// First will be the executing dll
string workingDir = Path.GetFullPath(Path.GetDirectoryName(arguments[0])!);
arguments.RemoveAt(0);

if (arguments.Count > 0 && arguments[0] == "--DEBUG")
{
    Debug.IsEnabled = true;
    arguments.RemoveAt(0);
}

if (Helpers.OsRequiresSudo())
{
    Console.WriteLine(
        "PLEASE NOTE: This script uses sudo to execute singularity commands on your behalf. For this reason you will see password requests at times when executing this script.");
    Console.WriteLine();
}

if (!await Helpers.SingularityExistsAsync())
{
    Console.WriteLine(
        "Singularity is not installed or not accessible but is required by this script. Please download and install Singularity. See https://sylabs.io/singularity/");
    Environment.Exit(exitFailure);
    return; // Silence uninitialized warning
}

ImageInfo imageInfo;
try
{
    imageInfo = Helpers.SingularityImageInfo();
}
catch (Exception ex)
{
    Console.WriteLine("Failed to generate singularity image name. Please check your environment's setup");
    Console.WriteLine(ex.Message);
    Environment.Exit(exitFailure);
    return; // Silence uninitialized warning
}

Console.WriteLine($"Using singularity image {imageInfo.FullName()}\n");

Console.WriteLine("Pulling singularity image to verify the latest version is present.\n");

// We only need to run a pull if a repo has been specified
if (!string.IsNullOrWhiteSpace(imageInfo.Repo))
{
    if (!await Commands.PullImageIfMissingAsync(imageInfo))
    {
        Console.WriteLine($"Failed to pull singularity image {imageInfo.FullName()}");
        Environment.Exit(exitFailure);
        return; // Silence uninitialized warning
    }
}

if (arguments.Count == 0)
{
    Console.WriteLine("\n** Welcome to Aptamer! **\n");
    await Commands.DisplayHelpAsync(imageInfo);
    Console.WriteLine();
    return;
}

var command = arguments[0];

switch (command)
{
    case Consts.Commands.HelpCmd:
        await Commands.DisplayHelpAsync(imageInfo);
        break;

    case Consts.Commands.PredictStructureCmd:
        await Commands.PredictStructuresAsync(imageInfo, workingDir, arguments.Skip(1).ToList());
        break;

    case Consts.Commands.CreateGraphCmd:
        await Commands.CreateGraphAsync(imageInfo, workingDir, arguments.Skip(1).ToList());
        break;

    default:
        Console.WriteLine("ERROR: UNRECOGNIZED FIRST PARAMETER");
        Console.WriteLine(
            $"This script accepts only '{Consts.Commands.PredictStructureCmd}', '{Consts.Commands.CreateGraphCmd}', and '{Consts.Commands.HelpCmd}' as its first parameter");
        break;
}

Console.WriteLine();
