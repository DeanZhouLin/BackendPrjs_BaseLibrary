using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Security.Cryptography;
using Com.BaseLibrary.Contract;
using Com.BaseLibrary.Utility;
using System.Net.Mail;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;

namespace Com.BaseLibrary.Contract
{
	[ServiceContract]
	public interface ICommonService : IServiceBase
	{
		[OperationContract(IsOneWay = true)]
		void SendEmail(
			string sender,
			string receivers,
			string ccList,
			string bccList,
			string subject,
			List<string> attachments,
			string body,
			string userName,
			string password,
			string smtpHost,
			int? port,
			bool enableSSL,
			int? timeOut,
			bool asyncSend);

		[OperationContract(IsOneWay = true)]
		void SendEmail2(
			string sender,
			string displayName,
			string receivers,
			string ccList,
			string bccList,
			string subject,
			List<string> attachments,
			string body,
			string userName,
			string password,
			string smtpHost,
			int? port,
			bool enableSSL,
			int? timeOut,
			bool asyncSend);

		[OperationContract(IsOneWay = true)]
		void TrackPageView(
			string currentPage,
			string refererPage,
			string sessionId,
			string userId,
			string account,
			string visitorId,
			string userAgent,
			string browser,
			string remoteIP,
			string domainName,
			string language,
			string httpMethod);


		[OperationContract(IsOneWay = true)]
		[WebInvoke(RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]

		void RecordPageView(
			string sid,
			string cid,
			string customerName,
			string visitedUrl,
			DateTime visitedTime,
			string urlReferrer,
			string userAgent,
			string browser,
			string browserVersion,
			string os,
			string userIP,
			string promotionNO,
			string unionTrackID,
			string httpMothed
			);
	}
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public class CommonService : ICommonService
	{
		private static ICommonService m_Instance;
		public static ICommonService Instance
		{
			get
			{
				if (m_Instance == null)
				{
					string url = EmailConfiguration.Current.ServiceUrl;
					if (StringUtil.IsNullOrEmpty(url))
					{
						return new CommonService();
					}
					else
					{
						m_Instance = ServiceFactory.CreateServiceClient<ICommonService>(url);
					}

				}
				return m_Instance;
			}
		}

		#region ICommonService 成员

		public void SendEmail(
			string sender,
			string receivers,
			string ccList,
			string bccList,
			string subject,
			List<string> attachments,
			string body,
			string userName,
			string password,
			string smtpHost,
			int? port,
			bool enableSSL,
			int? timeOut,
			bool asyncSend)
		{
			EmailUtil.DoSendEmail(
				sender,
				receivers,
				ccList,
				bccList,
				subject,
				attachments,
				body,
				userName,
				password,
				smtpHost,
				port,
				enableSSL,
				timeOut,
				asyncSend);
		}

		public void TrackPageView(
			string currentPage,
			string refererPage,
			string sessionId,
			string userId,
			string account,
			string visitorId,
			string userAgent,
			string browser,
			string remoteIP,
			string domainName,
			string language,
			string httpMethod)
		{
			// Construct the gif hit url.
			PageViewUtil.DoTrackPageView(
				currentPage,
				refererPage,
				sessionId,
				userId,
				account,
				visitorId,
				userAgent,
				browser,
				remoteIP,
				domainName,
				language,
				httpMethod);
		}
		[Obsolete("不再使用")]
		public void RecordPageView(
			string sid,
			string cid,
			string customerName,
			string visitedUrl,
			DateTime visitedTime,
			string urlReferrer,
			string userAgent,
			string browser,
			string browserVersion,
			string os,
			string userIP,
			string promotionNO,
			string unionTrackID,
			string httpMothed
			)
		{
			// Construct the gif hit url.
			PageViewUtil.RecordPageView(
				sid,
				cid,
				customerName,
				visitedUrl,
				visitedTime,
				urlReferrer,
				userAgent,
				browser,
				browserVersion,
				os,
				userIP,
				promotionNO,
				unionTrackID,
				httpMothed);
		}

		#endregion


		public void SendEmail2(string sender, string displayName, string receivers, string ccList, string bccList, string subject, List<string> attachments, string body, string userName, string password, string smtpHost, int? port, bool enableSSL, int? timeOut, bool asyncSend)
		{
			EmailUtil.DoSendEmail(
				 sender,
				 displayName,
				 receivers,
				 ccList,
				 bccList,
				 subject,
				 attachments,
				 body,
				 userName,
				 password,
				 smtpHost,
				 port,
				 enableSSL,
				 timeOut,
				 asyncSend);
		}
	}

}
