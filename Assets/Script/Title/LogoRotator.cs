using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//アイテムオブジェクトにアタッチ
//アイテムを回転させる
public class LogoRotator : MonoBehaviour
{
    public float offset;
    private float elapsed = 0f;
    private float _speed = 500f;
    private float _currentrotation;
    // Update is called once per frame
    void Update()
    {
        if(elapsed <= offset)
        {
            elapsed += Time.deltaTime;
        }
        else
        {
            _currentrotation = Mathf.Abs(transform.rotation.z);
            transform.Rotate(_speed * (_currentrotation+0.000002f) * new Vector3(0, 0, -1) * Time.deltaTime);    
        }
    }
}
