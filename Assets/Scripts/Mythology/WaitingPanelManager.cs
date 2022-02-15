using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaitingPanelManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TextMeshProUGUI totalPlayer;
    [SerializeField] TextMeshProUGUI playerCount;

    [SerializeField] public TextMeshProUGUI id;
    [SerializeField] public TextMeshProUGUI timer;

    [SerializeField] public RawImage indicator;

    [SerializeField] GameObject WaitingPanel;

    private void Start() {
        totalPlayer.text = PhotonNetwork.CurrentRoom.MaxPlayers.ToString();
        playerCount.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString();
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer) {
        playerCount.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString();
    }

    public void HideWaitingPanel() {
        id.color = Color.white;
        WaitingPanel.SetActive(false);
        indicator.color = Color.red;
    }

    public void SetTimer(int t) {
        timer.gameObject.SetActive(true);
        timer.text = t.ToString();
    }
}
