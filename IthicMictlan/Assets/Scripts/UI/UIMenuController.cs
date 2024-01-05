using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

/// <summary>
/// Clase encargada de controlar los canvas de la UI
/// </summary>
public class UIMenuController : MonoBehaviour
{
    bool canMove = true;
    float degreesMainMenu = 0;

    float degreesPlayOptions = 0;

    // Canvas Gropus
    [Header("All canvas groups in the main menu")]
    [SerializeField] private CanvasGroup mainMenuPanel;
    [SerializeField] private CanvasGroup playOptionsPanel;
    [SerializeField] private CanvasGroup configPanel;
    [SerializeField] private CanvasGroup creditsPanel;
    [SerializeField] private CanvasGroup roomBrowserPanel;
    [SerializeField] private CanvasGroup joinedRoomPanel;
    [SerializeField] private CanvasGroup errorPanel;
    [SerializeField] private CanvasGroup loadingPanel;
    [SerializeField] private CanvasGroup createRoomPanel;
    [SerializeField] private CanvasGroup nickNamePanel;

    // Textfields
    [Header("Textfields")]
    [SerializeField] private TMP_InputField roomNameInput;
    [SerializeField] private TMP_InputField nickNameInput;
    
    // Plain texts
    [Header("Plain texts")]
    [SerializeField] private TMP_Text loadingText;
    [SerializeField] private TMP_Text joinedNameText;
    [SerializeField] private TMP_Text errorText;

    // Buttons
    [Header("Buttons")]
    [SerializeField] private GameObject startGameButton;
    [SerializeField] private GameObject mainMenuButtonsGroup;
    [SerializeField] private GameObject playOptionsButtonsGroup;
    [SerializeField] private Button[] buttonList;





    /// <summary>
    /// Inicializa los valores de todos los paneles
    /// </summary>
    public void InitGameScreen()
    {
        startGameButton.SetActive(false);

        // Esconder todos los canvas groups
        mainMenuPanel.alpha = 1f;
        mainMenuPanel.interactable = true;
        mainMenuPanel.blocksRaycasts = true;

        playOptionsPanel.alpha = 0f;
        playOptionsPanel.interactable = false;
        playOptionsPanel.blocksRaycasts = false;

        configPanel.alpha = 0f;
        configPanel.interactable = false;
        configPanel.blocksRaycasts = false;

        creditsPanel.alpha = 0f;
        creditsPanel.interactable = false;
        creditsPanel.blocksRaycasts = false;

        roomBrowserPanel.alpha = 0f;
        roomBrowserPanel.interactable = false;
        roomBrowserPanel.blocksRaycasts = false;

        joinedRoomPanel.alpha = 0f;
        joinedRoomPanel.interactable = false;
        joinedRoomPanel.blocksRaycasts = false;

        errorPanel.alpha = 0f;
        errorPanel.interactable = false;
        errorPanel.blocksRaycasts = false;

        loadingPanel.alpha = 0f;
        loadingPanel.interactable = false;
        loadingPanel.blocksRaycasts = false;

        createRoomPanel.alpha = 0f;
        createRoomPanel.interactable = false;
        createRoomPanel.blocksRaycasts = false;

        nickNamePanel.alpha = 0f;
        nickNamePanel.interactable = false;
        nickNamePanel.blocksRaycasts = false;

        loadingText.text = "Cargando...";
    }

    public void SetLoadingText(string text) => loadingText.text = text;
    public string GetCreateRoomName() => roomNameInput.text;
    public string GetPlayerNickname() => nickNameInput.text;
    public void ActivateStartGameButton(bool status) => startGameButton.SetActive(status);
    public void SetJoinedRoomName(string roomName) => joinedNameText.text = "Room : " + roomName;
    public void SetErrorText(string errorName) => errorText.text = errorName;
    public void SetPlayerNickname(string nickname) => nickNameInput.text = nickname;

    public void ShowCanvasGroup(CanvasGroup canvasGroup)
    {
        LeanTween.alphaCanvas(canvasGroup, 1f, 0.5f).setOnComplete(() =>
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        });
    }

    public void HideCanvasGroup(CanvasGroup canvasGroup)
    {
        LeanTween.alphaCanvas(canvasGroup, 0f, 0.5f).setOnComplete(() =>
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        });
    }

    public CanvasGroup GetCanvasGroup(string canvasGroup)
    {
        CanvasGroup CGtoSend = new CanvasGroup();
        if(canvasGroup == "mainMenuPanel")
        {
            CGtoSend = mainMenuPanel;
        }
        else if(canvasGroup == "playOptionsPanel")
        {
            CGtoSend = playOptionsPanel;
        }
        else if(canvasGroup == "configPanel")
        {
            CGtoSend = configPanel;
        }
        else if(canvasGroup == "creditsPanel")
        {
            CGtoSend = creditsPanel;
        }
        else if(canvasGroup == "roomBrowserPanel")
        {
            CGtoSend = roomBrowserPanel;
        }
        else if(canvasGroup == "joinedRoomPanel")
        {
            CGtoSend = joinedRoomPanel;
        }
        else if(canvasGroup == "errorPanel")
        {
            CGtoSend = errorPanel;
        }
        else if(canvasGroup == "loadingPanel")
        {
            CGtoSend = loadingPanel;
        }
        else if(canvasGroup == "createRoomPanel")
        {
            CGtoSend = createRoomPanel;
        }
        else if(canvasGroup == "nickNamePanel")
        {
            CGtoSend = nickNamePanel;
        }
        return CGtoSend;
    }

     public bool GetCanvasGroupAlpha(CanvasGroup canvasGroup)
     {
        if(canvasGroup.alpha > 0)
        {
            return true;
        }else{
            return false;
        }
     }

    public void GoSelectorScreen()
    {
        HideCanvasGroup(GetCanvasGroup("joinedRoomPanel"));
        ShowCanvasGroup(GetCanvasGroup("characterSelectorPanel"));
    }

    public void DoubleClickPrevent(Button button)
    {
        button.interactable = false;
        StartCoroutine(EnableButtonAfterDelay(button, 2f));
    }

    public IEnumerator EnableButtonAfterDelay(Button button, float delay)
    {
        yield return new WaitForSeconds(delay);
        button.interactable = true;
    }

    public void ResetInputField(string inputField)
    {
        if(inputField == "roomNameInput")
        {
            roomNameInput.text = "";
        }   
        else if(inputField == "nickNameInput")
        {
            nickNameInput.text = "";
        }
    }

     
     private void Awake() {
        for(int i = 1; i < buttonList.Length; i++)
        {
            buttonList[i].interactable = false;
            if(i == 4)
            {
                buttonList[i].interactable = true;
            }
        }
        
    } 

    public void ChangeButtonSelectionMainMenu()
    {

        if (Input.GetKeyUp(KeyCode.UpArrow) && degreesMainMenu < 90f)
        {
            canMove = false;
            LeanTween.rotateAround(mainMenuButtonsGroup, Vector3.forward, 45, .5f).setOnComplete(() => {canMove = true; EnableButtonInPosition();});
            degreesMainMenu += 45;
        }
        else if (Input.GetKeyUp(KeyCode.DownArrow) && degreesMainMenu > -45f)
        {
            canMove = false;
            LeanTween.rotateAround(mainMenuButtonsGroup, Vector3.forward, -45, .5f).setOnComplete(() => {canMove = true; EnableButtonInPosition();});
            degreesMainMenu -= 45;
        }
        
    }
       public void ChangeButtonSelectionPlayOptions()
    {

        if (Input.GetKeyUp(KeyCode.UpArrow) && degreesPlayOptions < 45f)
        {
            canMove = false;
            LeanTween.rotateAround(playOptionsButtonsGroup, Vector3.forward, 45, .5f).setOnComplete(() => {canMove = true; EnableButtonInPosition();});
            degreesPlayOptions += 45;
        }
        else if (Input.GetKeyUp(KeyCode.DownArrow) && degreesPlayOptions > -45f)
        {
            canMove = false;
            LeanTween.rotateAround(playOptionsButtonsGroup, Vector3.forward, -45, .5f).setOnComplete(() => {canMove = true; EnableButtonInPosition();});
            degreesPlayOptions -= 45;
        }
        
    }

    public void EnableButtonInPosition()
    {
        for(int i = 0; i < buttonList.Length; i++)
        {
            buttonList[i].interactable = false;
        }

        //mainMenuButtonsGroup
        if(degreesMainMenu == 0)
        {
            buttonList[0].interactable = true;
        }
        else if(degreesMainMenu == 45)
        {
            buttonList[1].interactable = true;
        }
        else if(degreesMainMenu == 90)
        {
            buttonList[3].interactable = true;
        }
        else if(degreesMainMenu == -45)
        {
            buttonList[2].interactable = true;
        }
        //playOptionsButtonsGroup
        if(degreesPlayOptions == 0)
        {
            buttonList[4].interactable = true;
        }
        else if(degreesPlayOptions == 45)
        {
            buttonList[6].interactable = true;
        }
        else if(degreesPlayOptions == -45)
        {
            buttonList[5].interactable = true;
        }
    }

    private void Update() {

        if(canMove == true && GetCanvasGroupAlpha(mainMenuPanel)){
            ChangeButtonSelectionMainMenu();
        }

        if(canMove == true && GetCanvasGroupAlpha(playOptionsPanel)){
            ChangeButtonSelectionPlayOptions();
        }
    }

    public void GoToCredits()
    {
        SceneManager.LoadScene(2);
    }
}
