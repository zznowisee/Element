using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Device_Marker : Device
{

    private void Awake()
    {
        deviceType = DeviceType.Marker;
    }

    public override void Setup(HexCell cell_)
    {
        base.Setup(cell_);
        
    }
}
