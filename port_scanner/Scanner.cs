using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Lying_NTP;

namespace port_scanner
{
	public class Scanner
	{
        readonly string[] names = new[] { "IP", "Port", "Result", "Type" };
        const string Format = "{0,-13} {1,-8} {2, -10} {3, -8}\n";
        public void Scan(List<IPAddress> ips, List<int> ports)
		{
			Console.WriteLine(Format, names);
            Task.WhenAll(ips.Select(ip => Task.Run(async () => {
                if (await PingAsync(ip) != IPStatus.Success)
                    return ;
                await Task.WhenAll(ports.Select(port => CheckPort(ip, port)));

                //foreach (var port in ports)
                //    await CheckPort(ip, port);
                
            }))).Wait();
        }

        public async Task CheckPort(IPAddress ip, int port)
        {
            var tcpPortStatus = await CheckTcpPortAsync(ip, port);
            //if (tcpPortStatus != PortStatus.Open)
            //{
            //    var udpPortStatus = await ChechUdpPortAsync(ip, port);
            //    Console.WriteLine(Format, ip, port, udpPortStatus, "UDP");
            //}
            //else
                Console.WriteLine(Format, ip, port, tcpPortStatus, "TCP");
        }

        public async  Task<IPStatus> PingAsync(IPAddress ip)
		{
            using (var ping = new Ping())
            {
                var status = await ping.SendPingAsync(ip, 3000);
                return status.Status;
            }
        }

        public  PortStatus ChechUdpPort(IPAddress ip, int port)
        {
            var data = new byte[8];
            var ipEndPoint = new IPEndPoint(ip, port);

            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                socket.Connect(ipEndPoint);
                socket.ReceiveTimeout = 6000;
                socket.SendTimeout = 2000;
                
                try
                {
                    socket.Send(data);
                    socket.Receive(data);
                }
                catch (Exception e)
                {
                    return PortStatus.Unknown;
                }

                var a = 0;

                //switch (recieveTask.Status)
                //{
                //    case TaskStatus.RanToCompletion:
                //        return PortStatus.Open;
                //    case TaskStatus.Faulted:
                //        return PortStatus.Closed;
                //    default:
                //        return PortStatus.Unknown;
                //}
                //socket.ReceiveTimeout = 3000;

                //socket.Send(data);
                //socket.Receive(data);
                //socket.Close();
            }

            Console.WriteLine(Encoding.UTF8.GetString(data));
            return PortStatus.Unknown;
        }

        private byte[] GenerateNtpQuery()
        {
            return NtpFrame.GenerateQuery();

        }
		public async Task<PortStatus> CheckTcpPortAsync(IPAddress ip, int port)
		{
			using (var tcpClient = new TcpClient())
			{
                //tcpClient.Connect(ip, port);
                var connectTask = tcpClient.ConnectAsync(ip, port);
                await Task.WhenAny(connectTask, Task.Delay(3000));

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