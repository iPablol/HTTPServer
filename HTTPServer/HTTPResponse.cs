using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


internal class HTTPResponse(int code, string message) : Exception(message)
{
	public int code { get; private set; } = code;
	public string message { get; private set; } = message;

	public static implicit operator string(HTTPResponse response) => $"{response.code} {response.message}";

	public static HTTPResponse WithCode(int code) => All[code];

	// This was not typed by hand
	public static readonly Dictionary<int, HTTPResponse> All = new()
	{
		[100] = new(100, "Continue"),
		[101] = new(101, "Switching Protocols"),
		[102] = new(102, "Processing"),
		[103] = new(103, "Early Hints"),

		[200] = new(200, "OK"),
		[201] = new(201, "Created"),
		[202] = new(202, "Accepted"),
		[203] = new(203, "Non-Authoritative Information"),
		[204] = new(204, "No Content"),
		[205] = new(205, "Reset Content"),
		[206] = new(206, "Partial Content"),
		[207] = new(207, "Multi-Status"),
		[208] = new(208, "Already Reported"),
		[226] = new(226, "IM Used"),

		[300] = new(300, "Multiple Choices"),
		[301] = new(301, "Moved Permanently"),
		[302] = new(302, "Found"),
		[303] = new(303, "See Other"),
		[304] = new(304, "Not Modified"),
		[305] = new(305, "Use Proxy"),
		[307] = new(307, "Temporary Redirect"),
		[308] = new(308, "Permanent Redirect"),

		[400] = new(400, "Bad Request"),
		[401] = new(401, "Unauthorized"),
		[402] = new(402, "Payment Required"),
		[403] = new(403, "Forbidden"),
		[404] = new(404, "Not Found"),
		[405] = new(405, "Method Not Allowed"),
		[406] = new(406, "Not Acceptable"),
		[407] = new(407, "Proxy Authentication Required"),
		[408] = new(408, "Request Timeout"),
		[409] = new(409, "Conflict"),
		[410] = new(410, "Gone"),
		[411] = new(411, "Length Required"),
		[412] = new(412, "Precondition Failed"),
		[413] = new(413, "Payload Too Large"),
		[414] = new(414, "URI Too Long"),
		[415] = new(415, "Unsupported Media Type"),
		[416] = new(416, "Range Not Satisfiable"),
		[417] = new(417, "Expectation Failed"),
		[418] = new(418, "I'm a teapot"),
		[421] = new(421, "Misdirected Request"),
		[422] = new(422, "Unprocessable Entity"),
		[423] = new(423, "Locked"),
		[424] = new(424, "Failed Dependency"),
		[425] = new(425, "Too Early"),
		[426] = new(426, "Upgrade Required"),
		[428] = new(428, "Precondition Required"),
		[429] = new(429, "Too Many Requests"),
		[431] = new(431, "Request Header Fields Too Large"),
		[451] = new(451, "Unavailable For Legal Reasons"),

		[500] = new(500, "Internal Server Error"),
		[501] = new(501, "Not Implemented"),
		[502] = new(502, "Bad Gateway"),
		[503] = new(503, "Service Unavailable"),
		[504] = new(504, "Gateway Timeout"),
		[505] = new(505, "HTTP Version Not Supported"),
		[506] = new(506, "Variant Also Negotiates"),
		[507] = new(507, "Insufficient Storage"),
		[508] = new(508, "Loop Detected"),
		[510] = new(510, "Not Extended"),
		[511] = new(511, "Network Authentication Required"),
	};
}

