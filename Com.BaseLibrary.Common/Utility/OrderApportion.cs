using System;
using System.Collections.Generic;
using System.Linq;

namespace Com.BaseLibrary.Common.Utility
{
    public static class OrderApportion
    {
        /// <summary>
        /// 折扣分摊
        /// </summary>
        /// <param name="apportionObject">分摊对象列表</param>
        /// <param name="apportionTotal">分摊的折扣总金额</param>
        /// <param name="digits">分摊结果保留的小数位数</param>
        /// <returns>分摊后的对象列表</returns>
        public static List<ApportionModel> Apportion(List<ApportionModel> apportionObject, decimal apportionTotal, int digits)
        {
            decimal accuracy = (decimal)Math.Pow(0.1, (double)digits);
            decimal weights = (decimal)Math.Pow(10, (double)digits);
            decimal totalAmount = 0m;
            totalAmount = apportionObject.Sum(a => a.Basis);
            if (totalAmount <= 0m)//总金额为0不分摊
            {
                return apportionObject;
            }
            decimal modulus = apportionTotal / totalAmount;
            apportionObject.ForEach(a => a.Result = Math.Ceiling(a.Basis * modulus * weights) / weights);
            decimal margin = apportionObject.Sum(a => a.Result) - apportionTotal;
            if (margin > 0)
            {
                int a = (int)(margin / 0.0001m);
                List<ApportionModel> model = apportionObject.FindAll(b => b.Result >= accuracy).ToList();
                for (int i = 0; i < a; i++)
                {
                    ApportionModel m = model[i];
                    m.Result = m.Result - accuracy;
                }
            }
            return apportionObject;
        }
    }


    public class ApportionModel
    {
        /// <summary>
        /// 分摊项唯一索引
        /// </summary>
        public string ObjectNO { get; set; }
        /// <summary>
        /// 计算分摊比例的依据，例如：分摊优惠券，则该字段为自订单金额
        /// </summary>
        public decimal Basis { get; set; }
        /// <summary>
        /// 分摊的结果，例如：分摊优惠券，则该字段为子订单分摊到的优惠券金额
        /// </summary>
        public decimal Result { get; set; }
        /// <summary>
        /// 对象分组标识，与分摊逻辑无关
        /// </summary>
        public string ObjectGroup { get; set; }
        /// <summary>
        /// 对象个数，与分摊逻辑无关
        /// </summary>
        public int ObjectCount { get; set; }
    }
}
