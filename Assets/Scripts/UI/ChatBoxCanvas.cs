using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class ChatBoxCanvas : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text conversationText;
    [SerializeField] private Button sendButton;
    [SerializeField] private Button confirmButton;
    [SerializeField] private ScrollRect conversationScroll;
    [SerializeField, Min(0)] private int paddingLeft = 16;
    [SerializeField, Min(0)] private int paddingRight = 16;
    [SerializeField, Min(0)] private int paddingTop = 20;
    [SerializeField, Min(0)] private int paddingBottom = 24;
    [SerializeField] private bool logScrollDebug = true;

    public event Action<string> MessageSubmitted;
    public event Action ConfirmRequested;
    public string Conversation => conversationText != null
        ? conversationText.text
        : string.Empty;

    private Coroutine scrollRefreshRoutine;

    private void OnEnable()
    {
        sendButton?.onClick.AddListener(SubmitCurrentInput);
        confirmButton?.onClick.AddListener(ConfirmCurrentInput);
        inputField?.onSubmit.AddListener(HandleInputSubmitted);
        scrollRefreshRoutine = StartCoroutine(RefreshScrollAfterLayout("OnEnable"));
    }

    private void OnDisable()
    {
        sendButton?.onClick.RemoveListener(SubmitCurrentInput);
        confirmButton?.onClick.RemoveListener(ConfirmCurrentInput);
        inputField?.onSubmit.RemoveListener(HandleInputSubmitted);
    }

    public void AppendMessage(string speaker, string message)
    {
        if (conversationText == null || string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        string prefix = string.IsNullOrWhiteSpace(speaker)
            ? string.Empty
            : $"{speaker}: ";
        string separator = string.IsNullOrEmpty(conversationText.text)
            ? string.Empty
            : "\n\n";

        conversationText.text += $"{separator}{prefix}{message.Trim()}";

        if (!isActiveAndEnabled)
        {
            return;
        }

        if (scrollRefreshRoutine != null)
        {
            StopCoroutine(scrollRefreshRoutine);
        }

        scrollRefreshRoutine = StartCoroutine(RefreshScrollAfterLayout("AppendMessage"));
    }

    public void SubmitCurrentInput()
    {
        if (inputField == null)
        {
            return;
        }

        string input = inputField.text.Trim();
        if (string.IsNullOrWhiteSpace(input))
        {
            return;
        }

        inputField.text = string.Empty;
        MessageSubmitted?.Invoke(input);
        inputField.ActivateInputField();
    }

    public void ConfirmCurrentInput()
    {
        if (confirmButton != null && !confirmButton.interactable)
        {
            return;
        }

        if (inputField != null && !string.IsNullOrWhiteSpace(inputField.text))
        {
            SubmitCurrentInput();
        }

        ConfirmRequested?.Invoke();
    }

    public void SetConfirmInteractable(bool interactable)
    {
        if (confirmButton != null)
        {
            confirmButton.interactable = interactable;
        }
    }

    public void CloseWindow()
    {
        gameObject.SetActive(false);
    }

    private void HandleInputSubmitted(string _)
    {
        SubmitCurrentInput();
    }

    private IEnumerator RefreshScrollAfterLayout(string reason)
    {
        yield return null;

        ApplyConversationPadding();
        Canvas.ForceUpdateCanvases();
        conversationText?.ForceMeshUpdate();

        if (conversationText != null)
        {
            float preferredHeight = Mathf.Ceil(conversationText.preferredHeight);
            LayoutElement textLayout = conversationText.GetComponent<LayoutElement>();
            if (textLayout == null)
            {
                textLayout = conversationText.gameObject.AddComponent<LayoutElement>();
            }

            textLayout.minHeight = preferredHeight;
            textLayout.preferredHeight = preferredHeight;
            conversationText.rectTransform.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Vertical,
                preferredHeight);
            LayoutRebuilder.ForceRebuildLayoutImmediate(conversationText.rectTransform);
        }

        if (conversationScroll != null && conversationScroll.content != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(conversationScroll.content);

            RectTransform viewport = conversationScroll.viewport != null
                ? conversationScroll.viewport
                : conversationScroll.GetComponent<RectTransform>();
            float minimumContentHeight = viewport != null
                ? viewport.rect.height
                : 0f;
            float requiredContentHeight = conversationText != null
                ? Mathf.Max(
                    minimumContentHeight,
                    conversationText.preferredHeight + GetVerticalPadding())
                : minimumContentHeight;

            if (conversationScroll.content.rect.height < requiredContentHeight)
            {
                conversationScroll.content.SetSizeWithCurrentAnchors(
                    RectTransform.Axis.Vertical,
                    requiredContentHeight);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(conversationScroll.content);
        }

        Canvas.ForceUpdateCanvases();

        if (conversationScroll != null)
        {
            conversationScroll.StopMovement();
            conversationScroll.verticalNormalizedPosition = 0f;
        }

        if (logScrollDebug)
        {
            LogScrollState(reason);
        }

        scrollRefreshRoutine = null;
    }

    private void ApplyConversationPadding()
    {
        if (conversationScroll == null || conversationScroll.content == null)
        {
            return;
        }

        VerticalLayoutGroup layoutGroup =
            conversationScroll.content.GetComponent<VerticalLayoutGroup>();
        if (layoutGroup != null)
        {
            layoutGroup.padding = new RectOffset(
                paddingLeft,
                paddingRight,
                paddingTop,
                paddingBottom);
        }
    }

    private float GetVerticalPadding()
    {
        return paddingTop + paddingBottom;
    }

    private void LogScrollState(string reason)
    {
        if (conversationText == null)
        {
            Debug.LogError("[ChatScroll] Conversation Text is not assigned.", this);
            return;
        }

        if (conversationScroll == null)
        {
            Debug.LogError(
                $"[ChatScroll] {reason}: Conversation Scroll is not assigned. " +
                $"Text object: {GetObjectPath(conversationText.transform)}",
                this);
            return;
        }

        RectTransform content = conversationScroll.content;
        RectTransform viewport = conversationScroll.viewport != null
            ? conversationScroll.viewport
            : conversationScroll.GetComponent<RectTransform>();

        if (content == null || viewport == null)
        {
            Debug.LogError(
                $"[ChatScroll] {reason}: ScrollRect references are incomplete. " +
                $"Content={content != null}, Viewport={viewport != null}.",
                this);
            return;
        }

        float contentHeight = content.rect.height;
        float viewportHeight = viewport.rect.height;
        bool textIsInsideContent = conversationText.transform.IsChildOf(content);
        bool hasOverflow = contentHeight > viewportHeight + 0.5f;
        Scrollbar scrollbar = conversationScroll.verticalScrollbar;

        Debug.Log(
            $"[ChatScroll] {reason}: " +
            $"textPath={GetObjectPath(conversationText.transform)}, " +
            $"textInsideContent={textIsInsideContent}, " +
            $"preferredTextHeight={conversationText.preferredHeight:F1}, " +
            $"textRectHeight={conversationText.rectTransform.rect.height:F1}, " +
            $"contentHeight={contentHeight:F1}, viewportHeight={viewportHeight:F1}, " +
            $"hasOverflow={hasOverflow}, verticalEnabled={conversationScroll.vertical}, " +
            $"normalizedY={conversationScroll.verticalNormalizedPosition:F2}, " +
            $"scrollbarAssigned={scrollbar != null}, " +
            $"scrollbarActive={(scrollbar != null && scrollbar.gameObject.activeInHierarchy)}, " +
            $"scrollbarSize={(scrollbar != null ? scrollbar.size : -1f):F2}, " +
            $"visibility={conversationScroll.verticalScrollbarVisibility}.",
            this);
    }

    private static string GetObjectPath(Transform target)
    {
        string path = target.name;
        while (target.parent != null)
        {
            target = target.parent;
            path = $"{target.name}/{path}";
        }

        return path;
    }
}
