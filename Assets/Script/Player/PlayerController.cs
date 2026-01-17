using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using Unity.VisualScripting;
//プレイヤーオブジェクトにアタッチ
public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private int count;
    
    //現在の速度、激しい初速、収束後の速度、速度変化割合
    private float m_currentSpeed;
    [SerializeField] private float m_initialSpeed;
    [SerializeField] private float m_normalSpeed;
    [SerializeField] private float m_decayRate;
    private bool m_isInitialDash;
    //方向ベクトル
    private Vector3 m_moveInput;
    private Vector2 m_previousInput;

    [SerializeField] private GameObject explodePrefab;
    [SerializeField] private AudioClip gameOver;
    [SerializeField] private AudioClip wall;
    [SerializeField] private AudioClip move;
    [SerializeField] private AudioClip item;
    [SerializeField] private GameObject stageRootObject;
    private StageRoot stageRoot;
    private GameDirector gameDirector;
    private PlayerInput playerInput;
    public static float timerf;

    // Start is called before the first frame update
    public void ZeroVelocity()
    {
        rb.velocity = Vector3.zero;
    }
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        stageRoot = stageRootObject.GetComponent<StageRoot>();
        gameDirector = stageRootObject.GetComponent<GameDirector>();
        count = 0;
        rb = GetComponent<Rigidbody>();
        if (gameDirector.currentStage == 1)
        {
            transform.position = new Vector3(0,2,-90);
        }
        else if (gameDirector.currentStage == 2)
        {
            transform.position = new Vector3(0,34,-7);
        }
    }
    private void FixedUpdate()
    {
        if (m_isInitialDash)
        {
            m_currentSpeed = Mathf.Lerp(m_currentSpeed, m_normalSpeed, m_decayRate * Time.fixedDeltaTime);
            Vector3 direction = new Vector3 (m_moveInput.x, 0, m_moveInput.y);
            direction *= m_currentSpeed;
            direction.y = rb.velocity.y;
            rb.velocity = direction;
        }
        else
        {
            rb.velocity *= 0.9f;
        }
    }
    //キーボード入力で床を制御
    public void OnFloor1(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //_timerRf = true;
            //stageRoot.enableGround("RedGround");
            timerf = 0.21f;
        }
        if (context.canceled && timerf == 0.21f)
        {
            timerf = 0;
        }
    }
    public void OnFloor2(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            timerf = 0.46f;
        }
        if (context.canceled && timerf == 0.46f)
        {
            timerf = 0;
        }
    }
    public void OnFloor3(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            timerf = 0.78f;
        }
        if (context.canceled && timerf == 0.78f)
        {
            timerf = 0;
        }
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        // 1. 入力が始まった瞬間 (started)
        if (context.started)
        {
            m_isInitialDash = true;
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
                AudioManager.instance.PlaySE(move,0.1f);
            }
            m_moveInput = context.ReadValue<Vector2>();
            m_previousInput = m_moveInput;
        }

        // 3. 入力が終わった瞬間 (canceled)
        if (context.canceled)
        {
            m_isInitialDash = false;
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            AudioManager.instance.PlaySE(wall,0.2f);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        //アイテム獲得
        if (other.gameObject.CompareTag("PickUp"))
        {
            AudioManager.instance.PlaySE(item,0.2f);
            other.gameObject.SetActive(false);
            count += 1;
            gameDirector.SetItemCount(count);
        }
        //ゲームオーバー
        else if (other.gameObject.CompareTag("GameOver"))
        {
            Debug.Log("Game Over");
            Explode();
            // トランジションを開始！
            if (SceneTransitionManager.instance != null)
            {
                SceneTransitionManager.instance.IrisOust("MiniGame");
            }
        }
        //ゴール到達
        else if (other.gameObject.CompareTag("Goal"))
        {
            if (SceneTransitionManager.instance != null)
            {
                other.enabled = false;
                ZeroVelocity();
                playerInput.DeactivateInput(); 
                //SceneTransitionManager.instance.IrisOust("End");
                gameDirector.GoalProcess();

            }
        }
    }
    void Explode()
    {
        AudioManager.instance.PlaySE(gameOver,0.2f);
        Instantiate(explodePrefab,transform.position,Quaternion.identity);
        Destroy(gameObject);
    }
}
