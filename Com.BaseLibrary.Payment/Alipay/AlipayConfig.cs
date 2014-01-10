using System.Xml.Serialization;
using Com.BaseLibrary.Configuration;

namespace Com.Alipay
{
	/// <summary>
	/// 类名：Config
	/// 功能：基础配置类
	/// 详细：设置帐户有关信息及返回路径
	/// 版本：3.2
	/// 日期：2011-03-17
	/// 说明：
	/// 以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。
	/// 该代码仅供学习和研究支付宝接口使用，只是提供一个参考。
	/// 
	/// 如何获取安全校验码和合作身份者ID
	/// 1.用您的签约支付宝账号登录支付宝网站(www.alipay.com)
	/// 2.点击“商家服务”(https://b.alipay.com/order/myOrder.htm)
	/// 3.点击“查询合作者身份(PID)”、“查询安全校验码(Key)”
	/// </summary>
	[XmlRoot("alipayConfiguration")]
	public class AlipayConfiguration
	{
		#region 字段
		//private static string partner = "2088101171193303";
		//private static string key = "g2sr1zmpe6iy4mx2sw1pfblooa1k68py";
		//private static string seller_email = "alipay@jufine.com";
		//private static string return_url = "";
		//private static string notify_url = "";
		//private static string input_charset = "utf-8";
		//private static string sign_type = "MD5";
		#endregion

		public static AlipayConfiguration Current
		{
			get
			{
				return ConfigurationManager.LoadConfiguration<AlipayConfiguration>("ALIPAYCONFIGURATION", "Config/AlipayPayment.Config");
			}
		}

		public AlipayConfiguration()
		{
		}

		#region 属性
		/// <summary>
		/// 获取或设置合作者身份ID
		/// </summary>
		[XmlAttribute]
		public string PartnerID{get;set;}
	

		/// <summary>
		/// 获取或设置交易安全检验码
		/// </summary>
		[XmlAttribute]
		public string TradeKey { get; set; }

		/// <summary>
		/// 获取或设置签约支付宝账号或卖家支付宝帐户
		/// </summary>
		[XmlAttribute]
		public string SellerEmail { get; set; }

		/// <summary>
		/// 获取页面跳转同步通知页面路径
		/// </summary>
		[XmlAttribute()]
		public string ReturnUrl { get; set; }
		/// <summary>
		/// 获取服务器异步通知页面路径
		/// </summary>
		[XmlAttribute]
		public string NotifyUrl { get; set; }
		/// <summary>
		/// 获取字符编码格式
		/// </summary>
		[XmlAttribute]
		public string InputCharset { get; set; }

		/// <summary>
		/// 获取签名方式
		/// </summary>
		[XmlAttribute]
		public string SignType { get; set; }

		[XmlAttribute]
		public string RefundNotifyUrl { get; set; }

		[XmlAttribute]
		public bool NeedLog { get; set; }
		#endregion


		
	}
}