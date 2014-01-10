using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using Com.BaseLibrary.Payment.Jh99Bills;


namespace Com.BaseLibrary.Payment.Jh99Bills
{
	public class JH99BillsModel
	{
		#region 参数说明
		//人民币网关账号，该账号为11位人民币网关商户编号+01,该参数必填。

		public string merchantAcctId = ""; //test
		//public string merchantAcctId = "1002212484801"; //正式

		//编码方式，1代表 UTF-8; 2 代表 GBK; 3代表 GB2312 默认为1,该参数必填。
		public string inputCharset = "1";

		//接收支付结果的页面地址，该参数一般置为空即可。
		public  string pageUrl = "";

		//服务器接收支付结果的后台地址，该参数务必填写，不能为空。
		public  string bgUrl = "";

		//网关版本，固定值：v2.0,该参数必填。
		public string version = "v2.0";
		//语言种类，1代表中文显示，2代表英文显示。默认为1,该参数必填。
		public string language = "1";
		//签名类型,该值为4，代表PKI加密方式,该参数必填。
		public string signType = "4";

		
		////支付人姓名,可以为空。
		public string payerName = "客户";
		////支付人联系类型，1 代表电子邮件方式；2 代表手机联系方式。可以为空。
		public string payerContactType = "1";
		////支付人联系方式，与payerContactType设置对应，payerContactType为1，则填写邮箱地址；payerContactType为2，则填写手机号码。可以为空。
		public string payerContact = "";

		//商户订单号，以下采用时间来定义订单号，商户可以根据自己订单号的定义规则来定义该值，不能为空。
		public string orderId = DateTime.Now.ToString("yyyyMMddHHmmss");
		//订单金额，金额以“分”为单位，商户测试以1分测试即可，切勿以大金额测试。该参数必填。
		public string orderAmount = "1";
		//订单提交时间，格式：yyyyMMddHHmmss，如：20071117020101，不能为空。
		public string orderTime = DateTime.Now.ToString("yyyyMMddHHmmss");

		////商品名称，可以为空。
		//public string productName = "苹果";
		////商品数量，可以为空。
		//public string productNum = "5";
		////商品代码，可以为空。
		//public string productId = "";
		////商品描述，可以为空。
		//public string productDesc = "";
		////扩展字段1，商户可以传递自己需要的参数，支付完快钱会原值返回，可以为空。

		public string ext1 = "";
		//扩展自段2，商户可以传递自己需要的参数，支付完快钱会原值返回，可以为空。
		public string ext2 = "";
		//支付方式，一般为00，代表所有的支付方式。如果是银行直连商户，该值为10，必填。
		public string payType = "00";
		//银行代码，如果payType为00，该值可以为空；如果payType为10，该值必须填写，具体请参考银行列表。
		public string bankId = "";
		//同一订单禁止重复提交标志，实物购物车填1，虚拟产品用0。1代表只能提交一次，0代表在支付不成功情况下可以再提交。可为空。
		public string redoFlag = "";
		//快钱合作伙伴的帐户号，即商户编号，可为空。
		public string pid = "";
		// signMsg 签名字符串 不可空，生成加密签名串
		public string signMsg = "";
		#endregion


		public JH99BillsModel()
		{
			GenerateSign();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="soNO">订单编号</param>
		/// <param name="soAmount">订单金额</param>
		/// <param name="soTime">订单时间</param>
		/// <param name="productId">商品编号，可选</param>
		/// <param name="productName">商品名称，可选</param>
		/// <param name="productNum">订购数量</param>
		/// <param name="productDesc">商品描述</param>
		/// <param name="ext1">扩展字段1，商户可以传递自己需要的参数，支付完快钱会原值返回，可以为空</param>
		/// <param name="ext2"></param>
		/// <returns></returns>
		public JH99BillsModel(
			string soNO,
			decimal soAmount,
			DateTime soTime,
			string payType,
			string bankId
			)
		{
			this.merchantAcctId = JH99BillConfig.Current.PaymentMerchantID;
			pageUrl = JH99BillConfig.Current.PaymentReturnUrl;
			bgUrl = JH99BillConfig.Current.PaymentNotifyUrl;
			this.orderId = soNO;
			this.orderAmount = (soAmount * 100).ToString("0");
			this.orderTime = soTime.ToString("yyyyMMddHHmmss");
			this.payType = payType;
			if (payType == "00")
			{
				this.bankId = string.Empty;
			}
			else
			{
				this.bankId = bankId;
			}
			//拼接字符串
			GenerateSign();
		}

		private void GenerateSign()
		{
			StringBuilder signMsgVal = new StringBuilder();
			signMsgVal = Jh99BillPayment.AppendParam(signMsgVal, "inputCharset", inputCharset);
			signMsgVal = Jh99BillPayment.AppendParam(signMsgVal, "pageUrl", pageUrl);
			signMsgVal = Jh99BillPayment.AppendParam(signMsgVal, "bgUrl", bgUrl);
			signMsgVal = Jh99BillPayment.AppendParam(signMsgVal, "version", version);
			signMsgVal = Jh99BillPayment.AppendParam(signMsgVal, "language", language);
			signMsgVal = Jh99BillPayment.AppendParam(signMsgVal, "signType", signType);
			signMsgVal = Jh99BillPayment.AppendParam(signMsgVal, "merchantAcctId", merchantAcctId);

			signMsgVal = Jh99BillPayment.AppendParam(signMsgVal, "payerName", payerName);
			signMsgVal = Jh99BillPayment.AppendParam(signMsgVal, "payerContactType", payerContactType);
			signMsgVal = Jh99BillPayment.AppendParam(signMsgVal, "payerContact", payerContact);

			signMsgVal = Jh99BillPayment.AppendParam(signMsgVal, "orderId", orderId);
			signMsgVal = Jh99BillPayment.AppendParam(signMsgVal, "orderAmount", orderAmount);
			signMsgVal = Jh99BillPayment.AppendParam(signMsgVal, "orderTime", orderTime);
			//signMsgVal = AppendParam(signMsgVal, "productName", productName);
			//signMsgVal = AppendParam(signMsgVal, "productNum", productNum);
			//signMsgVal = AppendParam(signMsgVal, "productId", productId);
			//signMsgVal = AppendParam(signMsgVal, "productDesc", productDesc);
			signMsgVal = Jh99BillPayment.AppendParam(signMsgVal, "ext1", ext1);
			signMsgVal = Jh99BillPayment.AppendParam(signMsgVal, "ext2", ext2);
			signMsgVal = Jh99BillPayment.AppendParam(signMsgVal, "payType", payType);
			signMsgVal = Jh99BillPayment.AppendParam(signMsgVal, "bankId", bankId);

			signMsgVal = Jh99BillPayment.AppendParam(signMsgVal, "redoFlag", redoFlag);
			signMsgVal = Jh99BillPayment.AppendParam(signMsgVal, "pid", pid);

			signMsg = Jh99BillPayment.BuildSignMessage(
				signMsgVal.ToString(), 
				JH99BillConfig.Current.PfxPassword,
				HttpContext.Current.Server.MapPath(JH99BillConfig.Current.PfxFileName));


		}
		

	}
}
