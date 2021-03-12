using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;

namespace NetFirewall.Firewall
{
    public class FirewallRuleManager
    {
        private readonly IHostEnvironment environment;
        private readonly string RuleFullfilename;
        private const string Rulefilename = "FirewallRule.rule";
        private readonly SortedSet<int> DenyIPs;

        public FirewallRuleManager(IHostEnvironment environment)
        {
            this.environment = environment;
            this.RuleFullfilename = Path.Combine(environment.ContentRootPath, Rulefilename);
            this.DenyIPs = new SortedSet<int>();

            Init();
        }

        private void Init()
        {
            if (File.Exists(Rulefilename) == false) return;
            using (TextReader tr = new StreamReader(Rulefilename, Encoding.UTF8))
            {
                string strDenyIP;
                while (string.IsNullOrWhiteSpace(strDenyIP = tr.ReadLine()) == false)
                {
                    if (int.TryParse(strDenyIP, out int denyIP))
                        this.DenyIPs.Add(denyIP);
                }
            }
        }

        public void AddDenyIP(string ip)
        {
            if (IPAddress.TryParse(ip, out IPAddress address))
            {
                AddDenyIP(address);
            }
        }

        public void AddDenyIP(IPAddress ip)
        {
            if (IPAddress.IsLoopback(ip)) return;

            int _ip = ConvertIPAddressToInt32(ip);
            if (this.DenyIPs.Add(_ip))
            {
                using (TextWriter tw = new StreamWriter(Rulefilename, true, Encoding.UTF8))
                {
                    tw.WriteLine(_ip);
                }
            }
        }

        public bool IsDenyIP(string ip)
        {
            if (IPAddress.TryParse(ip, out IPAddress address))
            {
                return IsDenyIP(address);
            }
            return false;
        }

        public bool IsDenyIP(IPAddress ip)
        {
            if (IPAddress.IsLoopback(ip)) return false;

            int _ip = ConvertIPAddressToInt32(ip);
            return this.DenyIPs.Contains(_ip);
        }

        private int ConvertIPAddressToInt32(IPAddress ip)
        {
            return BitConverter.ToInt32(ip.GetAddressBytes(), 0);
        }
    }
}
