using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
	public static PopupManager Instance { get; private set; }
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

	public bool isPopupOpen = false;

	public GameObject popup;
	public GameObject nameInput;
	private Canvas canvas;

    Color32 quitRed = new(138, 8, 25, 255);
    Color32 playBlue = new(62, 138, 171, 255);

    // Empty variables in reference to the instantiated popup
    private GameObject instantiatedPopup;
	private GameObject Header;
	private TMP_Text header;
	private GameObject Body;
	private TMP_Text information;
	private GameObject Footer;
	private Button actionBtn;
	private Image actionBtnImage;
	private TMP_Text actionBtnText;
	private Button cancelBtn;
	private TMP_Text cancelBtnText;
	private Image line;

	public enum PopupType
	{
		QuitGameConfirm,
		StartReturnConfirm,
		DeleteSaveConfirm,
		PlaySaveConfirm,
		CreateSavePopup,
		QuitWithoutSavingConfirm
	}

	public void ShowPopup(PopupType popupType)
	{
		if(instantiatedPopup == null)
		{
			AssignPopupObjects();

			RectTransform rectTransform = instantiatedPopup.GetComponent<RectTransform>();
			rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
			rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
			rectTransform.pivot = new Vector2(0.5f, 0.5f);
			rectTransform.anchoredPosition = Vector2.zero;

			switch (popupType)
			{
				case PopupType.QuitGameConfirm:
					isPopupOpen = true;

					header.text = "Quit game?";
					information.text = "You will lose any unsaved progress.";
					actionBtnText.text = "Quit";
					actionBtnImage.color = quitRed;

					actionBtn.onClick.AddListener(() =>
					{
						if (SavesPanelUI.Instance.onExitSave && GameManager.Instance.isInGame)
						{
							GameManager.Instance.SaveGame(GameManager.Instance.currentSave);
						}

						GameManager.Instance.QuitGame();
					});
					cancelBtn.onClick.AddListener(() => ClosePopup());
					break;
				case PopupType.StartReturnConfirm:
					isPopupOpen = true;

					header.text = "Return to Start Menu?";
					information.text = "You will lose any unsaved progress.";
					actionBtnText.text = "Quit";
					actionBtnImage.color = quitRed;

					actionBtn.onClick.AddListener(() =>
					{
						if (SavesPanelUI.Instance.onExitSave)
						{
							GameManager.Instance.SaveGame(GameManager.Instance.currentSave);
							Debug.Log("awhh");
						}
						Debug.Log("haha");
						UIManager.Instance.SwitchToStart();
						ClosePopup();
					});
					cancelBtn.onClick.AddListener(() => ClosePopup());
					break;
				case PopupType.DeleteSaveConfirm:
					isPopupOpen = true;

					header.text = $"Delete save? ({SavesPanelUI.Instance.currentSave})";
					information.text = "This cannot be undone.";
					actionBtnText.text = "Delete";
					actionBtnImage.color = quitRed;

					actionBtn.onClick.AddListener(() =>
					{
						if (GameManager.Instance.currentSave == GameManager.Instance.defaultSave)
						{
							StartMenuUI.Instance.playDefaultText.text = "Play default save \n[No default save!]";
							GameManager.Instance.defaultSave = "";
							GameManager.Instance.currentSave = "";
							SaveSystem.SetDefaultSave("");
						}
						GameManager.Instance.DeleteSave();
						SavesPanelUI.Instance.InstantiateSaveButtons();
						SavesPanelUI.Instance.UpdateDeleteSaveButton();
						ClosePopup();
					});
					cancelBtn.onClick.AddListener(() => ClosePopup());
					break;
				case PopupType.PlaySaveConfirm:
					isPopupOpen = true;

					header.text = $"Play selected save? ({SavesPanelUI.Instance.currentSave})";

					information.text = "This will immediately start the game.";
					actionBtnText.text = "Play";
					actionBtnImage.color = playBlue;

					actionBtn.onClick.AddListener(() =>
					{
						GameManager.Instance.StartExistingGame();
						ClosePopup();
					});
					cancelBtn.onClick.AddListener(() => ClosePopup());
					break;
				case PopupType.CreateSavePopup:
					isPopupOpen = true;

					rectTransform.sizeDelta = new(500, 350);

					GameObject nameInputField = Instantiate(nameInput);
					nameInputField.transform.SetParent(Body.transform);
					nameInputField.transform.position = new(10, 0, 0);

					RectTransform nameInputRectTransform = nameInputField.GetComponent<RectTransform>();
					nameInputRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
					nameInputRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
					nameInputRectTransform.pivot = new Vector2(0.5f, 0.5f);
					nameInputRectTransform.anchoredPosition = Vector2.zero;
					nameInputRectTransform.localScale = new(1, 1, 1);

					RectTransform informationRect = information.GetComponent<RectTransform>();
					informationRect.position = new(0, 70, 0);
					RectTransform lineRect = line.GetComponent<RectTransform>();
					lineRect.position = new(0, -100, 0);
					RectTransform headerTextRect = header.GetComponent<RectTransform>();
					headerTextRect.sizeDelta = new(500, 0);
					RectTransform bodyTextRect = information.GetComponent<RectTransform>();
					bodyTextRect.anchorMin = new Vector2(0.5f, 0.5f);
					bodyTextRect.anchorMax = new Vector2(0.5f, 0.5f);
					bodyTextRect.pivot = new Vector2(0.5f, 0.5f);
					bodyTextRect.anchoredPosition = new(0, 70);
					bodyTextRect.localScale = new(1, 1, 1);
					bodyTextRect.sizeDelta = new(300, 50);
					RectTransform actionBtnRect = actionBtn.GetComponent<RectTransform>();
					actionBtnRect.anchorMin = new Vector2(0.5f, 0.5f);
					actionBtnRect.anchorMax = new Vector2(0.5f, 0.5f);
					actionBtnRect.pivot = new Vector2(0.5f, 0.5f);
					actionBtnRect.anchoredPosition = new(100, 0);
					RectTransform cancelBtnRect = cancelBtn.GetComponent<RectTransform>();
					cancelBtnRect.anchorMin = new Vector2(0.5f, 0.5f);
					cancelBtnRect.anchorMax = new Vector2(0.5f, 0.5f);
					cancelBtnRect.pivot = new Vector2(0.5f, 0.5f);
					cancelBtnRect.anchoredPosition = new(-100, 0);


					TMP_InputField nameField = Body.GetComponentInChildren<TMP_InputField>();
					string saveName = nameField.text;

					header.text = "Create new save name";
					header.fontSize = 36;
					header.alignment = TextAlignmentOptions.Center;
					information.text = "Enter name for your new save here";
					information.alignment = TextAlignmentOptions.Center;
					actionBtnText.text = "Play";
					actionBtnImage.color = playBlue;

					actionBtn.onClick.AddListener(() =>
					{
						saveName = nameField.text;

						GameManager.Instance.currentSave = saveName;

						//Debug.Log(SaveSystem.FindSavesBool(saveName));

						if (!string.IsNullOrEmpty(saveName) && !SaveSystem.FindSavesBool(saveName))
						{
							GameManager.Instance.CreateSave(saveName);
							GameManager.Instance.StartNewGame();
							ClosePopup();
						}
						else
						{
							StartCoroutine(StartMenuUI.Instance.SaveNameWarning());
							ClosePopup();
						}
					});
					cancelBtn.onClick.AddListener(() => ClosePopup());
					break;
				case PopupType.QuitWithoutSavingConfirm:
					isPopupOpen = true;

					header.text = "Return without saving?";

					information.text = "You have unsaved changes.";
					actionBtnText.text = "Discard Changes";
					cancelBtnText.text = "Keep Changes";

					actionBtn.onClick.AddListener(() =>
					{
						UIManager.Instance.SwitchToStart();
						GameManager.Instance.LoadSettings();
						SettingsMenuUI.Instance.didSaveSettings = false;
						SettingsMenuUI.Instance.didModifySettings = false;
						ClosePopup();
					});
					cancelBtn.onClick.AddListener(() =>
					{
						UIManager.Instance.SwitchToStart();
						GameManager.Instance.SaveSettings();
						SettingsMenuUI.Instance.didSaveSettings = false;
						SettingsMenuUI.Instance.didModifySettings = false;
						ClosePopup();
					});
					break;
			}
		}
	}

	public void ClosePopup()
	{
		Destroy(instantiatedPopup);
		isPopupOpen = false;

	}

	void AssignPopupObjects()
	{
		canvas = GameObject.Find("Settings Canvas").GetComponent<Canvas>();
		instantiatedPopup = Instantiate(popup, canvas.transform);

		Header = instantiatedPopup.transform.Find("Header").gameObject;
		header = Header.GetComponentInChildren<TMP_Text>();

		Body = instantiatedPopup.transform.Find("Body").gameObject;
		information = Body.GetComponentInChildren<TMP_Text>();

		Footer = instantiatedPopup.transform.Find("Footer").gameObject;
		actionBtn = Footer.transform.Find("Action Button").GetComponent<Button>();
		actionBtnImage = actionBtn.GetComponent<Image>();
		actionBtnText = actionBtn.GetComponentInChildren<TMP_Text>();
		cancelBtn = Footer.transform.Find("Cancel Button").GetComponent<Button>();
		cancelBtnText = cancelBtn.GetComponentInChildren<TMP_Text>();
		line = Body.transform.Find("Image").GetComponent<Image>();
	}
}
