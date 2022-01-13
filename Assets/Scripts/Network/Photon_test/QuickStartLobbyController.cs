using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickStartLobbyController : MonoBehaviourPunCallbacks {
    [SerializeField] private GameObject quickStartButton; // Button used to create or join a game.
    [SerializeField] private GameObject quickCancelButton; // Button used to stop searching for a game.
    [SerializeField] private int roomSize; // Number of player that can access a room.
    
    public override void OnConnectedToMaster() {
        PhotonNetwork.AutomaticallySyncScene = true; // Make it so whatever scene master client 
        quickStartButton.SetActive(true);
    }

    // Paired to the quick start button.
    public void QuickStart() {
        quickStartButton.SetActive(false);
        quickCancelButton.SetActive(true);
        PhotonNetwork.JoinRandomRoom(); // Try and join an existing room.
        Debug.Log("Quick start");
    }

    public override void OnJoinRandomFailed(short returnCode, string message) {
        Debug.Log("Failed to join a room");
        CreateRoom();
    }

    void CreateRoom() {
        Debug.Log("Creating a new room now");
        int randomRoomNumber = Random.Range(0, 10000);
        RoomOptions roomOptions = new RoomOptions() {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = (byte)roomSize
        };
        PhotonNetwork.CreateRoom("Room" + randomRoomNumber, roomOptions); // Attempting to create a new room.
        Debug.Log("The room number of the new room is : " + randomRoomNumber);
    }   

    public override void OnCreateRoomFailed(short returnCode, string message) {
        Debug.Log("Fail to create a new room... try again");
        CreateRoom();
    }

    // Paired with the quick cancel button.
    public void QuickCancel() {
        quickCancelButton.SetActive(false);
        quickStartButton.SetActive(true);
        PhotonNetwork.LeaveRoom();
    }

    public void AppExit() {
        Application.Quit();
    }
}
