using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ServerEndPoint;


internal class ServerEndPoint(string path, Method method, RequestHandler handler, HTTPServer server, bool requireKey = true)
{
	private string path = path;
	private Method method = method;

	// We don't need to support the HEAD nor OPTIONS method
	public RequestHandler HandleRequest { get; private set; } = (HTTPHeader[] headers, string body = "") =>
	{
		// Key is active
		if (requireKey && server.key != "")
		{
			try
			{
				HTTPHeader keyHeader = headers.First(x => x.key == "key");
				if (keyHeader.value != server.key)
					throw HTTPResponse.WithCode(403);
			}
			catch
			{
				throw HTTPResponse.WithCode(403);
			}
		}

		return handler(headers, body);
	};
	public delegate (HTTPResponse response, string[] headers, string body) RequestHandler(HTTPHeader[] headers, string body = "");

	public static implicit operator string(ServerEndPoint instance) => instance.path;
	public static implicit operator Method(ServerEndPoint instance) => instance.method;
}

