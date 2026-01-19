using UnityEngine;
using System.Collections;
//RedGroundプレハブにアタッチ
public class ChangeGroundType : MonoBehaviour
{
    [SerializeField] private AnimationCurve introCurve; // インスペクターでグラフを設定
    [SerializeField] private AnimationCurve periodCurve;
    private Coroutine currentCoroutine;
    private GameObject stageRoot;
    private Renderer targetRenderer;
    private MaterialPropertyBlock propBlock;
    private float mainDuration;
    void Start()
    {
        targetRenderer = GetComponent<Renderer>();
        propBlock = new MaterialPropertyBlock();
        ApplyInitialProperties();

        GetComponent<Collider>().enabled = false;
        //親スクリプトにリストとして追加
        stageRoot = transform.parent.parent.parent.gameObject;
        stageRoot.GetComponent<StageRoot>().addList(gameObject);
        mainDuration = StageRoot.mainDuration;
    }
    // 初期値を反映するためのメソッド（コルーチン外でも使うため分離）
    void ApplyInitialProperties()
    {
        targetRenderer.GetPropertyBlock(propBlock);
        propBlock.SetFloat("_VisualScale", 0.1f);
        propBlock.SetFloat("_Alpha", 0.95f);
        // 最初はプログレスバーを満たしておく（FillAmount = 1）
        propBlock.SetFloat("_FillAmount", 1.0f); 
        targetRenderer.SetPropertyBlock(propBlock);
    }
    public void enableGround()
    {
        StartFillAnimation();
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
        
        float outroDuration = 0.5f;//最後
        float introDuration = 0.3f;//最初
        float outroStartTime = mainDuration; // 4.5秒時点
        float totalDuration = outroStartTime + outroDuration;         // 5秒間
        float elapsed = 0f;
        float phase = 0f;

        
        while (elapsed < totalDuration)
        {

            if (!GameDirector.instance.isTimePaused)
            {
                elapsed += Time.deltaTime;
                // 現在のPropertyBlockの状態を取得
                targetRenderer.GetPropertyBlock(propBlock);

                //progress：トータルで0~1まで
                float progress = Mathf.Clamp01(elapsed / (totalDuration-outroDuration));
                propBlock.SetFloat("_FillAmount", 1-progress);


                //intro：イントロ部分で0~1まで。それ以降は1のまま。
                float introProgress = Mathf.Clamp01(elapsed / introDuration);
                float introCurveValue = introCurve.Evaluate(introProgress);


                //period：0~1で周期的に動く。
                float currentPeriod = Mathf.Lerp(1.2f, 0.3f, progress);
                phase += Time.deltaTime / currentPeriod;
                float pulse = phase % 1.0f;
                float periodCurveValue = periodCurve.Evaluate(pulse);

                //outro
                float outroProgress = Mathf.Clamp01((elapsed - outroStartTime) / outroDuration);
                float outroCurveValue = introCurve.Evaluate(outroProgress);
                //イントロ
                if(elapsed <= introDuration)
                {
                    float visualScale = Mathf.Lerp(0.1f, 1.0f, introCurveValue);
                    propBlock.SetFloat("_VisualScale", visualScale);
                }
                //イントロかつメイン
                if (elapsed <= outroStartTime)
                {
                    float emission = Mathf.Lerp(1.0f, 0.2f, periodCurveValue);
                    //propBlock.SetFloat("_EmissionIntensity", emission);
                    propBlock.SetFloat("_EmissionIntensity", 0.5f);
                }
                //アウトロ
                else if(outroStartTime < elapsed)
                {
                    float visualScale = Mathf.Lerp(1.0f, 0.1f, outroCurveValue);
                    propBlock.SetFloat("_VisualScale", visualScale);

                    if (colliderFlag)
                    {
                        GetComponent<Collider>().enabled = false;
                        colliderFlag = false;
                    }
                }
                targetRenderer.SetPropertyBlock(propBlock);
            }
            yield return null;
        }
        currentCoroutine = null;
    }
}