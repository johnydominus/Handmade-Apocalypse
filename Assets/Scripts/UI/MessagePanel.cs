using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class MessagePanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageHeader;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button closeButton;
    [SerializeField] private Image sphereIcon;
    [SerializeField] private float borderThickness = 4f;

    private System.Action onClose;

    public static GameObject prefab;

    // Static queue to manage message panels
    private static Queue<MessageData> messageQueue = new Queue<MessageData>();
    private static bool isProcessingQueue = false;

    // Flag to track if a panel is currently visible
    private static bool isPanelVisible = false;

    // For debugging
    private static int queueCounter = 0;
    private static int processCounter = 0;

    private void Awake()
    {
        closeButton.onClick.AddListener(Close);
    }

    public void Initialize(string header, string message, System.Action onClose, CardData card = null)
    {
        messageHeader.text = header;
        messageText.text = message;
        this.onClose = onClose;

        if (card != null)
        {
            // Apply sphere background color to this panel's own Image
            var background = GetComponent<Image>();
            if (background != null)
                background.color = CardVisualConfig.GetSphereColor(card.sphereType);

            // Apply polarity border via Outline component
            var outline = GetComponent<Outline>();
            if (outline == null)
                outline = gameObject.AddComponent<Outline>();
            outline.effectColor = CardVisualConfig.GetPolarityColor(card.cardPolarity);
            outline.effectDistance = new Vector2(borderThickness, borderThickness);
            outline.enabled = true;

            // Apply sphere icon
            if (sphereIcon != null && GameServices.Instance != null && GameServices.Instance.sphereIconConfig != null)
            {
                Sprite icon = GameServices.Instance.sphereIconConfig.GetIconForSphere(card.sphereType);
                if (icon != null)
                {
                    sphereIcon.sprite = icon;
                    sphereIcon.enabled = true;
                }
                else
                {
                    sphereIcon.enabled = false;
                }
            }
        }
        else
        {
            // Hide sphere icon for non-card messages
            if (sphereIcon != null)
                sphereIcon.enabled = false;
        }
    }

    // Structure to hold message data in the queue
    private struct MessageData
    {
        public string header;
        public string message;
        public System.Action onCloseCallback;
        public int queueId;
        public CardData card;

        public MessageData(string header, string message, System.Action onCloseCallback, int queueId, CardData card = null)
        {
            this.header = header;
            this.message = message;
            this.onCloseCallback = onCloseCallback;
            this.queueId = queueId;
            this.card = card;
        }
    }

    public static void Show(string header, string message, System.Action onCloseCallback = null)
    {
        Show(header, message, null, onCloseCallback);
    }

    public static void Show(string header, string message, CardData card, System.Action onCloseCallback)
    {
        int id = ++queueCounter;
        Debug.Log($"[MSG #{id}] Queueing message: '{header}'");

        // Skip empty messages but maintain their callbacks in sequence
        if (string.IsNullOrEmpty(header) && string.IsNullOrEmpty(message))
        {
            Debug.Log($"[MSG #{id}] Empty message, queueing silent callback");
            messageQueue.Enqueue(new MessageData("", "", onCloseCallback, id));
        }
        else
        {
            messageQueue.Enqueue(new MessageData(header, message, onCloseCallback, id, card));
        }

        // Start processing the queue if not already doing so
        if (!isProcessingQueue)
        {
            isProcessingQueue = true;
            ProcessMessageQueue();
        }
    }

    private static void ProcessMessageQueue()
    {
        if (isPanelVisible)
        {
            // Wait for current panel to close before processing next
            Debug.Log("Panel still visible, waiting for close");
            return;
        }

        if (messageQueue.Count == 0)
        {
            Debug.Log("Message queue empty, stopping processing");
            isProcessingQueue = false;
            return;
        }

        MessageData data = messageQueue.Dequeue();
        int processId = ++processCounter;

        Debug.Log($"[MSG #{data.queueId}] Processing message #{processId}: '{data.header}'");

        // Handle silent messages (no UI)
        if (string.IsNullOrEmpty(data.header) && string.IsNullOrEmpty(data.message))
        {
            Debug.Log($"[MSG #{data.queueId}] Executing silent callback for #{processId}");
            data.onCloseCallback?.Invoke();
            ProcessMessageQueue(); // Continue to next message
            return;
        }

        if (prefab == null)
        {
            Debug.LogError($"[MSG #{data.queueId}] Message prefab is not assigned!");
            data.onCloseCallback?.Invoke(); // Still execute callback
            ProcessMessageQueue(); // Continue to next message
            return;
        }

        Canvas canvas = GameObject.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError($"[MSG #{data.queueId}] Canvas not found!");
            data.onCloseCallback?.Invoke(); // Still execute callback
            ProcessMessageQueue(); // Continue to next message
            return;
        }

        isPanelVisible = true;
        Debug.Log($"[MSG #{data.queueId}] Creating panel for #{processId}: '{data.header}'");

        GameObject instance = Instantiate(prefab, canvas.transform);
        MessagePanel panel = instance.GetComponent<MessagePanel>();

        // Wrap the original callback
        System.Action wrappedCallback = () => {
            Debug.Log($"[MSG #{data.queueId}] Executing callback for #{processId}: '{data.header}'");

            try
            {
                // Execute the original callback
                data.onCloseCallback?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error in message callback: {e.Message}\n{e.StackTrace}");
            }

            isPanelVisible = false;

            // Process next message with a small delay to prevent frame issues
            Debug.Log($"[MSG #{data.queueId}] Scheduling next message after #{processId}");
            GameObject delayObject = new GameObject("MessageDelayHandler");
            DelayedCallback delayComponent = delayObject.AddComponent<DelayedCallback>();
            delayComponent.Setup(ProcessMessageQueue, 0.1f);
        };

        panel.Initialize(data.header, data.message, wrappedCallback, data.card);
    }

    public void Close()
    {
        Debug.Log($"Closing message: '{messageHeader.text}'");

        // Prevent multiple closure calls
        closeButton.onClick.RemoveAllListeners();

        if (onClose != null)
        {
            onClose.Invoke();
            onClose = null; // Prevent double invocation
        }

        Destroy(gameObject);
    }

    // Helper component to add small delays between message processing
    private class DelayedCallback : MonoBehaviour
    {
        private Action callback;
        private float delay;
        private float timer = 0;

        public void Setup(Action callback, float delay)
        {
            this.callback = callback;
            this.delay = delay;
        }

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= delay)
            {
                callback?.Invoke();
                Destroy(gameObject);
            }
        }
    }
}
