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
            //MemoryMonitoring memoryMonitoring = new MemoryMonitoring();
            //memoryMonitoring.listAllSensors();
            //Console.WriteLine("MaxMemory:" + memoryMonitoring.MaxMemory);

            httpServer =  new Socket(SocketType.Stream, ProtocolType.Tcp);
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
            while(true)
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

                Console.WriteLine(data);

                String resHeader = "HTTP/1.1 200 Everything is Fine\nServer: my_csharp_server\nContent-Type: text/html; charset: UTF-8\n\n";
                String resBody = "<!DOCTYE html> " +
                    "<html>" +
                    "<head><title>My Server</title></head>" +
                    "<body>" +
                    "<h4>Server Time is: " + time.ToString() + "</h4>" +
                    "</body></html>";

                String resStr = resHeader + resBody;

                byte[] resData = Encoding.ASCII.GetBytes(resStr);


                client.SendTo(resData, client.RemoteEndPoint);

                client.Close();
            }
        }
    }
}
