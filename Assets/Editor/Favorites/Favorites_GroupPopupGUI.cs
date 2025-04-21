using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using InfoUnit = FavoritesData.InfoUnit;

public static class Favorites_GroupPopupGUI
{
    public static int Popup(int selectIndex, string[] displayedOptions, params GUILayoutOption[] options)
    {
        int contrelId = GUIUtility.GetControlID(FocusType.Passive);

        var display = "Null";
        if (selectIndex >= 0 && selectIndex < displayedOptions.Length)
            display = displayedOptions[selectIndex];

        if (GUILayout.Button(display,EditorStyles.popup, options))
        {
            CustomPopup popup = new CustomPopup();
            popup.selectInfo = selectIndex;
            popup.popupInfo = new CustomPopupInfo(contrelId, selectIndex);
            CustomPopupInfo.instance = popup.popupInfo;
            PopupWindow.Show(CustomPopupTempStyle.Get(contrelId), popup);
        }

        if (Event.current.type == EventType.Repaint)
        {
            CustomPopupTempStyle.Set(contrelId, GUILayoutUtility.GetLastRect());
        }
        return CustomPopupInfo.Get(contrelId, selectIndex);
    }
}

public class CustomPopup : PopupWindowContent
{
    public int selectInfo;
    public CustomPopupInfo popupInfo;

    private InfoUnit editInfo;
    private string editStr;
    private bool isAdding;
    private string addStr;
    private Vector2 scrollPosition;
    private FavoritesData data => FavoritesData.Instance;
    
    public override void OnGUI(Rect rect)
    {
        GUILayout.Space(3);
        editorWindow.minSize = new Vector2(170, 270);
        
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        for (int i = 0; i < data.UnitList.Count; i++)
        {
            var option = data.UnitList[i];
            using (new GUILayout.HorizontalScope())
            {
                if (editInfo == option)
                {
                    if (GUILayout.Button("↑",GUILayout.Width(20)))
                    {
                        var targetIndex = Mathf.Clamp(i - 1, 0, data.UnitList.Count - 1);
                        data.MoveIndex(i, targetIndex);
                    }
                    if (GUILayout.Button("↓",GUILayout.Width(20)))
                    {
                        var targetIndex = Mathf.Clamp(i + 1, 0, data.UnitList.Count - 1);
                        data.MoveIndex(i, targetIndex);
                    }
                    editStr = GUILayout.TextField(editStr);
                    if (GUILayout.Button("✓",GUILayout.Width(20)))
                    {
                        editInfo = null;
                        data.ChangeInfoKey(option, editStr);
                    }
                }
                else
                {
                    GUIStyle system = new GUIStyle(selectInfo == i ? EditorStyles.miniButtonMid : EditorStyles.toolbarButton);

                    if (GUILayout.Button(option.key, system, GUILayout.Height(20)))
                    {
                        if(Event.current.button == 1)
                        {
                            var genericMenu = new GenericMenu();
                            genericMenu.AddItem(new GUIContent($"Rename"), false, () =>
                            {
                                editInfo = option;
                                editStr = option.key;
                            });
                            genericMenu.AddItem(new GUIContent($"Delect {option.key}"), false, () =>
                            {
                                if (EditorUtility.DisplayDialog("Are you sure", $"Confirm delete {option.key}", "Confirm", "Cancel"))
                                {
                                    data.DelList(option);
                                }
                            });
                            genericMenu.ShowAsContext();
                        }
                        else
                        {
                            selectInfo = i;
                            popupInfo.Set(i);
                            editorWindow.Close();
                        }
                    }
                }
            }
        }
        EditorGUILayout.EndScrollView();

        if (isAdding)
        {
            using (new GUILayout.HorizontalScope())
            {
                addStr = GUILayout.TextField(addStr);
                if (GUILayout.Button("✓",GUILayout.Width(20)))
                {
                    data.AddList(addStr);
                    isAdding = false;
                    addStr = null;
                }
                if (GUILayout.Button("x",GUILayout.Width(20)))
                {
                    isAdding = false;
                }
            }
        }
        else
        {
            if (GUILayout.Button("AddGroup"))
            {
                isAdding = true;
            }
        }
    }
}


public class CustomPopupTempStyle
{
    private static Dictionary<int, Rect> temp = new Dictionary<int, Rect>();
    public static Rect Get(int contrelId)
    {
        if (!temp.TryGetValue(contrelId, out var t))
        {
            return Rect.zero;
        }
        else
        {
            temp.Remove(contrelId);
            return t;
        }
    }
    public static void Set(int contrelId,Rect rect)
    {
        temp[contrelId] = rect;
    }
}

public class CustomPopupInfo
{
    public int SelectIndex { get; private set; }
    public int contrelId;
    public bool used;
    public static CustomPopupInfo instance;

    public CustomPopupInfo(int contrelId,int selectIndex)
    {
        this.contrelId = contrelId;
        this.SelectIndex = selectIndex;
    }

    public static int Get(int controlID, int selected)
    {
        if (instance == null)
        {
            return selected;
        }

        if (instance.contrelId == controlID && instance.used)
        {
            GUI.changed = selected != instance.SelectIndex;
            selected = instance.SelectIndex;
            instance = null;
        }

        return selected;
    }

    public void Set(int selected)
    {
        SelectIndex = selected;
        used = true;
    }
}