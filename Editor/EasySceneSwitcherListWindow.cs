#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;


public class EasySceneSwitcherListWindow : EditorWindow
{
    private List<string> SceneList;

    private void OnEnable()
    {
        SceneList = EasySceneSwitcher.GetAllScenesInProject();
    }

    private void OnGUI()
    {
        bool stripUnityExt = PlayerPrefs.GetInt(EasySceneSwitcherKeys.STRIP_UNITY_EXTENSION, 1) == 1;

        foreach (string sceneNameIter in SceneList)
        {
            string scenePath = sceneNameIter;
            string sceneName = Path.GetFileName(scenePath);

            if (stripUnityExt)
            {
                sceneName = Path.GetFileNameWithoutExtension(scenePath);
            }

            if (GUILayout.Button(sceneName))
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                }
            }
        }
    }
}
#endif
