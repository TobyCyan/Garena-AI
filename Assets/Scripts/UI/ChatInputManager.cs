using System;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class ChatInputManager : MonoBehaviour
{
    [SerializeField] private ChatBoxCanvas chatBox;
    [SerializeField] private GameManager gameManager;

    public event Action<string> FinalizedInputSubmitted;
    public string LastFinalizedInput { get; private set; }
    public string LastAiReply { get; private set; }

    private void Awake()
    {
        chatBox ??= GetComponent<ChatBoxCanvas>();
        gameManager ??= FindAnyObjectByType<GameManager>();
    }

    private void OnEnable()
    {
        if (chatBox == null)
        {
            Debug.LogError("ChatInputManager needs a ChatBoxCanvas reference.", this);
            return;
        }

        chatBox.MessageSubmitted += HandleMessageSubmitted;
        chatBox.ConfirmRequested += HandleConfirmRequested;
    }

    private void OnDisable()
    {
        if (chatBox == null)
        {
            return;
        }

        chatBox.MessageSubmitted -= HandleMessageSubmitted;
        chatBox.ConfirmRequested -= HandleConfirmRequested;
    }

    private void HandleMessageSubmitted(string input)
    {
        chatBox.AppendMessage("You", input);

        if (!TryResolveGameManager())
        {
            Debug.LogError(
                "ChatInputManager needs a GameManager to request an AI reply.",
                this);
            return;
        }

        gameManager.RequestChatReply(input, ReceiveAiReply);
    }

    public void ReceiveAiReply(string reply)
    {
        if (string.IsNullOrWhiteSpace(reply))
        {
            return;
        }

        LastAiReply = reply.Trim();
        chatBox.AppendMessage("AI", LastAiReply);
    }

    private void HandleConfirmRequested()
    {
        if (string.IsNullOrWhiteSpace(LastAiReply))
        {
            chatBox.AppendMessage("AI", "Describe a weapon before confirming.");
            return;
        }

        LastFinalizedInput = chatBox.Conversation.Trim();
        FinalizedInputSubmitted?.Invoke(LastFinalizedInput);

        if (!TryResolveGameManager())
        {
            Debug.LogError(
                "ChatInputManager needs a GameManager before it can execute pipelines.",
                this);
            return;
        }

        gameManager.ExecutePipelines(LastFinalizedInput);
        Debug.Log($"Finalized conversation: {LastFinalizedInput}", this);
        chatBox.AppendMessage("System", "Weapon request confirmed.");
    }

    private bool TryResolveGameManager()
    {
        gameManager ??= FindAnyObjectByType<GameManager>();
        return gameManager != null;
    }
}
