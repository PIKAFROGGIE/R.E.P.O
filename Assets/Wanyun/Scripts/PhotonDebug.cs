using Photon.Pun;
using UnityEngine;

public class PhotonDebug : MonoBehaviourPunCallbacks
{
    public override void OnLeftRoom()
    {
        Debug.LogError("LEFT ROOM!");
    }

    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        Debug.LogError("DISCONNECTED: " + cause);
    }

    public override void OnMasterClientSwitched(Photon.Realtime.Player newMaster)
    {
        Debug.LogWarning("MASTER SWITCHED to: " + newMaster.NickName);
    }
}
