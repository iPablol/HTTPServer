using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ServerEndPoint;


internal class ServerEndPoint(string path, Method method, RequestHandler handler)
{
	private string path = path;
	private Method method = method;

	public RequestHandler HandleRequest  { get; private set; } = handler;
	public delegate string RequestHandler(string[] headers, string body);



	public static implicit operator string(ServerEndPoint instance) => instance.path;
	public static implicit operator Method(ServerEndPoint instance) => instance.method;
}

