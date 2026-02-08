using UnityEngine;
using TMPro;

public class ChatMessage : MonoBehaviour
{
    [SerializeField] private TMP_Text senderLabel;
    [SerializeField] private TMP_Text messageLabel;

    // Initialize a bubble with sender and message text
    public void Init(string sender, string message)
    {
        if (senderLabel != null) senderLabel.text = sender ?? "";
        if (messageLabel != null) messageLabel.text = message ?? "";
        gameObject.SetActive(true);
    }
}
