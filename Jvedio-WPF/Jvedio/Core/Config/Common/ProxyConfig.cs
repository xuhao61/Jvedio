﻿using Jvedio.Core.Config.Base;
using Jvedio.Core.Logs;
using JvedioLib.Security;
using MihaZupan;
using SuperUtils.NetWork;
using System;
using System.Net;

namespace Jvedio.Core.Config
{
    public class ProxyConfig : AbstractConfig
    {
        private const int DEFAULT_TIMEOUT = 10;

        private ProxyConfig() : base("ProxyConfig")
        {
            ProxyMode = 1;
            ProxyType = 1;
            HttpTimeout = DEFAULT_TIMEOUT;
        }

        private static ProxyConfig _instance = null;

        public static ProxyConfig createInstance()
        {
            if (_instance == null) _instance = new ProxyConfig();

            return _instance;
        }

        /// <summary>
        /// 0-无代理 1-系统代理 2-自定义代理
        /// </summary>
        public long ProxyMode { get; set; }

        // 自定义代理配置
        public long ProxyType { get; set; } // 0-HTTP 1-SOCKS

        public string Server { get; set; }

        public long Port { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        private long _HttpTimeout = DEFAULT_TIMEOUT;

        public long HttpTimeout
        {
            get
            {
                return _HttpTimeout;
            }

            set
            {
                if (value <= 0)
                    _HttpTimeout = DEFAULT_TIMEOUT;
                else
                    _HttpTimeout = value;
            }
        }

        public IWebProxy GetWebProxy()
        {
            SuperWebProxy webProxy = new SuperWebProxy();
            if (ProxyMode == 1)
            {
                webProxy.ProxyMode = SuperUtils.NetWork.Enums.ProxyMode.System;
                return webProxy.GetWebProxy();
            }
            else if (ProxyMode == 2)
            {
                webProxy.ProxyMode = SuperUtils.NetWork.Enums.ProxyMode.Custom;
                webProxy.ProxyProtocol = ProxyType == 0 ? SuperUtils.NetWork.Enums.ProxyProtocol.HTTP : SuperUtils.NetWork.Enums.ProxyProtocol.SOCKS;
                webProxy.Server = Server;
                webProxy.Port = (int)Port;
                webProxy.Pwd = GetRealPwd();
                return webProxy.GetWebProxy();
            }

            return null;
        }

        public string GetRealPwd()
        {
            if (!string.IsNullOrEmpty(Password))
            {
                try
                {
                    return Encrypt.AesDecrypt(Password, 0);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }

            return string.Empty;
        }
    }
}
