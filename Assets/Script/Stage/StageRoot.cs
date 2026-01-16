using UnityEngine;
using System.Collections.Generic;
//ステージのルートオブジェクトにアタッチ
// 特定の色の床を一括でONにする
public class StageRoot : MonoBehaviour
{
    private List<GameObject> groundObject = new List<GameObject>();
    public float mainDuration = 4.5f;
    public static bool isTimePaused = false;
    void Start()
    {
        disableGround();
    }

    public void enableGround(string tag = "Ground")
    {
        foreach (var ground in groundObject) {
            if (ground.CompareTag(tag)){
                ground.GetComponent<ChangeGroundType>().enableGround();
            }
        }
    }
    public void disableGround(string tag = "Ground")
    {
        foreach (var ground in groundObject) {
            if (ground.CompareTag(tag)){
                //ground.GetComponent<ChangeGroundType>().disableGround();
            }
        }
    }
    public void addList(GameObject gameObject)
    {
        groundObject.Add(gameObject);
    }
}