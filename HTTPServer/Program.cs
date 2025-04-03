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

            var ipEndPoint = new IPEndPoint(IPAddress.Loopback, 83);
            TcpListener listener = new(ipEndPoint);

            try
            {
                listener.Start();
                using TcpClient handler = await listener.AcceptTcpClientAsync();
                await using NetworkStream stream = handler.GetStream();
                while (true)
                {
                    var buffer = new byte[1_024];
                    int received = await stream.ReadAsync(buffer);

                    string message = Encoding.UTF8.GetString(buffer, 0, received);
                    Console.WriteLine($"Read message: \"{message}\"");
                }
            }
            finally
            {
                listener.Stop();
            }
        }
    }
}
