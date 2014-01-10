using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using Com.BaseLibrary.Utility;

namespace Com.BaseLibrary.Payment.Jh99Bills
{
    public class Jh99BillCore
    {
		/// <summary>
		/// 商户编号
		/// </summary>
        public const string merchant_id = "10012140360";
		/// <summary>
		/// 支付账号
		/// </summary>
        public const string payment_merchant_id = "1001214036001";
        public const string PfxPassword = "1234";
        public const string PrivatePW = "1qaz2wsx";
       
        public const string RefundParamFormat = "merchant_id={0}version=bill_drawback_api_1command_type=001orderid={1}amount={2}postdate={3}txOrder={4}merchant_key={5}";
        public const string encodingName = "gb2312";
        public const string PaymentUrl = "https://sandbox.99bill.com/gateway/recvMerchantInfoAction.htm";
        public const string pfxFileName = "~/CER/99bill-rsa-test.pfx";
        public const string cerFileName = "~/CER/99bill[1].cert.rsa.20140803.cer";

		/// <summary>
		/// 人民币网关密钥
		/// </summary>
		public const string RMBGatewayMD5Key = "MNF9ZA78DMERD3UY";

		/// <summary>
		/// TY3Y75MSWW9GMMMS
		/// </summary>
        public const string AutoRefundMD5Key = "TY3Y75MSWW9GMMMS";
		public const string RefundUrl = "https://www.99bill.com/webapp/receiveDrawbackAction.do";

		/// <summary>
		/// 人民币网关查询接口密钥
		/// </summary>
		public const string RMBGatewayQueryMD5Key = "L3Z9TCF476SSLXTB";
		
        #region 字符串串联函数
        public static StringBuilder AppendParam(StringBuilder returnStr, string paramId, object paramValue)
        {
            if (returnStr.Length > 0)
            {
                if (!StringUtil.IsNullOrEmpty(paramValue))
                {
                    returnStr.AppendFormat("&{0}={1}", paramId, paramValue);
                }
            }
            else
            {
				if (!StringUtil.IsNullOrEmpty(paramValue))
                {
                    returnStr.AppendFormat("{0}={1}", paramId, paramValue);
                }
            }
            return returnStr;
        }
        #endregion
        public static string BuildSignMessage(string msgValue, string password, string pfxPath)
        {
            ///PKI加密
            ///编码方式UTF-8 GB2312  用户可以根据自己系统的编码选择对应的加密方式
            ///byte[] OriginalByte=Encoding.GetEncoding("GB2312").GetBytes(OriginalString);
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(msgValue.ToString());
            X509Certificate2 cert = new X509Certificate2(pfxPath, password, X509KeyStorageFlags.MachineKeySet);
            RSACryptoServiceProvider rsapri = (RSACryptoServiceProvider)cert.PrivateKey;
            RSAPKCS1SignatureFormatter f = new RSAPKCS1SignatureFormatter(rsapri);
            byte[] result;
            f.SetHashAlgorithm("SHA1");
            SHA1CryptoServiceProvider sha = new SHA1CryptoServiceProvider();
            result = sha.ComputeHash(bytes);
            return System.Convert.ToBase64String(f.CreateSignature(result)).ToString();
        }
        //功能函数。将字符串进行编码格式转换，并进行MD5加密，然后返回（1：要签名的参数，2：编码方式）。开始
        public static string GetMD5(string dataStr)
        {
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(System.Text.Encoding.GetEncoding(encodingName).GetBytes(dataStr));
            System.Text.StringBuilder sb = new System.Text.StringBuilder(32);
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString();
        }
        //功能函数。将字符串进行编码格式转换，并进行MD5加密，然后返回。结束
    }
}
