using UnityEngine;
using System.Collections;
//RedGroundプレハブにアタッチ
public class ChangeGroundType : MonoBehaviour
{
    [SerializeField] private Material groundMaterial;
    [SerializeField] private AnimationCurve introCurve; // インスペクターでグラフを設定
    [SerializeField] private AnimationCurve periodCurve;
    private Coroutine currentCoroutine;
    GameObject stageRoot;
    void Start()
    {
        groundMaterial.SetFloat("_VisualScale", 0.1f);
        groundMaterial.SetFloat("_Alpha", 0.95f);
        GetComponent<Collider>().enabled = false;
        disableGround();
        //親スクリプトにリストとして追加
        stageRoot = transform.parent.parent.parent.gameObject;
        stageRoot.GetComponent<StageRoot>().addList(gameObject);

    }

    public void enableGround()
    {
        StartFillAnimation();
    }
    public void disableGround()
    {
        
    }
    void StartFillAnimation()
    {
        // 既に動いている場合はリセットして再開
        if (currentCoroutine == null)
        {
            currentCoroutine = StartCoroutine(FillAndHideRoutine());
        }
        
    }
    IEnumerator FillAndHideRoutine()
    {
        bool colliderFlag = true;
        GetComponent<Collider>().enabled = true;
        //targetRenderer.enabled = true; // 表示
        float totalDuration = 5.0f;         // 5秒間
        float outroDuration = 0.5f;//最後
        float introDuration = 0.3f;//最初
        float outroStartTime = totalDuration - outroDuration; // 4.5秒時点
        float elapsed = 0f;
        float period = 1f;

        
        while (elapsed < totalDuration)
        {
            elapsed += Time.deltaTime;
            period *= 0.999f;
            float progress = Mathf.Clamp01(elapsed / (totalDuration-outroDuration));
            float introProgress = Mathf.Clamp01(elapsed / introDuration);
            float outroProgress = Mathf.Clamp01((elapsed - outroStartTime) / outroDuration);
            float periodProgress = Mathf.Clamp01(elapsed%period / period);

            float periodCurveValue = periodCurve.Evaluate(periodProgress);
            float introCurveValue = introCurve.Evaluate(introProgress);
            float outroCurveValue = introCurve.Evaluate(outroProgress);

            // 1. 色の塗りつぶし (5秒かけて)
            groundMaterial.SetFloat("_FillAmount", 1-progress);
            //イントロ
            if(introDuration > elapsed)
            {
                // 3. 見た目の大きさ（Scale）の演出 (1秒かけて 1.2 -> 1.0)
                float visualScale = Mathf.Lerp(0.1f, 1.0f, introCurveValue);
                groundMaterial.SetFloat("_VisualScale", visualScale);
            }
            //メイン
            else if (introDuration <= elapsed && elapsed <= outroStartTime)
            {
                // 2. 放射（Emission）の演出 (1秒かけて 5.0 -> 0.0)
                float emission = Mathf.Lerp(3.0f, 0.0f, periodCurveValue);
                groundMaterial.SetFloat("_EmissionIntensity", emission);
            }
            else if(outroStartTime < elapsed)
            {
                // 3. 見た目の大きさ（Scale）の演出 (1秒かけて 1.0 -> 0.0)
                float visualScale = Mathf.Lerp(1.0f, 0.1f, outroCurveValue);
                groundMaterial.SetFloat("_VisualScale", visualScale);

                if (colliderFlag)
                {
                    GetComponent<Collider>().enabled = false;
                    colliderFlag = false;
                }
            }
            yield return null;
        }
        currentCoroutine = null;
    }
}