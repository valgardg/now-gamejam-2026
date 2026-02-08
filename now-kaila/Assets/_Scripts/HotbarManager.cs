using UnityEngine;

public class HotbarManager : MonoBehaviour
{
    [Header("UI information")]
    public GameObject chatterBoxPanel;
    public GameObject socialMediaPanel;
    public GameObject audioSliderVisual;

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
    // Update is called once per frame
    void Update()
    {
        
    }
}
