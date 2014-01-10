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
    /// XML ���л�����
    /// </summary>
    public static class ObjectXmlSerializer
    {
        /// <summary>
        /// ��XML�ļ��ж�ȡXML��Ϣ�������л���һ������ΪT�Ķ���
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
        /// ��XML�ַ��������л���һ������ΪT�Ķ���
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
        /// ��һ������ΪT�Ķ������л���һ��XML�ı�
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
        ///   ��һ������ΪT�Ķ������л���һ��XML�ı�����д���ļ�
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="relativePath">���·��</param>
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
        ///  ��һ������ΪT�Ķ������л���һ��XML�ı�����д���ļ�
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fullPath">����·��</param>
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