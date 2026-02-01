using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPImageReceiver : MonoBehaviour
{
    public RawImage rawImage;
    public int port = 5053;

    UdpClient udpClient;
    Thread receiveThread;

    byte[] lastFrame;
    bool hasNewFrame = false;

    Texture2D tex;

    void Start()
    {
        tex = new Texture2D(2, 2, TextureFormat.RGB24, false);
        udpClient = new UdpClient(port);

        receiveThread = new Thread(ReceiveImage);
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    void ReceiveImage()
    {
        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, port);

        while (true)
        {
            try
            {
                byte[] data = udpClient.Receive(ref remoteEP);

                // SOLO guardar bytes
                lastFrame = data;
                hasNewFrame = true;
            }
            catch { }
        }
    }

    void Update()
    {
        if (hasNewFrame)
        {
            tex.LoadImage(lastFrame);
            rawImage.texture = tex;
            hasNewFrame = false;
        }
    }

    void OnApplicationQuit()
    {
        receiveThread?.Abort();
        udpClient?.Close();
    }
}