using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
//ゴールオブジェクトにアタッチ
public class GoalRotator : MonoBehaviour
{
    [SerializeField] private float duration = 1.5f;
    [SerializeField] private float heightOffset = 2.0f;
    [SerializeField] private float rotationSpeed = 720f;
    [SerializeField] private AnimationCurve landingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    private Vector3 targetPosition;
    private Quaternion targetRotation;

    private bool initialized = true;
    //出現時、ちょっと演出を加えながら出現
    void Awake()
    {
        // ゴールが置かれている「本来の位置と回転」を保存しておく
        targetPosition = transform.position;
        targetRotation = transform.rotation;
    }
    void OnEnable()
    {
        //最初は何もしない
        if (initialized)
        {
            initialized = false;
        }
        else
        {
            StartCoroutine(AnimateGoalAppearance());
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (initialized)
        {
            transform.Rotate(Vector3.up, 0.5f * rotationSpeed * Time.deltaTime);
        }
    }
    IEnumerator AnimateGoalAppearance()
    {
        float elapsed = 0f;

        // 1. 初期状態を設定（少し上で、回転している状態）
        Vector3 startPosition = targetPosition + Vector3.up * heightOffset;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;

            // 緩急（イージング）を適用
            float curveValue = landingCurve.Evaluate(progress);

            // 2. 位置の移動 (Lerp)
            transform.position = Vector3.Lerp(startPosition, targetPosition, curveValue);

            // 3. 回転の演出
            // 残り時間に応じて回転を弱めていく（1.0 -> 0.0）
            float currentRotation = rotationSpeed * (1.5f - progress);
            transform.Rotate(Vector3.up, currentRotation * Time.deltaTime);

            yield return null;
        }

        // 4. 最後にピタッと定位置に合わせる
        transform.position = targetPosition;
        //transform.rotation = targetRotation;

        // ここで「着地完了！」のパーティクルや音を鳴らすと最高です
        PlayLandingEffect();
    }

    void PlayLandingEffect()
    {
        initialized = true;
        // ここにエフェクトなどの処理を書く
    }
}
