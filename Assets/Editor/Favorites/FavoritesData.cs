using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class FavoritesData : ScriptableObject
{
    private const string path = "Assets/Editor/Favorites/FavoritesData.asset";

    public IReadOnlyList<InfoUnit> UnitList => unitList;
    
    [SerializeField] private List<InfoUnit> unitList = new List<InfoUnit>();

    private string[] _keys;
    public string[] Keys
    {
        get
        {
            if (_keys == null || _keys.Length != unitList.Count)
            {
                _keys = new string[unitList.Count];
            }
            for (var i = 0; i < unitList.Count; i++)
            {
                _keys[i] = unitList[i].key;
            }
            return _keys;
        }
    }
    
    public InfoUnit GetList(int index)
    {
        return index < unitList.Count ? unitList[index] : null;
    }
    
    public void AddList(string key)
    {
        unitList.Add(new InfoUnit(key));
        Save();
    }
    
    public void DelList(InfoUnit info)
    {
        unitList.Remove(info);
        Save();
    }

    public void MoveIndex(int index, int targetIndex)
    {
        (unitList[targetIndex], unitList[index]) = (unitList[index], unitList[targetIndex]);
        Save();
    }
    
    public void ChangeInfoKey(InfoUnit info, string key)
    {
        info.key = key;
        Save();
    }

    public void Save()
    {
        if (_Instance != null)
        {
            EditorUtility.SetDirty(_Instance);
        }
        //AssetDatabase.SaveAssets();
        AssetDatabase.SaveAssetIfDirty(AssetDatabase.GUIDFromAssetPath(path));
    }

    static void CreateData()
    {
        var asset = CreateInstance<FavoritesData>();
        AssetDatabase.CreateAsset(CreateInstance<FavoritesData>(), path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Selection.activeObject = asset;
    }

    [Serializable]
    public class InfoUnit
    {
        public string key;
        public List<Object> objs;

        private ReorderableList _rList;
        public ReorderableList rList
        {
            get
            {
                if (_rList == null)
                {
                    _rList = new ReorderableList(objs, typeof(UnityEngine.Object), true, true, true, true);

                    _rList.headerHeight = 0;
                    _rList.elementHeight = 30;
                    _rList.drawElementCallback = (Rect rect, int index, bool selected, bool focused) =>
                    {
                        rect.height = EditorGUIUtility.singleLineHeight + 10;
                        objs[index] = EditorGUI.ObjectField(rect, objs[index], typeof(UnityEngine.Object), true);
                    };
                    _rList.onAddCallback = list =>
                    {
                        ReorderableList.defaultBehaviours.DoAddButton(list);
                        Instance.Save();
                    };
                    _rList.onRemoveCallback = list =>
                    {
                        ReorderableList.defaultBehaviours.DoRemoveButton(list);
                        Instance.Save();
                    };
                    _rList.onReorderCallback = list =>
                    {
                        Instance.Save();
                    };
                }
                return _rList;
            }
        }

        public InfoUnit(){}
        public InfoUnit(string _key)
        {
            key = _key;
            objs = new List<Object> { };
        }
    }
    
    public static FavoritesData _Instance;
    public static FavoritesData Instance
    {
        get
        {
            if(_Instance == null)
            {
                if(!File.Exists(path))
                {
                    CreateData();
                }
                _Instance = AssetDatabase.LoadAssetAtPath<FavoritesData>(path);
            }
            return _Instance;
        }
    }
}