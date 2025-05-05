using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


internal class HTTPServer
{
	private IPEndPoint endPoint;

	private TcpListener listener;

	public bool stop;

	private string[] supportedVersions = { "HTTP/1.1" };

	public HTTPServer(IPAddress address, int port)
	{
		endPoint = new(address, port);
		listener = new TcpListener(endPoint);
	}

	public async void Run()
	{
		try
		{
			listener.Start();
			Console.WriteLine($"Listening on {endPoint.Address}:{endPoint.Port}");
			while (!stop)
			{
				if (listener.Pending())
				{
					HandleConnection();
				}
			}
		}
		finally
		{
			Console.WriteLine("Server closing...");
			listener.Stop();
		}
	}

	private async void HandleConnection()
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

		try
		{
			var request = ParseRequest(message);
			if (!supportedVersions.Contains(request.version)) throw HTTPResponse.WithCode(501);

			await Respond(stream, HTTPResponse.WithCode(300), $"Response to {message}");
		}
		catch (HTTPResponse ex)
		{
			await Respond(stream, ex);
			return;
		}
	}

	private (string method, string target, string version, string[] headers, string body) ParseRequest(string request)
	{
		Regex regex = new ("(.*) (.*) (.*)\r\n(.*)\r\n(.*)");
		if (regex.IsMatch(request))
		{
			var matches = regex.Match(request).Groups.Values.Select(x => x.Value).ToArray();
			return (matches[1], matches[2], matches[3], matches[4].Split('\n'), matches[5]);
		}
		throw HTTPResponse.WithCode(400);
	}

	private ValueTask Respond(NetworkStream stream, HTTPResponse response, string body = "")
	{
		// Currently only supports 1.1
		string version = supportedVersions[0];
		string headers = "";
		return stream.WriteAsync(Encoding.UTF8.GetBytes($"{version} {response.code} {response.message}\r\n{headers}\r\n{body}"));
	}

}

