using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using System; // InputActionを使うため
using TMPro;
using UnityEngine.TextCore.Text;
//ステージルートにアタッチ
//重要変数格納場所
public class GameDirector : MonoBehaviour
{
    public static GameDirector instance;
    void Awake() {
        if (instance != null && instance != this)
        {
            Debug.LogWarning("重複したGameDirectorを検出したので破棄しました");
            Destroy(this);
            return;
        }
        instance = this;
        Initialize();
    }

    [Header("参照設定(オブジェクト系)")]
    public GameObject player;//プレイヤー
    public Camera mainCamera;          // メインカメラ
    public GameObject stageRoot;      // ステージのルートオブジェクト
    [Header("数値変数設定")]
    public float goalDisplayTime = 0.3f;
    public int currentStage = 1;
    [Header("参照設定(Startで取得するのでD&D不要)")]
    public PlayerInput playerInput;    // プレイヤーのInputAction
    public PlayerController playerController;
    public CameraController cameraController;
    public Transform currentStageTransform;
    public GameObject goal;      // ゴールのオブジェクト
    public int itemNum;
    [Header("ゲーム状態変数(スクリプトでその都度変更)")]
    public bool isClear = false;
    public bool isTimePaused = false;//時間を進めるかどうか
    void Initialize()
    {
        playerInput = player.GetComponent<PlayerInput>();
        playerController = player.GetComponent<PlayerController>();
        cameraController = mainCamera.GetComponent<CameraController>();
        currentStageTransform = stageRoot.transform.Find($"Stage{currentStage}");//ステージ
        goal = currentStageTransform.Find("Goal").gameObject;//ゴール
        itemNum = currentStageTransform.Find("PickUpParent").childCount;//アイテムの総数
        goal.SetActive(false);
        StartTime();
    }

    public void StopTime()
    {
        playerInput.DeactivateInput(); 
        playerController.ZeroVelocity();
        isTimePaused = true;
    }
    public void StartTime()
    {
        playerInput.ActivateInput();
        cameraController.isCameraActive = true;
        isTimePaused = false;
    }
}