using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;

public class LobbyNetworkManager : MonoBehaviourPunCallbacks
{
    public static LobbyNetworkManager Instane;
    [SerializeField] TMP_Text waitBattletext;

    private void Awake()
    {
        Instane = this;
    }

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        WindowsManager.Layout.OpenLayout("Loading");
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        WindowsManager.Layout.OpenLayout("MainMenu");
    }

    public void ToBattleButton()
    {
        WindowsManager.Layout.OpenLayout("Auto_battle");
        WindowsManager.Layout.OpenLayout("Auto_battle");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        if(returnCode == (short)ErrorCode.NoRandomMatchFound)
        {
            waitBattletext.text = "Tu nikomu ne nushen";
            CreateNewRoom();
        }
    }
    void CreateNewRoom()

    {
        RoomOptions currentRoom = new RoomOptions();
        currentRoom.IsOpen = true;
        currentRoom.MaxPlayers = 2;
        PhotonNetwork .CreateRoom(RoomNameGenerator(), currentRoom);
           
    }

    string RoomNameGenerator()
    {
        string roomCode = null;
        short codeLength = 12;
        for(int i = 0; 1 < codeLength; i++)
        {
            char symbol = (char)Random.Range(65, 91);
            roomCode += symbol;
        }
        return roomCode;
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        if(returnCode == (short)ErrorCode.GameIdAlreadyExists)
        {
            CreateNewRoom() ;
        }
    }

    public override void OnCreatedRoom()
    {
        waitBattletext.text = "chekai kenta";
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        if(PhotonNetwork.IsMasterClient) return;
        waitBattletext.text = "Chas budet mahach, prigotovsya";
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if(!PhotonNetwork.IsMasterClient) return ;
        Room currentRoom = PhotonNetwork.CurrentRoom;
        currentRoom.IsOpen = false;

        waitBattletext.text = "Boinya cherez 3 secundi";
        Invoke(nameof(LoadingGameMap), 3f);
    }

    void LoadingGameMap()
    {
        PhotonNetwork.LoadLevel(1);
    }

    public void StopFindBattleButton()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        WindowsManager.Layout.OpenLayout("MainMenu");
    }
}
