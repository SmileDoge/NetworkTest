using SimpleNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Network : MonoBehaviour
{
    public static bool ServerBuild;
    void Awake()
    {
#if UNITY_SERVER
        Console.Clear();
        ServerBuild = true;
#else
        ServerBuild = false;
#endif
    }

    void Start()
    {
        if (ServerBuild)
        {
            Server.Start(10,7000);
            Console.WriteLine("Server");
        }
        else
        {
            var client = new Client();
            client.username = "Smile";
            client.Connect("127.0.0.1", 7000);
            Debug.Log("Client");
        }
    }

    
    void Update()
    {
        if (ServerBuild)
        {
            
        }
    }
}
