using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCP_Serwer
{
    class Program
    {
        const int port = 5591;
        static TcpListener listener;
        static void Main(string[] args)
            {
            try
            {
                listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
                listener.Start();
                Console.WriteLine("Ожидание подключений...");

                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    ClientObject clientObject = new ClientObject(client);

                    // создаем новый поток для обслуживания нового клиента
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (listener != null)
                    listener.Stop();
            }
        }
        public class ClientObject
        {
            public TcpClient client;
            public ClientObject(TcpClient tcpClient)
            {
                client = tcpClient;
            }
            
            
            static List<double> piList = new List<double>();
            static List<EventWaitHandle> handles = new List<EventWaitHandle>();


            public void Process()
            {

                NetworkStream stream = null;
                try
                {
                    stream = client.GetStream();
                    byte[] data = new byte[64]; // буфер для получаемых данных
                    while (true)
                    {
                        // получаем сообщение
                        StringBuilder builder = new StringBuilder();
                        int bytes = 0;
                        do
                        {
                            bytes = stream.Read(data, 0, data.Length);
                            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        }
                        while (stream.DataAvailable);

                        string message = builder.ToString();

                        
                        // отправляем обратно сообщение в верхнем регистре
                        // message = message.Substring(message.IndexOf(':') + 1).Trim().ToUpper();
                        if (message.IndexOf("r=") > -1)
                        {
                            try
                            {
                                double r1 = Convert.ToDouble(message.Split('&')[0].Split('=')[1]);
                                int n =Convert.ToInt32(message.Split('&')[1].Split('=')[1]);
                                Console.WriteLine(r1 + "  " + n);
                                for (int i = 0; i < n; i++)
                                {
                                    EventWaitHandle handle = new AutoResetEvent(false);
                                    new Thread(delegate () //указатель 
                                    { MonteCarloMethod(n, r1, handle); }).Start();
                                    handles.Add(handle);
                                }
                                WaitHandle.WaitAll(handles.ToArray());
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }

                        }





                        Console.WriteLine("Avarage of Pi is: {0}", piList.Average());
                        data = Encoding.Unicode.GetBytes(Convert.ToString( piList.Average()));
                        stream.Write(data, 0, data.Length);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    if (stream != null)
                        stream.Close();
                    if (client != null)
                        client.Close();
                }
            }
        
            static void MonteCarloMethod(int pointNumber,double radius, object handle)
            {
                Random r = new Random();
                AutoResetEvent wh = (AutoResetEvent)handle;
                double circle = 0;

                for (int i = 0; i < pointNumber; i++)
                {
                    if (IsCircle(1.0, r.NextDouble(), r.NextDouble()))
                    {
                        circle++;
                    }
                }
                piList.Add((4 * circle) / (double)pointNumber);
                wh.Set();
            }
            static bool IsCircle(double radius, double x, double y)
            {
                return ((x * x + y * y) <= radius * radius);
            }
        }
        }

    }

