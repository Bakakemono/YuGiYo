using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LobbyController : MonoBehaviourPunCallbacks
{
    [SerializeField] private int roomSize = 4; // Number of player that can access a room.

    [SerializeField] private TMP_InputField createRoomField;
    [SerializeField] private TMP_InputField joinRoomField;

    [SerializeField] private GameObject connectingText;
    [SerializeField] private GameObject MainMenu;

    private bool joiningRoom = false;
    private bool creatingRoom = false;

    public override void OnConnectedToMaster() {
        PhotonNetwork.AutomaticallySyncScene = true; // Make it so whatever scene master client
        connectingText.SetActive(false);
        MainMenu.SetActive(true);
    }

    public void JoinRandomRoom() {
        PhotonNetwork.JoinRandomRoom(); // Try and join an existing room.
    }

    public void CreateAndJoinRoom() {
        if(createRoomField.text == null || createRoomField.text == "") {
            return;
        }
        RoomOptions roomOptions = new RoomOptions() {
            IsVisible = false,
            IsOpen = true,
            MaxPlayers = (byte)roomSize
        };
        PhotonNetwork.CreateRoom(createRoomField.text, roomOptions);
    }

    public void JoinRoom() {
        if(joinRoomField.text == null || joinRoomField.text == "") {
            return;
        }
        joiningRoom = true;

        PhotonNetwork.JoinRoom(joinRoomField.text);
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

    public override void OnJoinRandomFailed(short returnCode, string message) {
        Debug.Log("Failed to join a room");
        if(joiningRoom) {
            Debug.Log("The room does not exist or the name is wrong !");
            joiningRoom = false;
            return;
        }
        CreateRoom();
    }

    public override void OnCreateRoomFailed(short returnCode, string message) {
        Debug.Log("Fail to create a new room... try again");
        if(creatingRoom) {
            Debug.Log("The room already exist or an error occured during the process !");
            creatingRoom = false;
            return;
        }
        CreateRoom();
    }

    // Paired with the quick cancel button.
    public void QuickCancel() {
        PhotonNetwork.LeaveRoom();
    }
}