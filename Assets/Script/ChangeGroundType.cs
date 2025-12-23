using UnityEngine;
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
        //マテリアルを変更・コライダーを有効か
        GetComponent<Renderer>().material = rigidMaterial;
        GetComponent<Collider>().enabled = true;
    }
    public void disableGround()
    {
        //マテリアルを変更・コライダーを無効化
        GetComponent<Renderer>().material = transMaterial;
        GetComponent<Collider>().enabled = false;
    }
}