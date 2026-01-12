using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using System; // InputActionを使うため
//ステージルートにアタッチ
public class GameDirector : MonoBehaviour
{
    [Header("参照設定")]
    [SerializeField] private GameObject player;
    [SerializeField] private Camera mainCamera;          // メインカメラ
    private PlayerInput playerInput;    // プレイヤーのInputAction
    private GameObject goal;      // ゴールのオブジェクト
    private PlayerController playerController;

    [Header("演出設定")]
    [SerializeField] private float cameraTransitionTime = 1.0f; // カメラが向くまでの時間
    [SerializeField] private float targetDistance = 5.0f;
    private Transform currentStage;
    private CameraController cameraController;
    private int itemNum;

    void Start()
    {
        playerInput = player.GetComponent<PlayerInput>();
        playerController = player.GetComponent<PlayerController>();
        cameraController = mainCamera.GetComponent<CameraController>();
        currentStage = transform.Find($"Stage{playerController.currentStage}");//ステージ
        goal = currentStage.Find("Goal").gameObject;//ゴール
        itemNum = currentStage.Find("PickUpParent").childCount;//アイテムの総数
        goal.SetActive(false);


    }
    public void SetItemCount(int count)
    {
        if (count == itemNum)
        {
            cameraController.isCameraActive = false;
            
            StartCoroutine(GoalCutsceneRoutine());
        }
    }

    IEnumerator GoalCutsceneRoutine()
    {
        // 1. プレイヤーの操作を無効にする・足場の時間を止める
        playerInput.DeactivateInput(); 
        playerController.ZeroVelocity();
        ChangeGroundType.isTimePaused = true;


        // 1. 開始時の位置と回転を記録
        Vector3 startPosition = mainCamera.transform.position;
        Quaternion startRotation = mainCamera.transform.rotation;

        // 2. 終了時の位置と回転を計算
        Vector3 goalPosition = goal.transform.position;
        
        // ゴールからカメラへの方向ベクトル（正規化して長さを1にする）
        Vector3 dirToCamera = startPosition - goalPosition;
        dirToCamera.y = 0;
        dirToCamera = dirToCamera.normalized;
        
        // 終了座標：ゴールの位置 + (カメラへの方向 * 指定距離)
        Vector3 endPosition = goalPosition + (dirToCamera * targetDistance);
        endPosition.y = goalPosition.y + targetDistance*Mathf.Tan(10);
        
        // 終了回転：その位置からゴールを見る回転
        Quaternion endRotation = Quaternion.LookRotation(goalPosition - endPosition);

        float elapsed = 0f;
        while (elapsed < cameraTransitionTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / cameraTransitionTime;
            float smoothT = Mathf.SmoothStep(0, 1, t);
            // SmoothStepを使って滑らかにカメラを向ける
            mainCamera.transform.position = Vector3.Lerp(startPosition, endPosition, smoothT);
            mainCamera.transform.rotation = Quaternion.Slerp(startRotation, endRotation, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }

        // 3. ゴール出現演出を開始（SetActiveにするだけで先ほどのスクリプトが動く）
        goal.SetActive(true);

        // 4. 出現演出が終わるまで待機（先ほど作ったduration分待つ）
        yield return new WaitForSeconds(2.0f);

        // 5. カメラをプレイヤーの方向に戻す
        elapsed = 0f;
        // プレイヤーを追従する際の基本の向き（固定ならその角度を計算）
        // ここでは単純に元に戻すと仮定
        while (elapsed < cameraTransitionTime)
        {
            startPosition = player.transform.position + cameraController.offset;
            elapsed += Time.deltaTime;
            float t = elapsed / cameraTransitionTime;
            float smoothT = Mathf.SmoothStep(0, 1, t);
            mainCamera.transform.position = Vector3.Lerp(endPosition, startPosition, smoothT);
            mainCamera.transform.rotation = Quaternion.Slerp(endRotation, startRotation, smoothT);
            yield return null;
        }

        // 6. プレイヤーの操作を有効に戻す
        playerInput.ActivateInput();
        cameraController.isCameraActive = true;
        ChangeGroundType.isTimePaused = false;
    }
}