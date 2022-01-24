using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ConnectableDevice : Device
{
    //after reading each command , get the target 
    public HexCell target;
    public List<ConnectableDevice> connectingDevices;
    [SerializeField] protected ConnectLine pfConnectLine;
    public ConnectLine connectLine;

    public virtual void Awake()
    {
        connectingDevices = new List<ConnectableDevice>();
    }

    public virtual void ConnectWithConnector(ConnectableDevice target)
    {
        connectingDevices.Add(target);
        target.connectingDevices.Add(this);
        target.connectLine = Instantiate(pfConnectLine, transform);
        target.connectLine.Setup(this, target);
    }

    public virtual void SplitWithConnector(ConnectableDevice target)
    {
        connectingDevices.Remove(target);
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

    public virtual void PutDownUp(Action releaserCallback, Action<Action> recieverCallback, BrushType type, float executeTime)
    {

    }
}
