using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;


using Com.BaseLibrary.Logging;
using Com.BaseLibrary.Collection;

namespace Com.BaseLibrary.Utility
{

    /// <summary>
    /// 自定义文件侦听类
    /// </summary>
	public class FileChangedEventHandler
	{	
		private int m_Timeout;
		private Dictionary<string, Timer> m_Timers;
		private static readonly Object SyncObject = new object();


		private FileChangedEventHandler()
		{			
			m_Timers = new Dictionary<string, Timer>(new StringEqualityComparer());
		}

		public FileChangedEventHandler(int timeout)
			: this()
		{
			m_Timeout = timeout;
		}

		public event FileSystemEventHandler ActualHandler;
		public void ChangeEventHandler(Object sender, FileSystemEventArgs e)
		{
			lock (SyncObject)
			{
				Timer t;
				if (m_Timers.ContainsKey(e.FullPath))
				{
					t = m_Timers[e.FullPath];
					t.Change(Timeout.Infinite, Timeout.Infinite);
					t.Dispose();
				}
				if (ActualHandler != null)
				{
					t = new Timer(TimerCallback, new FileChangeEventArg(sender, e), m_Timeout, Timeout.Infinite);
					m_Timers[e.FullPath] = t;
				}
			}
		}

		private void TimerCallback(Object state)
		{
			FileChangeEventArg arg = state as FileChangeEventArg;
			ActualHandler(arg.Sender, arg.Argument);
		}


		private class FileChangeEventArg
		{
			public Object Sender { get; private set; }
			public FileSystemEventArgs Argument { get; private set; }

			public FileChangeEventArg(Object sender, FileSystemEventArgs arg)
			{
				this.Sender = sender;
				this.Argument = arg;
			}


		}


	}
}