using UnityEngine;
using TMPro;

public class MediaPost : MonoBehaviour
{
    [SerializeField] private TMP_Text TXT_username;
    [SerializeField] private TMP_Text TXT_content;
    [SerializeField] private bool isClue;

    public void Initialize(string username, string content, string url = null)
    {
        TXT_username.text = username;
        TXT_content.text = content;
        if (url != null)
        {
            gameObject.GetComponent<LoadURLImage>().CallLoadImage(url);
        }
    }
}
