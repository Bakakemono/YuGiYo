using MLAPI;
using UnityEngine;

namespace HelloWorld {
    public class HelloWorldManager : MonoBehaviour {

        private void OnGUI() {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if(NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer) {
                StartButtons();
            }
            else {
                StatusLabels();

                SubmitNewPosition();
            }

            GUILayout.EndArea();
        }

        static void StartButtons() {
            if (GUILayout.Button("Host"))
                NetworkManager.Singleton.StartHost();

            if (GUILayout.Button("Client"))
                NetworkManager.Singleton.StartClient();

            if (GUILayout.Button("Server"))
                NetworkManager.Singleton.StartServer();
        }

        static void StatusLabels() {
            string mode = NetworkManager.Singleton.IsHost ? "Host" :
                NetworkManager.Singleton.IsServer ? "Server" : "Client";

            GUILayout.Label("Transport: " +
                NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
            GUILayout.Label("mode: " + mode);
        }

        static void SubmitNewPosition() {
            if(GUILayout.Button(NetworkManager.Singleton.IsServer ? "Move" : "Request position Change")) {
                if(NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId, out var networkClient)) {
                    var player = networkClient.PlayerObject.GetComponent<HelloWorldPlayer>();
                    if (player)
                        player.Move();
                }
            }
        }
    }
}
