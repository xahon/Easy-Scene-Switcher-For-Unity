#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;


public partial class EasySceneSwitcher
{
    internal const string DEFAULT_GENERATED_FILE_PATH_REL_TO_ASSETS = "Scripts/Editor/EasySceneSwitcher.part.cs";

    private static Regex NameCleanerRegex = new Regex(@"[^a-zA-Z0-9_]");

    internal static List<string> GetAllScenesInProject()
    {
        List<string> scenePaths = new List<string>();

        foreach (string file in Directory.EnumerateFiles(Application.dataPath, "*.unity", SearchOption.AllDirectories))
        {
            if (file.EndsWith(".unity"))
            {
                string fileFixedPath = file.Replace(Application.dataPath, "Assets").Replace("\\", "/");
                scenePaths.Add(fileFixedPath);
            }
        }
        return scenePaths;
    }

    [MenuItem("EasySceneSwitcher/Settings ...", priority = -1000)]
    private static void OpenSettings()
    {
        EasySceneSwitcherSettings settingsWindow = EditorWindow.GetWindow<EasySceneSwitcherSettings>(false, "Easy Scene Switcher Settings", true);
        settingsWindow.Show();
    }

    [MenuItem("EasySceneSwitcher/Open scene list ...", priority = -1000)]
    private static void OpenSceneList()
    {
        EasySceneSwitcherListWindow listWindow = EditorWindow.GetWindow<EasySceneSwitcherListWindow>(false, "Easy Scene Switcher", true);
        listWindow.Show();
    }

    [MenuItem("EasySceneSwitcher/Regenerate database", priority = -100)]
    internal static void RegenerateDatabase()
    {
        bool generateInlineList = PlayerPrefs.GetInt(EasySceneSwitcherKeys.GENERATE_SCENE_LIST_INLINE, 1) == 1;
        bool stripUnityExt = PlayerPrefs.GetInt(EasySceneSwitcherKeys.STRIP_UNITY_EXTENSION, 1) == 1;
        string targetFilePathRelToAssets = PlayerPrefs.GetString(EasySceneSwitcherKeys.GENERATED_FILE_PATH_REL_TO_ASSETS, null);
        if (string.IsNullOrEmpty(targetFilePathRelToAssets))
        {
            targetFilePathRelToAssets = DEFAULT_GENERATED_FILE_PATH_REL_TO_ASSETS;
        }
        PlayerPrefs.SetString(EasySceneSwitcherKeys.GENERATED_FILE_PATH_REL_TO_ASSETS, targetFilePathRelToAssets);

        if (!generateInlineList)
        {
            return;
        }

        Debug.Log($"Regenerating scene database \"{targetFilePathRelToAssets}\"");
        StringBuilder sourceCode = new StringBuilder();

        sourceCode.AppendLine("#if UNITY_EDITOR");
        sourceCode.AppendLine("using UnityEditor;");
        sourceCode.AppendLine("using UnityEditor.SceneManagement;");
        sourceCode.AppendLine("");
        sourceCode.AppendLine("public partial class EasySceneSwitcher {");

        List<string> allScenePaths = GetAllScenesInProject();

        for (int i = 0; i < allScenePaths.Count; i++)
        {
            string scenePath = allScenePaths[i];
            string sceneName = Path.GetFileName(scenePath);
            string sceneNameSanitized = NameCleanerRegex.Replace(scenePath, "_");

            if (stripUnityExt)
            {
                sceneName = Path.GetFileNameWithoutExtension(scenePath);
            }

            sourceCode.AppendLine($"    [MenuItem(\"EasySceneSwitcher/{sceneName}\")]");
            sourceCode.AppendLine($"    static void SwitchToScene_{sceneNameSanitized}() {{");
            sourceCode.AppendLine($"        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {{");
            sourceCode.AppendLine($"            EditorSceneManager.OpenScene(\"{scenePath}\", OpenSceneMode.Single);");
            sourceCode.AppendLine($"        }}");
            sourceCode.AppendLine($"    }}");

            if (i < EditorSceneManager.sceneCount - 1)
                sourceCode.AppendLine("");
        }

        sourceCode.AppendLine("}");
        sourceCode.AppendLine("#endif // UNITY_EDITOR");

        Directory.CreateDirectory(Path.GetDirectoryName(EasySceneSwitcherUtils.PathToAbs(targetFilePathRelToAssets)));
        File.WriteAllText(EasySceneSwitcherUtils.PathToAbs(targetFilePathRelToAssets), sourceCode.ToString());

        AssetDatabase.ImportAsset(EasySceneSwitcherUtils.PathToProj(targetFilePathRelToAssets), ImportAssetOptions.ForceUpdate);
    }

    [MenuItem("EasySceneSwitcher/Delete database", priority = -99)]
    internal static void DeleteDatabase()
    {
        string targetFilePathRelToAssets = PlayerPrefs.GetString(EasySceneSwitcherKeys.GENERATED_FILE_PATH_REL_TO_ASSETS, null);
        if (string.IsNullOrEmpty(targetFilePathRelToAssets))
        {
            targetFilePathRelToAssets = DEFAULT_GENERATED_FILE_PATH_REL_TO_ASSETS;
        }
        PlayerPrefs.SetString(EasySceneSwitcherKeys.GENERATED_FILE_PATH_REL_TO_ASSETS, targetFilePathRelToAssets);

        if (File.Exists(EasySceneSwitcherUtils.PathToAbs(targetFilePathRelToAssets)))
        {
            Debug.Log($"Deleting scene database \"{targetFilePathRelToAssets}\"");
            AssetDatabase.DeleteAsset(EasySceneSwitcherUtils.PathToProj(targetFilePathRelToAssets));
        }
    }
}
#endif