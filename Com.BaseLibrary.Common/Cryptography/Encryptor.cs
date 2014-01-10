using System;
using System.Text;
using System.Security.Cryptography;


namespace Com.BaseLibrary.Common.Cryptography
{
	/// <summary>
	/// 对称加密
	/// </summary>
	public static class Encryptor
	{
		/// <summary>
		/// 加密
		/// </summary>
		/// <param name="clearText">要加密的文本</param>
		/// <returns></returns>
		public static string Encrypt(string clearText)
		{
			return Encrypt(clearText, null);
		}
		/// <summary>
		/// 解密
		/// </summary>
		/// <param name="cipherText">要解密的文本</param>
		/// <returns></returns>
		public static string Decrypt(string cipherText)
		{
			return Decrypt(cipherText, null);
		}

		/// <summary>
		/// 加密
		/// </summary>
		/// <param name="clearText">要加密的文本</param>
		/// <param name="password">密钥</param>
		/// <returns></returns>
		public static string Encrypt(string clearText, string password)
		{
			if (string.IsNullOrEmpty(password))
			{
				clearText = clearText == null ? string.Empty : clearText;
				RijndaelManagedEncryptor ss = new RijndaelManagedEncryptor();
				return ss.Encrypt(clearText);
			}
			else
			{
				clearText = clearText == null ? string.Empty : clearText;
				string text = clearText + password;

				RijndaelManagedEncryptor ss = new RijndaelManagedEncryptor();
				return ss.Encrypt(text);
			}


		}

		/// <summary>
		/// 解密
		/// </summary>
		/// <param name="cipherText">要解密的文本</param>
		/// <param name="password">密钥</param>
		/// <returns></returns>
		public static string Decrypt(string cipherText, string password)
		{
			if (string.IsNullOrEmpty(password))
			{
				RijndaelManagedEncryptor ss = new RijndaelManagedEncryptor();
				return ss.Decrypt(cipherText);
			}
			else
			{
				RijndaelManagedEncryptor ss = new RijndaelManagedEncryptor();
				string plainText = ss.Decrypt(cipherText);
				if (!plainText.EndsWith(password))
				{
					throw new Exception("密码错误");
				}
				return plainText.Substring(0, plainText.LastIndexOf(password));
			}

		}



		/// <summary>
		/// 签名字符串
		/// </summary>
		/// <param name="prestr">需要签名的字符串</param>
		/// <param name="sign_type">签名类型</param>
		/// <param name="_input_charset">编码格式</param>
		/// <returns>签名结果</returns>
		public static string MD5Sign(string cleanText, string _input_charset)
		{
			StringBuilder sb = new StringBuilder(32);
			MD5 md5 = new MD5CryptoServiceProvider();
			byte[] t = md5.ComputeHash(Encoding.GetEncoding(_input_charset).GetBytes(cleanText));
			for (int i = 0; i < t.Length; i++)
			{
				sb.Append(t[i].ToString("x").PadLeft(2, '0'));
			}
			return sb.ToString();
		}

		public static string MD5Sign(string cleanText)
		{
			return MD5Sign(cleanText, "utf-8");
		}
	}

}
