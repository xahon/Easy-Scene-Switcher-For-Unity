#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;


public class EasySceneSwitcherSettings : EditorWindow
{
    private string PathToSaveFile;
    private bool StripUnityExtension;
    private bool GenerateSceneListInline;

    private void OnEnable()
    {
        PathToSaveFile = PlayerPrefs.GetString(EasySceneSwitcherKeys.GENERATED_FILE_PATH_REL_TO_ASSETS, EasySceneSwitcher.DEFAULT_GENERATED_FILE_PATH_REL_TO_ASSETS);
        StripUnityExtension = PlayerPrefs.GetInt(EasySceneSwitcherKeys.STRIP_UNITY_EXTENSION, 1) == 1;
        GenerateSceneListInline = PlayerPrefs.GetInt(EasySceneSwitcherKeys.GENERATE_SCENE_LIST_INLINE, 1) == 1;
    }

    void OnGUI()
    {
        PathToSaveFile = EditorGUILayout.TextField(new GUIContent("Generated file: ", "Path relative to Assets folder"), PathToSaveFile);
        StripUnityExtension = EditorGUILayout.Toggle(new GUIContent("Strip .unity extension", "Strip .unity extension from scene names"), StripUnityExtension);
        GenerateSceneListInline = EditorGUILayout.Toggle(new GUIContent("Generate scene list inline", "Generate scene list inside the menu bar"), GenerateSceneListInline);

        if (GUILayout.Button("Save"))
        {
            if (EasySceneSwitcherUtils.ValidatePathRelToAssets(PathToSaveFile))
            {
                EasySceneSwitcher.DeleteDatabase();

                PlayerPrefs.SetString(EasySceneSwitcherKeys.GENERATED_FILE_PATH_REL_TO_ASSETS, PathToSaveFile);
                PlayerPrefs.SetInt(EasySceneSwitcherKeys.STRIP_UNITY_EXTENSION, StripUnityExtension ? 1 : 0);
                PlayerPrefs.SetInt(EasySceneSwitcherKeys.GENERATE_SCENE_LIST_INLINE, GenerateSceneListInline ? 1 : 0);
                EasySceneSwitcher.RegenerateDatabase();
            }
            else
            {
                Debug.LogError("EasySceneSwitcher: Invalid path: " + PathToSaveFile);
            }
        }
    }
}
#endif
