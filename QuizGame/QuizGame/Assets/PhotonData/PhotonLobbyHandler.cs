using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class PhotonLobbyHandler : MonoBehaviourPunCallbacks
{
    public MainMenuHandler mainMenuHandler;

    public GameObject StartBtn;
    public TMP_InputField joinRoomNameInput;
    public TMP_InputField createRoomNameInput;

    public TextMeshProUGUI createRoomDetailTxt;
    public TextMeshProUGUI joinRoomDetailTxt;
    public TextMeshProUGUI publicRoomDetailTxt;
    public TextMeshProUGUI waitDetailTxt;

    public GameObject roomListContainer;
    public GameObject roomListItemPrefab;

    public GameObject noRoomAvailableTxt;

    public TextMeshProUGUI waitTimeTxt;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        noRoomAvailableTxt.SetActive(true); 
        ClearCustomTxts();
        StartBtn.SetActive(false);
   
     
        if(Application.internetReachability == NetworkReachability.NotReachable)
        {
            StartCoroutine(StartCheckNetwork());
        }
        else
        {
            mainMenuHandler.ActivatePanel(mainMenuHandler.loadingPanel); 
            Connect();
        }
      
    }
    private IEnumerator StartCheckNetwork()
    {
        mainMenuHandler.ActivatePanel(mainMenuHandler.noNetworkPanel);
        yield return new WaitUntil(predicate: () => Application.internetReachability != NetworkReachability.NotReachable);
        mainMenuHandler.ActivatePanel(mainMenuHandler.loadingPanel);
        Connect();



    }
    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (PhotonNetwork.InLobby)
            {
             mainMenuHandler.ActivatePanel(mainMenuHandler.mainPanel);
                SetPlayerProperty("playerLevel", "Beginner");
                SetName("Player2");
            }
            else
            {
                PhotonNetwork.JoinLobby();
            }
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    public void SetName(string nm)
    {
        PhotonNetwork.NickName = nm;
        SetPlayerProperty("playerName", nm);
    }
    public void Play()
    {
        PhotonNetwork.LoadLevel(mainMenuHandler.gamePlaySceneName);
    }

    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void CreateCustomRoom()
    {
        string roomName = createRoomNameInput.text;
        if (string.IsNullOrEmpty(roomName) || roomName.Length <= 3)
        {
            Debug.Log("Room name must be greater than 3 characters.");
            return;
        }
        RoomOptions roomOptions = new RoomOptions() { MaxPlayers = GameManager.Instance.totalPlayers,IsVisible=false };

        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable();
        customRoomProperties["TotalPlayers"] = GameManager.Instance.totalPlayers;
        roomOptions.CustomRoomProperties = customRoomProperties;
        roomOptions.CustomRoomPropertiesForLobby = new string[] {"TotalPlayers" };

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public void JoinCustomRoom()
    {
        string roomName = joinRoomNameInput.text;
        if (string.IsNullOrEmpty(roomName) || roomName.Length <= 3)
        {
            Debug.Log("Room name must be greater than 3 characters.");
            return;
        }
        PhotonNetwork.JoinRoom(roomName);
    }
    public void CreateRandomRoom()
    {
        string roomName = "Room" + Random.Range(100000, 1000000);
        RoomOptions roomOptions = new RoomOptions() { MaxPlayers = GameManager.Instance.totalPlayers,IsVisible=true };

        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable();
        customRoomProperties["TotalPlayers"] = GameManager.Instance.totalPlayers;
        roomOptions.CustomRoomProperties = customRoomProperties;
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "TotalPlayers" };

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }
    public void ClearCustomTxts()
    {
        createRoomDetailTxt.text="";
        joinRoomDetailTxt.text = "";
        publicRoomDetailTxt.text = "";
        joinRoomNameInput.text = "";
        createRoomNameInput.text = "";
    }
    public void UpdateWaitDetailTxt()
    {
        waitDetailTxt.text = "";
       if(PhotonNetwork.InRoom)
        {
            waitDetailTxt.text += $"Players: {PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers}\n\n" +
                $"LOBBY\n\n" +
                $"ROOM ID: {PhotonNetwork.CurrentRoom.Name}";
        }
    }
    public override void OnConnectedToMaster()
    {
        Connect();
    }

    public override void OnJoinedLobby()
    {
        Connect();
    }
    public override void OnJoinedRoom()
    {

        GameManager.Instance.isMultiPlayer = true;
        GameManager.Instance.totalPlayers = (int)PhotonNetwork.CurrentRoom.CustomProperties["TotalPlayers"];


        ClearCustomTxts();

        UpdateWaitDetailTxt();


        mainMenuHandler.ActivatePanel(mainMenuHandler.multiPlayerWaitingPanel);
        if (PhotonNetwork.IsMasterClient)
        {
            SetRoomNumber(Random.Range(0, 2));
           // StartCoroutine(JoinTimer());
        }
        Debug.Log("Joined a room successfully.");
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room created successfully.");
        ClearCustomTxts();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        publicRoomDetailTxt.text = "Failed to join a random room. Creating a new room...";
        Debug.Log("Failed to join a random room. Creating a new room...");
        CreateRandomRoom();
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        publicRoomDetailTxt.text = "Failed to create a room";
        createRoomDetailTxt.text = "Failed to create a room";
        Debug.Log("Failed to create a room: " + message);
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        joinRoomDetailTxt.text = "Failed to join a room";
        Debug.Log("Failed to join a room: " + message);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            StartCoroutine(StartCheckNetwork());
        }
        else
        {
            mainMenuHandler.ActivatePanel(mainMenuHandler.loadingPanel);
            Connect();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
       UpdateWaitDetailTxt();

        if (PhotonNetwork.IsMasterClient)
        {
            if(PhotonNetwork.CurrentRoom.PlayerCount>=PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                PhotonNetwork.CurrentRoom.IsVisible=false;

               StartBtn.SetActive(true);
            }
            else
            {
                PhotonNetwork.CurrentRoom.IsOpen = true;
                PhotonNetwork.CurrentRoom.IsVisible = true;

                StartBtn.SetActive(false);
            }
        }
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
      UpdateWaitDetailTxt();
        if (PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount >= PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
         

                StartBtn.SetActive(true);
            }
            else
            {
                PhotonNetwork.CurrentRoom.IsOpen = true;
               

               StartBtn.SetActive(false);
            }
        }
    }
    public void UpdateRoomList(List<RoomInfo> roomList)
    {
        foreach (Transform child in roomListContainer.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (RoomInfo room in roomList)
        {
            if(room.IsVisible&&room.IsOpen)
            {
                GameObject roomListItem = Instantiate(roomListItemPrefab, roomListContainer.transform);
                roomListItem.GetComponentInChildren<TextMeshProUGUI>().text = room.Name;
                roomListItem.GetComponentInChildren<Button>().onClick.AddListener(() => PhotonNetwork.JoinRoom(room.Name));
            }
          
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if(roomList.Count > 0)
        {
            noRoomAvailableTxt.SetActive(false);
        }
        else
        {
            noRoomAvailableTxt.SetActive(true);
        }
        UpdateRoomList(roomList);
    }
    IEnumerator JoinTimer()
    {
        for (int i = 20;i>0;i--)
        {
            waitTimeTxt.text=i.ToString();
            yield return new WaitForSeconds(1f);
        }
        waitTimeTxt.text = "";
        StartBtn.SetActive(true);
    }
    public void SetRoomNumber(int mapNumber)
    {
        if (PhotonNetwork.InRoom)
        {
            ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable();
            roomProperties["MapNumber"] = mapNumber; // "MapNumber" is the key for your property

            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
        }
        else
        {
            Debug.LogWarning("Not in a room. Cannot set room properties.");
        }
    }

    public void SetPlayerProperty(string key, object value)
    {
        // Create a hashtable to store the property
        ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();

        // Add the key-value pair to the hashtable
        playerProperties[key] = value;

        // Apply the properties to the local player
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
    }
}
