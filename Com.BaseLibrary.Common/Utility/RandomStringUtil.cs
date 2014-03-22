using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Com.BaseLibrary.Utility
{

    public class RandomStringGenerator
    {
        public RandomStringGenerator()
        {
            this.Minimum = DefaultMinimum;
            this.Maximum = DefaultMaximum;
            this.ConsecutiveCharacters = false;
            this.RepeatCharacters = true;
            this.ExcludeSymbols = false;
            this.Exclusions = null;
            rng = new RNGCryptoServiceProvider();
        }

        protected int GetCryptographicRandomNumber(int lBound, int uBound)
        {
            // 假定 lBound >= 0 && lBound < uBound
            // 返回一个 int >= lBound and < uBound
            uint urndnum;
            byte[] rndnum = new Byte[4];

            if (lBound == uBound - 1)
            {
                // 只有iBound返回的情况  
                return lBound;
            }

            uint xcludeRndBase = (uint.MaxValue - (uint.MaxValue % (uint)(uBound - lBound)));
            do
            {
                rng.GetBytes(rndnum);
                urndnum = System.BitConverter.ToUInt32(rndnum, 0);
            } while (urndnum >= xcludeRndBase);
            return (int)(urndnum % (uBound - lBound)) + lBound;
        }

        protected char GetRandomCharacter()
        {
            int upperBound = pwdCharArray.GetUpperBound(0);
            if (true == this.ExcludeSymbols)
            {
                upperBound = RandomStringGenerator.UBoundDigit;
            }

            int randomCharPosition = GetCryptographicRandomNumber(pwdCharArray.GetLowerBound(0), upperBound);
            char randomChar = pwdCharArray[randomCharPosition];
            return randomChar;
        }

        public string Generate()
        {
            // 得到minimum 和 maximum 之间随机的长度
            int pwdLength = GetCryptographicRandomNumber(this.Minimum, this.Maximum);
            StringBuilder pwdBuffer = new StringBuilder();
            pwdBuffer.Capacity = this.Maximum;
            // 产生随机字符
            char lastCharacter, nextCharacter;
            // 初始化标记
            lastCharacter = nextCharacter = '\n';

            for (int i = 0; i < pwdLength; i++)
            {
                nextCharacter = GetRandomCharacter();
                if (false == this.ConsecutiveCharacters)
                {
                    while (lastCharacter == nextCharacter)
                    {
                        nextCharacter = GetRandomCharacter();
                    }
                }

                if (false == this.RepeatCharacters)
                {
                    string temp = pwdBuffer.ToString();
                    int duplicateIndex = temp.IndexOf(nextCharacter);

                    while (-1 != duplicateIndex)
                    {
                        nextCharacter = GetRandomCharacter();
                        duplicateIndex = temp.IndexOf(nextCharacter);
                    }
                }

                if ((null != this.Exclusions))
                {
                    while (-1 != this.Exclusions.IndexOf(nextCharacter))
                    {
                        nextCharacter = GetRandomCharacter();
                    }
                }
                pwdBuffer.Append(nextCharacter);
                lastCharacter = nextCharacter;
            }


            if (null != pwdBuffer)
            {
                return pwdBuffer.ToString();
            }
            else
            {
                return String.Empty;
            }
        }


        public bool ConsecutiveCharacters
        {
            get { return this.hasConsecutive; }
            set { this.hasConsecutive = value; }
        }

        public bool ExcludeSymbols
        {
            get { return this.hasSymbols; }
            set { this.hasSymbols = value; }
        }

        public string Exclusions
        {
            get { return this.exclusionSet; }
            set { this.exclusionSet = value; }
        }

        public int Maximum
        {
            get { return this.maxSize; }
            set
            {
                this.maxSize = value;
                if (this.minSize >= this.maxSize)
                {
                    this.maxSize = RandomStringGenerator.DefaultMaximum;
                }
            }
        }

        public int Minimum
        {
            get { return this.minSize; }
            set
            {
                this.minSize = value;
                if (RandomStringGenerator.DefaultMinimum > this.minSize)
                {
                    this.minSize = RandomStringGenerator.DefaultMinimum;
                }
            }
        }

        public bool RepeatCharacters
        {
            get { return this.hasRepeating; }
            set { this.hasRepeating = value; }
        }
        private const int DefaultMaximum = 10;
        private const int DefaultMinimum = 6;
        private const int UBoundDigit = 61;
        private string exclusionSet;
        private bool hasConsecutive;
        private bool hasRepeating;
        private bool hasSymbols;
        private int maxSize;
        private int minSize;
        private char[] pwdCharArray = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789`~!@#$^*()-_=+[]{}\\|;:'\",./".ToCharArray();
        private RNGCryptoServiceProvider rng;


        // Get a random number string.
        private static String GetRandomNumber()
        {
            Random RandomClass = new Random();
            return RandomClass.Next(0x7fffffff).ToString();
        }
        public static string GenerateID()
        {
            string guid = GetRandomNumber() + Guid.NewGuid().ToString();
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] messageBytes = Encoding.UTF8.GetBytes(guid);
            byte[] sum = md5.ComputeHash(messageBytes);
            string md5String = BitConverter.ToString(sum);
            md5String = md5String.Replace("-", "");
            return md5String;
        }

    }
}
