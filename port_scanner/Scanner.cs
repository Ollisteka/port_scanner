using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace port_scanner
{
	public class Scanner
	{
		public void Scan(List<IPAddress> ips, List<int> ports)
		{
			var names = new[] {"IP", "Port", "Result"};
			const string format = "{0,-13} {1,-10} {2, -13}\n";
			Console.WriteLine(format, names);
			foreach (var ip in ips)
			{
				if (Ping(ip) != IPStatus.Success)
					continue;
				foreach (var port in ports)
				{
					var portStatus = CheckPort(ip, port);
					Console.WriteLine(format, ip, port, portStatus);
				}
			}
		}

		public IPStatus Ping(IPAddress ip)
		{
			using (var ping = new Ping())
			{
				var status = ping.Send(ip, 3000).Status;
				return status;
			}
		}

		public PortStatus CheckPort(IPAddress ip, int port)
		{
			using (var tcpClient = new TcpClient())
			{
				var connectTask = tcpClient.ConnectAsync(ip, port);
				Task.WaitAny(connectTask, Task.Delay(3000));

				switch (connectTask.Status)
				{
					case TaskStatus.RanToCompletion:
						return PortStatus.Open;
					case TaskStatus.Faulted:
						return PortStatus.Closed;
					default:
						return PortStatus.Unknown;
				}
			}
		}
	}
}