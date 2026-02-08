using System;

[Serializable]
public class DialogueOptionData
{
    public string text;
    // If empty or null, conversation ends
    public string nextNodeId;
}

[Serializable]
public class DialogueNodeData
{
    public string id;
    public string speaker;
    public string message;
    // Up to 2 options are displayed
    public DialogueOptionData[] options;
}

[Serializable]
public class DialogueData
{
    public string startNodeId;
    public DialogueNodeData[] nodes;
}