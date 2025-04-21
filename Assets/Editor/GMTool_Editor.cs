/*      ##              #      #     #     ##         ############              #     ##                          #       #               #         #      #           ##         
       ##             # #########    ##     ###       ##        ##             ###    ##                 #        ##########     #  #     ##        ## #   ##           ##    #   
      ####        ######## ## ##     ##  #   #  #     ##        ##             ##    ## #      ############       ##     ##      ##  #    ##     ########  ##     ##############  
     ##  ##         ##  ## ## ##     ##  #########    ############            ##     ## ##          ##            #########      ##  ##   ##              ##       ##   ##   #    
    ##    ###       ##  ########   ########           ##  #   #               #  #  ##   ##         ##            ##     ##      ##  ##   ##       #  #   ######    ## ## ## ##   
   ##       ##      ##  ## ## ##     ##  ##           ##  ##  ##             #  ### #    ###        ##            #########      ##  #    ##       ##  #  # ##        ##### #     
  ##       # ###  ######## ## ##     ##  ##           ############           ##### #      #         ##            ##     ##      ##       ##      ##  ##### ##      ##  ## # ##   
 #  ######### #     ##  ########     ## ###           ##  ##  ##               ##   #######         ##            #########      ##       ##      ##  ## #  ##    ###  ##  ## ##  
                    ##  #  ## #      ### ##           ##  ##  ## #            ##                    ##            ##     ##      ##       ##     #  # # # # ##     # ######## #   
    #       #       ##     ##       ###  ##           #############          #####  #     #         ##            ##     ## #    ##      ###        ###   ###         # ##  #     
    ##########      ##  ########   ####  ##           ## ## #   #            ##     ########        ##         ###############   ##   #  ## #        #     #            ##     #  
    ##     ##       ####   ##      # ##  ##           ## ##  # ###                # ##   ##         ##              #  ##        ## ##  ##  ##      ####  ###     ############### 
    ##     ##     ####     ##        ## ##           ##  ##  ###                ##  ##   ##         ##             ###  ##       ####   ##  ##     ## ##### ##          ##        
    ##     ##      #       ##  #     ## ##           ##  ## # ###            ####   ##   ##         ##    #       ###    ###    ####   ##    ##    #   # #   ##         ##        
    #########          ##########   ### #            #  ####   ####           #     #######   ##############     ##       ###    #    ##     ##   #     ##   ###        ##        
    #      #                         # #            #    #       #                  #    #                      #          #        ##       #   #     #      #         ##        */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class GMTool_Editor : EditorWindow
{
    private int _popupIndex;
    private string[] _otherOptions;
    private Dictionary<string, Action> _outherOptionsDic = new Dictionary<string, Action> {
        {"None", ()=> { } },
        {"功能1", () => { window.功能1(); } },
        {"功能2/功能2-1", () => { window.功能2_1(); } },
        };

    private bool _commonFoldout;
    private Vector2 _svPosStep1;
    private void OnGUI()
    {
        this.Repaint();

        _svPosStep1 = GUILayout.BeginScrollView(_svPosStep1);

        #region GM
        EditorGUILayout.Space();
        Common();
        #endregion

        #region Info
        EditorGUILayout.Space();
        Info();
        #endregion

        #region Misc
        EditorGUILayout.Space();
        Misc();
        #endregion

        #region Other
        EditorGUILayout.Space();
        OtherOptions();
        #endregion

        GUILayout.EndScrollView();
    }

    #region Common
    private string _gmCommand;
    private int _gmItemParam1;
    private int _gmItemParam2;
    private void Common()
    {
        _commonFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_commonFoldout, "GM");
        EditorGUILayout.EndFoldoutHeaderGroup();
        if (_commonFoldout)
        {
            GUILayout.BeginHorizontal();
            _gmCommand = EditorGUILayout.TextField(_gmCommand);
            if (GUILayout.Button("发送GM命令"))
            {
                SendGM(_gmCommand);
                GUIUtility.keyboardControl = 0;
            }
            GUILayout.EndHorizontal();

            #region 获取物品
            GUILayout.BeginHorizontal();
            GUILayout.Label("物品Id:");
            _gmItemParam1 = EditorGUILayout.IntField(_gmItemParam1);
            GUILayout.Label("物品数量:");
            _gmItemParam2 = EditorGUILayout.IntField(_gmItemParam2);
            if (GUILayout.Button("获取"))
            {
                SendGM("item," + _gmItemParam1 + "," + _gmItemParam2);
            }
            GUILayout.EndHorizontal();
            #endregion
        }
    }
    private void SendGM(string str)
    {
    }
    #endregion

    #region Info
    private bool _dateTimeFoldout;
    void Info()
    {
        _dateTimeFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_dateTimeFoldout, "Info");
        EditorGUILayout.EndFoldoutHeaderGroup();
        if (_dateTimeFoldout)
        {
            GUILayout.Label("服务器时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            GUILayout.Label($"玩家注册第 {999} 天");
            GUILayout.Label($"跨服状态: {true}");
        }
    }
    #endregion
    
    #region Misc
    private bool _miscFoldout;
    void Misc()
    {
        _miscFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_miscFoldout, "Misc");
        EditorGUILayout.EndFoldoutHeaderGroup();
        if (_miscFoldout)
        {
            GUILayout.BeginHorizontal();
            Time.timeScale = EditorGUILayout.Slider("时间缩放", Time.timeScale, 0, 20);
            if (GUILayout.Button("重置"))
            {
                Time.timeScale = 1;
            }
            GUILayout.EndHorizontal();
        }
    }
    #endregion

    #region OtherOptions
    private Vector2 _svPosOther;
    void OtherOptions()
    {
        _svPosOther = GUILayout.BeginScrollView(_svPosOther);
        
        _popupIndex = EditorGUILayout.Popup("其他功能：", _popupIndex, _otherOptions);
        EditorGUILayout.Space();
        _outherOptionsDic[_otherOptions[_popupIndex]]?.Invoke();
        EditorGUILayout.Space();
        
        GUILayout.EndScrollView();
    }
    #endregion

    void 功能1()
    {
        
    }
    
    void 功能2_1()
    {
        
    }
    
    void 功能2()
    {
        
    }

    public static string[] CMD(string cmdStr)
    {
        var pStartInfo = new ProcessStartInfo("cmd.exe");
        pStartInfo.CreateNoWindow = false;
        pStartInfo.UseShellExecute = false;
        pStartInfo.RedirectStandardError = true;
        pStartInfo.RedirectStandardInput = true;
        pStartInfo.RedirectStandardOutput = true;
        pStartInfo.StandardErrorEncoding = Encoding.UTF8;
        pStartInfo.StandardOutputEncoding = Encoding.UTF8;

        var p = Process.Start(pStartInfo);
        p.StandardInput.WriteLine(cmdStr);
        p.StandardInput.WriteLine("exit");

        var output = p.StandardOutput.ReadToEnd();
        var error = p.StandardError.ReadToEnd();
        p.WaitForExit();
        p.Close();
        if (!string.IsNullOrEmpty(error))
        {
            Debug.Log(" Output: " + output);
            Debug.LogError(" Error: " + error);
        }
        return new string[] { output, error };
    }

    private void OnLostFocus()
    {
        GUIUtility.keyboardControl = 0;
    }

    private void OnEnable()
    {
        Init();
    }

    public void Init()
    {
        _otherOptions = _outherOptionsDic.Keys.ToArray();
    }

    [MenuItem("Tools/GMTool_Editor &`")]
    static void OpenWindow()
    {
        if (window != null)
            window.Focus();
        else
            window.Show();

        window.minSize = new Vector2(200, 200);
        
        window.Init();
    }

    static GMTool_Editor _window;
    public static GMTool_Editor window
    {
        get
        {
            if (_window == null)
                _window = (GMTool_Editor)EditorWindow.GetWindow(typeof(GMTool_Editor), false, "GMTool");
            return _window;
        }
    }
}
