public class PathConfig
{
    public static readonly string Root = "./Res/";
    public static readonly string Log = "./Debug/";
    public static readonly string Config = Root + "Config/";
    public static readonly string Gift = Root + "Gift/";
    public static readonly string Guest = Root + "Guest/";
}

public class SceneName
{
    public static readonly string LogoScene = "LogoScene";
    public static readonly string MainScene = "MainScene";
}

public class GameConfig
{
    public static bool IsLogEnable = true;
    public static readonly int MaxDepth = 1000;
    public static readonly int MaxSortingOrder = 100;
}