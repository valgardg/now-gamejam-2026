using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MediaPost : MonoBehaviour
{
    [SerializeField] private TMP_Text TXT_username;
    [SerializeField] private TMP_Text TXT_content;
    [SerializeField] private bool isClue = false;

    // Button on a child object that users click
    [SerializeField] private Button clickButton;

    // Dialogue system reference (optional; will fallback to FindObjectOfType)
    [SerializeField] private ChatterBoxDialogueController dialogueController;

    // Conversation metadata for clue posts
    private string conversationStartNodeId;
    private string initialUserMessage;

    public void Initialize(
        string username,
        string content,
        string url = null,
        bool isCluePost = false,
        string conversationStartNodeId = null,
        string initialUserMessage = null
    )
    {
        TXT_username.text = username;
        TXT_content.text = content;
        isClue = isCluePost;

        this.conversationStartNodeId = conversationStartNodeId;
        this.initialUserMessage = initialUserMessage;

        if (url != null)
        {
            gameObject.GetComponent<LoadURLImage>().CallLoadImage(url);
        }

        // Wire button click once on spawn
        if (clickButton != null)
        {
            clickButton.onClick.RemoveAllListeners();
            clickButton.onClick.AddListener(OnClicked);
        }
    }

    private void OnClicked()
    {
        if (!isClue)
        {
            Debug.Log("I am a regular post");
            return;
        }

        // Ensure the chatter box panel is visible (optional UX)
        var hotbar = FindObjectOfType<HotbarManager>();
        if (hotbar != null && hotbar.chatterBoxPanel != null)
        {
            hotbar.chatterBoxPanel.SetActive(true);
        }

        // Resolve controller and trigger the conversation
        var controller = dialogueController ?? FindObjectOfType<ChatterBoxDialogueController>();
        if (controller == null)
        {
            Debug.LogWarning("ChatterBoxDialogueController not found in scene.");
            return;
        }

        if (string.IsNullOrEmpty(conversationStartNodeId))
        {
            Debug.LogWarning("Clue post missing conversationStartNodeId.");
            return;
        }

        controller.TriggerConversation(conversationStartNodeId, initialUserMessage);

        // Count the clue discovery
        hotbar?.UpdateClueCounter(1);
        Debug.Log("I am a clue");
    }

    // Back-compat if other components still call this
    public void wasClicked()
    {
        OnClicked();
    }
}
