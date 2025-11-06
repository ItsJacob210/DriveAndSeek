using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
//manages countdown timer and displays win state for splitscreen ui

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public int timerCount = 0;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI timerTextRight;
    public TextMeshProUGUI resultTextRight;
    //center overlay elements shown at end of game
    public GameObject endOverlayRoot; //assign a panel or canvas group root; kept inactive by default
    public TextMeshProUGUI centerResultText; //big center line: "Cop Wins!" or "Robber Wins!"
    private bool gameEnded;
    
    //start is called once before the first execution of update after the monobehaviour is created
    void Start()
    {
        if (timerText != null) timerText.text = timerCount.ToString();
        if (timerTextRight != null) timerTextRight.text = timerCount.ToString();
        InvokeRepeating("CountDownTimer", 1, 1);
    }

    //update is called once per frame
    void Update()
    {

    }

    private void CountDownTimer()
    {
        if (gameEnded) { CancelInvoke(); return; }

        timerCount--;
        if (timerCount == 0)
        {
            RobberWins();
            CancelInvoke();
        }
        if (timerText != null) timerText.text = timerCount.ToString();
        if (timerTextRight != null) timerTextRight.text = timerCount.ToString();
    }

    public void CopWins()
    {
        if (gameEnded) return;
        gameEnded = true;
        Time.timeScale = 0f;
        if (resultText != null) { resultText.text = "you win"; resultText.gameObject.SetActive(true); }
        if (resultTextRight != null) { resultTextRight.text = "you lose"; resultTextRight.gameObject.SetActive(true); }
        if (centerResultText != null) centerResultText.text = "Cop Wins!";
        if (endOverlayRoot != null) endOverlayRoot.SetActive(true);
    }

    public void RobberWins()
    {
        if (gameEnded) return;
        gameEnded = true;
        Time.timeScale = 0f;
        if (resultText != null) { resultText.text = "you lose"; resultText.gameObject.SetActive(true); }
        if (resultTextRight != null) { resultTextRight.text = "you win"; resultTextRight.gameObject.SetActive(true); }
        if (centerResultText != null) centerResultText.text = "Robber Wins!";
        if (endOverlayRoot != null) endOverlayRoot.SetActive(true);
    }

    public void RestartToStart()
    {
        //reloads current scene so the start overlay appears again
        Time.timeScale = 1f;
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
