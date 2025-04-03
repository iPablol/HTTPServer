using System.Net.Sockets;
using System.Net;
using System.Text;

namespace HTTPServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Test();
            while (true)
            {
            }
        }

        static async void Test()
        {

            var ipEndPoint = new IPEndPoint(IPAddress.Loopback, 80);
            TcpListener listener = new(ipEndPoint);

            try
            {
                listener.Start();
                while (true)
                {
                    if (listener.Pending())
                    {
                        HandleConnection(listener);
                    }
                }
            }
            finally
            {
                listener.Stop();
            }
        }

        static async void HandleConnection(TcpListener listener)
        {
            using TcpClient handler = await listener.AcceptTcpClientAsync();
            await using NetworkStream stream = handler.GetStream();
            var buffer = new byte[1_024];
            int received = await stream.ReadAsync(buffer);

            string message = Encoding.UTF8.GetString(buffer, 0, received);
            Console.WriteLine($"Read message: \"{message}\"");

            await stream.WriteAsync(Encoding.UTF8.GetBytes($"Response to \"{message}\""));
        }
    }
    
}
