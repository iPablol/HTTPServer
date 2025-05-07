using System.Net.Sockets;
using System.Net;
using System.Text;


string[] quitCommands =
{
	"quit", "q", "bye", "logout", "stop", "shutdown"
};
var server = new HTTPServer(IPAddress.Loopback, 80);
new Task(server.Run).Start();
string command = "";
while (!quitCommands.Contains(command.ToLower()))
{
	command = Console.ReadLine();
	if (command.StartsWith("key "))
	{
		server.key = command.Split(' ')[1];
	}
}

server.stop = true;
