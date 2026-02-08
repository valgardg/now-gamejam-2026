using UnityEngine;
using TMPro;

public class HotbarManager : MonoBehaviour
{
    [Header("UI information")]
    public GameObject chatterBoxPanel;
    public GameObject socialMediaPanel;
    public GameObject audioSliderVisual;
    public TMP_Text clueCounter;

    private int clueCount = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        chatterBoxPanel.SetActive(true);
        socialMediaPanel.SetActive(true);
        audioSliderVisual.SetActive(false);
    }

    public void ToggleChatterBoxPanel()
    {
        chatterBoxPanel.SetActive(!chatterBoxPanel.activeSelf);
    }

    public void ToggleSocialMediaPanel()
    {
        socialMediaPanel.SetActive(!socialMediaPanel.activeSelf);
    }
    public void ToggleAudioSliderVisual()
    {
        audioSliderVisual.SetActive(!audioSliderVisual.activeSelf);
    }

   public void UpdateClueCounter(int amount)
   {
       clueCount += amount;
       clueCounter.text = $"Clues: {clueCount}";
   }
    public void QuitGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
