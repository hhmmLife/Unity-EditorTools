using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;

public class MiscTools
{
    [MenuItem("GameObject/挂特么哪了!", false, -99)]
    private static void FindScriptsWithSelectedObject()
    {
        GameObject selectedObject = Selection.activeGameObject;
        Component[] allComponent = null;
        
        if (PrefabStageUtility.GetPrefabStage(selectedObject) != null)
        {
            GameObject prefabRoot = PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot;
            allComponent = prefabRoot.GetComponentsInChildren<Component>();
        }
        else
        {
            allComponent = UnityEngine.Object.FindObjectsOfType<Component>();
        }

        var list = new HashSet<int> { selectedObject.GetInstanceID() };
        foreach (Component comp in selectedObject.GetComponents<Component>())
        {
            list.Add(comp.GetInstanceID());
        }

        foreach (Component com in allComponent)
        {
            if (com != null && com != selectedObject)
            {
                SerializedObject so = new SerializedObject(com);
                SerializedProperty sp = so.GetIterator();
                while (sp.NextVisible(true))
                {
                    if (sp.propertyType == SerializedPropertyType.ObjectReference)
                    {
                        if(list.Contains(sp.objectReferenceInstanceIDValue))
                        {
                            Selection.activeObject = com;
                            EditorGUIUtility.PingObject(com.gameObject);
                            GUIUtility.systemCopyBuffer = sp.name;
                            Debug.LogError($"{com.gameObject.name}       {sp.name}", com.gameObject);
                        }
                    }
                }
            }
        }
    }

}

