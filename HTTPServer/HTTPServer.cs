
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


internal class HTTPServer
{
	private IPEndPoint endPoint;

	public string key = "";

	private TcpListener listener;

	public bool stop;

	private string[] supportedVersions = ["HTTP/1.1"];

	private readonly List<ServerEndPoint> endpoints;

	private Resources resources = new();
	

	public HTTPServer(IPAddress address, int port)
	{
		endPoint = new(address, port);
		listener = new TcpListener(endPoint);
		endpoints = 
		[
			new ServerEndPoint("/ping", Method.GET, (headers, body) => 
			(HTTPResponse.WithCode(200), [HTTPHeader.ContentType(ContentHeader.text.plain)], "pong"), 
			this, false),

			new ServerEndPoint("/secret", Method.GET, (headers, body) => 
			(HTTPResponse.WithCode(200), [HTTPHeader.ContentType(ContentHeader.text.plain)], "You found the key!"), 
			this),

			new ServerEndPoint("/get", Method.GET, (headers, body) =>
			(HTTPResponse.WithCode(200), [HTTPHeader.ContentType(ContentHeader.application.json)], JsonSerializer.Serialize(resources.Get())),
			this),

			new ServerEndPoint("/add", Method.POST, (headers, body) =>
			{
				Resource? resource = JsonSerializer.Deserialize<Resource>(body);
				if (resource is Resource r)
				{
					resources.Add(r);
					throw HTTPResponse.WithCode(201);
				}
				else
				{
					throw HTTPResponse.WithCode(400);
				}
			}, this),

			new ServerEndPoint("/remove", Method.DELETE, (headers, body) =>
			{
				// Only by position for now
				int pos = int.Parse(body);
				if (pos < 0)
				{
					throw HTTPResponse.WithCode(400);
				}
				if (pos > resources.Get().Count)
				{
					throw HTTPResponse.WithCode(404);
				}
				resources.Remove(pos);
				throw HTTPResponse.WithCode(200);

			}, this)
		];
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
			$"-------------------------------\n" +
			$"{message}\n" +
			$"-------------------------------\n");

		try
		{
			var request = ParseRequest(message);
			if (!supportedVersions.Contains(request.version)) throw HTTPResponse.WithCode(505);
			ServerEndPoint? endpoint = null;

			// This approach does not support polymorphism of endpoints
			endpoint = (from x in endpoints where x == request.target select x).FirstOrDefault() ?? throw HTTPResponse.WithCode(404);

			if (endpoint != null)
			{
				Method method = Enum.Parse<Method>(request.method);
				if (endpoint != method) throw HTTPResponse.WithCode(405);
				try
				{
					(HTTPResponse response, string[] headers, string body) = endpoint.HandleRequest(request.headers, request.body);
					await Respond(stream, response, headers, body);
				}
				catch (HTTPResponse)
				{
					// Don't override responses with 500
					throw;
				}
				catch
				{
					throw HTTPResponse.WithCode(500);
				}
			}
			else
			{
				throw HTTPResponse.WithCode(404);
			}

		}
		catch (HTTPResponse ex)
		{
			await Respond(stream, ex, []);
			return;
		}
	}

	private (string method, string target, string version, HTTPHeader[] headers, string body) ParseRequest(string request)
	{
		Regex regex = new ("^(\\w+)\\s+([^\\s]+)\\s+(HTTP/1.1+)\\r\\n((?:[^\\r\\n]+\\r\\n)*?)\\r\\n(.*)");
		if (regex.IsMatch(request))
		{
			var matches = regex.Match(request).Groups.Values.Select(x => x.Value).ToArray();
			return (matches[1], matches[2], matches[3], matches[4].Split("\r\n", StringSplitOptions.RemoveEmptyEntries).Select(x => new HTTPHeader(x)).ToArray(), matches[5]);
		}
		throw HTTPResponse.WithCode(400);
	}

	private ValueTask Respond(NetworkStream stream, HTTPResponse response, string[] headers, string body = "")
	{
		// Currently only supports 1.1
		string version = supportedVersions[0];
		headers = headers.Append(HTTPHeader.ServerInfoHeader()).ToArray();
		return stream.WriteAsync(Encoding.UTF8.GetBytes($"{version} {response.code} {response.message}\r\n{String.Join("\r\n", headers)}\r\n\r\n{body}"));
	}

}

internal enum Method
{
	GET, POST, PUT, HEAD, DELETE
}

