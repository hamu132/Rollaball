using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//アイテムオブジェクトにアタッチ
//アイテムを回転させる
public class Rotator : MonoBehaviour
{


    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);
    }
}
