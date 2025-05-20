using UnityEngine;
using UnityEngine.UI;

public class AsteroidPanelOpener : MonoBehaviour
{
    // Reference to the panel prefab
    [SerializeField] private GameObject asteroidPanelPrefab;

    // Reference to the panel instance (will be instantiated)
    private GameObject panelInstance;

    private void Start()
    {
        // Get or add button component
        Button button = GetComponent<Button>();
        if (button == null)
        {
            button = gameObject.AddComponent<Button>();
            Debug.Log("Added Button component to Asteroid threat bar");
        }

        // Clear any existing listeners and add our own
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OpenPanel);

        Debug.Log("AsteroidPanelOpener initialized on " + gameObject.name);
    }

    public void OpenPanel()
    {
        Debug.Log("OpenPanel method called!");

        // First, try to find an existing panel
        AsteroidCounteractionPanel existingPanel = FindFirstObjectByType<AsteroidCounteractionPanel>();

        if (existingPanel != null)
        {
            Debug.Log("Found existing panel - opening it");
            existingPanel.OpenPanel();
            return;
        }

        // If no existing panel and we have a prefab, instantiate it
        if (asteroidPanelPrefab != null)
        {
            Debug.Log("Instantiating panel from prefab");

            // Destroy previous instance if it exists
            if (panelInstance != null)
            {
                Destroy(panelInstance);
            }

            // Find the canvas to parent to
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("No Canvas found in scene!");
                return;
            }

            // Instantiate the panel
            panelInstance = Instantiate(asteroidPanelPrefab, canvas.transform);

            // Get the panel component and open it
            AsteroidCounteractionPanel panel = panelInstance.GetComponent<AsteroidCounteractionPanel>();
            if (panel != null)
            {
                panel.OpenPanel();
            }
            else
            {
                Debug.LogError("Instantiated prefab doesn't have AsteroidCounteractionPanel component!");
            }
        }
        else
        {
            Debug.LogError("No panel prefab assigned and no existing panel found!");
        }
    }
}
