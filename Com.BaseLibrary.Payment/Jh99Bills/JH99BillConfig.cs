using System.Xml.Serialization;
using Com.BaseLibrary.Configuration;

namespace Com.BaseLibrary.Payment.Jh99Bills
{
	[XmlRoot("payment")]
	public class JH99BillConfig
	{
		public static JH99BillConfig Current
		{
			get
			{
				return ConfigurationManager.LoadConfiguration<JH99BillConfig>("JH99BILLCONFIG", "Config/99billPayment.Config");
			}
		}

		[XmlAttribute]
		public string Encoding { get; set; }
		[XmlAttribute]
		public string MerchantID { get; set; }

		[XmlAttribute]
		public string PaymentMerchantID { get; set; }
		[XmlAttribute]
		public string PaymentUrl { get; set; }
		[XmlAttribute]
		public string PaymentNotifyUrl { get; set; }
		[XmlAttribute]
		public string PaymentReturnUrl { get; set; }
		[XmlAttribute]
		public string PfxPassword { get; set; }
		[XmlAttribute]
		public string PrivatePW { get; set; }
		[XmlAttribute]
		public string PfxFileName { get; set; }
		[XmlAttribute]
		public string CerFileName { get; set; }

		[XmlAttribute]
		public string RMBGatewayMD5Key { get; set; }

		[XmlAttribute]
		public string AutoRefundMD5Key { get; set; }
		[XmlAttribute]
		public string RefundUrl { get; set; }
		[XmlAttribute]
		public string RefundParamFormat { get; set; }

		[XmlAttribute]
		public string RMBGatewayQueryMD5Key { get; set; }
		[XmlAttribute]
		public bool NeedLog { get; set; }
	}
}
