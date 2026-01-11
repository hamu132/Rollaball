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

        float phase = 0f;

        
        while (elapsed < totalDuration)
        {
            elapsed += Time.deltaTime;
            //progress：トータルで0~1まで
            float progress = Mathf.Clamp01(elapsed / (totalDuration-outroDuration));

            groundMaterial.SetFloat("_FillAmount", 1-progress);


            //intro：イントロ部分で0~1まで。それ以降は1のまま。
            float introProgress = Mathf.Clamp01(elapsed / introDuration);
            float introCurveValue = introCurve.Evaluate(introProgress);


            //period：0~1で周期的に動く。
            float currentPeriod = Mathf.Lerp(1.2f, 0.3f, progress);
            phase += Time.deltaTime / currentPeriod;
            float pulse = phase % 1.0f;
            float periodCurveValue = periodCurve.Evaluate(pulse);

            //outro：アウトロ部分で0~1まで。それ以前は0のまま。
            float outroProgress = Mathf.Clamp01((elapsed - outroStartTime) / outroDuration);
            float outroCurveValue = introCurve.Evaluate(outroProgress);
            //イントロ
            if(elapsed <= introDuration)
            {
                float visualScale = Mathf.Lerp(0.1f, 1.0f, introCurveValue);
                groundMaterial.SetFloat("_VisualScale", visualScale);
            }
            //イントロかつメイン
            if (elapsed <= outroStartTime)
            {
                float emission = Mathf.Lerp(3.0f, 0.0f, periodCurveValue);
                groundMaterial.SetFloat("_EmissionIntensity", emission);
            }
            //アウトロ
            else if(outroStartTime < elapsed)
            {
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