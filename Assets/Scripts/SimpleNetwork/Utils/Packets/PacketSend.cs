using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleNetwork
{
    class ServerSend
    {
        public static void SendData(int _id, Packet _packet)
        {
            _packet.WriteLength();
            Console.WriteLine(Server.clients[_id].socket.Client.RemoteEndPoint.ToString
                ());
            Server.clients[_id].SendData(_packet);
        }

        public static void SendDataToAllPlayer(Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                Server.clients[i].SendData(_packet);
            }
        }

        public static void Welcome(int id, string welcomeString)
        {
            var _packet = new Packet((int)ServerPackets.welcome);
            _packet.Write(id);
            _packet.Write(welcomeString);
            SendData(id, _packet);
        }
    }

    class ClientSend
    {
        public static void SendData(Packet _packet)
        {
            _packet.WriteLength();
            Client.instance.SendData(_packet);
        }

        public static void WelcomeReceived()
        {
            var packet = new Packet((int)ClientPackets.welcomeReceived);
            packet.Write(Client.instance.id);
            packet.Write(Client.instance.username);
            SendData(packet);
        }
    }
}
