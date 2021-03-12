using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace NetFirewall
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

// https://zhuanlan.zhihu.com/p/259162269#%E6%8B%A6%E6%88%AA%E6%8E%89%E9%BB%91%E5%90%8D%E5%8D%95%E4%B8%AD%E7%9A%84IP
// https://github.com/ldqk/Masuit.MyBlogs/tree/master/src/Masuit.MyBlogs.Core/Extensions/Firewall
// https://github.com/ldqk/Masuit.MyBlogs/blob/master/src/Masuit.MyBlogs.Core/Extensions/Firewall/FirewallAttribute.cs