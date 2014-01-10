using System;
using System.Xml;
using System.ServiceModel;
using System.Configuration;
using System.Web.Services.Protocols;
using Com.BaseLibrary.ExceptionHandle;

namespace Com.BaseLibrary.Service
{

	/// <summary>
	/// 异常构建工具
	/// </summary>
	public class BizExceptionBuilder
	{
		/// <summary>
		/// 创建一个异常
		/// </summary>
		/// <param name="name">异常信息的Name</param>
		/// <param name="parameters">异常信息的格式化参数</param>
		/// <returns></returns>
		public static BizException BuildException(object name, params object[] parameters)
		{
			return new BizException(name, parameters);
		}

		/// <summary>
		/// 构建一个要返回客户端的异常
		/// </summary>
		/// <param name="ex"></param>
		/// <returns></returns>
		public static Exception BuildException(Exception ex)
		{

			//如果异常已经是SoapException或FaultException，直接返回
			if (ex is SoapException)
			{
				return ex as SoapException;
			}
			else if (ex is FaultException)
			{
				return ex as FaultException;
			}


			BizException bixEx = null;
			if (ex is BizException)
			{
				bixEx = ex as BizException;
			}
			else
			{
				bixEx = new BizException("UnhandledException", ex);
			}
			return BuildReturnException(bixEx);
		}

		/// <summary>
		/// 根据BizException构建一个要返回客户端的异常
		/// </summary>
		/// <param name="ex"></param>
		/// <returns></returns>
		private static Exception BuildReturnException(BizException ex)
		{
			string HostType = ConfigurationManager.AppSettings["HostType"];
			if (HostType == "WebService")
			{
				return BuildSoapException(ex);
			}
			else if (HostType == "WCF")
			{
				return new FaultException(ex.Message);
			}
			
			return ex;
		}

		/// <summary>
		/// 根据BizException创建一个SoapException
		/// </summary>
		/// <param name="ex"></param>
		/// <returns></returns>
		private static SoapException BuildSoapException(BizException ex)
		{
			System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
			System.Xml.XmlNode node = doc.CreateNode(XmlNodeType.Element, SoapException.DetailElementName.Name, SoapException.DetailElementName.Namespace);
			XmlText data = doc.CreateTextNode(ex.Message);
			node.AppendChild(data);
			return new SoapException("Fault occurred", SoapException.ClientFaultCode, string.Empty, node);

		}

		
	}
}
