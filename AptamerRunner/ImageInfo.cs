using System.Text;

namespace AptamerRunner;

public class ImageInfo
{
    public string? Repo { get; }
    public string Image { get; }
    public string? Tag { get; }

    public ImageInfo(string? repo, string image, string? tag)
    {
        Repo = repo;
        Image = image ?? throw new ArgumentNullException(nameof(image));
        Tag = tag;
    }

    public string FullName()
    {
        var sb = new StringBuilder();
        
        // If repository is empty the user is trying to use a locally tagged image and doesn't want the repo specified
        if (!string.IsNullOrWhiteSpace(Repo))
        {
            sb.Append($"{Repo}/");
        }

        sb.Append(Image);

        // User implying ":latest" if not set
        if (!string.IsNullOrWhiteSpace(Tag))
        {
            sb.Append($":{Tag}");
        }

        return sb.ToString();
    }
}
