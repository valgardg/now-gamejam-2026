using UnityEngine;

public class SocialMediaPostSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MediaPost postPrefab; // fallback / legacy
    [SerializeField] private MediaPost postPrefabWithoutImage; // used when no post image is supplied
    [SerializeField] private MediaPost postPrefabWithImage;    // used when a post image is supplied
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
            // Decide which prefab to use based on presence of a post image
            bool hasPostImage =
                !string.IsNullOrEmpty(postData.postImageUrl) ||
                !string.IsNullOrEmpty(postData.postImageResource);

            MediaPost prefabToUse = null;
            if (hasPostImage && postPrefabWithImage != null)
            {
                prefabToUse = postPrefabWithImage;
            }
            else if (!hasPostImage && postPrefabWithoutImage != null)
            {
                prefabToUse = postPrefabWithoutImage;
            }
            else
            {
                // Fallback to legacy single prefab if specific ones are not assigned
                prefabToUse = postPrefab;
            }

            MediaPost postInstance = Instantiate(prefabToUse, contentParent);

            // Pass conversation metadata and image data
            postInstance.Initialize(
                postData.username,
                postData.content,
                postData.profileImageUrl,
                postData.postImageUrl,
                postData.isCluePost,
                postData.conversationStartNodeId,
                postData.initialUserMessage,
                postData.profileImageResource,
                postData.postImageResource
            );
        }
    }
}
