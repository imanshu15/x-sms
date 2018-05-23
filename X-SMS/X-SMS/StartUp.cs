using System;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(X_SMS.StartUp))]
namespace X_SMS
{
    public class StartUp
    {
        public void Configuration(IAppBuilder app) {
            app.MapSignalR();
        }
    }
}