[System.Serializable]
public class SocialMediaPostData
{
    public string username;
    public string content;
    public string imageUrl;
}

[System.Serializable]
public class SocialMediaPostList
{
    public SocialMediaPostData[] posts;
}
