using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


internal class HTTPServer
{
	private IPEndPoint endPoint;

	private TcpListener listener;

	public bool stop;

	public HTTPServer(IPAddress address, int port)
	{
		endPoint = new(address, port);
		listener = new TcpListener(endPoint);
	}

	public async void Run()
	{

		var ipEndPoint = new IPEndPoint(IPAddress.Loopback, 80);
		TcpListener listener = new(ipEndPoint);

		try
		{
			listener.Start();
			Console.WriteLine($"Listening on {endPoint.Address}:{endPoint.Port}");
			while (!stop)
			{
				if (listener.Pending())
				{
					HandleConnection(listener);
				}
			}
		}
		finally
		{
			Console.WriteLine("Server closing...");
			listener.Stop();
		}
	}

	private async void HandleConnection(TcpListener listener)
	{
		using TcpClient handler = await listener.AcceptTcpClientAsync();
		await using NetworkStream stream = handler.GetStream();
		var buffer = new byte[1_024];
		int received = await stream.ReadAsync(buffer);

		string message = Encoding.UTF8.GetString(buffer, 0, received);
		Console.WriteLine($"Read message:\n" +
			$"-------------------------------" +
			$" {message}" +
			$"-------------------------------");

		await stream.WriteAsync(Encoding.UTF8.GetBytes($"Response to \"{message}\""));
	}

}

