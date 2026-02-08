using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ChatterBoxDialogueController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private string jsonFileName = "chatterDialogue";

    [Header("UI")]
    [SerializeField] private TMP_Text speakerLabel;
    [SerializeField] private TMP_Text messageLabel;
    [SerializeField] private Button optionButtonA;
    [SerializeField] private TMP_Text optionLabelA;
    [SerializeField] private Button optionButtonB;
    [SerializeField] private TMP_Text optionLabelB;

    [Header("Behavior")]
    [SerializeField] private bool autoStartOnEnable = true;

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

        if (nodesById.TryGetValue(dialogue.startNodeId, out currentNode))
        {
            RenderNode(currentNode);
        }
        else
        {
            Debug.LogError($"Start node id '{dialogue.startNodeId}' not found in {jsonFileName}.");
        }
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
        speakerLabel.text = node.speaker ?? "";
        messageLabel.text = node.message ?? "";

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

    private void SetOptionVisible(Button btn, TMP_Text label, bool visible)
    {
        btn.gameObject.SetActive(visible);
        if (label != null) label.gameObject.SetActive(visible);
    }

    private void ChooseOption(DialogueOptionData option)
    {
        string nextId = option.nextNodeId;
        if (string.IsNullOrEmpty(nextId))
        {
            EndConversation();
            return;
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
        speakerLabel.text = "";
        messageLabel.text = "";
        SetOptionVisible(optionButtonA, optionLabelA, false);
        SetOptionVisible(optionButtonB, optionLabelB, false);

        var logic = GetComponent<ChatterBoxLogic>();
        if (logic != null) logic.CloseApplication();
    }
}