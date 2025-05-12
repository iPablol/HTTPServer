using System.Net.Sockets;
using System.Net;
using System.Text;


string[] quitCommands =
{
	"quit", "q", "bye", "logout", "stop", "shutdown"
};

int port = 80;
do
{
	Console.WriteLine("Select a port to start the server on");
}
while (!int.TryParse(Console.ReadLine(), out port));

var server = new HTTPServer(IPAddress.Loopback, port);
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
