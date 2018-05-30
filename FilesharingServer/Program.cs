using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FilesharingServer
{
    class Program
    {
        private const string SERVER_IP = "127.0.0.1";
        private const int SERVER_PORT = 3535;

        static void Main(string[] args)
        {
            ClientConnect clientConnect = new ClientConnect();
            TcpListener server = new TcpListener(IPAddress.Parse(SERVER_IP), SERVER_PORT);
            try
            {
                server.Start();
                clientConnect.StartWork(server);
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                server.Stop();
            }
        }
    }
}
