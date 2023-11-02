using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace JoshBowersDev.DevTestST.Networking
{
    public class NetworkManager : MonoBehaviourPunCallbacks
    {
        public static NetworkManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(Instance);
            }

            Initialize();

            PhotonNetwork.AddCallbackTarget(this);
        }

        public void Initialize()
        {
            PhotonNetwork.SerializationRate = 10;
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();
            Debug.Log("Connected to the MasterServer");

            RoomOptions roomOptions = new RoomOptions();
            roomOptions.IsOpen = true;
            roomOptions.IsVisible = true;
            TypedLobby lobbyOptions = new TypedLobby("SpecularTheory", LobbyType.Default);

            if (!PhotonNetwork.JoinOrCreateRoom("DialMania", roomOptions, lobbyOptions))
            {
                Debug.LogError("Could not create or join a room.");
            }
            else
            {
                PhotonNetwork.LocalPlayer.NickName = "Player " + Mathf.RoundToInt(Random.Range(0, 25));
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);

            Application.Quit();
        }
    }
}