using UnityEngine;
using UnityEngine.UI;

public class HelpScreenManager : MonoBehaviour
{
    [SerializeField]
    private Button HelpOpenButton;
    [SerializeField]
    private GameObject HelpPanel;

    private bool helpMenuEnabled = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HelpOpenButton.onClick.AddListener(() => ToggleHelpMenu());
    }

    void ToggleHelpMenu()
    {
        helpMenuEnabled = !helpMenuEnabled;
        HelpPanel.SetActive(helpMenuEnabled);
    }
}
