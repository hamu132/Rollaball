using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//カメラオブジェクトにアタッチ
public class CameraController : MonoBehaviour
{
    public GameObject player;
    private Vector3 offset;
    public bool isCameraActive;
    // Start is called before the first frame update
    void Start()
    {
        //offset = transform.position - player.transform.position;
        offset = new Vector3(0, 10, -10);
        isCameraActive = true;
    }


    // Update is called once per frame
    void LateUpdate()
    {
        if(player != null && isCameraActive)
        {
            transform.position = player.transform.position + offset;
        }
    }
}
