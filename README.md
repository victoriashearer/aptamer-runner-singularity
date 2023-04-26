# Publishing
Publishing this application requires .NET 6 or higher.

From root of the repo run the following where &lt;rid&gt; is the appropriate runtime identifier from the [.NET RID Catalog](https://learn.microsoft.com/en-us/dotnet/core/rid-catalog). You can then copy the binary from the bin/Release/net6.0 directory and move it where you like for usage.

`dotnet publish ./AptamerRunner/AptamerRunner.csproj --configuration Release --self-contained true --runtime <rid>`

For more information on self-contained deployments see the [official Microsoft documentation](https://learn.microsoft.com/en-us/dotnet/core/deploying/single-file/overview?tabs=cli).

## Some Common RIDs (as of 2023-04-26)
### Windows
| Description | RID |
|-------------|-----|
| x64 not version-specific | win-x64 |
| x86 not version-specific | win-x86 |
| arm64 not version-specific | win-arm64 |

### Linux
| Description | RID |
|-------------|-----|
| x64 not distribution-specific | linux-x64 |
| musl-x64 not distribution-specific | linux-musl-x64 |
| arm not distribution-specific | linux-arm |
| arm64 not distribution-specific | linux-arm64 |

### macOS
| Description | RID |
|-------------|-----|
| not version-specific (Min version 10.12 Sierra) | os-x64 |
| 10.10 Yosemite | osx.10.10-x64 |
| 10.11 El Capitan | osx.10.11-x64 |


# Usage Notes
The tool assumes a default working directory of the same directory in which the tool resides for `predict-structures` unless the `-o/--output` is used. For `create-graph` the working directory is the directory which contains the input file.
