using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCP_Client
{
    class Program
    {
        const int port = 5591;
        const string address = "127.0.0.1";
        static void Main(string[] args)
        {
          
            TcpClient client = null;
            try
            {
                client = new TcpClient(address, port);
                NetworkStream stream = client.GetStream();

                while (true)
                {
                    string message; /*= Console.ReadLine();*/
                    message = "r=";
                    Console.WriteLine("inter radius");
                    
                    message += Console.ReadLine();
                    message += "&n=";
                    Console.WriteLine("inter n");
                    message += Console.ReadLine();
                    
                    Console.WriteLine(message);

                    message = String.Format( message);
                    // преобразуем сообщение в массив байтов
                    byte[] data = Encoding.Unicode.GetBytes(message);
                    // отправка сообщения
                    stream.Write(data, 0, data.Length);

                    // получаем ответ
                    data = new byte[64]; // буфер для получаемых данных
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    message = builder.ToString();
                    Console.WriteLine("Serwer: {0}", message);
                    Console.ReadKey();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            //finally
            //{
            //    client.Close();
            //}
        }
    }
}
