using System;
using UnityEngine;

public class KeyButton : MonoBehaviour
{

    public string code;
    public bool isOneShot;

    [NonSerialized]
    public bool isOn;

    public void setOn(bool isOn)
    {
        this.isOn = isOn;
    }
}
