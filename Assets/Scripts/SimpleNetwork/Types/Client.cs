using System;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

namespace SimpleNetwork
{
    public class Client
    {
        public static Client instance = null;

        public int id;
        public string username;

        #region Private
        private string _address;
        private int _port;
        private TcpClient _tcpClient;
        private NetworkStream _stream;

        private const int _dataBufferSize = 4096;
        private byte[] _receiveBuffer;
        private Packet _receivedData;
        #endregion

        public delegate void PacketHandler(Packet _packet);
        private Dictionary<int, PacketHandler> _packetHandlers = new Dictionary<int, PacketHandler>();

        public void Connect(string ipAddress, int port)
        {
            if (instance != null) { return; }
            instance = this;
            InitializeClientData();

            _address = ipAddress;
            _port = port;

            _tcpClient = new TcpClient
            {
                ReceiveBufferSize = _dataBufferSize,
                SendBufferSize = _dataBufferSize
            };

            _receiveBuffer = new byte[_dataBufferSize];

            _tcpClient.BeginConnect(_address, _port, connectCallback, _tcpClient);
        }

        private void InitializeClientData()
        {
            _packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)ServerPackets.welcome, ClientHandle.Welcome },
            { (int)ServerPackets.spawnPlayer, ClientHandle.SpawnPlayer },
            { (int)ServerPackets.playerPosition, ClientHandle.PlayerPosition },
            { (int)ServerPackets.playerRotation, ClientHandle.PlayerRotation },
        };
            Debug.Log("Initialized packets.");
        }

        public void SendData(Packet _packet)
        {
            try
            {
                if (_tcpClient != null)
                {
                    _stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                }
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error sending data to server via TCP: {_ex}");
            }
        }

        private void connectCallback(IAsyncResult _result)
        {
            _tcpClient.EndConnect(_result);
            
            if (!_tcpClient.Connected)
            {
                return;
            }

            _stream = _tcpClient.GetStream();

            _receivedData = new Packet();

            _stream.BeginRead(_receiveBuffer, 0, _dataBufferSize, receiveCallback, null);
        }

        private void receiveCallback(IAsyncResult _result)
        {
            try
            {
                var _byteLength = _stream.EndRead(_result);
                if (_byteLength <= 0)
                {
                    return;
                }
               
                
                var _data = new byte[_byteLength];
                Array.Copy(_receiveBuffer, _data, _byteLength);
                
                _receivedData.Reset(HandleData(_data));
                _stream.BeginRead(_receiveBuffer, 0, _dataBufferSize, receiveCallback, null);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        private bool HandleData(byte[] _data)
        {
            int _packetLength = 0;

            _receivedData.SetBytes(_data);

            if (_receivedData.UnreadLength() >= 4)
            {
                _packetLength = _receivedData.ReadInt();
                if (_packetLength <= 0)
                {
                    return true;
                }
            }

            while (_packetLength > 0 && _packetLength <= _receivedData.UnreadLength())
            {
                byte[] _packetBytes = _receivedData.ReadBytes(_packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        int _packetId = _packet.ReadInt();
                        _packetHandlers[_packetId](_packet);
                    }
                });

                _packetLength = 0;
                if (_receivedData.UnreadLength() >= 4)
                {
                    _packetLength = _receivedData.ReadInt();
                    if (_packetLength <= 0)
                    {
                        return true;
                    }
                }
            }

            if (_packetLength <= 1)
            {
                return true;
            }

            return false;
        }

    }
}