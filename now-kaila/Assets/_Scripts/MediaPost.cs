using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Networking;

public class MediaPost : MonoBehaviour
{
    [SerializeField] private TMP_Text TXT_username;
    [SerializeField] private TMP_Text TXT_content;
    [SerializeField] private bool isClue = false;

    // New: separate image targets
    [SerializeField] private Image profileImageTarget;
    [SerializeField] private Image postImageTarget;

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
        string profileImageUrl = null,
        string postImageUrl = null,
        bool isCluePost = false,
        string conversationStartNodeId = null,
        string initialUserMessage = null,
        string profileImageResource = null,
        string postImageResource = null
    )
    {
        TXT_username.text = username;
        TXT_content.text = content;
        isClue = isCluePost;

        this.conversationStartNodeId = conversationStartNodeId;
        this.initialUserMessage = initialUserMessage;

        // Load profile image (resource preferred)
        SetImageFromResourceOrUrl(profileImageTarget, profileImageResource, profileImageUrl);

        // Load post image (resource preferred)
        SetImageFromResourceOrUrl(postImageTarget, postImageResource, postImageUrl);

        // Wire button click once on spawn
        if (clickButton != null)
        {
            clickButton.onClick.RemoveAllListeners();
            clickButton.onClick.AddListener(OnClicked);
        }
    }

    private void SetImageFromResourceOrUrl(Image target, string resourcePath, string url)
    {
        if (target == null) return;

        // Prefer local resource if provided
        if (!string.IsNullOrEmpty(resourcePath))
        {
            var sprite = Resources.Load<Sprite>(resourcePath);
            if (sprite != null)
            {
                target.sprite = sprite;
                return;
            }
            else
            {
                Debug.LogWarning($"MediaPost: Resource sprite not found at '{resourcePath}'. Falling back to URL, if any.");
            }
        }

        // Fallback to URL
        if (!string.IsNullOrEmpty(url))
        {
            StartCoroutine(LoadImageFromURL(target, url));
        }
    }

    IEnumerator LoadImageFromURL(Image target, string url)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
                yield break;
            }

            Texture2D texture = DownloadHandlerTexture.GetContent(request);

            Sprite sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f)
            );

            target.sprite = sprite;
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
