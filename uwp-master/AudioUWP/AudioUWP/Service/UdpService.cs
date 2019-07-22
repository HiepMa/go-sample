using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AudioUWP
{
    public class UdpService
    {
        private static readonly string ServerAddress = "192.168.82.12";
        private static readonly int ServerPort = 11111;

        UdpClient udpClient;
        IPEndPoint ep;

        protected UdpService() {
            udpClient = new UdpClient();
            ep = new IPEndPoint(IPAddress.Parse(ServerAddress), ServerPort);
            udpClient.Connect(ep);
        }

        protected UdpService(String ipAddress)
        {
            udpClient = new UdpClient();
            ep = new IPEndPoint(IPAddress.Parse(ipAddress), ServerPort);
            udpClient.Connect(ep);
        }

        private static UdpService _instance = null;

        public static UdpService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UdpService();
                }
                return _instance;

            }
        }

        public static void startup(String ipAddress) {
            
            if (_instance == null)
            {
                _instance = new UdpService(ipAddress);
            }
        }


        public void StartClient()
        {
            try
            {
                Debug.WriteLine("=============================");
                Debug.WriteLine("Client Start");
                udpClient.Send(new byte[] { 1, 2, 3, 4, 5 }, 5);
                Debug.WriteLine("Connect " + ep.ToString());
                var receicedata = udpClient.Receive(ref ep);
                Debug.WriteLine("Connect");
                Debug.WriteLine("receive data from " + ep.ToString());
                Console.Read();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void SendMessage(byte[] arrayBytes, int bytes)
        {
            try
            {
                udpClient.Send(arrayBytes, bytes);
                Debug.WriteLine("Connect " + ep.ToString());
                // var receicedata = udpClient.Receive(ref ep);
                // Debug.WriteLine("receive data from " + ep.ToString());

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public Byte[] ReceiveMessage() {
            var remoteEP = new IPEndPoint(IPAddress.Any, 11111);
            var receicedata = udpClient.Receive(ref remoteEP);
            return receicedata;
        }

        public void StartServerThread() {
            Thread sendAudioThread = new Thread(ServerStart);
            sendAudioThread.Start();
        }

        public void ServerStart()
        {            
            UdpClient udpserver = new UdpClient(11111);
            Debug.WriteLine("=============================");
            Debug.WriteLine("Server Start");
            while (true)
            {
                var remoteEP = new IPEndPoint(IPAddress.Any, 11111);
                var data = udpserver.Receive(ref remoteEP);
                Debug.WriteLine("receice data from " + remoteEP.ToString());
                udpserver.Send(new byte[] { 1 }, 1, remoteEP);
            }
        }
    }
}
