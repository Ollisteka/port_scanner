﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace port_scanner
{
	internal class Program
	{
		private static void Main(string[] args)
		{
            // IPAddress.Parse("10.97.135.51"),
            var ips = new List<IPAddress> { IPAddress.Parse("80.83.248.25"), IPAddress.Loopback };
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