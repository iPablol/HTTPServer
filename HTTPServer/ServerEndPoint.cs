using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ServerEndPoint;


internal class ServerEndPoint(string path, Method method, RequestHandler handler, HTTPServer server)
{
	private string path = path;
	private Method method = method;

	public RequestHandler HandleRequest  { get; private set; } = (HTTPHeader[] headers, string body) =>
	{
		// Key is activated
		if (server.key != "")
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
	public delegate string RequestHandler(HTTPHeader[] headers, string body);

	public static implicit operator string(ServerEndPoint instance) => instance.path;
	public static implicit operator Method(ServerEndPoint instance) => instance.method;
}

