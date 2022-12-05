using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace AptamerRunner;

public static class Commands
{
    public static async Task<bool> PullImageIfMissingAsync(ImageInfo imageInfo)
    {
        List<string> arguments = new List<string>()
        {
            "pull",
            imageInfo.FullName()
        };

        return await Helpers.ExecuteCmdAsync(arguments);
    }

    public static async Task DisplayHelpAsync(ImageInfo imageInfo)
    {
        Console.WriteLine(
            "This application simplifies execution of the aptamer scripts by using Docker to execute commands instead of the requirement of installing the relevant tools."
                .WordWrap(Consts.WordWrapLength));
        Console.WriteLine("See below for the help of each of the command accepted by this script.");
        Console.WriteLine();

        StringBuilder sb = new StringBuilder();
        sb.AppendLine(@"###############################");
        sb.AppendLine(@"### predict-structures Help ###");
        sb.AppendLine(@"###############################");
        var predictHelp = sb.ToString();

        sb.Clear();
        sb.AppendLine(@"#########################");
        sb.AppendLine(@"### create-graph Help ###");
        sb.AppendLine(@"#########################");
        var graphHelp = sb.ToString();

        var bashCommand =
            $"echo \"{predictHelp}\"; python3 -m aptamer.predict_structures --help; echo; echo; echo \"${graphHelp}\"; python3 -m aptamer.create_graph --help";

        Console.WriteLine("EXTRACTING HELP CONTENT. Please wait...");

        await Helpers.ExecuteCmdAsync(new List<string>()
        {
            "run", "--rm",
            imageInfo.FullName(),
            "bash", "-c",
            bashCommand
        });

        Console.WriteLine();
        Console.WriteLine();

        Console.WriteLine("########################################");
        Console.WriteLine("### Overriding Docker Image Defaults ###");
        Console.WriteLine("########################################");
        Console.WriteLine("By default, this script will use the docker image $DEFAULT_IMAGE_SPECIFICATION");
        Console.WriteLine("(repository: $DEFAULT_REPOSITORY, image: $DEFAULT_IMAGE, tag: $DEFAULT_TAG)");
        Console.WriteLine();
        Console.WriteLine(
            "If you woud like to change the image, tag, or repository used set the environment variables APTAMER_IMAGE, APTAMER_TAG, or APTAMER_REPOSITORY respectively. Each will be used in place of the respective default."
                .WordWrap(Consts.WordWrapLength));
        Console.WriteLine();
    }

    public static async Task PredictStructuresAsync(ImageInfo imageInfo, string workingDir, List<string> arguments)
    {
        var localArgs = new List<string>(arguments);

        if (localArgs.Count == 0)
        {
            Console.WriteLine("predict-structures requires a path to an input file\n");
            return;
        }

        string filePathArg = localArgs[0];
        localArgs.RemoveAt(0);

        string outputDirDefault = $"{workingDir}/data";

        if (!File.Exists(filePathArg))
        {
            Console.WriteLine($"Must specify the path to a file. {filePathArg} is not a file");
            return;
        }
        else if (Regex.IsMatch(filePathArg, @"\s+"))
        {
            Console.WriteLine(
                "ERROR: The path to your input file contains spaces. This application does not support spaces in paths as they are difficult to manage.");
            return;
        }

        filePathArg = Path.GetFullPath(filePathArg);
        string inputFileName = Path.GetFileName(filePathArg);
        string inputFileDir = Path.GetDirectoryName(filePathArg)!;

        string outputDir = outputDirDefault;

        List<string> parsedParams = new List<string>();

        bool outOptionFound = false;

        while (localArgs.Count > 0)
        {
            var nextArg = localArgs[0];
            localArgs.RemoveAt(0);

            if (nextArg == "-o" || nextArg == "--output")
            {
                if (outOptionFound)
                {
                    Console.WriteLine("ERROR: Out directory option was specified more than once");
                    return;
                }

                outOptionFound = true;

                // The next param is supposed to be the path
                if (localArgs.Count > 0)
                {
                    var outputPathArg = localArgs[0];
                    localArgs.RemoveAt(0);

                    if (Regex.IsMatch(outputPathArg, @"\s+"))
                    {
                        Console.WriteLine(
                            "ERROR: The path to your output directory contains spaces. This script does not support spaces in paths as they are difficult to manage.");
                        return;
                    }

                    if (File.Exists(outputPathArg))
                    {
                        Console.WriteLine("ERROR: Output path option specified but it matched a file");
                        return;
                    }

                    outputDir = Path.GetFullPath(outputPathArg);
                }
                else
                {
                    Console.WriteLine("ERROR: Output path option specified but no path provided after it");
                    return;
                }
            }
            else
            {
                parsedParams.Add(nextArg);
            }
        }

        if (!Directory.Exists(outputDir))
        {
            Console.WriteLine($"Output directory ${outputDir} does not exist. Creating ${outputDir}");
            Directory.CreateDirectory(outputDir);
        }

        var dockerArgs = new List<string>()
        {
            "run", "--rm",
        };

        // If non-windows must set --user or we end up with root owned files
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            var userId = await Helpers.NixUserIdAsync();
            var group = await Helpers.NixUserGroupAsync();
            dockerArgs.AddRange(new List<string>()
            {
                "--user", $"{userId}:{group}"
            });
        }
        

        dockerArgs.AddRange(new List<string>()
        {
            "--mount", $"type=bind,source={inputFileDir},destination=/files,ro=true",
            "--mount", $"type=bind,source={outputDir},destination=/data",
            imageInfo.FullName(),
            "predict-structures", $"/files/{inputFileName}"
        });

        dockerArgs.AddRange(parsedParams);

        await Helpers.ExecuteCmdAsync(dockerArgs);
    }

    public static async Task CreateGraphAsync(ImageInfo imageInfo, string workingDir, List<string> arguments)
    {
        var localArgs = new List<string>(arguments);

        if (localArgs.Count == 0)
        {
            Console.WriteLine("create-graph requires a path to an input file\n");
            return;
        }

        string filePathArg = localArgs[0];
        localArgs.RemoveAt(0);
        
        if (!File.Exists(filePathArg))
        {
            Console.WriteLine($"Must specify the path to a file. {filePathArg} is not a file");
            return;
        }
        else if (Regex.IsMatch(filePathArg, @"\s+"))
        {
            Console.WriteLine(
                "ERROR: The path to your input file contains spaces. This application does not support spaces in paths as they are difficult to manage.");
            return;
        }

        filePathArg = Path.GetFullPath(filePathArg);
        string inputFileName = Path.GetFileName(filePathArg);
        string inputFileDir = Path.GetDirectoryName(filePathArg)!;
        
        var dockerArgs = new List<string>()
        {
            "run", "--rm",
        };

        // If non-windows must set --user or we end up with root owned files
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            var userId = await Helpers.NixUserIdAsync();
            var group = await Helpers.NixUserGroupAsync();
            dockerArgs.AddRange(new List<string>()
            {
                "--user", $"{userId}:{group}"
            });
        }
        

        dockerArgs.AddRange(new List<string>()
        {
            "--mount", $"type=bind,source={inputFileDir},destination=/files",
            imageInfo.FullName(),
            "create-graph", $"/files/{inputFileName}"
        });

        dockerArgs.AddRange(localArgs);

        await Helpers.ExecuteCmdAsync(dockerArgs);
    }
}