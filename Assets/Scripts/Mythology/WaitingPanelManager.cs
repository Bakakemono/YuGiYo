using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaitingPanelManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TextMeshProUGUI totalPlayer;
    [SerializeField] TextMeshProUGUI playerCount;

    [SerializeField] public TextMeshProUGUI id;
    [SerializeField] public TextMeshProUGUI timer;

    [SerializeField] GameObject WaitingPanel;

    private void Start() {
        totalPlayer.text = PhotonNetwork.CurrentRoom.MaxPlayers.ToString();
        playerCount.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString();
        //if(PhotonNetwork.CurrentRoom.MaxPlayers == PhotonNetwork.CurrentRoom.PlayerCount)
        //    WaitingPanel.SetActive(false);
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer) {
        playerCount.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString();
        //if(PhotonNetwork.CurrentRoom.MaxPlayers == PhotonNetwork.CurrentRoom.PlayerCount)
        //    WaitingPanel.SetActive(false);
    }

    public void HideWaitingPanel() {
        id.color = Color.black;
        WaitingPanel.SetActive(false);
    }

    public void SetTimer(int t) {
        timer.gameObject.SetActive(true);
        timer.text = t.ToString();
    }
}
