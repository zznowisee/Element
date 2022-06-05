using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Device_Extender : Device
{
    public Data_Extender data;
    GameObject sprite;
    public override void OnEnable()
    {
        base.OnEnable();
        sprite = transform.Find("sprite").gameObject;
        sprite.transform.rotation = Quaternion.Euler(Vector3.forward * -30f);
        deviceType = DeviceType.Extender;
    }

    public override void Setup(HexCell cell_)
    {

        base.Setup(cell_);
    }

    public override void Dragging()
    {
        base.Dragging();

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Rotate(RotateDirection.CCW);
        }else if (Input.GetKeyDown(KeyCode.E))
        {
            Rotate(RotateDirection.CW);
        }

        void Rotate(RotateDirection rotateDirection)
        {
            switch (rotateDirection)
            {
                //ccw
                case RotateDirection.CCW:
                    data.direction = data.direction.Previous();
                    sprite.transform.rotation = Quaternion.Euler(sprite.transform.eulerAngles + Vector3.forward * 60f);
                    break;
                //cw
                case RotateDirection.CW:
                    data.direction = data.direction.Next();
                    sprite.transform.rotation = Quaternion.Euler(sprite.transform.eulerAngles - Vector3.forward * 60f);
                    break;
            }
        }
    }
}
