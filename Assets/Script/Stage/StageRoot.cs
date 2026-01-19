using UnityEngine;
using System.Collections.Generic;
//ステージのルートオブジェクトにアタッチ
// 特定の色の床を一括でONにする・固定変数を設定
public class StageRoot : MonoBehaviour
{
    private List<GameObject> groundObject = new List<GameObject>();
    //固定の変数を設定
    [Header("ステージに関する固有変数")]
    public static float mainDuration = 4.5f;//床の生成時間

    public void enableGround(string tag = "Ground")
    {
        foreach (var ground in groundObject) {
            if (ground.CompareTag(tag)){
                ground.GetComponent<ChangeGroundType>().enableGround();
            }
        }
    }
    public void addList(GameObject gameObject)
    {
        groundObject.Add(gameObject);
    }
}