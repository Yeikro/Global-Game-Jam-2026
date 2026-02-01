using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

// ==========================
// CLASES DE DATOS BASE (JSON)
// ==========================
[Serializable]
public class Punto
{
    public float x;
    public float y;
    public float z;

    public Vector3 ToVector3()
    {
        // Invertimos Y para que la cara se vea al derecho
        return new Vector3(x, 1f - y, z);
    }
}

[Serializable]
public class Ojos
{
    public List<Punto> izquierdo;
    public List<Punto> derecho;
}

[Serializable]
public class Cejas
{
    public List<Punto> izquierda;
    public List<Punto> derecha;
}

[Serializable]
public class FaceData
{
    public List<Punto> puntos;
    public Ojos ojos;
    public Cejas cejas;
    public List<Punto> boca;
}

// ==========================
// SCRIPT PRINCIPAL
// ==========================
public class FaceDataReceiver : MonoBehaviour
{
    [Header("Configuración UDP")]
    public int listenPort = 5052;
    private UdpClient udpClient;
    private Thread receiveThread;

    [Header("Datos JSON (raw)")]
    [TextArea(2, 6)]
    public string lastJson;

    [Header("Datos deserializados")]
    public FaceData faceData = new FaceData();

    [Header("Listas convertidas a Vector3")]
    public Vector3[] puntos;
    public Vector3[] ojoIzquierdo;
    public Vector3[] ojoDerecho;
    public Vector3[] cejaIzquierda;
    public Vector3[] cejaDerecha;
    public Vector3[] boca;

    [Header("Listas normalizadas (por distancia de ojos)")]
    public Vector3[] puntosNormalizados;
    public Vector3[] ojoIzquierdoNormalizado;
    public Vector3[] ojoDerechoNormalizado;
    public Vector3[] cejaIzquierdaNormalizada;
    public Vector3[] cejaDerechaNormalizada;
    public Vector3[] bocaNormalizada;

    public float distanciaOjos = 1f; // para debug

    private void Start()
    {
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
        Debug.Log($"🟢 Escuchando UDP en puerto {listenPort}");
    }

    private void ReceiveData()
    {
        udpClient = new UdpClient(listenPort);
        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

        try
        {
            while (true)
            {
                byte[] data = udpClient.Receive(ref remoteEP);
                string json = Encoding.UTF8.GetString(data);

                FaceData newData = JsonUtility.FromJson<FaceData>(json);
                if (newData != null)
                {
                    lock (this)
                    {
                        faceData = newData;
                        lastJson = json;

                        // Conversión a Vector3[]
                        puntos = ConvertirLista(newData.puntos);
                        ojoIzquierdo = ConvertirLista(newData.ojos?.izquierdo);
                        ojoDerecho = ConvertirLista(newData.ojos?.derecho);
                        cejaIzquierda = ConvertirLista(newData.cejas?.izquierda);
                        cejaDerecha = ConvertirLista(newData.cejas?.derecha);
                        boca = ConvertirLista(newData.boca);

                        // Calcular distancia entre los centros de los ojos
                        distanciaOjos = CalcularDistanciaOjos(ojoIzquierdo, ojoDerecho);

                        // Evitar división por cero
                        float factor = (distanciaOjos > 0.0001f) ? distanciaOjos : 1f;

                        // Generar versiones normalizadas
                        puntosNormalizados = Normalizar(puntos, factor);
                        ojoIzquierdoNormalizado = Normalizar(ojoIzquierdo, factor);
                        ojoDerechoNormalizado = Normalizar(ojoDerecho, factor);
                        cejaIzquierdaNormalizada = Normalizar(cejaIzquierda, factor);
                        cejaDerechaNormalizada = Normalizar(cejaDerecha, factor);
                        bocaNormalizada = Normalizar(boca, factor);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("❌ Error UDP: " + e.Message);
        }
    }

    private Vector3[] ConvertirLista(List<Punto> lista)
    {
        if (lista == null) return new Vector3[0];
        Vector3[] arr = new Vector3[lista.Count];
        for (int i = 0; i < lista.Count; i++)
            arr[i] = lista[i].ToVector3();
        return arr;
    }

    private float CalcularDistanciaOjos(Vector3[] ojoIzq, Vector3[] ojoDer)
    {
        if (ojoIzq == null || ojoDer == null || ojoIzq.Length == 0 || ojoDer.Length == 0)
            return 1f;

        // Usamos el promedio de los puntos de cada ojo como referencia
        Vector3 centroIzq = Vector3.zero;
        foreach (var p in ojoIzq) centroIzq += p;
        centroIzq /= ojoIzq.Length;

        Vector3 centroDer = Vector3.zero;
        foreach (var p in ojoDer) centroDer += p;
        centroDer /= ojoDer.Length;

        return Vector3.Distance(centroIzq, centroDer);
    }

    private Vector3[] Normalizar(Vector3[] lista, float factor)
    {
        if (lista == null) return new Vector3[0];
        Vector3[] result = new Vector3[lista.Length];
        for (int i = 0; i < lista.Length; i++)
            result[i] = lista[i] / factor;
        return result;
    }

    private void Update()
    {
        if (puntos != null && puntos.Length > 0)
        {
            //Debug.Log($"✅ {puntos.Length} puntos. Distancia ojos: {distanciaOjos:F3}");
        }
    }

    private void OnApplicationQuit()
    {
        receiveThread?.Abort();
        udpClient?.Close();
    }
}
