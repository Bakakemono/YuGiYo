using Photon.Pun;
using UnityEngine;
using System.IO;

public class NetworkSpawner : MonoBehaviourPunCallbacks
{
    [SerializeField] string prefabsFolderName;
    [SerializeField] string playerPrefabName;
    [SerializeField] string cardManagerPrefabName;
    [SerializeField] string gameManagerPrefabName;

    private void Start() {
        if(PhotonNetwork.IsMasterClient) {
            SpawnGameManager();
        }
    }

    public void SpawnPlayer() {
        PhotonNetwork.Instantiate(
            Path.Combine(prefabsFolderName, playerPrefabName),
            Vector3.zero,
            Quaternion.identity
            );
    }

    public void SpawnCardManager() {
        PhotonNetwork.Instantiate(
            Path.Combine(prefabsFolderName, cardManagerPrefabName),
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
