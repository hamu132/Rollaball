using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    private Vector3 offset;
    private Vector2 m_mousePosition;
    [SerializeField] private float mouseSensitivity;
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
        float mouseX = mouseDelta.x * mouseSensitivity * Time.deltaTime;
        // 2. 上下の回転（カメラのローカル座標だけを回す）
        float mouseY = mouseDelta.y * mouseSensitivity * Time.deltaTime;
        
        // 回転量を蓄積（-= なのはマウスを上に動かしたらカメラを上に向けたいため）
        _xRotation -= mouseY;
        _yRotation -= mouseX;
        
        // 【重要】上下の回転角度を制限（-90〜90度）
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);
        _yRotation = Mathf.Clamp(_yRotation, -90f, 90f);

        // カメラの回転に適用
        transform.localRotation = Quaternion.Euler(_xRotation, 0f, _yRotation);
    }
}
