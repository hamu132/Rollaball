using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
//プレイヤーオブジェクトにアタッチ
public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private int count;
    public TextMeshProUGUI countText;
    public GameObject winTextObject;
    //現在の速度、激しい初速、収束後の速度、速度変化割合
    private float m_currentSpeed;
    [SerializeField] private float m_initialSpeed;
    [SerializeField] private float m_normalSpeed;
    [SerializeField] private float m_decayRate;
    private bool m_isInput;
    //方向ベクトル
    private Vector3 m_moveInput;
    private Vector2 m_previousInput;

    [SerializeField] private GameObject explodePrefab;
    [SerializeField] private AudioClip gameOver;
    // Start is called before the first frame update
    void Start()
    {
        count = 0;
        rb = GetComponent<Rigidbody>();
        SetCountText();
        winTextObject.SetActive(false);
    }
    private void FixedUpdate()
    {
        if (m_isInput)
        {
            //方向ベクトル
            Vector3 direction = new Vector3 (m_moveInput.x, 0, m_moveInput.y);
            Vector3 camForward = Camera.main.transform.forward;
            Vector3 camRight = Camera.main.transform.right;
            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();
            Vector3 desiredMoveDir = camForward * direction.z + camRight * direction.x;

            //速度
            m_currentSpeed = Mathf.Lerp(m_currentSpeed, m_normalSpeed, m_decayRate * Time.fixedDeltaTime);
            desiredMoveDir *= m_currentSpeed;
            desiredMoveDir.y = rb.velocity.y;
            rb.velocity = desiredMoveDir;
        }
        else
        {
            rb.velocity *= 0.9f;
        }
    }
    //キーボード入力で床を制御
    public void OnFloor(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Floor");
        }
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        // 1. 入力が始まった瞬間 (started)
        if (context.started)
        {
            m_isInput = true;
            m_currentSpeed = m_initialSpeed;
            m_moveInput = context.ReadValue<Vector2>();
            m_previousInput = m_moveInput;
        }

        // 2. 入力が継続中 (performed)
        // 押したときは通過話した時は通過しないようにしたい
        if (context.performed)
        {
            //縦横→ナナメの場合だけ発動
            if(m_previousInput.x * m_previousInput.y == 0)
            {
                m_currentSpeed = m_initialSpeed;
            }
            m_moveInput = context.ReadValue<Vector2>();
            m_previousInput = m_moveInput;
        }

        // 3. 入力が終わった瞬間 (canceled)
        if (context.canceled)
        {
            m_isInput = false;
        }
    }
    public void OnFire(InputAction.CallbackContext context)
    {
        Debug.Log("動いた");
    }
    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();
        if (count >= 10)
        {
            Destroy(GameObject.FindGameObjectWithTag("Enemy"));
            winTextObject.SetActive(true);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        //アイテム獲得
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            count += 1;
            SetCountText();
        }
        //ゲームオーバー
        else if (other.gameObject.CompareTag("GameOver"))
        {
            Debug.Log("Game Over");
            Explode();
            // トランジションを開始！
            if (SceneTransitionManager.instance != null)
            {
                SceneTransitionManager.instance.StartReloadSequence();
            }
        }
    }
    //敵との衝突
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
            winTextObject.gameObject.SetActive(true);
            winTextObject.GetComponent<TextMeshProUGUI>().text = "You Lose!";
        }
    }
    void Explode()
    {
        AudioManager.instance.PlaySE(gameOver);
        Instantiate(explodePrefab,transform.position,Quaternion.identity);
        Destroy(gameObject);
    }
}
