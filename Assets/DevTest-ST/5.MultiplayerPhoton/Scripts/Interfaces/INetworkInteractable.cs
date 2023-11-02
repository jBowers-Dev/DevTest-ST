using Photon.Pun;

namespace JoshBowersDev.DevTestST.Networking
{
    public interface INetworkInteractable
    {
        void OnBeginInteraction() { }

        void OnUpdateInteraction(object value) { }

        void OnEndInteraction() { }

        [PunRPC]
        public void OnSyncIsInteractable(bool val) { }

        [PunRPC]
        void OnSyncNetworkUpdate(object val) { }
    }
}