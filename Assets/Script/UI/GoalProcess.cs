using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Splines; // 必須

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
    private float _moveTime = 2f;
    private float _blackTime = 0.5f;

    //ゴールに触れた瞬間に発動
    public void Goal()
    {
        topBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
        bottomBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
        StartCoroutine(GoalProcessRoutine());
        StartCoroutine(MoveWithCurveRoutine());
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
        while (elapsed < _moveTime)
        {
            elapsed += Time.deltaTime;
            
            // 0から1の間の線形な進捗率(t)を計算
            float t = Mathf.Clamp01(elapsed / _moveTime);
            
            // AnimationCurveを使用して緩急をつけた値(0~1)を取得
            float curvedT = landingCurve.Evaluate(t);
            
            // splineAnimateのNormalizedTime（0~1）に反映
            GameDirector.instance.splineAnimate.NormalizedTime = curvedT;

            yield return null;
        }

        // 最後に確実に1（終了地点）にする
        GameDirector.instance.splineAnimate.NormalizedTime = 1f;
    }
}