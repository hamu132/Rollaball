using UnityEngine;
using UnityEngine.InputSystem;
//プレイヤーオブジェクトにアタッチ
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    
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
    public static float timerf;



    // Start is called before the first frame update
    public void ZeroVelocity()
    {
        rb.velocity = Vector3.zero;
    }
    public void OffGravity()
    {
        rb.isKinematic = true;
    }
    void Start()
    {
        if (GameDirector.instance.currentStage == 1)
        {
            transform.position = new Vector3(0,2,-90);
        }
        else if (GameDirector.instance.currentStage == 2)
        {
            transform.position = new Vector3(5,34,-7);
        }
        else if (GameDirector.instance.currentStage == 3)
        {
            transform.position = new Vector3(0,91,-17.5f);
        }
    }
    private void FixedUpdate()
    {
        if (!rb.isKinematic)
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
                Vector3 v = new Vector3(rb.velocity.x*0.5f,rb.velocity.y,rb.velocity.z*0.5f);
                rb.velocity =v;
            }
        }
    }
    //キーボード入力で床を制御
    public void OnFloor1(InputAction.CallbackContext context)
    {
        if (context.performed)timerf = 0.31f;
        if (context.canceled && timerf == 0.31f)timerf = 0;
    }
    public void OnFloor2(InputAction.CallbackContext context)
    {
        if (context.performed)timerf = 0.57f;
        if (context.canceled && timerf == 0.57f)timerf = 0;
    }
    public void OnFloor3(InputAction.CallbackContext context)
    {
        if (context.performed)timerf = 0.83f;
        if (context.canceled && timerf == 0.83f)timerf = 0;
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
            Destroy(other.gameObject);
            GameDirector.instance.CheckItemCount();
            UpdateText.instance.SetItemCount();
        }
        //ゲームオーバー
        else if (other.gameObject.CompareTag("GameOver"))
        {
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
            other.enabled = false;
            GameDirector.instance.SetSpline();
            UIDirector.instance.goalProcess.Goal();
            GameDirector.instance.isClear = true;
        }
    }

    void Explode()
    {
        AudioManager.instance.PlaySE(gameOver,0.2f);
        Instantiate(explodePrefab,transform.position,Quaternion.identity);
        Destroy(gameObject);
    }
}
