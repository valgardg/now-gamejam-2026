using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Collections.Generic;

public class ChatterBoxDialogueController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private string jsonFileName = "chatterDialogue";

    [Header("Chat UI")]
    [SerializeField] private Transform contentParent;
    [SerializeField] private ChatMessage otherUserMessagePrefab;
    [SerializeField] private ChatMessage playerMessagePrefab;
    [SerializeField] private GameObject typingMessagePrefab;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private bool autoScrollToBottom = true;

    [Header("Options UI")]
    [SerializeField] private Button optionButtonA;
    [SerializeField] private TMP_Text optionLabelA;
    [SerializeField] private Button optionButtonB;
    [SerializeField] private TMP_Text optionLabelB;

    [Header("Behavior")]
    [SerializeField] private bool autoStartOnEnable = true;
    [SerializeField] private string playerName = "You";
    [SerializeField] private float responseDelaySeconds = 1f;

    private DialogueData dialogue;
    private Dictionary<string, DialogueNodeData> nodesById;
    private DialogueNodeData currentNode;

    private void OnEnable()
    {
        if (autoStartOnEnable) StartConversation();
    }

    public void StartConversation()
    {
        LoadDialogue();
        if (dialogue == null) return;

        ClearContent();

        if (nodesById.TryGetValue(dialogue.startNodeId, out currentNode))
        {
            RenderNode(currentNode);
        }
        else
        {
            Debug.LogError($"Start node id '{dialogue.startNodeId}' not found in {jsonFileName}.");
        }
    }

    /// <summary>
    /// Triggers a conversation starting at a specific node. Optionally, prepend a player's initial message.
    /// </summary>
    /// <param name="startNodeId">The id of the node to start from (usually the other user's first reply).</param>
    /// <param name="initialUserMessage">Optional: if provided, a player bubble is added first, then the conversation advances to startNode after a delay.</param>
    public void TriggerConversation(string startNodeId, string initialUserMessage = null)
    {
        LoadDialogue();
        if (dialogue == null) return;

        // Hide any stale options from previous state
        SetOptionVisible(optionButtonA, optionLabelA, false);
        SetOptionVisible(optionButtonB, optionLabelB, false);

        // If the player starts, add their bubble and then advance after the configured delay
        if (!string.IsNullOrEmpty(initialUserMessage))
        {
            if (playerMessagePrefab != null && contentParent != null)
                AddMessage(playerMessagePrefab, playerName, initialUserMessage);

            StartCoroutine(AdvanceAfterDelay(startNodeId));
            return;
        }

        // Otherwise, render the starting node immediately
        if (nodesById != null && nodesById.TryGetValue(startNodeId, out currentNode))
        {
            RenderNode(currentNode);
        }
        else
        {
            Debug.LogError($"Start node id '{startNodeId}' not found in {jsonFileName}.");
        }
    }

    private void ClearContent()
    {
        /*
        if (contentParent == null) return;
        for (int i = contentParent.childCount - 1; i >= 0; i--)
            Destroy(contentParent.GetChild(i).gameObject);
        */
    }

    private void LoadDialogue()
    {
        TextAsset json = Resources.Load<TextAsset>(jsonFileName);
        if (json == null)
        {
            Debug.LogError($"Dialogue JSON '{jsonFileName}.json' not found in a Resources folder.");
            return;
        }

        dialogue = JsonUtility.FromJson<DialogueData>(json.text);
        if (dialogue == null || dialogue.nodes == null || dialogue.nodes.Length == 0)
        {
            Debug.LogError($"Dialogue JSON '{jsonFileName}.json' is empty or malformed.");
            return;
        }

        nodesById = new Dictionary<string, DialogueNodeData>(dialogue.nodes.Length);
        foreach (var n in dialogue.nodes)
        {
            if (!string.IsNullOrEmpty(n.id)) nodesById[n.id] = n;
        }
    }

    private void RenderNode(DialogueNodeData node)
    {
        currentNode = node;

        // Append other user's bubble
        if (otherUserMessagePrefab != null && contentParent != null)
            AddMessage(otherUserMessagePrefab, node.speaker ?? "", node.message ?? "");

        optionButtonA.onClick.RemoveAllListeners();
        optionButtonB.onClick.RemoveAllListeners();

        SetOptionVisible(optionButtonA, optionLabelA, false);
        SetOptionVisible(optionButtonB, optionLabelB, false);

        int count = node.options != null ? node.options.Length : 0;

        if (count >= 1)
        {
            var opt = node.options[0];
            optionLabelA.text = opt.text ?? "";
            SetOptionVisible(optionButtonA, optionLabelA, true);
            optionButtonA.onClick.AddListener(() => ChooseOption(opt));
        }
        if (count >= 2)
        {
            var opt = node.options[1];
            optionLabelB.text = opt.text ?? "";
            SetOptionVisible(optionButtonB, optionLabelB, true);
            optionButtonB.onClick.AddListener(() => ChooseOption(opt));
        }
    }

    private void AddMessage(ChatMessage prefab, string sender, string message)
    {
        var bubble = Instantiate(prefab, contentParent);
        bubble.Init(sender, message);
        RefreshLayout();
    }

    private void RefreshLayout()
    {
        if (contentParent == null) return;
        StartCoroutine(RefreshLayoutNextFrame());
    }

    private IEnumerator RefreshLayoutNextFrame()
    {
        // Wait until end of frame so Unity's layout system processes additions
        yield return new WaitForEndOfFrame();

        Canvas.ForceUpdateCanvases();
        var rect = contentParent as RectTransform;
        if (rect != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }

        if (scrollRect != null && autoScrollToBottom)
        {
            // Scroll to bottom after rebuild for a chat-like feel
            scrollRect.verticalNormalizedPosition = 0f;
            Canvas.ForceUpdateCanvases();
        }
    }

    private void SetOptionVisible(Button btn, TMP_Text label, bool visible)
    {
        btn.gameObject.SetActive(visible);
        if (label != null) label.gameObject.SetActive(visible);
    }

    private void ChooseOption(DialogueOptionData option)
    {
        // Append player's bubble
        if (playerMessagePrefab != null && contentParent != null)
            AddMessage(playerMessagePrefab, playerName, option.text ?? "");

        // Disable options while waiting for next message
        SetOptionVisible(optionButtonA, optionLabelA, false);
        SetOptionVisible(optionButtonB, optionLabelB, false);

        string nextId = option.nextNodeId;
        if (string.IsNullOrEmpty(nextId))
        {
            EndConversation();
            return;
        }

        // Advance after a short delay to simulate texting
        StartCoroutine(AdvanceAfterDelay(nextId));
    }

    private IEnumerator AdvanceAfterDelay(string nextId)
    {
        GameObject typingInstance = null;
        if (typingMessagePrefab != null && contentParent != null)
        {
            typingInstance = Instantiate(typingMessagePrefab, contentParent);
            RefreshLayout();
        }

        yield return new WaitForSeconds(responseDelaySeconds);

        if (typingInstance != null)
        {
            Destroy(typingInstance);
            RefreshLayout();
        }

        if (nodesById.TryGetValue(nextId, out var next))
        {
            RenderNode(next);
        }
        else
        {
            Debug.LogWarning($"Next node id '{nextId}' not found. Ending conversation.");
            EndConversation();
        }
    }

    public void EndConversation()
    {
        SetOptionVisible(optionButtonA, optionLabelA, false);
        SetOptionVisible(optionButtonB, optionLabelB, false);
        // Do not close the application; leave the chat view as-is.
    }
}