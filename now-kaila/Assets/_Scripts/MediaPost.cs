using UnityEngine;
using TMPro;

public class MediaPost : MonoBehaviour
{
    [SerializeField] private TMP_Text TXT_username;
    [SerializeField] private TMP_Text TXT_content;
    [SerializeField] private bool isClue = false;

    public void Initialize(string username, string content, string url = null, bool isCluePost = false)
    {
        TXT_username.text = username;
        TXT_content.text = content;
        isClue = isCluePost;
        if (url != null)
        {
            gameObject.GetComponent<LoadURLImage>().CallLoadImage(url);
        }
    }
}
