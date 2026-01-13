using UnityEngine;
using System.Collections.Generic;
//ステージのルートオブジェクトにアタッチ
public class StageRoot : MonoBehaviour
{
    private List<GameObject> groundObject = new List<GameObject>();
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