using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickStartRoomController : MonoBehaviourPunCallbacks
{
    [SerializeField] private int multiPlayerSceneIndex; // Number for the build index to the multiplay scene.

    public override void OnEnable() {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable() {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public override void OnJoinedRoom() {
        Debug.Log("Joined room");
        StartGame();
    }

    private void StartGame() {
        if (PhotonNetwork.IsMasterClient) {
            Debug.Log("Start Game");
            PhotonNetwork.LoadLevel(multiPlayerSceneIndex);
        }
    }
}
