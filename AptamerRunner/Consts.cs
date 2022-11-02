namespace AptamerRunner;

public static class Consts
{
    public const int WordWrapLength = 80;
    
    public const string DefaultImage = "thiel-aptamer";
    public const string DefaultTag = "latest";
    //public const string DefaultRepository = "ghcr.io/ui-icts";
    public const string DefaultRepository = "ghcr.io/eaembree";

    public static class Commands
    {
        public const string HelpCmd = "help";
        public const string PredictStructureCmd = "predict-structures";
        public const string CreateGraphCmd = "create-graph";
    }
}

public static class EnvVar
{
    public const string ImageOverride = "APTAMER_IMAGE";
    public const string TagOverride = "APTAMER_TAG";
    public const string RepositoryOverride = "APTAMER_REPOSITORY";
}