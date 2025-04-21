using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Favorites_Editor : EditorWindow
{
    private int popupIndex;
    private FavoritesData data => FavoritesData.Instance;

    private void OnGUI()
    {
        this.Repaint();

        EditorGUILayout.Space();

        popupIndex = Favorites_GroupPopupGUI.Popup(popupIndex, data.Keys);        

        EditorGUILayout.Space();
        EditorGUILayout.Space(); 

        var info = data.GetList(popupIndex);
        info?.rList.DoLayoutList();
    }

    [MenuItem("Tools/Favorites")]
    static void CreateWindow()
    {
        GetWindow(typeof(Favorites_Editor), false, "Favorites");
    }
}