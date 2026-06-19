namespace AbiogenesisModel.Lib.Pipeline;

public static class Constants
{
    public const string DefaultKey = "default";

    public const string ConfigDirFormat = "config\\{0}";

    public const string YmlFilesPattern = "*.yml";

    public static string GeneralConfigPath { get; } = string.Format(ConfigDirFormat, "general.yml");
}