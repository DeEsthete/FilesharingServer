using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FilesharingServer
{
    public class ClientConnect
    {
        public void StartWork(TcpListener server)
        {
            while (true)
            {
                Work(server);
            }
        }

        public async void Work(TcpListener server)
        {
            //server.Start();
            while (true)
            {
                TcpClient client = await server.AcceptTcpClientAsync();
                await ClientLoop(client);
            }
        }

        private Task ClientLoop(TcpClient client)
        {
            return Task.Run(() =>
            {
                while (true)
                {
                    int bytes;
                    byte[] buffer = new byte[1024];
                    StringBuilder stringBuilder = new StringBuilder();
                    using (var networkStream = client.GetStream())
                    {
                        do
                        {
                            bytes = networkStream.Read(buffer, 0, buffer.Length);
                            stringBuilder.Append(Encoding.Default.GetString(buffer, 0, bytes));
                        }
                        while (networkStream.DataAvailable);
                    }

                    FileTransport file = JsonConvert.DeserializeObject<FileTransport>(stringBuilder.ToString());
                    File.WriteAllBytes(Directory.GetCurrentDirectory() + @"\files" + file.FileName + file.Expansion, file.Data);
                }
            });
        }
    }
}
