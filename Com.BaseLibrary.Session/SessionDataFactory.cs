using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Memcached.ClientLibrary;
using Com.BaseLibrary.Configuration;
using Com.BaseLibrary.Utility;

namespace Com.BaseLibrary.Session
{
    public class SessionDataFactory
    {
        public static SessionDataFactory Current = new SessionDataFactory();
        SockIOPool pool = null;
        MemcachedClient mc = null;
        public SessionData GetSessionData(string sessionID)
        {
            object sessionData = mc.Get(sessionID);
            if (sessionData == null)
            {
                sessionData = new SessionData();
            }
            return sessionData as SessionData;
        }
        SessionDataFactory()
        {
            InitSockPool();
        }
        ~SessionDataFactory()
        {
            if (pool != null)
            {
                pool.Shutdown();
            }
        }
        private void InitSockPool()
        {

            //string[] serverlist = { "10.0.0.90:11211" };
            string[] serverlist = ConfigurationHelper.GetAppSetting("MemCacheServerList").Split(',');
            pool = SockIOPool.GetInstance();
            pool.SetServers(serverlist);
            pool.HashingAlgorithm = HashingAlgorithm.NewCompatibleHash;
            pool.InitConnections = 3;
            pool.MinConnections = 3;
            pool.MaxConnections = 1000;
            pool.SocketConnectTimeout = 1000;
            pool.SocketTimeout = 3000;
            pool.MaintenanceSleep = 30;
            pool.Failover = true;
            pool.Nagle = false;
            pool.Initialize();
            mc = new MemcachedClient();
            mc.EnableCompression = false;
        }
        public void UpdateSession(string SessionID, SessionData Session)
        {
            if (Session.HasChanged || Session.Count > 0)
            {
                Session.HasChanged = false;
                if (Session.IsNew)
                {
                    Session.IsNew = false;
                    mc.Set(SessionID, Session, DateTime.Now.AddMinutes(25));
                }
                else
                {
                    if (Session.Count == 0)
                    {
                        mc.Delete(SessionID);
                    }
                    else
                    {
                        mc.Replace(SessionID, Session, DateTime.Now.AddMinutes(25));
                    }
                }
            }
        }
    }
}
