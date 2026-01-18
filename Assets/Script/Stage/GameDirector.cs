using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using System; // InputActionを使うため
using TMPro;
using UnityEngine.TextCore.Text;
//ステージルートにアタッチ
//ゴールした時にカメラを動かし、時間を止める
public class GameDirector : MonoBehaviour
{
    [Header("参照設定")]
    [SerializeField] private GameObject player;
    [SerializeField] private Camera mainCamera;          // メインカメラ
    private PlayerInput playerInput;    // プレイヤーのInputAction
    public GameObject goal;      // ゴールのオブジェクト
    private PlayerController playerController;
    public TextMeshProUGUI countText;

    private Transform currentStageTransform;
    private CameraController cameraController;
    private int itemNum;
    public int currentStage = 1;
    void Start()
    {
        playerInput = player.GetComponent<PlayerInput>();
        playerController = player.GetComponent<PlayerController>();
        cameraController = mainCamera.GetComponent<CameraController>();
        currentStageTransform = transform.Find($"Stage{currentStage}");//ステージ
        goal = currentStageTransform.Find("Goal").gameObject;//ゴール
        itemNum = currentStageTransform.Find("PickUpParent").childCount;//アイテムの総数
        goal.SetActive(false);
        StartTime();
    }
    public void SetItemCount(int count)
    {
        countText.text = "Count: " + count.ToString() + "/" + itemNum;
        if (count == itemNum)
        {
            cameraController.LookAtGoal();
        }
    }
    public void StopTime()
    {
        playerInput.DeactivateInput(); 
        playerController.ZeroVelocity();
        StageRoot.isTimePaused = true;
    }
    public void StartTime()
    {
        playerInput.ActivateInput();
        cameraController.isCameraActive = true;
        StageRoot.isTimePaused = false;
    }

    [Header("シネマティック演出")]
    [SerializeField] private RectTransform topBar;
    [SerializeField] private RectTransform bottomBar;
    [SerializeField] private float barTargetHeight = 100f; // 黒枠の最終的な高さ

    [SerializeField] private RectTransform resultPanel;
    private float _blackTime = 0.5f;
    public float goalDisplayTime = 0.3f;
    public void GoalProcess()
    {
        topBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
        bottomBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
        StartCoroutine(GoalProcessRoutine());
    }
    IEnumerator GoalProcessRoutine()
    {
        StopTime();
        // 黒枠を出す演出
        float elapsed = 0f;
        while (elapsed < _blackTime) // 0.5秒かけて出す
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _blackTime;
            float currentHeight = Mathf.Lerp(0, barTargetHeight, Mathf.SmoothStep(0, 1, t));

            // 上下の高さを更新
            topBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, currentHeight);
            bottomBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, currentHeight);
            
            yield return null;
        }
        //カメラを少しズラす
        cameraController.clear();

        // 1. 開始位置（画面外右）と目標位置（中央 0）を設定
        Vector2 startPos = new Vector2(2000f, 0f);
        Vector2 targetPos = new Vector2(1000f, 0f);

        // パネルを確実に開始位置へ
        resultPanel.anchoredPosition = startPos;

        // 2. スライドのアニメーション
        elapsed = 0f;
        while (elapsed < goalDisplayTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / goalDisplayTime;
            
            // 強めに緩急（SmoothStep）をつけると「シュッ」と入って止まる感じになります
            resultPanel.anchoredPosition = Vector2.Lerp(startPos, targetPos, Mathf.SmoothStep(0, 1, t));
            
            yield return null;
        }

        resultPanel.anchoredPosition = targetPos; // 最後にピタッと合わせる
    }
}