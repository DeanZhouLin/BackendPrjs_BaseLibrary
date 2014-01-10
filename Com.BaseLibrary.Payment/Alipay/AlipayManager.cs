using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Com.BaseLibrary.Utility;

namespace Com.Alipay
{
    public class AlipayManager
    {
        private static readonly string NotifyUrl = ConfigurationHelper.GetAppSetting("NotifyUrl");

        public static string DoRefund(
           string batch_no,
           string refund_date,
           string batch_num,
           string detail_data)
        {
            //把请求参数打包成数组
            SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();
            sParaTemp.Add("batch_no", batch_no);
            sParaTemp.Add("refund_date", refund_date);
            sParaTemp.Add("batch_num", batch_num);
            sParaTemp.Add("detail_data", detail_data);
            //构造即时到账批量退款无密接口返回XML处理结果
            AlipayService ali = new AlipayService(NotifyUrl);
            XmlDocument xmlDoc = ali.Refund_fastpay_by_platform_nopwd(sParaTemp);
            StringBuilder sbxml = new StringBuilder();
            string nodeIs_success = xmlDoc.SelectSingleNode("/alipay/is_success").InnerText;
            if (nodeIs_success != "T")//请求不成功的错误信息
            {
                sbxml.Append("错误：" + xmlDoc.SelectSingleNode("/alipay/error").InnerText);
            }
            else//请求成功的支付返回宝处理结果信息
            {
                sbxml.Append(xmlDoc.SelectSingleNode("/alipay/is_success").InnerText);
            }
            return sbxml.ToString();
        }

        public static XmlDocument QuerySingleOrder(string trade_no, string sono)
        {
            ////////////////////////////////////////////////////////////////////////////////////////////////

            //把请求参数打包成数组
            SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();

            sParaTemp.Add("trade_no", trade_no);
            sParaTemp.Add("out_trade_no", sono);
            AlipayService service = new AlipayService("");
            //建立请求

            //请在这里加上商户的业务逻辑程序代码
            //——请根据您的业务逻辑来编写程序（以下代码仅作参考）——
            try
            {
                return service.CreateSingleOrderQuery(sParaTemp);
            }
            catch (Exception exp)
            {
                return null;
            }
        }

        public static XmlDocument DoRefund(string batchNO, string tradeNO, decimal refundAmount)
        {
            //把请求参数打包成数组
            SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();
			sParaTemp.Add("batch_no", batchNO);
            sParaTemp.Add("refund_date", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
            sParaTemp.Add("batch_num", "1");
            string detail_data = string.Format("{0}^{1}^退款", tradeNO, refundAmount.ToString("0.00"));
            sParaTemp.Add("detail_data", detail_data);
            //构造即时到账批量退款无密接口返回XML处理结果
			AlipayService ali = new AlipayService(AlipayConfiguration.Current.RefundNotifyUrl);
            return ali.Refund_fastpay_by_platform_nopwd(sParaTemp);

        }
    }
}
