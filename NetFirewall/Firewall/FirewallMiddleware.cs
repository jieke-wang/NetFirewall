# define test

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Http;

namespace NetFirewall.Firewall
{
    public class FirewallMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly FirewallRuleManager firewallRuleManager;

        public FirewallMiddleware(RequestDelegate next, FirewallRuleManager firewallRuleManager)
        {
            this._next = next;
            this.firewallRuleManager = firewallRuleManager;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            IPAddress remoteIpAddress = GetIp(context);
            if(remoteIpAddress != null && firewallRuleManager.IsDenyIP(remoteIpAddress))
            {
#if test
                Console.WriteLine($"IP: {remoteIpAddress}");
                Console.WriteLine($"Uri: {GetAbsoluteUri(context.Request)}");
                Console.WriteLine($"Headers: {JsonSerializer.Serialize(context.Request.Headers)}");
#endif
                //context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                //await context.Response.StartAsync();

                IConnectionLifetimeFeature connectionLifetimeFeature = context.Features.Get<IConnectionLifetimeFeature>();
                if(connectionLifetimeFeature != null)
                {
                    connectionLifetimeFeature.Abort();
                }
                else
                {
                    context.Abort();
                }
                return;
            }

            await this._next(context);
        }

        private IPAddress GetIp(HttpContext context)
        {
            IPAddress remoteIpAddress = context.Connection.RemoteIpAddress;
            if(remoteIpAddress != null)
            {
                // If we got an IPV6 address, then we need to ask the network for the IPV4 address 
                // This usually only happens when the browser is on the same machine as the server.
                if (remoteIpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    remoteIpAddress = Dns.GetHostEntry(remoteIpAddress).AddressList
            .First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                }
            }

#if test
            Console.WriteLine(remoteIpAddress);
#endif

            return remoteIpAddress;
        }

        private string GetAbsoluteUri(HttpRequest request)
        {
            return new StringBuilder()
                .Append(request.Scheme)
                .Append("://")
                .Append(request.Host)
                .Append(request.PathBase)
                .Append(request.Path)
                .Append(request.QueryString)
                .ToString();
        }
    }
}
