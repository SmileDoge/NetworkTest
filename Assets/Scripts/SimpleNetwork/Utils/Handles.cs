using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SimpleNetwork
{
    //Events called in Server
    class ServerHandle
    {
        public static void WelcomeReceived(int id, Packet packet)
        {
            Console.WriteLine("Real ID: " + id + "\nReceived ID: " + packet.ReadInt());
            Console.WriteLine("Username: " + packet.ReadString());
        }

        public static void PlayerMovement(int id, Packet packet)
        {

        }
    }
    //Events called in Client
    class ClientHandle
    {
        public static void Welcome(Packet packet)
        {
            Client.instance.id = packet.ReadInt();
            Debug.Log(packet.ReadString());

            ClientSend.WelcomeReceived();
        }

        public static void SpawnPlayer(Packet packet)
        {

        }

        public static void PlayerPosition(Packet packet)
        {

        }

        public static void PlayerRotation(Packet packet)
        {

        }
    }
}
