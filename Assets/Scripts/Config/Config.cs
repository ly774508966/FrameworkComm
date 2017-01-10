using System.IO;

public class PathConfig
{
    public static string Root
    {
        get
        {
            string path = "./Res/";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }
    }

    public static string Debug
    {
        get
        {
            string path = "./Debug/";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }
    }

    public static string Config
    {
        get
        {
            string path = Root + "Config/";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }
    }
}

public class SceneName
{
    public static readonly string LogoScene = "LogoScene";
    public static readonly string MainScene = "MainScene";
}

public class GameConfig
{
    public static readonly bool IsLogEnable = true;
    public static readonly int MaxDepth = 1000;
    public static readonly int MaxSortingOrder = 100;
}