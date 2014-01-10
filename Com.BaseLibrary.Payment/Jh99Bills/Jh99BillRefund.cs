using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using Com.BaseLibrary.Payment.Jh99BillsRefundQuery;

namespace Com.BaseLibrary.Payment.Jh99Bills
{
    public class Jh99BillRefund
    {
        public static RefundResult DoRefund(long transactionId, string orderid, decimal amount)
        {
            StringBuilder queryString = new StringBuilder();
			string postdate = DateTime.Now.ToString("yyyyMMddHHmmss");
            string strAmount = amount.ToString("0.00");
            string signMsgVal = String.Format(JH99BillConfig.Current.RefundParamFormat,
                JH99BillConfig.Current.MerchantID,
                 orderid,
                 strAmount,
                 postdate,
                 transactionId,
                 JH99BillConfig.Current.AutoRefundMD5Key
                 );

            string signMsg = Jh99BillCore.GetMD5(signMsgVal).ToUpper();
            Jh99BillCore.AppendParam(queryString, "merchant_id", JH99BillConfig.Current.MerchantID);
            Jh99BillCore.AppendParam(queryString, "version", "bill_drawback_api_1");
            Jh99BillCore.AppendParam(queryString, "command_type", "001");
            Jh99BillCore.AppendParam(queryString, "mac", signMsg);
            Jh99BillCore.AppendParam(queryString, "txOrder", transactionId);
            Jh99BillCore.AppendParam(queryString, "amount", strAmount);
            Jh99BillCore.AppendParam(queryString, "postdate", postdate);
            Jh99BillCore.AppendParam(queryString, "orderid", orderid);
            Jh99BillCore.AppendParam(queryString, "merchant_key", JH99BillConfig.Current.AutoRefundMD5Key);
            WebRequest myWebRequest = WebRequest.Create(JH99BillConfig.Current.RefundUrl);
            myWebRequest.Method = "POST";
            myWebRequest.ContentType = "application/x-www-form-urlencoded";
            using (Stream streamReq = myWebRequest.GetRequestStream())
            {
                byte[] byteArray = Encoding.GetEncoding(JH99BillConfig.Current.Encoding).GetBytes(queryString.ToString());
                streamReq.Write(byteArray, 0, byteArray.Length);
            }
            RefundResult refundResult = new RefundResult();
            using (WebResponse myWebResponse = myWebRequest.GetResponse())
            {
                StreamReader sr = new StreamReader(myWebResponse.GetResponseStream());
                string res = sr.ReadToEnd();
                res = res.Replace("\r\n", "");
                res = res.Replace("\t", "");
                //加载XML数据
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(res);

                //以下为获取返回的XML数据相应节点的值开始(节点名称一定要大写：如MERCHANT)
                //商户编号：merchant
                ///与提交订单时的签名类型保持一致
                //string rtnmerchant = xmlDoc.SelectSingleNode("//MERCHANT").InnerText.ToString();

                ////退款流水号：txorder
                /////与提交订单时的签名类型保持一致
                refundResult.RefundTradeNO = xmlDoc.SelectSingleNode("//TXORDER").InnerText.ToString();

                ////原 商户订单编号：orderid
                /////与提交订单时的商户订单号保持一致 
                //string rtnorderid = xmlDoc.SelectSingleNode("//ORDERID").InnerText.ToString();

                ////退款金额：amount
                /////与提交订单时的商户订单提交时间保持一致  
                string rtnamount = xmlDoc.SelectSingleNode("//AMOUNT").InnerText.ToString();
                refundResult.RefundTradeAmount = decimal.Parse(rtnamount);
                //退 款申请结果：result
                ///固定选择值：Y、N
                ///Y 表示退款申请成功；N 表示退款申请失败
                string rtnresult = xmlDoc.SelectSingleNode("//RESULT").InnerText.ToString();
                //错误信息：code
                ///英文或中文字符串,详细可参见接口文档提示信息


                refundResult.ReturnCode = xmlDoc.SelectSingleNode("//CODE").InnerText.ToString();
                refundResult.IsSuccess = rtnresult == "Y";
                return refundResult;
            }
        }

        //功能函数。将变量值不为空的参数组成字符串（1：返回字符串，2：参数名,3：参数值）
        private static string appendParam(string returnStr, string paramId, string paramValue)
        {
            if (returnStr != "")
            {
                if (paramValue != "")
                {
                    returnStr += paramId + "=" + paramValue;
                }
            }
            else
            {
                if (paramValue != "")
                {
                    returnStr = paramId + "=" + paramValue;
                }
            }
            return returnStr;
        }

        public static List<GatewayRefundQueryResultDto> QueryRefund(
            DateTime startDate,
            DateTime endDate,
            int queryType,
            string customerBatchId,
            string orderId,
            string rOrderId,
            string requestPage,
            string status,
            string refundTradeNo)
        {
            try
            {

                GatewayRefundQueryRequest request = new GatewayRefundQueryRequest();
                request.version = "v2.0";//查询接口版本
                request.signType = "1";//MD5加密
                request.merchantAcctId = JH99BillConfig.Current.PaymentMerchantID;
				request.startDate = startDate.ToString("yyyyMMdd");
				request.endDate = endDate.ToString("yyyyMMdd");
                request.customerBatchId = customerBatchId;
				request.orderId = orderId;
                request.ROrderId = rOrderId;
                request.requestPage = requestPage;
                request.seqId = refundTradeNo;
                //request.status = "0";

                StringBuilder signMsgVal = new StringBuilder();
                signMsgVal = Jh99BillCore.AppendParam(signMsgVal, "version", request.version);
                signMsgVal = Jh99BillCore.AppendParam(signMsgVal, "signType", request.signType);
                signMsgVal = Jh99BillCore.AppendParam(signMsgVal, "merchantAcctId", request.merchantAcctId);
                //macVal = Jh99BillCore.AppendParam(macVal, "dateQueryType", dateQueryType);
                signMsgVal = Jh99BillCore.AppendParam(signMsgVal, "startDate", request.startDate);
                signMsgVal = Jh99BillCore.AppendParam(signMsgVal, "endDate", request.endDate);
                // macVal = Jh99BillCore.AppendParam(macVal, "finishDateStart", finishDateStart);
                //macVal = Jh99BillCore.AppendParam(macVal, "finishDateEnd", finishDateEnd);
                signMsgVal = Jh99BillCore.AppendParam(signMsgVal, "customerBatchId", request.customerBatchId);
                signMsgVal = Jh99BillCore.AppendParam(signMsgVal, "orderId", request.orderId);
                signMsgVal = Jh99BillCore.AppendParam(signMsgVal, "requestPage", request.requestPage);
                signMsgVal = Jh99BillCore.AppendParam(signMsgVal, "rOrderId", request.ROrderId);
                signMsgVal = Jh99BillCore.AppendParam(signMsgVal, "seqId", request.seqId);
                signMsgVal = Jh99BillCore.AppendParam(signMsgVal, "status", request.status);
                signMsgVal = Jh99BillCore.AppendParam(signMsgVal, "key", JH99BillConfig.Current.RMBGatewayQueryMD5Key);
                request.signMsg = Jh99BillCore.GetMD5(signMsgVal.ToString()).ToUpper();
                GatewayRefundQueryClient refundQueryClient = new GatewayRefundQueryClient("gatewayRefundQuery");

                GatewayRefundQueryResponse response = refundQueryClient.query(request);
                signMsgVal = new StringBuilder();
                signMsgVal = Jh99BillCore.AppendParam(signMsgVal, "version", response.version);
                signMsgVal = Jh99BillCore.AppendParam(signMsgVal, "signType", Convert.ToString(response.signType));
                signMsgVal = Jh99BillCore.AppendParam(signMsgVal, "merchantAcctId", response.merchantAcctId);
                signMsgVal = Jh99BillCore.AppendParam(signMsgVal, "recordCount", Convert.ToString(response.recordCount));
                signMsgVal = Jh99BillCore.AppendParam(signMsgVal, "pageCount", Convert.ToString(response.pageCount));
                signMsgVal = Jh99BillCore.AppendParam(signMsgVal, "currentPage", response.currentPage);
                signMsgVal = Jh99BillCore.AppendParam(signMsgVal, "pageSize", Convert.ToString(response.pageSize));
                //signMsgVal = Jh99BillCore.AppendParam(signMsgVal, "errCode", response.errCode);
                signMsgVal = Jh99BillCore.AppendParam(signMsgVal, "key", JH99BillConfig.Current.RMBGatewayQueryMD5Key);
                string signMsg = Jh99BillCore.GetMD5(signMsgVal.ToString()).ToUpper();
                if (signMsg == response.signMsg)
                {
                    if (response.recordCount > 0)
                    {
                        return response.results.ToList();
                    }
                }
                else
                {
                    //发生了严重的错误
                }
                return new List<GatewayRefundQueryResultDto>();

            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }

    public class RefundResult
    {
        public bool IsSuccess { get; set; }
        public string ReturnCode { get; set; }
        public string RefundTradeNO { get; set; }
        public decimal RefundTradeAmount { get; set; }

    }
}
