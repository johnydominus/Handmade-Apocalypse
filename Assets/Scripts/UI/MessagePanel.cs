using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MessagePanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageHeader;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button closeButton;

    private System.Action onClose;

    public static GameObject prefab;

    private void Awake()
    {
        closeButton.onClick.AddListener(Close);
    }

    public void Initialize(string header, string message, System.Action onClose)
    {
        messageHeader.text = header;
        messageText.text = message;
        this.onClose = onClose;

        closeButton.onClick.AddListener(Close);
    }

    public static void Show(string header, string message, System.Action onCloseCallback = null)
    {
        if (prefab == null)
        {
            Debug.Log("Message prefab is not assigned");
            return;
        }
        Debug.Log("Message prefab found...");

        Canvas canvas = GameObject.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.Log("Canvas not found");
            return;
        }

        Debug.Log("Canvas found...");
        Debug.Log("Initializing message...");

        GameObject instance = Instantiate(prefab, canvas.transform);
        MessagePanel panel = instance.GetComponent<MessagePanel>();
        panel.Initialize(header, message, onCloseCallback);

        Debug.Log("Message shown...");
    }

    public void Close()
    {
        Debug.Log("Closing message...");
        onClose?.Invoke();
        Destroy(gameObject);
    }
}
