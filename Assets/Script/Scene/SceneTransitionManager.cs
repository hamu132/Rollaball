using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
//SceneManagerオブジェクトにアタッチ
public class SceneTransitionManager : MonoBehaviour
{
    // どこからでもアクセスできるようにする（シングルトン）
    public static SceneTransitionManager instance;

    [Header("UI References")]
    public GameObject transitionCanvas; // Canvas全体（普段は隠すため）
    public RectTransform maskCircleRect; // 丸いマスクのサイズを制御用

    [Header("Settings")]
    public float transitionDuration = 1.0f; // 暗転にかかる時間
    public Vector2 maxMaskSize = new Vector2(3000f, 3000f); // 画面を覆い尽くす巨大なサイズ

    void Awake()
    {
        // シーンをロードしてもこのオブジェクトを破壊しない設定
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // 重複して存在しないようにする
        }

        // 最初はキャンバスを非表示にしておく
        transitionCanvas.SetActive(false);
    }

    // 外部（プレイヤーの死亡時など）からこのメソッドを呼ぶ
    public void IrisOust(string sceneName)
    {
        StartCoroutine(TransitionCoroutine(sceneName));
    }
    //ボタンをクリックしたときに呼ばれる
    public void OnClick(string str)
    {
        StartCoroutine(TransitionCoroutine(str));
    }
    IEnumerator TransitionCoroutine(string str)
    {
        // 1. 演出開始準備
        transitionCanvas.SetActive(true);
        maskCircleRect.sizeDelta = maxMaskSize; // 最初は巨大な穴（画面が見えている）
        float timer = 0f;

        // 2. 暗転（アイリスアウト）：穴を小さくしていく
        while (timer < transitionDuration)
        {
            timer += Time.deltaTime;
            // 巨大サイズからゼロ（真っ黒）へ滑らかに変化
            maskCircleRect.sizeDelta = Vector2.Lerp(maxMaskSize, Vector2.zero, timer / transitionDuration);
            yield return null;
        }
        maskCircleRect.sizeDelta = Vector2.zero; // 念のため完全にゼロにする

        // 3. シーン読み込み（真っ黒な状態で待機）
        // 少しウェイトを入れると余韻が出ます
        yield return new WaitForSeconds(0.5f); 
        
        // 次のシーンを再読み込み
        SceneManager.LoadScene(str);

        // シーン読み込み完了待ち（最低1フレーム待つ）
        //yield return null;
        yield return new WaitForSeconds(1f); 

        // 4. 明転（アイリスイン）：穴を大きくしていく
        timer = 0f;
        while (timer < transitionDuration)
        {
            timer += Time.deltaTime;
            // ゼロから巨大サイズへ変化
            maskCircleRect.sizeDelta = Vector2.Lerp(Vector2.zero, maxMaskSize, timer / transitionDuration);
            yield return null;
        }
        maskCircleRect.sizeDelta = maxMaskSize;

        // 5. 終了処理
        transitionCanvas.SetActive(false);
    }
}