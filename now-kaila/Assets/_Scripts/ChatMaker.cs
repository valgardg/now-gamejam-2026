using UnityEngine;

public class ChatMaker : MonoBehaviour
{
    public GameObject chatPrefab;
    [SerializeField] private Transform contentParent;
 
    public void CreateChat()
    {
        //instantiate chatPrefab within the content UI element
        Instantiate(chatPrefab, contentParent);

    }
}
