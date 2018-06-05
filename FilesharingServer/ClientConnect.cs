using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FilesharingServer
{
    public class ClientConnect
    {
        private TcpClient client;

        public async void StartWork(TcpListener server)
        {
            Console.WriteLine("Сервер запущен!");
            try
            {
                server.Start();
                while (true)
                {
                    client = await server.AcceptTcpClientAsync();
                    ThreadPool.QueueUserWorkItem(new WaitCallback(Work), null);

                    server.Start();
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (server != null)
                {
                    server.Stop();
                    Console.WriteLine("Сервер прекратил работу!");
                }
            }
        }

        private void Work(object obj)
        {
            using (NetworkStream stream = client.GetStream())
            {
                try
                {
                    byte[] buffer = new byte[1024];
                    while (true)
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        int bytes;
                        do
                        {
                            bytes = stream.Read(buffer, 0, buffer.Length);
                            stringBuilder.Append(Encoding.Default.GetString(buffer, 0, bytes));
                        } while (stream.DataAvailable);

                        FileTransport file = JsonConvert.DeserializeObject<FileTransport>(stringBuilder.ToString());
                        Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\files\");
                        using (FileStream fileStream = new FileStream(Directory.GetCurrentDirectory() + @"\files\" + file.FileName, FileMode.Create))
                        {
                            fileStream.Write(file.Data, 0, file.Data.Length);
                        }
                        Console.WriteLine("Был загружен новый файл");
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }
                finally
                {
                    client.Close();
                }
            }
        }
    }
}
