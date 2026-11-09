using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Draws score, health bar, artifact counter and the win / game-over screen.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int score;
    public int artifacts;
    public int artifactsToWin = 5;
    public bool isGameOver;
    public bool hasWon;

    [Range(0.3f, 1.5f)] public float uiScale = 0.7f;   // make this smaller to shrink all the text
    public float taskShowSeconds = 10f;                // how long the task text stays on screen

    Health playerHealth;

    void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
    }

    void Start()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) playerHealth = p.GetComponent<Health>();
    }

    public void AddScore(int amount)
    {
        if (!isGameOver) score += amount;
    }

    public void AddArtifact()
    {
        if (isGameOver) return;
        artifacts++;
        if (artifacts >= artifactsToWin)
        {
            hasWon = true;
            GameOver();
        }
    }

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        Time.timeScale = 0f;
    }

    void Update()
    {
        if (isGameOver && Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            Restart();
    }

    // Black text in the middle of the screen with a thin white outline so it stays readable.
    void DrawTask(float w, float h)
    {
        string text = "YOUR TASK: collect " + artifactsToWin + " ancient artifacts.\n" +
                      "Enemy ships steal them - sink a ship with a gold dot to get them back!";
        var style = new GUIStyle(GUI.skin.label)
        {
            fontSize = 30, fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter, wordWrap = true
        };
        var rect = new Rect(w / 2f - 550, h / 2f - 120, 1100, 120);

        style.normal.textColor = Color.white;
        foreach (var o in new[] { new Vector2(-2, 0), new Vector2(2, 0), new Vector2(0, -2), new Vector2(0, 2) })
            GUI.Label(new Rect(rect.x + o.x, rect.y + o.y, rect.width, rect.height), text, style);

        style.normal.textColor = Color.black;
        GUI.Label(rect, text, style);
    }

    void Restart() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    void OnGUI()
    {
        // Scale everything that is drawn here (text, bar, buttons)
        GUI.matrix = Matrix4x4.Scale(new Vector3(uiScale, uiScale, 1f));
        float w = Screen.width / uiScale;
        float h = Screen.height / uiScale;

        var tex = Texture2D.whiteTexture;
        var gold = new Color(1f, 0.85f, 0.2f);
        var label = new GUIStyle(GUI.skin.label) { fontSize = 26, fontStyle = FontStyle.Bold };
        label.normal.textColor = Color.white;

        // Health bar (top-left)
        float pct = playerHealth != null ? (float)playerHealth.Current / playerHealth.maxHealth : 0f;
        GUI.color = Color.black;
        GUI.DrawTexture(new Rect(20, 20, 224, 28), tex);
        GUI.color = Color.Lerp(Color.red, Color.green, pct);
        GUI.DrawTexture(new Rect(22, 22, 220 * pct, 24), tex);
        GUI.color = Color.white;

        GUI.Label(new Rect(20, 54, 400, 40), "Score: " + score, label);

        var goldLabel = new GUIStyle(label);
        goldLabel.normal.textColor = gold;
        GUI.Label(new Rect(20, 88, 400, 40), "Artifacts: " + artifacts + " / " + artifactsToWin, goldLabel);

        if (!isGameOver)
        {
            if (Time.timeSinceLevelLoad < taskShowSeconds) DrawTask(w, h);
            return;
        }

        GUI.color = new Color(0, 0, 0, 0.6f);
        GUI.DrawTexture(new Rect(0, 0, w, h), tex);
        GUI.color = Color.white;

        var center = new GUIStyle(label) { alignment = TextAnchor.MiddleCenter, fontSize = 48 };
        center.normal.textColor = hasWon ? gold : Color.white;
        float cx = w / 2f, cy = h / 2f;
        GUI.Label(new Rect(cx - 300, cy - 100, 600, 70), hasWon ? "YOU WIN!" : "GAME OVER", center);

        center.fontSize = 28;
        center.normal.textColor = Color.white;
        string sub = hasWon
            ? "You recovered all " + artifactsToWin + " artifacts.  Score: " + score
            : "Artifacts: " + artifacts + " / " + artifactsToWin + "   Score: " + score;
        GUI.Label(new Rect(cx - 400, cy - 30, 800, 50), sub, center);
        if (GUI.Button(new Rect(cx - 110, cy + 40, 220, 50), "Play again (R)")) Restart();
    }
}
