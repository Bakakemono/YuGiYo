using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTest : MonoBehaviourPunCallbacks
{
    PhotonView view;
    int playerIndex;

    private void Start() {
        view = GetComponent<PhotonView>();
        playerIndex = PhotonNetwork.CurrentRoom.PlayerCount;
    }

    private void FixedUpdate() {
        if(view.IsMine) {
            view.RPC("RPC_UpdatePlayerPos", RpcTarget.AllBuffered);
            view.RPC("RPC_SayHello", RpcTarget.AllBuffered, "Hello", "Beaches " + playerIndex);
        }
    }
    [PunRPC]
    void RPC_UpdatePlayerPos() {
        transform.position = new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10));
    }

    [PunRPC]
    void RPC_SayHello(string message1, string message2) {
        Debug.Log(message1 + " " + message2);
    }
}
