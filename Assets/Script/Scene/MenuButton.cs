using UnityEngine;
using UnityEngine.UI;

//ボタンオブジェクトにアタッチ
public class MenuButton : MonoBehaviour
{
    [SerializeField] private string targetSceneName = "Title";

    void Start()
    {
        // 自分のオブジェクトについているButtonコンポーネントを取得
        Button btn = GetComponent<Button>();

        // ボタンが押されたら、SceneTransitionManagerのOnClickを呼ぶように「後付け」する
        if (btn != null)
        {
            btn.onClick.AddListener(() => {
                if (SceneTransitionManager.instance != null)
                {
                    SceneTransitionManager.instance.OnClick(targetSceneName);
                }
            });
        }
    }
}