using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Shared.Helper
{
    public class PRC
    {
        public int PID { get; set; }
        public int Port { get; set; }
        public string Protocol { get; set; }
    }
    public class ProcManager
    {
        public void KillByPort(int port)
        {
            List<PRC> processes = GetAllProcesses();
            if (processes.Any(p => p.Port == port))
                try
                {
                    Process.GetProcessById(processes.First(p => p.Port == port).PID).Kill();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            else
            {
                Console.WriteLine("No process to kill!");
            }
        }

        public List<PRC> GetAllProcesses()
        {
            ProcessStartInfo pStartInfo = new ProcessStartInfo();
            pStartInfo.FileName = "netstat.exe";
            pStartInfo.Arguments = "-a -n -o";
            pStartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            pStartInfo.UseShellExecute = false;
            pStartInfo.RedirectStandardInput = true;
            pStartInfo.RedirectStandardOutput = true;
            pStartInfo.RedirectStandardError = true;

            Process process = new Process()
            {
                StartInfo = pStartInfo
            };
            process.Start();

            StreamReader soStream = process.StandardOutput;

            string output = soStream.ReadToEnd();
            if (process.ExitCode != 0)
                throw new Exception("somethign broke");

            List<PRC> result = new List<PRC>();

            string[] lines = Regex.Split(output, "\r\n");
            foreach (string line in lines)
            {
                if (line.Trim().StartsWith("Proto"))
                    continue;

                string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                int len = parts.Length;
                if (len > 2)
                    result.Add(new PRC
                    {
                        Protocol = parts[0],
                        Port = int.Parse(parts[1].Split(':').Last()),
                        PID = int.Parse(parts[len - 1])
                    });


            }
            return result;
        }

        public List<PRC> PortInUse(int port)
        {
            ProcessStartInfo pStartInfo = new ProcessStartInfo();
            pStartInfo.FileName = "netstat.exe";
            pStartInfo.Arguments = $"-a -n -o | findstr {port}";
            pStartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            pStartInfo.UseShellExecute = false;
            pStartInfo.RedirectStandardInput = true;
            pStartInfo.RedirectStandardOutput = true;
            pStartInfo.RedirectStandardError = true;

            Process process = new Process()
            {
                StartInfo = pStartInfo
            };
            process.Start();

            StreamReader soStream = process.StandardOutput;

            string output = soStream.ReadToEnd();
            if (process.ExitCode != 0)
                throw new Exception("somethign broke");

            List<PRC> result = new List<PRC>();

            string[] lines = Regex.Split(output, "\r\n");
            foreach (string line in lines)
            {
                if (line.Trim().StartsWith("Proto"))
                    continue;

                string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                int len = parts.Length;
                if (len > 2)
                    result.Add(new PRC
                    {
                        Protocol = parts[0],
                        Port = int.Parse(parts[1].Split(':').Last()),
                        PID = int.Parse(parts[len - 1])
                    });


            }
            return result;
        }
    }
}
