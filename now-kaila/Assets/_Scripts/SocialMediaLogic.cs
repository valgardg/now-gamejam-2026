using UnityEngine;
using UnityEngine.EventSystems;

public class SocialMediaLogic : MonoBehaviour, IDragHandler
{
    [Header("UI information")]
    public Canvas canvas;
    public GameObject socialMediaPanel;
    private RectTransform rectTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rectTransform = socialMediaPanel.GetComponent<RectTransform>();
    }
    public void CloseApplication()
    {
        socialMediaPanel.SetActive(false);
    }
    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
}