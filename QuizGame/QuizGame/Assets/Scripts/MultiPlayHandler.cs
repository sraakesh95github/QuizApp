using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiPlayHandler : MonoBehaviourPunCallbacks
{
   
    public PhotonView pv;
    public GamePlayHandler gamePlayHandler;

    public int playerId;

    private void Awake()
    {
        playerId = PhotonNetwork.LocalPlayer.ActorNumber;
    }

    private void Start()
    {
  
        gamePlayHandler.otherPlayerName = (string)GetPlayerProperty("playerName", "Player2");
        gamePlayHandler.otherPlayerRank = (string)GetPlayerProperty("playerLevel", "Beginner");
        gamePlayHandler.uiHandler.UpdatePlayer2Data(gamePlayHandler.otherPlayerName, gamePlayHandler.otherPlayerRank);
    }

    [PunRPC]
    public void Answered(int _playerId, int score)
    {
        if(_playerId!=playerId)
        {
            gamePlayHandler.otherPlayerScore = score;
            gamePlayHandler.uiHandler.UpdatePlayer2Score(score);
            gamePlayHandler.otherPlayerAnswered = true;
        }
    }

    public Player GetOtherPlayer()
    {
        return PhotonNetwork.LocalPlayer == PhotonNetwork.PlayerList[0] ? PhotonNetwork.PlayerList[1] : PhotonNetwork.PlayerList[0];
    }

    public object GetPlayerProperty(string key, object defaultValue = null)
    {
        // Get the local player's custom properties
        ExitGames.Client.Photon.Hashtable playerProperties = GetOtherPlayer().CustomProperties;

        // Check if the property exists, if so return it, otherwise return the default value
        if (playerProperties.ContainsKey(key))
        {
            return playerProperties[key];
        }

        return defaultValue;
    }
    public void Home()
    {
        PhotonNetwork.Disconnect();
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        SceneManager.LoadScene(0);
    }
}
