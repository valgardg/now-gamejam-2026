using UnityEngine;

public class SocialMediaPostSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MediaPost postPrefab;
    [SerializeField] private Transform contentParent;

    [Header("Data")]
    [SerializeField] private string jsonFileName = "socialMediaPosts";

    void Start()
    {
        SpawnPosts();
    }

    private void SpawnPosts()
    {
        TextAsset jsonAsset = Resources.Load<TextAsset>(jsonFileName);

        if (jsonAsset == null)
        {
            Debug.LogError($"Could not find {jsonFileName}.json in Resources folder");
            return;
        }

        SocialMediaPostList postList =
            JsonUtility.FromJson<SocialMediaPostList>(jsonAsset.text);

        foreach (SocialMediaPostData postData in postList.posts)
        {
            MediaPost postInstance = Instantiate(postPrefab, contentParent);

            // Pass conversation metadata for clue posts
            postInstance.Initialize(
                postData.username,
                postData.content,
                postData.imageUrl,
                postData.isCluePost,
                postData.conversationStartNodeId,
                postData.initialUserMessage
            );
        }
    }
}
