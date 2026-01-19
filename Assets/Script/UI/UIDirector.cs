using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
//UI全体の管理を行うディレクタークラス
public class UIDirector : MonoBehaviour
{
    public static UIDirector instance;
    void Awake() {
        if (instance != null && instance != this)
        {
            Debug.LogWarning("重複したUIDirectorを検出したので破棄しました");
            Destroy(this);
            return;
        }
        instance = this;
        Initialize();
    }
    [Header("参照設定(オブジェクト系)")]
    public GoalProcess goalProcess;
    public UpdateText updateText;

    //[Header("参照設定(Startで取得するのでD&D不要)")]
    
    void Initialize()
    {
    }
}