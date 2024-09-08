using UnityEngine;
using UnityEngine.UI;

public class PanelControl : MonoBehaviour
{
    public GameObject panel;  // The panel to hide

    void Start()
    {
        // Ensure the button is properly linked
        Button Button = panel.GetComponentInChildren<Button>();
        if (Button != null)
        {
            Button.onClick.AddListener(HidePanel);
        }
    }

    // This method will hide the panel when called
    public void HidePanel()
    {
        panel.SetActive(false);  // Hide the panel
    }
}

