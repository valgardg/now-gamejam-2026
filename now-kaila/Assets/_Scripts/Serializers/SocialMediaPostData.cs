using System;

[Serializable]
public class SocialMediaPostData
{
    public string username;
    public string content;

    // New image fields
    public string profileImageUrl;       // replaces old imageUrl
    public string postImageUrl;          // optional: image shown within the post

    // Optional local resource paths (Assets/Resources/...).
    // Example: "Art/KailaProfile" to load Assets/Resources/Art/KailaProfile (Sprite)
    public string profileImageResource;
    public string postImageResource;

    public bool isCluePost;

    // Conversation metadata for clue posts
    public string conversationStartNodeId;   // e.g. "intro"
    public string initialUserMessage;        // optional, e.g. "Hey! Long time no see."
}

[Serializable]
public class SocialMediaPostList
{
    public SocialMediaPostData[] posts;
}
