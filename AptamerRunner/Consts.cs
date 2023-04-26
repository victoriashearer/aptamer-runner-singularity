namespace AptamerRunner;

public static class Consts
{
    public const int WordWrapLength = 80;
    
    public static class Commands
    {
        public const string HelpCmd = "help";
        public const string PredictStructureCmd = "predict-structures";
        public const string CreateGraphCmd = "create-graph";
    }
}

public static class DockerDefaults
{
    public const string Image = "thiel-aptamer";
    public const string Tag = "latest";
    public const string Repository = "ghcr.io/ui-icts";
    
    public static ImageInfo ImageInfo()
    {
        return new ImageInfo(
            repo: Repository,
            image: Image,
            tag: Tag);
    }
}

public static class EnvVar
{
    public const string ImageOverride = "APTAMER_IMAGE";
    public const string TagOverride = "APTAMER_TAG";
    public const string RepositoryOverride = "APTAMER_REPOSITORY";
}