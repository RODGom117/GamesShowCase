using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

    /// <summary>
    /// Clase encargada de instanciar y destruir los botones de las rooms
    /// </summary>
public class PhotonCreateRoomButtons : MonoBehaviour
{
    [SerializeField] private PhotonRoomButton photonRoomButtonPrefab;
    [SerializeField] private Transform photonRoomButtonParent;

    private List<PhotonRoomButton> photonRoomButtons = new List<PhotonRoomButton>();

    /// <summary>
    /// Instancia un boton de room y configura su informacion
    /// </summary>
    /// <param name="roomInfo"></param>
    public void InstantiateRoomButton(RoomInfo roomInfo)
    {
        // Filtra para saber si hay espacio y si la room esta disponible
        if (roomInfo.PlayerCount == roomInfo.MaxPlayers && roomInfo.RemovedFromList)
        {
            return;
        }

        // Si ya hay un boton para la room la salta
        foreach (PhotonRoomButton roomButton in photonRoomButtons)
        {
            if (roomButton.RoomInfo.Name == roomInfo.Name)
            {
                return;
            }
        }

        PhotonRoomButton photonRoomButton = Instantiate(photonRoomButtonPrefab, photonRoomButtonParent);
        photonRoomButton.SetRoomInfo(roomInfo);
        photonRoomButtons.Add(photonRoomButton);

        photonRoomButton.transform.localScale = Vector3.one;
        photonRoomButton.transform.localRotation = Quaternion.identity;
    }

    /// <summary>
    /// Destruye los botones de rooms
    /// </summary>
    /// <param name="roomList"></param>
    public void DestroyNotListedRoomButtons(List<RoomInfo> roomList)
    {
        List<PhotonRoomButton> RoomsToRemove = new List<PhotonRoomButton>();

        // Itera la lista buscando botones en la lista de rooms
        foreach(PhotonRoomButton photoRoomButton in photonRoomButtons)
        {
            bool found = false;
            foreach(RoomInfo roomInfo in roomList)
            {
                if(photoRoomButton.RoomInfo.Name == roomInfo.Name)
                {
                    found = true;
                    break;
                }
            }

            if(found)
            {
                RoomsToRemove.Add(photoRoomButton);
                Destroy(photoRoomButton.gameObject);
            }
        }

        // Remueve los botones de la lista
        foreach(PhotonRoomButton photonRoomButton in RoomsToRemove)
        {
            photonRoomButtons.Remove(photonRoomButton);
        }
    }
}
