using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenuUI : MonoBehaviour
{
    public static SettingsMenuUI Instance { get; private set; }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    [Header("Menu")]
    public GameObject settingsMenu;
	[Header("Labels")]
	public Button audioLabel;
	public Button videoLabel;
	public Button savesLabel;
	public Button controlsLabel;
	private void Start()
	{
		audioLabel.onClick.AddListener(OpenAudioPanel);
		videoLabel.onClick.AddListener(OpenVideoPanel);
		savesLabel.onClick.AddListener(OpenSavesPanel);
		controlsLabel.onClick.AddListener(OpenControlPanel);
	}
	public void CloseAllSettingsPanels() // bad cuz hard coded unlike closeallmenus but i could care less right now
	{
		AudioPanelUI.Instance.audioPanel.SetActive(false);
		VideoPanelUI.Instance.videoPanel.SetActive(false);
		SavesPanelUI.Instance.savesPanel.SetActive(false);
		ControlsPanelUI.Instance.controlsPanel.SetActive(false);
	}
	public void OpenAudioPanel()
	{
		CloseAllSettingsPanels();
		AudioPanelUI.Instance.audioPanel.SetActive(true);
	}
	public void OpenVideoPanel()
	{
		CloseAllSettingsPanels();
		VideoPanelUI.Instance.videoPanel.SetActive(true);
	}
	public void OpenSavesPanel()
	{
		if (!GameManager.Instance.isInGame)
		{
			CloseAllSettingsPanels();
			SavesPanelUI.Instance.savesPanel.SetActive(true);
		}
	}
	public void OpenControlPanel()
	{
		CloseAllSettingsPanels();
		ControlsPanelUI.Instance.controlsPanel.SetActive(true);
	}
}
