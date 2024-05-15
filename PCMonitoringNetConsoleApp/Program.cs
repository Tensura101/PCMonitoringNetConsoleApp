using PCMonitoringConsoleApp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace PCMonitoringNetConsoleApp
{
    internal class Program
    {
        private static Socket httpServer;
        private static int port = 5050;

        static void Main(string[] args)
        {
            httpServer = new Socket(SocketType.Stream, ProtocolType.Tcp);
            Thread th = new Thread(new ThreadStart(httpConnect));
            th.Start();
        }


        private static void httpConnect()
        {
            try
            {
                IPEndPoint endPoint =  new IPEndPoint(IPAddress.Any, port);
                httpServer.Bind(endPoint);
                httpServer.Listen(1);
                httpListener();
            }
            catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }
        }


        private static void httpListener()
        {
            MemoryMonitoring memoryMonitoring = new MemoryMonitoring();
            GPUMonitoring gpuMonitoring = new GPUMonitoring();
            CPUMonitoring cpuMonitoring = new CPUMonitoring();
            Console.WriteLine(Monitoring.monitorsToJson());
            while (true)
            {
                DateTime time  = DateTime.Now;

                String data = "";
                byte[] bytes = new byte[2048];

                Socket client = httpServer.Accept();
                while(true)
                {
                    int numBytes = client.Receive(bytes);
                    data += Encoding.ASCII.GetString(bytes, 0, numBytes);

                    if (data.IndexOf("\r\n") > -1)
                        break;
                }

                Monitoring.updateStates();

                Console.WriteLine(data);

                String resHeader  = "HTTP/1.1 200 OK\r\n" +
                                        "Access-Control-Allow-Origin: *\r\n" + // Allow requests from any origin
                                        "Content-Type: text/json\r\n\r\n";
                String resBody = Monitoring.monitorsToJson();

                String resStr = resHeader + resBody;

                byte[] resData = Encoding.ASCII.GetBytes(resStr);


                client.SendTo(resData, client.RemoteEndPoint);

                client.Close();
            }
        }
    }
}
