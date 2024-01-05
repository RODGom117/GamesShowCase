using System.Collections;
using System.Collections.Generic;
using Heroes.Ignacio;
using Heroes.Maira;
using Heroes.Rosa;
using Heroes.Teo;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class UISelectScreenManager : MonoBehaviour
{
    public static UISelectScreenManager instance;
    [SerializeField] private TMP_Text selectedCharacter;
    [SerializeField] private TMP_Text charcterDescription;
    [SerializeField] private CanvasGroup characterSelectorPanel;
    [SerializeField] private Button readyButton;
    [SerializeField] private Button startGameButton;
    [SerializeField] private TMP_Text waitingText;
    [SerializeField] private PhotonMatchManager photonMatchManager;
    [SerializeField] private Button[] characterButtons;
    [SerializeField] private PhotonView photonView;
    [SerializeField] private Spawner xoloSpawner;
    [SerializeField] private Sprite[] normalSprites;
    [SerializeField] private Sprite[] selectedSprites;
    [SerializeField] private MairaSounds mairaSounds;
    [SerializeField] private RosaSounds rosaSounds;
    [SerializeField] private TeoSounds teoSounds;
    [SerializeField] private NachoSounds nachoSounds;
    [SerializeField] private GameObject aListener;

    [SerializeField] private CanvasGroup deathScreen;
    [SerializeField] private CanvasGroup endScreen;
    [SerializeField] private TMP_Text timer;
    [SerializeField] private TMP_Text endText;

    private int HeroID;
    private int playersReady;
    private bool isCharacterSelected;

    [SerializeField] private HeroStats[] stats;
    private HeroStats heroData;

    private void Awake() {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    void Start() {
        playersReady = 0;
        selectedCharacter.text = "Selecciona a tu guerrero";
        charcterDescription.text = "...";
        isCharacterSelected = false;
        readyButton.interactable = false;
        waitingText.gameObject.SetActive(false);
        characterSelectorPanel.alpha = 1f;
        characterSelectorPanel.interactable = true;
        characterSelectorPanel.blocksRaycasts = true;

        deathScreen.alpha = 0f;
        deathScreen.interactable = false;
        deathScreen.blocksRaycasts = false;

        endScreen.alpha = 0f;
        endScreen.interactable = false;
        endScreen.blocksRaycasts = false;

        if(PhotonNetwork.IsMasterClient)
        {
            startGameButton.gameObject.SetActive(true);
        }
        else
        {
            startGameButton.gameObject.SetActive(false);
        }
    }

    public void SetCharacterInfo(string name, string desc)
    {
        if(isCharacterSelected)
        {
            return;
        }
        selectedCharacter.text = name;
        charcterDescription.text = desc;
        readyButton.interactable = true;

        if(name == "Maira")
        {
            HeroID = 0;
            heroData = stats[0];
            ChangeSelectedSprite(0);
            mairaSounds.PlaySelectionVoice();
        }
        if(name == "Teo")
        {
            HeroID = 1;
            heroData = stats[1];
            ChangeSelectedSprite(1);
            teoSounds.PlaySelectionVoice();
        }
        if(name == "Ignacio")
        {
            HeroID = 2;
            heroData = stats[2];
            ChangeSelectedSprite(2);
            nachoSounds.PlaySelectionVoice();
        }
        if(name == "Rosa")
        {
            HeroID = 3;
            heroData = stats[3];
            ChangeSelectedSprite(3);
            rosaSounds.PlaySelectionVoice();
        }

    }

    public void StartMatch()
    {
        xoloSpawner.SetSpawns();
        photonView.RPC("HideSelectScreen", RpcTarget.All);
        photonMatchManager.StartGame();
    }

    public void SelectCharacter()
    {
        isCharacterSelected = true;
        readyButton.interactable = false;
        waitingText.gameObject.SetActive(true);

        photonMatchManager.ChangeStatSent(PhotonNetwork.LocalPlayer.ActorNumber, 0, HeroID);
        photonMatchManager.ChangeStatSent(PhotonNetwork.LocalPlayer.ActorNumber, 1, (int)heroData.healthAttributes.maxHealth);
        photonView.RPC("DisableButton", RpcTarget.All, HeroID);

        SpawnPointManager.instance.SpawnPlayer(HeroID);
    }

    [PunRPC]
    public void HideSelectScreen()
    {
        characterSelectorPanel.alpha = 0f;
        characterSelectorPanel.interactable = false;
        characterSelectorPanel.blocksRaycasts = false;

        GameObject[] target = GameObject.FindGameObjectsWithTag("Hero");
        //TODO: make this better.
        for (int i = 0; i < target.Length; i++)
        {
            HeroesCombatRosa combatScriptRosa = target[i].GetComponent<HeroesCombatRosa>();
            PlayerManagerRosaPhoton managerScriptRosa = target[i].GetComponent<PlayerManagerRosaPhoton>();
            
            HeroesCombatMaira combatScriptMaira = target[i].GetComponent<HeroesCombatMaira>();
            PlayerManagerMairaPhoton managerScriptMaira = target[i].GetComponent<PlayerManagerMairaPhoton>();
            
            HeroesCombatIgnacio combatScriptIgnacio = target[i].GetComponent<HeroesCombatIgnacio>();
            PlayerManagerIgnacioPhoton managerScriptIgnacio = target[i].GetComponent<PlayerManagerIgnacioPhoton>();
            HeroesCombatTeo combatScriptTeo = target[i].GetComponent<HeroesCombatTeo>();
            PlayerManagerTeo managerScriptTeo = target[i].GetComponent<PlayerManagerTeo>();
            
            // For ROSA
            if(combatScriptRosa != null && managerScriptRosa != null)
            {
                target[i].GetComponent<PlayerManagerRosaPhoton>().enabled = true;
                target[i].GetComponent<HeroesCombatRosa>().enabled = true;
                target[i].GetComponent<PlayerMovement>().enabled = true;
                target[i].GetComponent<HealthSystem>().enabled = true;
                target[i].GetComponent<PlayerManagerRosaPhoton>().ActivationUI();

            } 
            else if(combatScriptMaira != null && managerScriptMaira != null) 
            {
                target[i].GetComponent<PlayerManagerMairaPhoton>().enabled = true;
                target[i].GetComponent<HeroesCombatMaira>().enabled = true;
                target[i].GetComponent<PlayerMovement>().enabled = true;
                target[i].GetComponent<HealthSystem>().enabled = true;
                target[i].GetComponent<PlayerManagerMairaPhoton>().ActivationUI();

            }
            else if(combatScriptTeo != null && managerScriptTeo != null) 
            {
                target[i].GetComponent<PlayerManagerTeo>().enabled = true;
                target[i].GetComponent<HeroesCombatTeo>().enabled = true;
                target[i].GetComponent<PlayerMovement>().enabled = true;
                target[i].GetComponent<HealthSystem>().enabled = true;
                target[i].GetComponent<PlayerManagerTeo>().ActivationUI();

            }
            else if(combatScriptIgnacio != null && managerScriptIgnacio != null) 
            {
                target[i].GetComponent<PlayerManagerIgnacioPhoton>().enabled = true;
                target[i].GetComponent<HeroesCombatIgnacio>().enabled = true;
                target[i].GetComponent<PlayerMovement>().enabled = true;
                target[i].GetComponent<HealthSystem>().enabled = true;

            }
            else {
                target[i].GetComponent<Heroes.PlayerManager>().enabled = true;
                target[i].GetComponent<HeroesCombat>().enabled = true;
                target[i].GetComponent<PlayerMovement>().enabled = true;
                target[i].GetComponent<HealthSystem>().enabled = true;
            }
            
            aListener.SetActive(false);

            /*if (photonView.IsMine)
            {
                target[i].GetComponentInChildren<Canvas>().enabled = true;
            }*/
            /*if (HeroID == 3)
            {
                target[i].GetComponent<PlayerManagerRosaPhoton>().enabled = true;
                target[i].GetComponent<HeroesCombatRosa>().enabled = true;
                target[i].GetComponent<PlayerMovement>().enabled = true;
                target[i].GetComponent<HealthSystem>().enabled = true;
            }
            else
            {
                target[i].GetComponent<Heroes.PlayerManager>().enabled = true;
                target[i].GetComponent<HeroesCombat>().enabled = true;
                target[i].GetComponent<PlayerMovement>().enabled = true;
                target[i].GetComponent<HealthSystem>().enabled = true;
            }*/

        }    

    }

    [PunRPC]
    public void DisableButton(int ID)
    {
        characterButtons[ID].interactable = false;
        playersReady++;
        if(PhotonNetwork.IsMasterClient)
        {
            if(playersReady == 1)
            {
                startGameButton.interactable = true;
            }
        }
    }

    public void ChangeSelectedSprite(int id){
        for (int i = 0; i < characterButtons.Length; i++)
        {
            characterButtons[i].GetComponent<Image>().sprite = normalSprites[i];
        }
        characterButtons[id].GetComponent<Image>().sprite = selectedSprites[id];
    }

    public void ToggleDeathScreen(int status)
    {
        if(status == 1)
        {
            deathScreen.alpha = 1f;
            deathScreen.interactable = true;
            deathScreen.blocksRaycasts = true;
        }
        else
        {
            deathScreen.alpha = 0f;
            deathScreen.interactable = false;
            deathScreen.blocksRaycasts = false;
        }
    }

    public void ToggleEndScreen(int status)
    {
        if(status == 1)
        {
            endScreen.alpha = 1f;
            endScreen.interactable = true;
            endScreen.blocksRaycasts = true;
        }
        else
        {
            endScreen.alpha = 0f;
            endScreen.interactable = false;
            endScreen.blocksRaycasts = false;
        }
    }

    public void SetTimer(int time)
    {
        timer.text = time.ToString();
    }

    public void SetEndText(string text)
    {
        endText.text = text;
    }

    public void ReturnToMenu()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel(0);
    }
}
