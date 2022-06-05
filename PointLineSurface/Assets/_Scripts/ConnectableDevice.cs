using System.Collections;
using UnityEngine;
using System;

public class ConnectableDevice : Device
{
    public bool beConnectedThisFrame = false;
    //after reading each command , get the target 
    public Device_Connector connectingConnector;
    [SerializeField] protected ConnectLine pfConnectLine;
    public ConnectLine connectLine;

    public virtual void ConnectWithConnector(Device_Connector target)
    {
        beConnectedThisFrame = true;
        connectingConnector = target;
        target.connectingDevices.Add(this);
        connectLine = Instantiate(pfConnectLine, transform);
        connectLine.Setup(this, target);
        StartCoroutine(ResetConnectState());
        IEnumerator ResetConnectState()
        {
            yield return new WaitForEndOfFrame();
            beConnectedThisFrame = false;
        }
    }

    public virtual void SplitWithConnector(Device_Connector target)
    {
        connectingConnector = null;
        target.connectingDevices.Remove(this);
        Destroy(connectLine.gameObject);
    }

    public void MoveAsChild(Action releaserCallback, Action<Action> recieverCallback, HexCell target, float executeTime)
    {
        StartCoroutine(MoveToTarget(releaserCallback, recieverCallback, target, executeTime));
    }

    public virtual IEnumerator MoveToTarget(Action releaserCallback, Action<Action> callback, HexCell target, float executeTime)
    {
        return null;
    }

    public void MoveAsChild(Action releaserCallback, Action<Action> recieverCallback, Direction moveDir, float executeTime)
    {
        HexCell target = cell.GetNeighbor(moveDir);
        StartCoroutine(MoveToTarget(releaserCallback, recieverCallback, target, executeTime));
    }

    public virtual void Putup(Action releaserCallback, Action<Action> recieverCallback, float executeTime) { }
    public virtual void Putdown(Action releaserCallback, Action<Action> recieverCallback, float executeTime) { }
}
