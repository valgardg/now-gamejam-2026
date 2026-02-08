[System.Serializable]
public class SocialMediaPostData
{
    public string username;
    public string content;
    public string imageUrl;
    public bool isCluePost;
}

[System.Serializable]
public class SocialMediaPostList
{
    public SocialMediaPostData[] posts;
}
