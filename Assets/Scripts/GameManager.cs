using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public int timerCount = 0;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI timerTextRight;
    public TextMeshProUGUI resultTextRight;
    private bool gameEnded;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (timerText != null) timerText.text = timerCount.ToString();
        if (timerTextRight != null) timerTextRight.text = timerCount.ToString();
        InvokeRepeating("CountDownTimer", 1, 1);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void CountDownTimer()
    {
        if (gameEnded) { CancelInvoke(); return; }

        timerCount--;
        if (timerCount == 0)
        {
            print("Time Out");
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
        if (resultText != null) { resultText.text = "Cop Wins!"; resultText.gameObject.SetActive(true); }
        if (resultTextRight != null) { resultTextRight.text = "Cop Wins!"; resultTextRight.gameObject.SetActive(true); }
    }
}
