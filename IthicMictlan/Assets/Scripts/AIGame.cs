using UnityEngine;
using Photon.Pun;

public class AIGame : MonoBehaviourPunCallbacks
{
    public GameObject[] playerPrefabs;  // Lista de prefabs para distintos personajes

    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                // Verifica la cantidad de jugadores en la sala al iniciar el juego
                CheckPlayersInRoom();
            }
        }
    }

    void CheckPlayersInRoom()
    {
        // Obtiene la cantidad actual de jugadores en la sala
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        // Si no hay suficientes jugadores, crea instancias adicionales
        while (playerCount < 4)
        {
            // Incrementa el contador antes de instanciar para evitar bucles infinitos
            playerCount++;

            // Elige un personaje aleatorio de la lista de prefabs
            GameObject selectedPrefab = playerPrefabs[Random.Range(0, playerPrefabs.Length)];

            // Crea una nueva instancia del personaje y sincronízala a través de la red
            GameObject newPlayer = PhotonNetwork.Instantiate(selectedPrefab.name, Vector3.zero, Quaternion.identity);

            // Configura el nuevo jugador para seguir al jugador maestro (el que creó la sala)
            if (PhotonNetwork.IsMasterClient)
            {
                newPlayer.GetComponent<YouPlayerControllerScript>().SetMasterPlayer();
            }
        }
    }
}
