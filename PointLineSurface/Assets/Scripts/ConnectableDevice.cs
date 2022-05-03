using System.Collections;
using UnityEngine;
using System;

public class ConnectableDevice : Device
{
    //after reading each command , get the target 
    public HexCell target;
    public Connector connectingConnector;
    [SerializeField] protected ConnectLine pfConnectLine;
    public ConnectLine connectLine;

    public virtual void ConnectWithConnector(Connector target)
    {
        if(target != connectingConnector)
        {
            connectingConnector = target;
            target.connectingDevices.Add(this);
            connectLine = Instantiate(pfConnectLine, transform);
            connectLine.Setup(this, target);
        }
    }

    public virtual void SplitWithConnector(Connector target)
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

    public virtual void PutDownUp(Action releaserCallback, Action<Action> recieverCallback, BrushState state, float executeTime) { }
}
