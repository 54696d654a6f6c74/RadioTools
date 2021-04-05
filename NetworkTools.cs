using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System;

namespace RadioTools
{
    public static class NetworkTools
    {
        public static bool Ping(IPAddress ip)
        {
            Ping ping = new Ping();
            try
            {
                PingReply reply = ping.Send(ip, Settings.dat.timeout);
                return reply.Status == IPStatus.Success ? true : false;
            }
            catch { return false; }
        }

        public static string GetHostName(IPAddress ip)
        {
            try
            {
                IPHostEntry entry = Dns.GetHostEntry(ip);
                Logger.Println("Found hostname for: " + ip.ToString());
                return entry.HostName;
            }
            catch (SocketException){
                Logger.Println("No hostname found for: " + ip.ToString());
            }
            return null;
        }

        public static List<ConnectionDetails> Scan()
        {
            List<ConnectionDetails> alive = new List<ConnectionDetails>();

            Stopwatch timer = Stopwatch.StartNew();

            Threads.StartTreadedJob(Threads.CalcReqThreads(Settings.dat.range), Work);

            timer.Stop();

            Logger.Print("Finished scanning in: ");
            Logger.Println(timer.Elapsed.ToString());

            Logger.Println(String.Format("Found {0} active devices", alive.Count));

            foreach(ConnectionDetails con in alive)
                Logger.Println(con.IP.ToString() + "\t" + con.alive);

            return alive;

            void Work(int start, int end)
            {
                for(int i = start ; i < end; i++)
                {
                    ConnectionDetails conn = new ConnectionDetails(IPAddress.Parse(Settings.dat.subnet+i));
                    if(conn.alive)
                        alive.Add(conn);
                }
            }
        }

        public static void CreateCommand(string script, string name)
        {
            List<ConnectionDetails> targets = Serializer.LoadJSON<List<ConnectionDetails>>("connections");

            UTF8Encoding encoder = new UTF8Encoding();

            Threads.StartTreadedJob(Threads.CalcReqThreads(targets.Count), Work);

            void Work(int start, int end)
            {
                TcpClient client = new TcpClient();

                Request newCMDReq = new Request(encoder, Settings.dat.newCMDRequest, script, name);
                byte[] cmdSize = BitConverter.GetBytes(newCMDReq.dataEncoded[0].Length);
                byte[] contName = Request.ContainerizeName(encoder, name);
                
                for(int i = start; i < end; i++)
                {
                    try{
                        client.Connect(targets[i].IP, Settings.dat.connectionPort);

                        string response = encoder.GetString(SendNewCMDScript(newCMDReq, cmdSize, contName, client));

                        Logger.Println(String.Format("Response from {0}:\n{1}\n", targets[i].IP, response));
                    }
                    catch
                    {
                        Logger.Println("Failed to establish connection with: " + targets[i].IP.ToString());
                    }
                }
            }
        }

        public static void CallCommand(string name)
        {
            List<ConnectionDetails> targets = Serializer.LoadJSON<List<ConnectionDetails>>("connections");

            UTF8Encoding encoder = new UTF8Encoding();

            Threads.StartTreadedJob(Threads.CalcReqThreads(targets.Count), Work);

            void Work(int start, int end)
            {
                TcpClient client = new TcpClient();

                Request callCMDReq = new Request(encoder, Settings.dat.callCMDRequest, name);
                byte[] contName = Request.ContainerizeName(encoder, name);
                
                for(int i = start; i < end; i++)
                {
                    try
                    {
                        client.Connect(targets[i].IP, Settings.dat.connectionPort);
                        Logger.Println("Connected!");
                        Logger.Println(name);

                        string response = encoder.GetString(CallCMD(callCMDReq, contName, client));

                        Logger.Println(string.Format("Response from {0}:\n{1}\n", targets[i].IP, response));
                    }
                    catch
                    {
                        Logger.Println("Failed to establish connection wtih: " + targets[i].IP.ToString());
                    }
                }
            }
        }

        private static byte[] SendNewCMDScript(Request req, byte[] cmdSize, byte[] containName, TcpClient client)
        {
            NetworkStream nStream = client.GetStream();

            nStream.Write(req.reqEncoded, 0, req.reqEncoded.Length);

            nStream.Write(cmdSize, 0, cmdSize.Length);
            nStream.Write(req.dataEncoded[0], 0, req.dataEncoded[0].Length);
            
            nStream.Write(containName, 0, containName.Length);

            byte[] responseEncoded = new byte[Settings.dat.responseSize];
            int numBytes = nStream.Read(responseEncoded, 0, responseEncoded.Length);

            nStream.Close();

            return responseEncoded;
        }

        private static byte[] CallCMD(Request req, byte[] contName, TcpClient client)
        {
            NetworkStream nSteram = client.GetStream();

            nSteram.Write(req.reqEncoded, 0, req.reqEncoded.Length);

            nSteram.Write(contName, 0, contName.Length);

            byte[] responseEncoded = new byte[Settings.dat.responseSize];
            int numBytes = nSteram.Read(responseEncoded, 0, responseEncoded.Length);

            nSteram.Close();

            return responseEncoded;
        }
    }
}
