# Publishing
Publishing this application requires .NET 6 or higher.

From root of the repo run the following where &lt;rid&gt; is the appropriate runtime identifier from the [.NET RID Catalog](https://learn.microsoft.com/en-us/dotnet/core/rid-catalog). You can then copy the binary from the bin/Release/net6.0 directory and move it where you like for usage.

`dotnet publish ./AptamerRunner/AptamerRunner.csproj --configuration Release --self-contained true --runtime <rid>`

For more information on self-contained deployments see the [official Microsoft documentation](https://learn.microsoft.com/en-us/dotnet/core/deploying/single-file/overview?tabs=cli).

# Usage Notes
The tool assumes a default working directory of the same directory in which the tool resides for `predict-structures` unless the `-o/--output` is used. For `create-graph` the working directory is the directory which contains the input file.