using System;
using System.Security.Principal;
using System.Runtime.InteropServices;


namespace Com.BaseLibrary.Web
{
	/// <summary>
	/// Specifies what application the web browser is.
	/// </summary>
	public class WebImpersonation : IDisposable
	{
		public const int LOGON32_LOGON_INTERACTIVE = 9;
		public const int LOGON32_PROVIDER_DEFAULT = 0;
		WindowsImpersonationContext ImpersonationContext;

		[DllImport("advapi32.dll", SetLastError = true)]
		public static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword,
			int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		public extern static bool CloseHandle(IntPtr handle);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public extern static bool DuplicateToken(IntPtr ExistingTokenHandle,
			int SECURITY_IMPERSONATION_LEVEL, ref IntPtr DuplicateTokenHandle);


		private bool impersonateValidUser(String domain, String userName, String password)
		{
			string pDomainName = domain;
			string pUser = userName;
			string pPassword = password;

			IntPtr tokenHandle = new IntPtr(0);
			IntPtr dupeTokenHandle = new IntPtr(0);

			const int LOGON32_PROVIDER_DEFAULT = 0;
			//This parameter causes LogonUser to create a primary token.
			const int LOGON32_LOGON_INTERACTIVE = 9;
			const int SecurityImpersonation = 2;

			tokenHandle = IntPtr.Zero;

			dupeTokenHandle = IntPtr.Zero;

			try
			{
				// Call LogonUser to obtain a handle to an access token.
				bool returnValue = LogonUser(pUser, pDomainName, pPassword,
					LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT,
					ref tokenHandle);


				if (false == returnValue)
				{
					int ret = Marshal.GetLastWin32Error();
					//					string vTempError = GetErrorMessage(ret);
					return false;
				}

				bool retVal = DuplicateToken(tokenHandle, SecurityImpersonation, ref dupeTokenHandle);
				if (false == retVal)
				{
					CloseHandle(tokenHandle);
					return false;
				}

				// The token that is passed to the following constructor must 
				// be a primary token in order to use it for impersonation.
				WindowsIdentity newId = new WindowsIdentity(dupeTokenHandle);
				ImpersonationContext = newId.Impersonate();


			}
			catch
			{
				return false;
			}
			return true;
		}



		public WebImpersonation(string userName, string password, string domain)
		{
			if (impersonateValidUser(domain, userName, password))
			{

			}
			else
			{
				throw new Exception("”√ªß√˚/√‹¬Î¥ÌŒÛ");
			}

		}

		public void Undo()
		{
			if (ImpersonationContext != null)
			{
				ImpersonationContext.Undo();
				ImpersonationContext.Dispose();
				ImpersonationContext = null;
			}
		}

		public void Dispose()
		{
			Undo();
		}


	}
}