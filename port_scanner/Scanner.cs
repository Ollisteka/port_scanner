using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


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

            }))).Wait();
        }

        public async Task CheckPort(IPAddress ip, int port)
        {
            var tcpPortStatus = await CheckTcpPortAsync(ip, port);
            if (tcpPortStatus != PortStatus.Open)
            {
                var udpPortStatus = await ChechUdpPort(ip, port);
                if (udpPortStatus == PortStatus.Open)
                    Console.WriteLine(Format, ip, port, udpPortStatus, "UDP");
                else Console.WriteLine(Format, ip, port, tcpPortStatus, "*");
            }
            else
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

        public async Task<PortStatus> ChechUdpPort(IPAddress ip, int port)
        {
            var data = new byte[8];
            var ipEndPoint = new IPEndPoint(ip, port);

            using (var udpClient = new UdpClient())
            {
                try
                {
                    udpClient.Connect(ipEndPoint);
                    udpClient.Client.ReceiveTimeout = 3000;
                    udpClient.Send(data, data.Length);
                   
                    var receiveTask = udpClient.ReceiveAsync();
                    if (await Task.WhenAny(receiveTask, Task.Delay(3000)) == receiveTask)
                    {
                        Console.WriteLine(Encoding.ASCII.GetString(receiveTask.Result.Buffer));
                        return PortStatus.Closed;
                    }
                    return PortStatus.Open;
                }
                catch (Exception e)
                {
                    return PortStatus.Unknown;
                }
            }
        }

        public async Task<PortStatus> CheckTcpPortAsync(IPAddress ip, int port)
        {
            using (var tcpClient = new TcpClient())
            {
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