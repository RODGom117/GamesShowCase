using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// Clase encargada de la conexion a la red
/// </summary>
public class PhotonConnector : MonoBehaviourPunCallbacks
{
    private const int MAX_PLAYER = 4;
    [SerializeField] private UIMenuController UIMenuController;
    [SerializeField] private PhotonCreateRoomButtons photonCreateRoomButtons;
    [SerializeField] private PhotonCreatePlayerNameLabel photonCreatePlayerNameLabel;
    private bool nicknameSet;
    private string savedNickname;

    // Conectarse al servidor, que hostea lobbies y los lobbies tienen rooms
    private void Start() {
        nicknameSet = false;
        savedNickname = PlayerPrefs.GetString("PlayerNickname", "Default Nickname");
        UIMenuController.InitGameScreen();
        //Iniciar la conexcion usando las settings colocadas en el unity editor. Esto se conecta al servidor de photon
        PhotonNetwork.ConnectUsingSettings();

    }

    /// <summary>
    /// Sobreescribir para mostrar canvas de cargando y unirse a un lobby
    /// </summary>
    public override void OnConnectedToMaster() {
        UIMenuController.SetLoadingText("Joining Lobby...");

        // Dice a los jugadores que todos deben cargar la misma escena
        PhotonNetwork.AutomaticallySyncScene = true;

        // Una vez verificamos que estamos conectados al servidor, nos unimos a un lobby
        PhotonNetwork.JoinLobby();
    }

    /// <summary>
    /// Sobreescribir para mostrar canvas de nickname y menu principal
    /// </summary>
    public override void OnJoinedLobby() {

        // Si te unisste a un lobby, lees la informacion y muestras los canvas correspondientes
        UIMenuController.HideCanvasGroup(UIMenuController.GetCanvasGroup("loadingPanel"));
        if(!nicknameSet)
        {
            UIMenuController.SetPlayerNickname(savedNickname);
            UIMenuController.ShowCanvasGroup(UIMenuController.GetCanvasGroup("nickNamePanel"));
            nicknameSet = true;
        }
        UIMenuController.ShowCanvasGroup(UIMenuController.GetCanvasGroup("mainMenuPanel"));

        // Coloca un nickname al azar
        if (string.IsNullOrEmpty(PhotonNetwork.NickName))
        {
            PhotonNetwork.NickName = "Player" + Random.Range(0, 1000);
        }
    }

    /// <summary>
    /// Coloca el nickname del jugador desde la UI, usada en el editor
    /// </summary>
    public void SetPlayerNicknameFromUI()
    {
        PhotonNetwork.NickName = UIMenuController.GetPlayerNickname();
        savedNickname = UIMenuController.GetPlayerNickname();
        PlayerPrefs.SetString("PlayerNickname", savedNickname);
    }

    /// <summary>
    /// Crea una room con la info de la UI, usada en el editor
    /// </summary>
    public void CreateRoom()
    {
        string roomName = UIMenuController.GetCreateRoomName();
        if(roomName != "")
        {
            UIMenuController.HideCanvasGroup(UIMenuController.GetCanvasGroup("createRoomPanel"));

            RoomOptions roomOptions = new RoomOptions
            {
                MaxPlayers = MAX_PLAYER
            };
            PhotonNetwork.CreateRoom(roomName, roomOptions);
            
            UIMenuController.SetLoadingText("Creating Room...");
            UIMenuController.ShowCanvasGroup(UIMenuController.GetCanvasGroup("loadingPanel"));
        }
    }

    /// <summary>
    /// Sobreescribir para mostrar canvas de room actual
    /// </summary>
    public override void OnJoinedRoom()
    {
        UIMenuController.HideCanvasGroup(UIMenuController.GetCanvasGroup("loadingPanel"));
        UIMenuController.ShowCanvasGroup(UIMenuController.GetCanvasGroup("joinedRoomPanel"));
        UIMenuController.SetJoinedRoomName(PhotonNetwork.CurrentRoom.Name);

        ListPlayersInRoom();

        // Activa el boton de start solo si el juagdor es el master client
        UIMenuController.ActivateStartGameButton(PhotonNetwork.IsMasterClient);
    }

    /// <summary>
    /// Sobreescribir para mostrar al nuevo jugador en el canvas UI
    /// </summary>
    /// <param name="newPlayer"></param>
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        photonCreatePlayerNameLabel.AddPlayerNicknameIntoList(newPlayer.NickName);
    }

    /// <summary>
    /// Sobreescribir para mostrar los jugadores actuales cuando uno sale
    /// </summary>
    /// <param name="otherPlayer"></param>
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        ListPlayersInRoom();
    }

    /// <summary>
    /// Enlista los jugadores en la room
    /// </summary>
    private void ListPlayersInRoom()
    {
        photonCreatePlayerNameLabel.RemoveAllPlayerNamesFromList();

        // Obtiene la lista de jugadores en la room, trata de no invocarlo mucho, la info viene del servidor
        Player [] players = PhotonNetwork.PlayerList;

        photonCreatePlayerNameLabel.AddPlayerNicknameIntoList(PhotonNetwork.LocalPlayer.NickName);

        foreach (Player player in players)
        {
            if(player == PhotonNetwork.LocalPlayer)
            {
                continue;
            }

            photonCreatePlayerNameLabel.AddPlayerNicknameIntoList(player.NickName);
        }
    }

    /// <summary>
    /// Sobreescribir para mostrar canvas de error si falla algo al creae la room
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        // https://doc-api.photonengine.com/en/pun/v1/class_error_code.html
        string errorMessage = "Error(" + returnCode.ToString() + "): \n";
        switch (returnCode)
        {
            case ErrorCode.GameIdAlreadyExists:
                errorMessage += "The room name already exists! Try another one";
                break;
            case ErrorCode.InternalServerError:
                errorMessage += "Internal server error! Try again later";
                break;
            case ErrorCode.ServerFull:
                errorMessage += "The server is full! Try again later";
                break;
            default:
                errorMessage += message;
                break;
        }
        UIMenuController.ShowCanvasGroup(UIMenuController.GetCanvasGroup("errorPanel"));
        UIMenuController.SetErrorText(errorMessage);
    }

    /// <summary>
    /// Salir de la sala y volver al lobby, usada en el editor
    /// </summary>
    public void LeaveRoom()
    {
        UIMenuController.SetLoadingText("Leaving Room...");
        UIMenuController.ShowCanvasGroup(UIMenuController.GetCanvasGroup("loadingPanel"));
        UIMenuController.HideCanvasGroup(UIMenuController.GetCanvasGroup("joinedRoomPanel"));

        PhotonNetwork.LeaveRoom();
    }

    /// <summary>
    /// Sobreescribir para mostrar canvas de menu principal al salir de un room
    /// </summary>
    public override void OnLeftRoom()
    {
        UIMenuController.HideCanvasGroup(UIMenuController.GetCanvasGroup("loadingPanel"));
        UIMenuController.ShowCanvasGroup(UIMenuController.GetCanvasGroup("playOptionsPanel"));
    }

    /// <summary>
    /// Sobreescribir para mostrar rooms activas en la UI cuando se actualizan
    /// </summary>
    /// <param name="roomList"></param>
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // https://forum.unity.com/threads/photon-pun-how-to-handle-onroomlistupdate-function.1145306/
        photonCreateRoomButtons.DestroyNotListedRoomButtons(roomList);

        // Enlista todas las salas del lobby
        foreach (RoomInfo roomInfo in roomList)
        {
            photonCreateRoomButtons.InstantiateRoomButton(roomInfo);
        }
    }

    /// <summary>
    /// Unise a la room seleccionada
    /// </summary>
    /// <param name="roomInfo"></param>
    public void JoinRoom(RoomInfo roomInfo)
    {
        UIMenuController.SetLoadingText("Joining Room...");
        UIMenuController.ShowCanvasGroup(UIMenuController.GetCanvasGroup("loadingPanel"));
        UIMenuController.HideCanvasGroup(UIMenuController.GetCanvasGroup("roomBrowserPanel"));

        PhotonNetwork.JoinRoom(roomInfo.Name);

        // Remember that we already have a function that handles the jopining room UI so we doesn't need to do it here
    }

    /// <summary>
    /// Sobreescribir para activar el boton de start para el nuevo master client
    /// </summary>
    /// <param name="master"></param>
    public override void OnMasterClientSwitched(Player master) => UIMenuController.ActivateStartGameButton(PhotonNetwork.IsMasterClient);

    /// <summary>
    /// Salir del juego desde el menu principal, usada en el editor
    /// </summary>
    public void QuitGame() => Application.Quit();

    /// <summary>
    /// Iniciar el juego, usada en el editor
    /// </summary>
    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1);
    }
}
