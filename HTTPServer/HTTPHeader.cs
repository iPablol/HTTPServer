using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


internal record struct HTTPHeader
{
	public HTTPHeader(string header)
	{
		string[] kvp = header.Split(':');
		// Keys are case insensitive: https://developer.mozilla.org/en-US/docs/Web/HTTP/Guides/Messages#http2_messages
		key = kvp[0].ToLower(); value = kvp[1].Substring(1);
	}

	public string key;
	public string value;
}

