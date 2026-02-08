using System;

[Serializable]
public class SocialMediaPostData
{
    public string username;
    public string content;
    public string imageUrl;
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
