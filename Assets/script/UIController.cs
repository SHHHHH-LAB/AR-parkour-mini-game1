using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [Header("UI元素")]
    public Button characterChangeButton;
    public Button startButton;
    public Text timeText;
    public Text scoreText;
    public GameObject gameEndUI;

    [Header("参数设置")]
    public float time = 30f;
    public bool Begin = false;

    private Control_Player player;
    public static UIController Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // 强制初始时间（防止Inspector误设）
        time = 30f;

        GameObject playerObj = GameObject.FindGameObjectWithTag("character");
        if (playerObj != null)
            player = playerObj.GetComponent<Control_Player>();

        if (startButton != null)
            startButton.onClick.AddListener(GameStart);
        if (characterChangeButton != null)
            characterChangeButton.onClick.AddListener(characterChang);

        if (gameEndUI != null)
            gameEndUI.SetActive(false);
    }

    void Update()
    {
        if (!Begin || player == null) return;

        float score = player.fenShu;
        scoreText.text = score.ToString();

        if (time > 0 && score > 0)
        {
            time -= Time.deltaTime;
            // 统一格式 MM:SS
            timeText.text = string.Format("{0:d2}:{1:d2}", (int)time / 60, (int)time % 60);
        }
        else
        {
            // 触发失败（时间到或分数归零）
            GameOver();
        }
    }

    public void GameStart()
    {
        Begin = true;
        Time.timeScale = 1f;
    }

    public void characterChang()
    {
        SceneManager.LoadScene("CharacterScene");
    }

    public void GameOver()
    {
        Begin = false;
        Time.timeScale = 0;
        if (gameEndUI != null) gameEndUI.SetActive(true);
        // 保留当前时间显示，不要强制设为 "00:00"
        // 这样玩家能看到失败时的真实剩余时间
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}