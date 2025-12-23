using UnityEngine;
using System.Collections;
//プレハブにアタッチ
public class ChangeGroundType : MonoBehaviour
{
    [SerializeField] private Material transMaterial;
    [SerializeField] private Material rigidMaterial;
    void Start()
    {
        disableGround();
    }

    public void enableGround()
    {
        StopCoroutine("crack1");
        //マテリアルを変更・コライダーを有効化
        GetComponent<Renderer>().material = rigidMaterial;
        GetComponent<Collider>().enabled = true;
        StartCoroutine("crack1");
    }
    public void disableGround()
    {
        //マテリアルを変更・コライダーを無効化
        GetComponent<Renderer>().material = transMaterial;
        GetComponent<Collider>().enabled = false;
    }
    IEnumerator crack1()
    {
        //ここに処理を書く

        //1フレーム停止
        yield return new WaitForSeconds(2);

        //ここに再開後の処理を書く
        disableGround();
    }
    IEnumerator crack2()
    {
        //ここに処理を書く

        //1フレーム停止
        yield return null;

        //ここに再開後の処理を書く
    }
}