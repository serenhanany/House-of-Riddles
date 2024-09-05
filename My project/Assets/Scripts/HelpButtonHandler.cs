using UnityEngine;
using UnityEngine.UI;

public class HelpButtonHandler : MonoBehaviour
{
    public GameObject helpLabel;  // Drag the Text label here in the inspector

    // This function is called when the Help button is clicked
    public void OnHelpButtonClick()
    {
        // Toggle the label visibility
        helpLabel.SetActive(!helpLabel.activeSelf);
    }
}
