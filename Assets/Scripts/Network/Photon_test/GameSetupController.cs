using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameSetupController : MonoBehaviour {

    void Start() {
        CreatePlayer();
    }

    private void CreatePlayer() {
        Debug.Log("Creating Player");
        PhotonNetwork.Instantiate(
            Path.Combine("PhotonPrefabs", "PhotonPlayer"),
            new Vector3(Random.Range(-2.0f, 2.0f), Random.Range(-2.0f, 2.0f), Random.Range(-2.0f, 2.0f)),
            Quaternion.identity
            );
    }
}
