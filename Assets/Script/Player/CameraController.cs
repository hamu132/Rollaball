using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//カメラオブジェクトにアタッチ
public class CameraController : MonoBehaviour
{
    public GameObject player;
    private Vector3 offset;
    public bool isCameraActive;

    [Header("演出設定")]
    [SerializeField] private float cameraTransitionTime = 1.0f; // カメラが向くまでの時間
    [SerializeField] private float targetDistance = 5.0f;
    // Start is called before the first frame update
    void Start()
    {
        offset = new Vector3(0, 10, -10);
        isCameraActive = true;
    }


    // Update is called once per frame
    void LateUpdate()
    {
        if(player != null && isCameraActive)
        {
            transform.position = player.transform.position + offset;
        }
    }

    public void LookAtGoal()
    {
        isCameraActive = false;
        StartCoroutine(GoalCutsceneRoutine());
    }
    IEnumerator GoalCutsceneRoutine()
    {
        // 1. プレイヤーの操作を無効にする・足場の時間を止める
        GameDirector.instance.StopTime();
        

        // 1. 開始時の位置と回転を記録
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;

        // 2. 終了時の位置と回転を計算
        Vector3 goalPosition = GameDirector.instance.goal.transform.position;
        
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
            transform.position = Vector3.Lerp(startPosition, endPosition, smoothT);
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }

        // 3. ゴール出現演出を開始（SetActiveにするだけで先ほどのスクリプトが動く）
        GameDirector.instance.goal.SetActive(true);

        // 4. 出現演出が終わるまで待機（先ほど作ったduration分待つ）
        yield return new WaitForSeconds(2.0f);

        // 5. カメラをプレイヤーの方向に戻す
        elapsed = 0f;
        // プレイヤーを追従する際の基本の向き（固定ならその角度を計算）
        // ここでは単純に元に戻すと仮定
        while (elapsed < cameraTransitionTime)
        {
            startPosition = player.transform.position + offset;
            elapsed += Time.deltaTime;
            float t = elapsed / cameraTransitionTime;
            float smoothT = Mathf.SmoothStep(0, 1, t);
            transform.position = Vector3.Lerp(endPosition, startPosition, smoothT);
            transform.rotation = Quaternion.Slerp(endRotation, startRotation, smoothT);
            yield return null;
        }

        // 6. プレイヤーの操作を有効に戻す
        GameDirector.instance.StartTime();
    }
    public void clear()
    {
        isCameraActive = false;
        StartCoroutine(ClearCutsceneRoutine());
    }
    IEnumerator ClearCutsceneRoutine()
    {
        float elapsed = 0f;
        float offset = 4f;
        float k = 0.3f;
        Vector3 startPosition = transform.position;
        Vector3 goalPosition = GameDirector.instance.goal.transform.position;
        Vector3 direction = goalPosition - startPosition;
        Vector3 endPosition = startPosition + k*direction;
        endPosition.x += offset;
        while (elapsed < GameDirector.instance.goalDisplayTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / GameDirector.instance.goalDisplayTime;
            transform.position = Vector3.Lerp(startPosition,endPosition,t);
            yield return null;
        }
    }

}
