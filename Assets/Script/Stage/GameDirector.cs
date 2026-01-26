using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using System; // InputActionを使うため
using TMPro;
using UnityEngine.TextCore.Text;
using UnityEngine.Splines;
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
    public SplineController splineController;
    [Header("数値変数設定")]
    public float goalDisplayTime = 0.3f;
    public int currentStage = 1;
    [Header("初期化で取得するのでD&D不要(途中変更なし)")]
    public PlayerInput playerInput;    // プレイヤーのInputAction
    public SplineAnimate splineAnimate;
    public PlayerController playerController;
    public CameraController cameraController;
    public Transform currentStageTransform;
    public GameObject goal;      // ゴールのオブジェクト
    public int itemNum;
    [Header("ゲーム状態変数(スクリプトでその都度変更)")]
    public bool isClear = false;
    public bool isTimePaused = false;//時間を進めるかどうか
    public float currentItemCount = 0;
    void Initialize()
    {
        playerInput = player.GetComponent<PlayerInput>();
        splineAnimate = player.GetComponent<SplineAnimate>();
        playerController = player.GetComponent<PlayerController>();
        cameraController = mainCamera.GetComponent<CameraController>();
        currentStageTransform = stageRoot.transform.Find($"Stage{currentStage}");//ステージ
        goal = currentStageTransform.Find("Goal").gameObject;//ゴール
        itemNum = currentStageTransform.Find("PickUpParent").childCount;//アイテムの総数
        goal.SetActive(false);
        StartTime();
        test();
    }

    void test()
    {
        cameraController.LookAtGoal();
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
    //アイテムを獲得した時発動
    public void CheckItemCount()
    {
        currentItemCount++;
        if (currentItemCount == itemNum) cameraController.LookAtGoal();
    }
    //ゴールに触れた時発動
    public void SetSpline()
    {
        Vector3 playerPosition = player.transform.position;
        Vector3 goalPosition = goal.transform.position;
        goalPosition.y += 2f;
        Vector3 middlePosition = playerPosition;
        middlePosition.y += 3f;
        // splineController.SetKnotWorldPosition(0,playerPosition);
        // splineController.SetKnotWorldPosition(1,middlePosition);
        // splineController.SetKnotWorldPosition(2,goalPosition);
        splineController.SetKnotWorldPosition(0,playerPosition);
        splineController.SetKnotWorldPosition(1,middlePosition);
        splineController.SetKnotWorldPosition(2,goalPosition);
    }
}