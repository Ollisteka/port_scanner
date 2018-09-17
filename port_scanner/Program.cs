using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace port_scanner
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var site = "cs.usu.edu.ru";
			var siteIp = Dns.GetHostAddresses(site);
			var ips = new List<IPAddress> {siteIp[0]};
			var left = GetPortAsInput("Left edge:");
			var right = GetPortAsInput("Right edge:");
			Console.WriteLine();
			var ports = Enumerable.Range(left, right - left + 1).ToList();
			var scanner = new Scanner();
			scanner.Scan(ips, ports);
		}

		public static int GetPortAsInput(string message)
		{
			Console.WriteLine(message);
			return int.Parse(Console.ReadLine());
		}
	}
}