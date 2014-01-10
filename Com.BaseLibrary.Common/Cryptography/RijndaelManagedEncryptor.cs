using System;
using System.Collections.Generic;

using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Com.BaseLibrary.Common.Cryptography
{
    internal class RijndaelManagedEncryptor
    {
        byte[] Key = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };
        byte[] IV = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };


        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="clearText"></param>
        /// <returns></returns>
        public string Encrypt(string clearText)
        {
            RijndaelManaged myRijndael = null;
            MemoryStream msEncrypt = null;
            CryptoStream csEncrypt = null;
            StreamWriter swEncrypt = null;
            try
            {
                myRijndael = new RijndaelManaged { Key = Key, IV = IV };
                ICryptoTransform encryptor = myRijndael.CreateEncryptor(myRijndael.Key, myRijndael.IV);

                msEncrypt = new MemoryStream();
                csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
                swEncrypt = new StreamWriter(csEncrypt);

                swEncrypt.Write(clearText);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (swEncrypt != null)
                    swEncrypt.Close();
                if (csEncrypt != null)
                    csEncrypt.Close();
                if (msEncrypt != null)
                    msEncrypt.Close();
                // Clear the RijndaelManaged object.
                if (myRijndael != null)
                    myRijndael.Clear();
            }
            byte[] encryptData = msEncrypt.ToArray();
            return Convert.ToBase64String(encryptData);
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        public string Decrypt(string cipherText)
        {
            RijndaelManaged myRijndael = null;
            MemoryStream msDecrypt = null;
            CryptoStream csDecrypt = null;
            StreamReader srDecrypt = null;
            string text = string.Empty;
            try
            {
                myRijndael = new RijndaelManaged { Key = Key, IV = IV };
                ICryptoTransform decryptor = myRijndael.CreateDecryptor(myRijndael.Key, myRijndael.IV);

                byte[] ss = Convert.FromBase64String(cipherText);
                msDecrypt = new MemoryStream(ss);
                csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                srDecrypt = new StreamReader(csDecrypt);
                text = srDecrypt.ReadToEnd();
            }
            catch
            {
                throw;
            }
            finally
            {
                // Clean things up.
                // Close the streams.
                if (srDecrypt != null)
                    srDecrypt.Close();
                if (csDecrypt != null)
                    csDecrypt.Close();
                if (msDecrypt != null)
                    msDecrypt.Close();
                // Clear the RijndaelManaged object.
                if (myRijndael != null)
                    myRijndael.Clear();
            }
            return text;
        }
    }
}
