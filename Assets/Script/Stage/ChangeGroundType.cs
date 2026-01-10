using UnityEngine;
using System.Collections;
//RedGroundプレハブにアタッチ
public class ChangeGroundType : MonoBehaviour
{
    [SerializeField] private Material groundMaterial;
    [SerializeField] private AnimationCurve introCurve; // インスペクターでグラフを設定
    private Coroutine currentCoroutine;
    GameObject stageRoot;
    void Start()
    {
        disableGround();
        //親スクリプトにリストとして追加
        stageRoot = transform.parent.parent.parent.gameObject;
        stageRoot.GetComponent<StageRoot>().addList(gameObject);

    }

    public void enableGround()
    {
        // StopCoroutine("crack1");
        //マテリアルを変更・コライダーを有効化
        GetComponent<Renderer>().material = groundMaterial;
        GetComponent<Collider>().enabled = true;
        // StartCoroutine("crack1");
        StartFillAnimation();
    }
    public void disableGround()
    {
        //マテリアルを変更・コライダーを無効化
        // GetComponent<Collider>().enabled = false;
        // groundMaterial.SetFloat("_Alpha", 0.1f);
        // groundMaterial.SetFloat("_FillAmount", 1f);

    }
    void StartFillAnimation()
    {
        // 既に動いている場合はリセットして再開
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(FillAndHideRoutine());
    }
    IEnumerator FillAndHideRoutine()
    {
        //targetRenderer.enabled = true; // 表示
        float totalDuration = 5.0f;         // 5秒間
        float outroDuration = 0.5f;//最後
        float introDuration = 0.3f;//最初
        float outroStartTime = totalDuration - outroDuration; // 4.5秒時点
        float elapsed = 0f;
        float period = 1f;

        groundMaterial.SetFloat("_Alpha", 0.9f);
        while (elapsed < totalDuration)
        {
            elapsed += Time.deltaTime;
            period *= 0.999f;
            float progress = elapsed / totalDuration;
            float introProgress = Mathf.Clamp01(elapsed / introDuration);
            float outroProgress = Mathf.Clamp01((elapsed - outroStartTime) / outroDuration);
            float periodProgress = Mathf.Clamp01(elapsed%period / period);

            float periodCurveValue = introCurve.Evaluate(periodProgress);
            float outroCurveValue = introCurve.Evaluate(outroProgress);

            // 1. 色の塗りつぶし (5秒かけて)
            groundMaterial.SetFloat("_FillAmount", progress);

            // 2. 放射（Emission）の演出 (1秒かけて 5.0 -> 0.0)
            float emission = Mathf.Lerp(3.0f, 0.0f, periodCurveValue);
            groundMaterial.SetFloat("_EmissionIntensity", emission);

            if(introDuration > elapsed)
            {
                // 3. 見た目の大きさ（Scale）の演出 (1秒かけて 1.2 -> 1.0)
                float visualScale = Mathf.Lerp(1.2f, 1.0f, introProgress);
                groundMaterial.SetFloat("_VisualScale", visualScale);
            }
            else if(outroStartTime < elapsed)
            {
                // 3. 見た目の大きさ（Scale）の演出 (1秒かけて 1.0 -> 0.0)
                float visualScale = Mathf.Lerp(1.0f, 0f, outroCurveValue);
                groundMaterial.SetFloat("_VisualScale", visualScale);
            }
            yield return null;
        }

        // 5秒経ったら非表示
        GetComponent<Collider>().enabled = false;
        // groundMaterial.SetFloat("_Alpha", 0.1f);
        // groundMaterial.SetFloat("_VisualScale", 1f);
    }
    // IEnumerator crack1()
    // {
    //     //ここに処理を書く

    //     //1フレーム停止
    //     yield return new WaitForSeconds(2);

    //     //ここに再開後の処理を書く
    //     disableGround();
    // }
}