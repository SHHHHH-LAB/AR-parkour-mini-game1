using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class Control_Player : MonoBehaviour
{
    [Header("移动参数")]
    public float laneOffset = 1.2f;
    public float moveSpeed = 8f;

    [Header("跳跃参数")]
    public float jumpForce = 4f;
    public float jumpDuration = 1f;
    private bool isGrounded = true;
    private float jumpTimer = 0;

    [Header("悬空检测")]
    public float airborneTolerance = 0.8f; // 允许悬空的最大时间（秒），超过则失败

    [Header("UI 按钮")]
    public Button leftButton;
    public Button rightButton;
    public Button jumpButton;

    private int currentLane = 0;
    private Rigidbody rb;
    private Animator m_Anim;

    public float fenShu = 100;

    private int runParamID;
    private int jumpParamID;
    private bool isJumping = false;

    // 缓冲（AR 启动时给角色下落时间）
    private bool started = false;
    private float startDelayTimer = 0f;
    private const float START_BUFFER = 1.5f;

    // 悬空计时器（用于宽容判定）
    private float airborneTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        m_Anim = GetComponent<Animator>();

        runParamID = Animator.StringToHash("isRun");
        jumpParamID = Animator.StringToHash("isJump");

        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.useGravity = false;
        rb.drag = 0;
        rb.angularDrag = 0;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        m_Anim.SetBool(runParamID, false);

        currentLane = Mathf.RoundToInt(transform.localPosition.x / laneOffset);

        if (leftButton != null) leftButton.onClick.AddListener(MoveLeft);
        if (rightButton != null) rightButton.onClick.AddListener(MoveRight);
        if (jumpButton != null) jumpButton.onClick.AddListener(Jump);

        SetButtonInteractable(false);
    }

    void FixedUpdate()
    {
        if (UIController.Instance == null) return;
        bool gameBegin = UIController.Instance.Begin;
        SetButtonInteractable(gameBegin);

        if (!gameBegin) return;

        // 动画控制
        if (isGrounded && !isJumping)
            m_Anim.SetBool(runParamID, true);
        else if (!isGrounded && !isJumping)
            m_Anim.SetBool(runParamID, false);

        // 左右跑道移动（只修改 X 轴，Y 轴完全由物理控制）
        Vector3 targetWorldPos = transform.parent.TransformPoint(
            new Vector3(currentLane * laneOffset, 0, 0)
        );
        Vector3 targetPos = new Vector3(
            targetWorldPos.x,
            rb.position.y,      // 保持当前 Y，不干预
            rb.position.z
        );
        Vector3 smoothPos = Vector3.Lerp(rb.position, targetPos, Time.fixedDeltaTime * moveSpeed);
        rb.MovePosition(smoothPos);
    }

    void Update()
    {
        if (UIController.Instance == null) return;
        bool gameBegin = UIController.Instance.Begin;

        // 重力控制：仅在非地面时开启重力
        if (gameBegin && !rb.useGravity && !isGrounded)
            rb.useGravity = true;

        if (!gameBegin) return;

        // ----- 缓冲期（让角色自然下落） -----
        if (!started)
        {
            started = true;
            startDelayTimer = START_BUFFER;
            if (!rb.useGravity) rb.useGravity = true;
            return;
        }
        if (startDelayTimer > 0)
        {
            startDelayTimer -= Time.deltaTime;
            return;
        }

        // ----- 正式游戏逻辑（已删除掉入深渊失败） -----

        // 悬空检测（带宽容计时）
        if (!isJumping && !isGrounded)
        {
            airborneTimer += Time.deltaTime;
            if (airborneTimer >= airborneTolerance)
            {
                Debug.Log($"失败：悬空超过 {airborneTolerance} 秒");
                UIController.Instance.GameOver();
                return;
            }
        }
        else
        {
            airborneTimer = 0f; // 一旦回到地面或跳跃，重置计时
        }

        // 跳跃超时保护（强制落地）
        if (isJumping && !isGrounded)
        {
            jumpTimer += Time.deltaTime;
            if (jumpTimer >= jumpDuration * 0.6f)
            {
                Land();
            }
        }
    }

    // ---------- 移动和跳跃 ----------
    void MoveLeft()
    {
        if (UIController.Instance == null) return;
        if (currentLane > -1 && UIController.Instance.Begin)
            currentLane--;
    }

    void MoveRight()
    {
        if (UIController.Instance == null) return;
        if (currentLane < 1 && UIController.Instance.Begin)
            currentLane++;
    }

    void Jump()
    {
        if (UIController.Instance == null) return;
        if (!UIController.Instance.Begin || !isGrounded || isJumping) return;

        isJumping = true;
        isGrounded = false;
        jumpTimer = 0;
        airborneTimer = 0f;

        if (!rb.useGravity) rb.useGravity = true;

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        m_Anim.SetTrigger(jumpParamID);
        m_Anim.SetBool(runParamID, false);
    }

    // ---------- 落地处理（不修改位置） ----------
    void Land()
    {
        if (!isJumping && isGrounded) return;

        isGrounded = true;
        isJumping = false;
        jumpTimer = 0;
        airborneTimer = 0f;

        // 关闭重力，冻结垂直速度
        if (rb.useGravity) rb.useGravity = false;
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        // 恢复跑步动画
        if (UIController.Instance != null && UIController.Instance.Begin)
            m_Anim.SetBool(runParamID, true);
        else
            m_Anim.SetBool(runParamID, false);
    }

    // ---------- 碰撞检测（仅更新状态） ----------
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("碰撞到: " + collision.collider.name + ", Tag: " + collision.collider.tag);
        if (collision.collider.CompareTag("Ground") && !isGrounded)
        {
            Land();
        }
    }

    // 不依赖 OnCollisionStay 修正位置，只用来更新状态
    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Ground") && !isGrounded)
        {
            Land();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("触发碰撞器: " + other.name + ", Tag: " + other.tag);
        if (UIController.Instance == null || !UIController.Instance.Begin) return;
        if (other.CompareTag("Obstacle") && fenShu > 0)
        {
            fenShu -= 20;
            Debug.Log($"扣分后分数: {fenShu}");
        }
    }

    // ---------- 辅助方法 ----------
    void SetButtonInteractable(bool state)
    {
        if (leftButton != null) leftButton.interactable = state;
        if (rightButton != null) rightButton.interactable = state;
        if (jumpButton != null) jumpButton.interactable = state;
    }

    public void AddScore(int num) => fenShu += num;
    public void SendEvent() { }
}