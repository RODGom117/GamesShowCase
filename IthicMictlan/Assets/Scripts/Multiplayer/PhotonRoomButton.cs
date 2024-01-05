using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Clase encargada de guardar la info de la room y actualizar la UI
/// </summary>
public class PhotonRoomButton : MonoBehaviour
{
    [SerializeField] private TMP_Text roomNameText;
    private RoomInfo _roomInfo;
    public RoomInfo RoomInfo => _roomInfo;
    private PhotonConnector _photonConnector;

    private void Start() {
        _photonConnector = FindObjectOfType<PhotonConnector>();
        GetComponent<Button>().onClick.AddListener(JoinRoom);
    }

    /// <summary>
    /// Configura la informacion de la room y actualiza la UI
    /// </summary>
    /// <param name="roomInfo"></param>
    public void SetRoomInfo(RoomInfo roomInfo)
    {
        this._roomInfo = roomInfo;
        roomNameText.text = roomInfo.Name;
        name = "Room " + roomInfo.Name;
    }

    /// <summary>
    /// Une al jugador a la room seleccionada en la UI
    /// </summary>
    private void JoinRoom()
    {
        _photonConnector.JoinRoom(_roomInfo);
    }
}
