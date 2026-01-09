using UnityEngine;
//Stage1-Groundオブジェクトにアタッチ
public class Ground : MonoBehaviour
{
    void Start()
    {
        disableGround();
    }

    public void enableGround(string tag = "Ground")
    {
        Debug.Log("Enable Ground Called");
        // 0～個数-1までの子を順番に配列に格納
        for (var i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.CompareTag(tag))
            {
                //マテリアルを変更・コライダーを有効化
                child.GetComponent<ChangeGroundType>().enableGround();
                Debug.Log("Enable Ground");
            }
        }
    }
    public void disableGround(string tag = "Ground")
    {
        // 0～個数-1までの子を順番に配列に格納
        for (var i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.CompareTag(tag))
            {
                //マテリアルを変更・コライダーを無効化
                child.GetComponent<ChangeGroundType>().disableGround();
            }
        }
    }
}