using System;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using Com.BaseLibrary.Logging;
using System.Xml;
using System.Text;

namespace Com.BaseLibrary.Utility
{
    /// <summary>
    /// XML 序列化工具
    /// </summary>
    public static class ObjectXmlSerializer
    {
        /// <summary>
        /// 从XML文件中读取XML信息并反序列化出一个类型为T的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static T LoadFromXml<T>(string fileName) where T : class
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(fs);
            }
        }

        /// <summary>
        /// 从XML字符串反序列化出一个类型为T的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlMessage"></param>
        /// <returns></returns>
        public static T LoadFromXmlMessage<T>(string xmlMessage) where T : class
        {
            using (StringReader sReader = new StringReader(xmlMessage))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(sReader);
            }

        }

        /// <summary>
        /// 把一个类型为T的对象序列化成一段XML文本
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string ToStringXmlMessage<T>(T t) where T : class
        {
             StringBuilder sb = new StringBuilder();

            using (XmlWriter sWriter = XmlWriter.Create(sb))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(sWriter, t);
                return sb.ToString();
            }


        }
        /// <summary>
        ///   把一个类型为T的对象序列化成一段XML文本，并写入文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="relativePath">相对路径</param>
        /// <param name="t"></param>
        public static void Save<T>(string relativePath, T t) where T : class
        {
            string fullPath = PathUtil.GetFullFilePath(relativePath);
            using (XmlWriter sWriter = XmlWriter.Create(fullPath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(sWriter, t);


                //sWriter

                //File.WriteAllText(fullPath, sWriter.ToString());


            }
        }

        /// <summary>
        ///  把一个类型为T的对象序列化成一段XML文本，并写入文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fullPath">绝对路径</param>
        /// <param name="t"></param>
        public static void SaveAs<T>(string fullPath, T t) where T : class
        {

            using (XmlWriter sWriter = XmlWriter.Create(fullPath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(sWriter, t);
            }
        }

    }
}