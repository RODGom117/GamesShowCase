using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


/// <summary>
/// Clase encargada de crear el label del nombre del jugador
/// </summary>
public class PhotonCreatePlayerNameLabel : MonoBehaviour
{
    [SerializeField] private GameObject photonPlayerNameLabelPrefab;
    [SerializeField] private Transform photonPlayerNameLabelParent;
    [SerializeField] private List<TMP_Text> playerNameLabels = new List<TMP_Text>();

    /// <summary>
    /// Agrega el nombre del jugador a la lista
    /// </summary>
    /// <param name="playerNickname"></param>
    public void AddPlayerNicknameIntoList(string playerNickname)
    {
        GameObject playerNameLabel = Instantiate(photonPlayerNameLabelPrefab, photonPlayerNameLabelParent);

        foreach (var playerName in playerNameLabel.GetComponentsInChildren<TMP_Text>())
        {
            playerName.text = playerNickname;
            playerNameLabels.Add(playerName);
        }
    }

    /// <summary>
    /// Remueve todos los nombres de la lista
    /// </summary>
    public void RemoveAllPlayerNamesFromList()
    {
        if(playerNameLabels == null || playerNameLabels.Count == 0)
        {
            return;
        }
        foreach(TMP_Text playerNameLabel in playerNameLabels)
        {
            Destroy(playerNameLabel.transform.parent.gameObject);
        }

        playerNameLabels.Clear();
    }

    /// <summary>
    /// Remueve el nombre del jugador de la lista
    /// </summary>
    /// <param name="playerNickname"></param>
    public void RemovePlayerFromList(string playerNickname)
    {
        if (playerNameLabels == null || playerNameLabels.Count == 0)
        {
            return;
        }

        TMP_Text playerNameLabelToRemove = null;

        foreach (TMP_Text playerNameLabel in playerNameLabels)
        {
            if(playerNameLabel.text == playerNickname)
            {
                Destroy(playerNameLabel.transform.parent.gameObject);
                playerNameLabelToRemove = playerNameLabel;
            }
        }
        playerNameLabels.Remove(playerNameLabelToRemove);
    }
}
