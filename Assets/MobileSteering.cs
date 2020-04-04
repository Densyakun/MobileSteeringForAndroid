using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class MobileSteering : MonoBehaviour
{

    public GameObject obj;
    public Transform steeringImage;
    public GameObject startImage;
    public InputField ipInput;
    public InputField portInput;
    public KeyButton[] buttons;
    public Toggle sendAnytimeToggle;

    private UdpClient udp;
    private Quaternion ic = Quaternion.identity;
    private bool[] lastOn;

    void Start()
    {
        lastOn = new bool[buttons.Length];
        Input.gyro.enabled = true;
    }

    void Update()
    {
        var q = ic * gyroToQuaternion(Input.gyro.attitude);
        var r = 1f - Mathf.Repeat(q.eulerAngles.z / 180f + 1f, 2f);

        obj.transform.localRotation = Quaternion.Inverse(q);
        steeringImage.localScale = new Vector3(r, 1f, 1f);

        if (udp != null)
        {
            send("ST" + r);

            for (var n = 0; n < buttons.Length; n++)
            {
                if (!sendAnytimeToggle.isOn && buttons[n].isOneShot)
                {
                    if (buttons[n].isOn)
                    {
                        send(buttons[n].code + "2");
                        buttons[n].isOn = false;
                    }
                }
                else if (sendAnytimeToggle.isOn || buttons[n].isOn != lastOn[n])
                {
                    send(buttons[n].code + (buttons[n].isOn ? "1" : "0"));
                    lastOn[n] = buttons[n].isOn;
                }
            }
        }
    }

    public Quaternion gyroToQuaternion(Quaternion q)
    {
        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }

    public void connect()
    {
        try
        {
            udp = new UdpClient(ipInput.text, int.Parse(portInput.text));
            startImage.SetActive(false);
        }
        catch { };
    }

    public void send(string text)
    {
        var sendBytes = System.Text.Encoding.ASCII.GetBytes(text);
        udp.Send(sendBytes, sendBytes.Length);
    }

    public void reset()
    {
        ic = Quaternion.Inverse(gyroToQuaternion(Input.gyro.attitude));
    }
}
