using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


internal record struct HTTPHeader
{
	// This is for helping parse responses
	public HTTPHeader(string header)
	{
		string[] kvp = header.Split(':');
		// Keys are case insensitive: https://developer.mozilla.org/en-US/docs/Web/HTTP/Guides/Messages#http2_messages
		key = kvp[0].ToLower(); value = kvp[1].Substring(1);
	}

	public string key;
	public string value;

	public static implicit operator HTTPHeader(string header) => new (header);

	// This is for helping construct headers
	public static string ContentType<T>(T type) where T : Enum
	{
		if (typeof(T).DeclaringType != typeof(ContentHeader))
		{
			throw new ArgumentException("Enum must be from HTTPHeader.ContentHeader");
		}
		return $"Content-Type: {typeof(T).Name}/{typeof(T).GetEnumName(type)}";
	}

	public static string ContentLength(string body) => "Content-Length: " + Encoding.UTF8.GetBytes(body).Length;

	public static string ServerInfoHeader() => $"Server: custom\r\nDate: {DateTime.UtcNow.ToString("r")}";
}

internal static partial class ContentHeader
{
	internal enum text
	{
		plain,
		html,
		css,
		javascript
	}

	internal enum application
	{
		json,
		xml,
		javascript,
		pdf,
		zip
	}

	internal enum image
	{
		jpeg,
		png,
		gif,
		webp
	}

	internal enum audio
	{
		mpeg,
		ogg,
		wav
	}

	internal enum video
	{
		mp4,
		webm
	}

}

