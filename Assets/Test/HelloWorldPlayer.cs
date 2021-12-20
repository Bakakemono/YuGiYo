using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;

namespace HelloWorld {
    public class HelloWorldPlayer : NetworkBehaviour {
        public NetworkVariableVector3 position = new NetworkVariableVector3(new NetworkVariableSettings {
            WritePermission = NetworkVariablePermission.ServerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        });

        public override void NetworkStart() {
            Move();
        }

        public void Move() {
            if (NetworkManager.Singleton.IsServer) {
                var randomPosition = GetRandomPositionOnPlane();
                transform.position = randomPosition;
                position.Value = randomPosition;
            }
            else {
                SubmitPositionRequestServerRpc();
            }
        }

        [ServerRpc]
        void SubmitPositionRequestServerRpc(ServerRpcParams rpcParams = default) {
            position.Value = GetRandomPositionOnPlane();
        }

        static Vector3 GetRandomPositionOnPlane() {
            return new Vector3(Random.Range(-3.0f, 3.0f), 1.0f, Random.Range(-3.0f, 3.0f));
        }

        private void Update() {
            transform.position = position.Value;
        }
    }
}
