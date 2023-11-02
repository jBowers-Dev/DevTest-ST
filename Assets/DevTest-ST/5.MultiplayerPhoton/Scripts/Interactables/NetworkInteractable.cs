using JoshBowersDev.DevTestST.AdvancedInteraction;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace JoshBowersDev.DevTestST.Networking
{
    [RequireComponent(typeof(PhotonView))]
    public class NetworkInteractable : MonoBehaviour, INetworkInteractable, IPunOwnershipCallbacks, IPunObservable
    {
        [SerializeField]
        private int _messageThreshold = 1;

        private int _messageCounter = 0;

        private PhotonView _photonView;
        private IInteractable _interactable;
        private IInteractableHelper[] _interactableHelpers;
        private object _data = null;

        private void OnEnable()
        {
            if (_interactable == null)
            {
                try
                {
                    _interactable = GetComponent<IInteractable>();
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"No IInteractable component found on this game object.\n {e}", this);
                    enabled = false;
                    return;
                }
            }

            PhotonNetwork.AddCallbackTarget(this);

            _interactable.BeginInteraction += OnBeginInteraction;
            _interactable.UpdateInteraction += OnUpdateInteraction;
            _interactable.EndInteraction += OnEndInteraction;
        }

        private void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);

            _interactable.BeginInteraction -= OnBeginInteraction;
            _interactable.UpdateInteraction -= OnUpdateInteraction;
            _interactable.EndInteraction -= OnEndInteraction;
        }

        private void Start()
        {
            _photonView = GetComponent<PhotonView>();
            if (_photonView.Owner == null)
            {
                _photonView.RequestOwnership();
            }

            if (GetComponents<IInteractable>() != null)
                _interactableHelpers = GetComponents<IInteractableHelper>();
        }

        public void OnBeginInteraction()
        {
            if (!_interactable.IsInteractable)
                return;

            if (_photonView.Owner == null)
            {
                if (PhotonNetwork.LogLevel == PunLogLevel.Informational)
                    Debug.Log($"No ownership, {PhotonNetwork.LocalPlayer.NickName} is taking over {name}.");

                _photonView.RequestOwnership();
                _interactable.SetInteractable(true);
                _photonView.RPC("OnSyncIsInteractable", RpcTarget.Others, false);
            }
            else
            {
                if (_photonView.IsMine)
                {
                    _interactable.SetInteracting(true);
                    _photonView.RPC("OnSyncIsInteractable", RpcTarget.Others, false);

                    if (PhotonNetwork.LogLevel == PunLogLevel.Informational)
                        Debug.Log($"{name} already has ownership.");
                }
                else
                {
                    _photonView.RequestOwnership();

                    if (PhotonNetwork.LogLevel == PunLogLevel.Informational)
                        Debug.Log($"{name} is requesting ownership.");
                }
            }
        }

        public void OnUpdateInteraction(object val)
        {
            if (_photonView.IsMine)
                _data = val;
        }

        public void OnEndInteraction()
        {
            _interactable.SetInteracting(false);
            if (_photonView != null)
            {
                _photonView.RPC("OnSyncIsInteractable", RpcTarget.Others, true);
            }
        }

        [PunRPC]
        public void OnSyncIsInteractable(bool val)
        {
            if (_interactable.IsInteractable == val)
                return;
            _interactable.SetInteractable(val);
            if (_interactableHelpers != null)
            {
                int count = _interactableHelpers.Length;
                for (int i = 0; i < count; i++)
                {
                    _interactableHelpers[i].OnInteractableUpdated(val); // Alert each helper that may be attached
                }
            }
        }

        [PunRPC]
        public void OnSyncNetworkUpdate(object val)
        {
            _interactable.ManualControl(val);
            int count = _interactableHelpers.Length;
            for (int i = 0; i < count; i++)
            {
                _interactableHelpers[i].OnUpdateInteraction(val);
            }
        }

        // Pun Callbacks

        public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)

        {
            // If this isn't the target view, return
            if (targetView != _photonView)
                return;

            if (PhotonNetwork.LogLevel == PunLogLevel.Informational)
                Debug.Log($"{requestingPlayer.NickName} requested ownership.");

            // As long as the current player is not already interacting and is the current owner, transfer
            if (!_interactable.IsInteracting &&
                (targetView.Owner == PhotonNetwork.LocalPlayer || targetView.Owner == null))
            {
                if (PhotonNetwork.LogLevel == PunLogLevel.Informational)
                    Debug.Log($"{requestingPlayer.NickName} was granted ownership.");
                _photonView.TransferOwnership(requestingPlayer);
                _interactable.SetInteractable(false);
            }
            else
            {
                if (PhotonNetwork.LogLevel == PunLogLevel.Informational)
                    Debug.Log($"{requestingPlayer.NickName} was denied ownership.");
            }
        }

        public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
        {
            if (targetView != _photonView)
                return;

            // If this targetview is now mine, set it to interacting
            if (targetView.IsMine)
            {
                _interactable.SetInteracting(true);
            }
        }

        public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
        {
            if (targetView != _photonView)
                return;

            if (PhotonNetwork.LogLevel == PunLogLevel.Informational)
                Debug.LogError($"{targetView.name} failed to transfer from {senderOfFailedRequest.NickName}");
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            // Synchronize so that players who join late will get the most up to date interactable data.
            if (stream.IsWriting)
            {
                stream.SendNext(_data);
            }
            else
            {
                _data = stream.ReceiveNext();
                OnSyncNetworkUpdate(_data);
            }
        }
    }
}