using System;

public enum ConnectableType
{
    Brush,
    Connector
}

public interface IConnectable
{
    public ConnectableType connectableType { get; set; }
    void ConnectWithConnector(Action releaserCallback, Action<Action> recieverCallback, Connector connector_);
    void SplitWithConnector(Action releaserCallback, Action<Action> recieverCallback, Connector connector_);
    void PutDownByConnector(Action releaserCallback, Action<Action> recieverCallback, float executeTime);
    void PutUpByConnector(Action releaserCallback, Action<Action> recieverCallback, float executeTime);
    void RotateWithConnector(Action releaserCallback, Action<Action> recieverCallback, RotateDirection rd, float executeTime);
    void MoveWithConnector(Action releaserCallback, Action<Action> recieverCallback, Direction md, float executeTime);
}
