
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.BaseLibrary.Payment.Jh99BillsOrderQuery;

namespace Com.BaseLibrary.Payment.Jh99Bills
{
    public class Jh99BillOrder
    {
        public static List<GatewayOrderDetail> QueryOrder(
            DateTime startTime,
            DateTime endTime,
            string orderId,
            string requestPage)
        {
            try
            {
                GatewayOrderQueryRequest request = new GatewayOrderQueryRequest();
                // request.
                request.merchantAcctId = JH99BillConfig.Current.PaymentMerchantID;
                request.inputCharset = "1";
                request.version = "v2.0";
                request.signType = 1;
                request.orderId = orderId;
                request.queryType = string.IsNullOrEmpty(orderId) ? 1 : 0;
                request.queryMode = 1;
                if (string.IsNullOrEmpty(orderId))
                {
					request.startTime = startTime.ToString("yyyyMMddHHmmss");
					request.endTime = endTime.ToString("yyyyMMddHHmmss");
                }
               
                request.requestPage = requestPage;

                StringBuilder signMsgVal = new StringBuilder();
                signMsgVal = Jh99BillCore.AppendParam(signMsgVal, "inputCharset", request.inputCharset);
                signMsgVal = Jh99BillCore.AppendParam(signMsgVal, "version", request.version);
                signMsgVal = Jh99BillCore.AppendParam(signMsgVal, "signType", request.signType);
                signMsgVal = Jh99BillCore.AppendParam(signMsgVal, "merchantAcctId", request.merchantAcctId);
                signMsgVal = Jh99BillCore.AppendParam(signMsgVal, "queryType", request.queryType);
                signMsgVal = Jh99BillCore.AppendParam(signMsgVal, "queryMode", request.queryMode);
                signMsgVal = Jh99BillCore.AppendParam(signMsgVal, "startTime", request.startTime);
                signMsgVal = Jh99BillCore.AppendParam(signMsgVal, "endTime", request.endTime);
                signMsgVal = Jh99BillCore.AppendParam(signMsgVal, "requestPage", request.requestPage);
                signMsgVal = Jh99BillCore.AppendParam(signMsgVal, "orderId", request.orderId);
                signMsgVal = Jh99BillCore.AppendParam(signMsgVal, "key", JH99BillConfig.Current.RMBGatewayQueryMD5Key);
                request.signMsg = Jh99BillCore.GetMD5(signMsgVal.ToString()).ToUpper();

                GatewayOrderQueryClient refundQueryClient = new GatewayOrderQueryClient("gatewayOrderQuery");
                GatewayOrderQueryResponse response = refundQueryClient.gatewayOrderQuery(request);

                //if (!string.IsNullOrEmpty(response.errCode))
                //{
                //    Logger.CurrentLogger.DoWrite("支付服务", "支付服务", "错误", "查询支付订单接口失败", "错误代码：" + response.errCode);
                //}

                signMsgVal = new StringBuilder();
                signMsgVal = Jh99BillCore.AppendParam(signMsgVal, "version", response.version);
                signMsgVal = Jh99BillCore.AppendParam(signMsgVal, "signType", Convert.ToString(response.signType));
                signMsgVal = Jh99BillCore.AppendParam(signMsgVal, "merchantAcctId", response.merchantAcctId);
                signMsgVal = Jh99BillCore.AppendParam(signMsgVal, "errCode", response.errCode);
                signMsgVal = Jh99BillCore.AppendParam(signMsgVal, "currentPage", response.currentPage);
                signMsgVal = Jh99BillCore.AppendParam(signMsgVal, "pageCount", response.pageCount);
                signMsgVal = Jh99BillCore.AppendParam(signMsgVal, "pageSize", response.pageSize);
                signMsgVal = Jh99BillCore.AppendParam(signMsgVal, "recordCount", response.recordCount);
                //signMsgVal = Jh99BillCore.AppendParam(signMsgVal, "errCode", response.errCode);
                signMsgVal = Jh99BillCore.AppendParam(signMsgVal, "key", JH99BillConfig.Current.RMBGatewayQueryMD5Key);
                string signMsg = Jh99BillCore.GetMD5(signMsgVal.ToString()).ToUpper();
                if (signMsg == response.signMsg)
                {
                    if (response.orders != null)
                    {
                        return response.orders.ToList();
                    }
                }
                else
                {
                    //发生了严重的错误
                }
                return new List<GatewayOrderDetail>();

            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}
