using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

[InitializeOnLoad]
public class MiddleClickUIEditor
{
    private static EventSystem _eventSystem;
    private static Vector2 _lastMouseDownPos;
    private static List<GameObject> _lastClickGoList;
    private static int _lastFrameCount;

    static MiddleClickUIEditor()
    {
        EditorApplication.update -= Update;
        EditorApplication.update += Update;
    }

    private static void Update()
    {
        if (EventSystem.current == null) return;
        if (_lastFrameCount == Time.frameCount) return;
        
        _lastFrameCount = Time.frameCount;

        if (Input.GetMouseButtonUp(2))
        {
            OnClick();
        }
    }

    private static void OnClick()
    {
        _lastClickGoList ??= new List<GameObject>();
        if (Vector2.Distance(_lastMouseDownPos, Input.mousePosition) > 5)
        {
            _lastClickGoList.Clear();
            _lastMouseDownPos = Input.mousePosition;
        }

        PointerEventData eventData = new PointerEventData(_eventSystem)
        {
            pressPosition = Input.mousePosition,
            position = Input.mousePosition
        };

        foreach (var gr in Resources.FindObjectsOfTypeAll<GraphicRaycaster>())
        {
            var results = new List<RaycastResult>();
            gr.Raycast(eventData, results);
            foreach (var result in results)
            {
                if (
                    result.gameObject.GetComponent<Button>() ||
                    result.gameObject.GetComponent<Toggle>() ||
                    result.gameObject.GetComponent<Text>() ||
                    result.gameObject.GetComponent<TextMeshProUGUI>() ||
                    result.gameObject.GetComponent<InputField>() ||
                    result.gameObject.GetComponent<Image>()
                )
                {
                    var selectGo = result.gameObject;
                    if (selectGo != null && !_lastClickGoList.Contains(selectGo))
                    {
                        Selection.activeObject = selectGo;
                        _lastClickGoList.Add(selectGo);
                        Debug.Log(selectGo.name, selectGo.gameObject);
                        return;
                    }
                }
            }
        }

        _lastClickGoList.Clear();
    }
}
