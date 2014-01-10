using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using Com.BaseLibrary.Utility;

namespace Com.BaseLibrary.Payment.Jh99Bills
{
    public class Jh99BillPayment
    {
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
            byte[] t = md5.ComputeHash(System.Text.Encoding.GetEncoding(JH99BillConfig.Current.Encoding).GetBytes(dataStr));
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
