using Photon.Pun;
using UnityEngine;
using System.IO;

public class NetworkSpawner : MonoBehaviourPunCallbacks
{
    [SerializeField] string prefabsFolderName;
    [SerializeField] string playerPrefabName;
    [SerializeField] string handPrefabName;
    [SerializeField] string fieldPrefabName;
    [SerializeField] string gameManagerPrefabName;

    private void Start() {
        if(PhotonNetwork.IsMasterClient)
            SpawnGameManager();
    }

    void SpawnPlayer() {
        PhotonNetwork.Instantiate(
            Path.Combine(prefabsFolderName, playerPrefabName),
            Vector3.zero,
            Quaternion.identity
            );
    }

    void SpawnHandDisplayer() {
        PhotonNetwork.Instantiate(
            Path.Combine(prefabsFolderName, handPrefabName),
            Vector3.zero,
            Quaternion.identity
            );
    }

    void SpawnFieldDisplayer() {
        PhotonNetwork.Instantiate(
            Path.Combine(prefabsFolderName, fieldPrefabName),
            Vector3.zero,
            Quaternion.identity
            );
    }
    
    void SpawnGameManager() {
        PhotonNetwork.Instantiate(
            Path.Combine(prefabsFolderName, gameManagerPrefabName),
            Vector3.zero,
            Quaternion.identity
            );
    }
}
