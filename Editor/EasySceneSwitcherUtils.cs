#if UNITY_EDITOR
using System.IO;
using UnityEngine;

public static class EasySceneSwitcherUtils
{
    internal static string PathToAbs(string pathRelToAssets)
    {
        return Path.Combine(Application.dataPath, pathRelToAssets).Replace("\\", "/");
    }

    internal static string PathToProj(string pathRelToAssets)
    {
        return Path.Combine("Assets", pathRelToAssets).Replace("\\", "/");
    }

    internal static bool ValidatePathRelToAssets(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }
        if (path.StartsWith("Assets"))
        {
            return false;
        }
        if (path.Length < 2)
        {
            return false;
        }
        if (path[1] == ':')
        {
            return false;
        }
        return true;
    }
}
#endif
