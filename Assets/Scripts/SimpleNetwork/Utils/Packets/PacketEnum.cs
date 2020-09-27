using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleNetwork
{
    //Sent from server to client
    enum ServerPackets
    {
        welcome = 1,
        spawnPlayer,
        playerPosition,
        playerRotation
    }
    //Sent from client to server
    enum ClientPackets 
    {
        welcomeReceived = 1,
        playerMovement
    }
}
