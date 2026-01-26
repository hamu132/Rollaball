using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Splines;
using System; // 必須

//黒いシネマティック演出・ゴール用UI演出
//GoalCanvasオブジェクトにアタッチ
public class GoalProcess : MonoBehaviour
{
    [Header("シネマティック演出")]
    [SerializeField] private RectTransform topBar;
    [SerializeField] private RectTransform bottomBar;
    [SerializeField] private float barTargetHeight = 100f; // 黒枠の最終的な高さ
    [SerializeField] private RectTransform resultPanel;
    [Header("ベジェ曲線")]
    [SerializeField] private AnimationCurve landingCurve;

    private GameObject player;
    private GameObject goal;
    private float _moveTime = 0.8f;
    private float _blackTime = 0.5f;

    //ゴールに触れた瞬間に発動
    public void Goal()
    {
        topBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
        bottomBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
        StartCoroutine(GoalProcessRoutine());
        StartCoroutine(MoveWithCurveRoutine());
    }
    void Start()
    {
        player = GameDirector.instance.player;
        goal = GameDirector.instance.goal;
    }
    IEnumerator GoalProcessRoutine()
    {
        GameDirector.instance.StopTime();
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
        GameDirector.instance.cameraController.clear();

        // 1. 開始位置（画面外右）と目標位置（中央 0）を設定
        Vector2 startPos = new Vector2(2000f, 0f);
        Vector2 targetPos = new Vector2(1000f, 0f);

        // パネルを確実に開始位置へ
        resultPanel.anchoredPosition = startPos;

        // 2. スライドのアニメーション
        elapsed = 0f;
        while (elapsed < GameDirector.instance.goalDisplayTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / GameDirector.instance.goalDisplayTime;
            
            // 強めに緩急（SmoothStep）をつけると「シュッ」と入って止まる感じになります
            resultPanel.anchoredPosition = Vector2.Lerp(startPos, targetPos, Mathf.SmoothStep(0, 1, t));
            
            yield return null;
        }

        resultPanel.anchoredPosition = targetPos; // 最後にピタッと合わせる
    }
    private IEnumerator MoveWithCurveRoutine()
    {
        GameDirector.instance.playerController.OffGravity();
        float elapsed = 0f;

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        elapsed = 0f;


        //[回転の変数]
        Vector3 direction = goal.transform.position - player.transform.position;
        direction.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        // 1.ゴールの方向を向く
        while (Quaternion.Angle(player.transform.rotation, targetRotation)<0.1f)
        {
            float rotationSpeed = 10f;
            Quaternion rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            GameDirector.instance.playerController.SetRotation(rotation);
            yield return null;
        }


        float apexT = GameDirector.instance.splineController.CalculateApexT();
        //縮む→戻ると同時にジャンプ→頂上で一回転→到着
        //カメラ：到着する時にさらにアップ
        while (elapsed < _moveTime)
        {
            elapsed += Time.deltaTime;
            float rawProgress = Mathf.Clamp01(elapsed / _moveTime); // 0〜1の線形な時間
            float easedProgress = landingCurve.Evaluate(rawProgress);// 緩急がついた進捗

            // 頂点（0.5）が apexT に来るように補正してスプラインに渡す
            // 簡単な比率計算で、easedProgressが0.5の時に実世界のapexTになるように調整
            float finalT;
            if (easedProgress <= 0.5f) {
                finalT = Mathf.Lerp(0f, apexT, easedProgress * 2f);
            } else {
                finalT = Mathf.Lerp(apexT, 1f, (easedProgress - 0.5f) * 2f);
            }
            GameDirector.instance.splineAnimate.NormalizedTime = finalT;

            // 2. ★空中回転の計算
            float flipAngle = 0f;
            float rotationStartT = 0.2f;
            float rotationEndT = 0.8f;
            float flipTotalAngle = 360f;
            if (rawProgress >= rotationStartT && rawProgress <= rotationEndT)
            {
                // 回転区間内での進捗（0〜1）を計算
                float flipT = (rawProgress - rotationStartT) / (rotationEndT - rotationStartT);
                // なだらかに回し始めるなら、ここにも SmoothStep を使うと綺麗です
                float smoothedFlipT = Mathf.SmoothStep(0, 1, flipT);
                flipAngle = smoothedFlipT * flipTotalAngle;
            }

            // 3. ★回転の適用
            Quaternion flipRotation = Quaternion.Euler(0, 0, flipAngle);
            GameDirector.instance.playerController.SetRotation(flipRotation);
            yield return null;
        }
    }

}