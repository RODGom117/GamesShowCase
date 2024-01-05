using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonMatchManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public enum EventsCodes : byte
    {
        NewPlayer, // 0 
        ChangeStat, // 1
        AllDeath, // 2
        CamazotsDead // 3
    }

    [SerializeField] private List<PhotonPlayerInfo> _playersInfo = new List<PhotonPlayerInfo>();
    [SerializeField] private UISelectScreenManager selectScreenManager;
    
    public enum GameStates
    {
        SelectingCharacters,
        Playing,
        EndGame
    }
    
    public GameStates gameState = GameStates.SelectingCharacters;
    public float waitTimeAfterEnding = 10;

    // Start is called before the first frame update
    void Start()
    {
        // Si el jugador no esta conectado, etonces regresamos al menu principal
        // No usamos PhotonNetwork.LoadLevel() porque queremos llamar la escena de forma local unicamente
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene(0);
        }
    }

    void Update()
    {
        CheckGameState();
    }

    void StartMatch()
    {
        // Start the game
        gameState = GameStates.Playing;
    }

    public void OnEvent(EventData photonEvent)
    {
        // Valores de eventos hasta el 200 para el usuario, mas de 200 son reservados para photon
        if (photonEvent.Code < 200)
        {
            // Castea evento y es recibido por photon
            EventsCodes eventCode = (EventsCodes) photonEvent.Code;

            Debug.Log("Evento recibido: " + eventCode);

            //Guarda la info del evento
            object[] data = (object[]) photonEvent.CustomData;

            // Hacer algo con el codigo e info recibida
            switch(eventCode)
            {
                case EventsCodes.NewPlayer:
                    NewPlayerReceived(data);
                    break;
                case EventsCodes.ChangeStat:
                    ChangeStatReceived(data);
                    break;
                case EventsCodes.AllDeath:
                    // AllDeathReceived(data);
                    break;
                case EventsCodes.CamazotsDead:
                    // CamazotsDeadReceived(data);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    #region Eventos para enviar y recibir de photon

        public void NewPlayerSent(string playerName)
        {
            // Crea un arreglo de objetos para enviar
            object[] package = new object[5];
            package[0] = playerName;
            package[1] = 10;
            package[2] = 0f;
            package[3] = PhotonNetwork.LocalPlayer.ActorNumber;

            // Envia el paquete a todos los jugadores en la room
            PhotonNetwork.RaiseEvent(
                (byte) EventsCodes.NewPlayer, //Tipo de evento que mandaremos
                package, // Datos que mandaremos
                new RaiseEventOptions {Receivers = ReceiverGroup.MasterClient}, // A quien le mandaremos los datos
                SendOptions.SendReliable
                );
        }

        public void NewPlayerReceived(object[] dataReceived)
        {
            //Unbox de la info recibida, no olvidar castear la informacion correctamente por los tipos
            string playerName = (string)dataReceived[0];
            int heroID = (int)dataReceived[1];
            float health = (float)dataReceived[2];
            int playerID = (int)dataReceived[3];

            // Check if the player with the same ActorNumber is already in the list
            bool playerExists = _playersInfo.Exists(player => player.PlayerID == playerID);

            if (!playerExists)
            {
                PhotonPlayerInfo newPlayer = new PhotonPlayerInfo(playerName, heroID, health, playerID);
                _playersInfo.Add(newPlayer);
            }
        }

        public void ChangeStatSent(int sendingActor, int statToUpdate, int AmountToChange)
        {
            object[] package = new object[3];
            package[0] = sendingActor;
            package[1] = statToUpdate;
            package[2] = AmountToChange;
            
            PhotonNetwork.RaiseEvent(
                (byte) EventsCodes.ChangeStat,
                package,
                new RaiseEventOptions {Receivers = ReceiverGroup.All},
                SendOptions.SendReliable
            );
        }

        public void ChangeStatReceived(object[] dataReceived)
        {
            int sendingActor = (int) dataReceived[0];
            int statToUpdate = (int) dataReceived[1];
            int amountToChange = (int) dataReceived[2];
            

            Debug.Log("THIS IS PLAYERS INFO "+ _playersInfo.Count);

            for (int i = 0; i < _playersInfo.Count; i++)
            {
                if (_playersInfo[i].PlayerID == sendingActor)
                {
                    switch (statToUpdate)
                    {
                        case 0:
                            _playersInfo[i].HeroID = amountToChange;
                            Debug.Log($"Player [{_playersInfo[i].PlayerName}] has [{_playersInfo[i].HeroID}] as hero ID");
                            break;
                        case 1:
                            _playersInfo[i].Health += amountToChange;
                            if(_playersInfo[0].Health <= 0)
                            {
                                _playersInfo[i].Health = 0;
                                Debug.Log($"Player [{_playersInfo[i].PlayerName}] has [{_playersInfo[i].Health}] of health");
                                Debug.Log($"Player [{_playersInfo[i].PlayerName}] is death");

                            }
                            else
                            {
                                Debug.Log($"Player [{_playersInfo[i].PlayerName}] has [{_playersInfo[i].Health}] of health");
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }

    #endregion
        
        private void CheckGameState()
        {
            if (gameState == GameStates.EndGame)
            {
                // EndGame();
            }
        }
        
        public void StartGame()
        {
            gameState = GameStates.Playing;
        }

        public void EndGame()
        {
            Debug.Log("AHHHHHHH");
            gameState = GameStates.EndGame;
        /*     // Set the game state to end game
             gameState = GameStates.EndGame;
            
             // Stops automatic player instantiation
            FindObjectOfType<PhotonSpawnPlayer>().Instance.StopAllCoroutines();
            
            // Release the cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            if (PhotonNetwork.IsMasterClient)
            {
                // Wait for the end of the game and then go mack to the main menu
                StartCoroutine(CorBackToMainMenu());
                
                // Destroy all the multiplayer objects
                PhotonNetwork.DestroyAll();
            }*/
        }

    public int GetActorID(string playerNickName)
        {
            for (int i = 0; i < _playersInfo.Count; i++)
            {
                if (_playersInfo[i].PlayerName == playerNickName)
                {
                    Debug.Log($"[ Returning PlayerActor ID {_playersInfo[i].PlayerID}]");
                    return _playersInfo[i].PlayerID;
                }
            }

            return -1;
        }
}
