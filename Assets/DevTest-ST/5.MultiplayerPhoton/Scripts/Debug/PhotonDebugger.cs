using DG.Tweening;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class PhotonDebugger : MonoBehaviour
{
    public TextMeshProUGUI NetworkStateText;
    public TextMeshProUGUI IPAddressText;
    public TextMeshProUGUI RoomNameText;
    public TextMeshProUGUI RegionNameText;
    public TextMeshProUGUI PlayerCountText;

    private string _networkClientState => PhotonNetwork.NetworkClientState.ToString();
    private string _ipAddress => PhotonNetwork.ServerAddress;
    private string _roomName => PhotonNetwork.CurrentRoom.Name;
    private string _region => PhotonNetwork.CloudRegion;
    private int _playerCount => PhotonNetwork.CurrentRoom.PlayerCount;

    private RectTransform _rectTransform;
    private bool _isActive = false;
    private float _yVal = 0f;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _yVal = _rectTransform.sizeDelta.y;
    }

    private void Update()
    {
        if (!gameObject.activeSelf)
            return;

        if (NetworkStateText.text != _networkClientState)
            NetworkStateText.text = _networkClientState;
        if (IPAddressText.text != _ipAddress)
            IPAddressText.text = _ipAddress;
        if (PhotonNetwork.CurrentRoom != null && RoomNameText.text != _roomName)
            RoomNameText.text = _roomName;
        if (RegionNameText.text != _region)
            RegionNameText.text = _region;
        if (PhotonNetwork.CurrentRoom != null && PlayerCountText.text != _playerCount.ToString())
            PlayerCountText.text = _playerCount.ToString();
    }
}