using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NetworkController : MonoBehaviourPunCallbacks
{
    // Documentation : https://doc.photonengine.com/en-us/pun/current/getting-started/pun-intro
    // API-Scripting : https://doc-api.photonengine.com/en/pun/v2/index.html

    // How to resolve bug between standalone build and editor connexion
    // link : https://doc.photonengine.com/en-us/realtime/current/connection-and-authentication/regions

    private void Start() {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() {
        Debug.Log("We are now connected to the : " + PhotonNetwork.CloudRegion + " Server ! ");
    }

    [SerializeField] TextMeshProUGUI region;

    private void Update() {
        region.text = PhotonNetwork.CloudRegion;
    }
}
