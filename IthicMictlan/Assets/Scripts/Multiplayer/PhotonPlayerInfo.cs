using System;

[Serializable]
public class PhotonPlayerInfo
{
    public string PlayerName;
    public int HeroID;
    public float Health;
    public int PlayerID;

    public PhotonPlayerInfo()
    {
        PlayerName = "";
        HeroID = 0;
        Health = 0.0f;
        PlayerID = 0;
    }

    public PhotonPlayerInfo(string playerName, int heroID, float health, int playerID)
    {
        PlayerName = playerName;
        HeroID = heroID;
        Health = health;
        PlayerID = playerID;
    }
}
