using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class LoadURLImage : MonoBehaviour
{
    [SerializeField] private string imageUrl = "https://cdn.pixabay.com/photo/2015/10/05/22/37/blank-profile-picture-973460_1280.png";
    [SerializeField] private Image targetImage;

    // New: optional auto loading toggle (disabled by default)
    [SerializeField] private bool autoLoadOnStart = false;

    void Start()
    {
        if (autoLoadOnStart && targetImage != null && !string.IsNullOrEmpty(imageUrl))
        {
            StartCoroutine(LoadImageTo(targetImage, imageUrl));
        }
    }

    public void CallLoadImage(string imageUrl)
    {
        if (targetImage == null)
        {
            Debug.LogWarning("LoadURLImage: targetImage is null.");
            return;
        }
        StartCoroutine(LoadImageTo(targetImage, imageUrl));
    }

    // New: load into an arbitrary Image target
    public void CallLoadImageTo(Image target, string imageUrl)
    {
        if (target == null)
        {
            Debug.LogWarning("LoadURLImage: provided target Image is null.");
            return;
        }
        StartCoroutine(LoadImageTo(target, imageUrl));
    }

    IEnumerator LoadImageTo(Image target, string url)
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
            Debug.Log("Updated target image!");
        }
    }
}
