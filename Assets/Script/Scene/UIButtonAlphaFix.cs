using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
//ボタンオブジェクトにアタッチ
public class UIButtonAlphaFix : MonoBehaviour
{
    [SerializeField] [Range(0f, 1f)] private float alphaThreshold = 0.1f;

    void Start()
    {
        // Imageコンポーネントを取得
        Image img = GetComponent<Image>();
        
        if (img != null)
        {
            // 透明度のしきい値を設定（0.1なら、アルファ値が0.1以下の場所はクリックを無視する）
            img.alphaHitTestMinimumThreshold = alphaThreshold;
        }
    }
}