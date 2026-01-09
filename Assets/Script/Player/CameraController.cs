using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    private Vector3 offset;
    [SerializeField] private float mouseSensitivityX;
    [SerializeField] private float mouseSensitivityY;
    private float _xRotation;
    private float _yRotation;
    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - player.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(player != null)
        {
            transform.position = player.transform.position + offset;
        }
    }
    public void OnVision(InputAction.CallbackContext context)
    {
        // マウスの移動量を取得
        Vector2 mouseDelta = context.ReadValue<Vector2>();

        // 1. 左右の回転（プレイヤーの体全体を回す）
        float mouseX = mouseDelta.x * mouseSensitivityX * Time.deltaTime;
        // 2. 上下の回転（カメラのローカル座標だけを回す）
        float mouseY = mouseDelta.y * mouseSensitivityY * Time.deltaTime;
        
        // 回転量を蓄積（-= なのはマウスを上に動かしたらカメラを上に向けたいため）
        _xRotation -= mouseY;
        _yRotation += mouseX;
        
        // 【重要】上下の回転角度を制限（-90〜90度）
        _xRotation = Mathf.Clamp(_xRotation, -45f, 45f);

        // カメラの回転に適用
        transform.localRotation = Quaternion.Euler(_xRotation, 0, 0f);
        transform.parent.localRotation = Quaternion.Euler(0f, _yRotation, 0f);
    }
    public float getRotation()
    {
        return _yRotation;
    }
}
