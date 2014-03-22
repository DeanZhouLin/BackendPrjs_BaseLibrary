/*****************************************************************
 * 
 * 
使用:
<asp:TextBox  Title="用户名" Rel="req|useraccount" ID="txtUserName" runat="server" ></asp:TextBox>
    Title为文本框的名称，提示错误时使用。（区分大小写）
    Rel为验证类型。（区分大小写）

验证类型：（不区分大小写）
req								不能为空
email							email格式（aaa@bbb.ccc，没有长度限制）
domain							域名验证（mail地址@后面部分，不包括@）
useraccount					用户帐号（^[a-zA-Z0-9\!\$\%\^\&\*\-_\{\}\|\~\?\.]{1,500}$ 目前为数字，字母和部分符号的组合）
phone							手机号码（8位或者11位）
tel								电话
datetime                        日期
number						数字
decimal                     小数的精度验证 decimal:1
chn								中文
en_num                          英文和数字组合
pinum                           正整数   
len								验证长度：
									len:[min]~[max]
									*表示0或者无穷大（在min为表示0，在max位表示无穷大）
									min==max表示长度必须相等
									eg：
										<input id="txtPassword" name="txtPassword" title="密码"   rel="ipt-validate|req|len:6~20"   class="input"  runat="Server" type="text" />
numrange					验证数字范围,必须同时有number验证。	numrange:[min]~[max]
 
  
 DropDownList：
 req                              selectedindex == 0 时，提示用户没有选择
 invalidindex                 当用户选择指定的index时，提示选择错误(当有此选项时，将不再验证selectedindex是否为0，除非0包含在invalidindex中)
                                    eg:
                                    <asp:DropDownList ID="DropDownList1" Title="dd" Rel="req" runat="server">   // selectedindex == 0 时，提示错误
                                    <asp:DropDownList ID="DropDownList1" Title="dd" Rel="invalidindex:1,2,3" runat="server"> //selectedindex为1，2或者3时，提示错误
 * 
 * 
 ******************************************************************/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace Com.BaseLibrary.Utility
{
    public class ValidationHelper
    {
        public static bool ValidateControl(out string message, out WebControl firstControl, params WebControl[] controlList)
        {
            bool result = true;
            bool hasSetFocus = false;
            message = string.Empty;
            firstControl = null;

            foreach (WebControl wc in controlList)
            {
                if (wc is TextBox)
                {
                    string temp = ValidateTextBox(wc as TextBox);
                    if (!hasSetFocus && !string.IsNullOrEmpty(temp))
                    {
                        firstControl = wc;
                        hasSetFocus = true;
                    }
                    if (!string.IsNullOrEmpty(temp))
                    {
                        message += "- " + temp;
                    }
                }

                if (wc is DropDownList)
                {
                    if (!(wc as DropDownList).Visible)
                        continue;
                    string temp = ValidateDropDownList(wc as DropDownList);
                    if (!hasSetFocus && !string.IsNullOrEmpty(temp))
                    {
                        firstControl = wc;
                        hasSetFocus = true;
                    }
                    if (!string.IsNullOrEmpty(temp))
                    {
                        message += "- " + temp;
                    }
                }
            }

            if (!string.IsNullOrEmpty(message))
            {
                result = false;
            }
            return result;
        }

        private static string ValidateDropDownList(DropDownList ddl)
        {
            if (ddl.Attributes["Rel"] == null || ddl.Attributes["Title"] == null)
            {
                return string.Empty;
            }
            bool checkReq = false;
            bool checkInvalidIndex = false;
            string errorTxt = string.Empty;
            string cmd = string.Empty;
            cmd = ddl.Attributes["Rel"].ToUpper();
            string[] cmds = cmd.Split('|');
            string[] invalidIndex = null;

            foreach (string command in cmds)
            {
                if (command == "REQ")
                {
                    checkReq = true;
                }
                if (command.IndexOf(":") >= 0)
                {
                    var nv = command.Split(':');
                    if (nv[0] == "INVALIDINDEX")
                    {
                        checkInvalidIndex = true;
                        invalidIndex = nv[1].Split(',');
                    }
                }
            }
            string title = ddl.Attributes["Title"].ToString();
            if (checkReq && !checkInvalidIndex)
            {
                if (ddl.SelectedIndex == 0)
                {
                    errorTxt += "" + title + "没有选择。\n";
                    return errorTxt;
                }
            }

            if (checkInvalidIndex && invalidIndex != null)
            {
                foreach (string index in invalidIndex)
                {
                    if (Converter.ToInt32(index, -1) == ddl.SelectedIndex)
                    {
                        errorTxt += "" + title + "选择错误。\n";
                        return errorTxt;
                    }
                }

            }
            return "";
        }

        private static string ValidateTextBox(TextBox tb)
        {
            if (tb.Attributes["Rel"] == null || tb.Attributes["Title"] == null)
            {
                return string.Empty;
            }
            bool checkNull = false;  bool checkPun = false; bool checkReq = false; bool checkEmail = false; bool checkDomain = false; bool checkConfirm = false; bool checkDateConfirm = false; bool checkMPhone = false; bool checkTel = false; bool checkNumber = false; bool checkDecimal = false; bool checkCHN = false; bool checkUserAccount = false; bool checkLength = false; bool checkNumericRange = false; bool checkDecimalRange = false; bool checkEnAndNumber = false; bool checkPositiveInteger = false; bool checkPositiveIntegerAll = false;
            bool checkWebSiteUrl = false;
            WebControl confirmTarget;
            string lenMin = string.Empty; string lenMax = string.Empty;
            string errorTxt = string.Empty;
            string precision = string.Empty;

            string cmd = string.Empty;
            cmd = tb.Attributes["Rel"].ToString().ToUpper();
            string[] cmds = cmd.Split('|');

            foreach (string command in cmds)
            {
                if (command=="WEBSITEURL")
                {
                    checkWebSiteUrl = true;
                }
                if (command == "NULL")
                {
                    checkNull = true;
                }
                if (command == "PUN")
                {
                    checkPun = true;
                }
                if (command == "REQ")
                {
                    checkReq = true;
                }
                if (command == "EMAIL")
                {
                    checkEmail = true;
                }
                if (command == "DOMAIN")
                {
                    checkDomain = true;
                }
                if (command == "USERACCOUNT")
                {
                    checkUserAccount = true;
                }
                if (command == "PHONE")
                {
                    checkMPhone = true;
                }
                if (command == "TEL")
                {
                    checkTel = true;
                }
                if (command == "DATETIME")
                {
                    checkDateConfirm = true;
                }
                if (command == "NUMBER")
                {
                    checkNumber = true;
                }
                if (command == "CHN")
                {
                    checkCHN = true;
                }
                if (command == "EN_NUM")
                {
                    checkEnAndNumber = true;
                }
                if (command == "PINUM")
                {
                    checkPositiveInteger = true;
                }
                if (command == "PINUMALL")
                {
                    checkPositiveIntegerAll = true;
                }
                if (command.IndexOf(":") >= 0)
                {
                    var nv = command.Split(':');
                    if (nv[0] == "LEN")
                    {
                        checkLength = true;
                        lenMin = nv[1].Split('~')[0];
                        lenMax = nv[1].Split('~')[1];
                    }
                }
                if (command.IndexOf(":") >= 0)
                {
                    var nv = command.Split(':');
                    if (nv[0] == "NUMRANGE")
                    {
                        checkNumericRange = true;
                        lenMin = nv[1].Split('~')[0];
                        lenMax = nv[1].Split('~')[1];
                    }
                }
                if (command.IndexOf(":") >= 0)
                {
                    var nv = command.Split(':');
                    if (nv[0] == "CHECKDECIMAL")
                    {
                        checkDecimalRange = true;
                        lenMin = nv[1].Split('~')[0];
                        lenMax = nv[1].Split('~')[1];
                    }
                }
                if (command.IndexOf(":") >= 0)
                {
                    var nv = command.Split(':');
                    if (nv[0] == "DECIMAL")
                    {
                        checkDecimal = true;
                        precision = nv[1];
                    }
                }
            }

            string iptValue = tb.Text;
            string title = tb.Attributes["Title"].ToString();
            if (checkNull)
            {
                if (StringUtil.IsNullOrEmpty(iptValue))
                {
                    errorTxt += "" + title + "不能为空。\n";
                    return errorTxt;
                }
            }
            if (checkReq)
            {
                if (StringUtil.IsNullOrEmpty(iptValue))
                {
                    errorTxt += "" + title + "没有填写。\n";
                    return errorTxt;
                }
            }
            else
            {
                if (StringUtil.IsNullOrEmpty(iptValue))
                {
                    return "";
                }
            }

            if (checkEmail)
            {
                if (!StringUtil.IsEmail(iptValue))
                {
                    errorTxt += "" + title + "格式错误。\n";
                    return errorTxt;
                }
            }

            if (checkWebSiteUrl)
            {
                if (!StringUtil.IsWebSiteUrl(iptValue))
                {
                    errorTxt += "" + title + "格式错误。\n";
                    return errorTxt;
                }
            }

            if (checkDomain)
            {
                if (!StringUtil.IsEmail("aaa@" + iptValue))
                {
                    errorTxt += "" + title + "格式错误。\n";
                    return errorTxt;
                }
            }
            if (checkUserAccount)
            {
                if (!StringUtil.IsUserAccount(iptValue))
                {
                    errorTxt += "" + title + "格式错误。\n";
                    return errorTxt;
                }
            }

            if (checkNumber)
            {
                if (!StringUtil.IsNumber(iptValue))
                {
                    errorTxt += "" + title + "格式错误。\n";
                    return errorTxt;
                }
            }

            if (checkDecimal)
            {
                if (!StringUtil.IsDecimal(iptValue, precision))
                {
                    errorTxt += "" + title + "格式错误。\n";
                    return errorTxt;
                }
            }

            if (checkEnAndNumber)
            {
                if (!StringUtil.IsENAndNumChars(iptValue))
                {
                    errorTxt += "" + title + "格式错误。\n";
                    return errorTxt;
                }
            }
            if (checkPositiveInteger)
            {
                if (!StringUtil.IsPositiveInteger(iptValue))
                {
                    errorTxt += "" + title + "请输入正整数。\n";
                    return errorTxt;
                }
            }
            if (checkPun)
            {
                if (!StringUtil.IsInteger(iptValue))
                {
                    errorTxt += "" + title + "格式错误。\n";
                    return errorTxt;
                }
            }
            if (checkPositiveIntegerAll)
            {
                if (!StringUtil.IsPositiveIntegerAll(iptValue))
                {
                    errorTxt += "" + title + "格式错误。\n";
                    return errorTxt;
                }
            }
            if (checkMPhone)
            {
                if (!StringUtil.IsLengthedNumber(iptValue, 11) && !StringUtil.IsLengthedNumber(iptValue, 8))
                {
                    errorTxt += "" + title + "格式错误。\n";
                    return errorTxt;
                }
            }
            if (checkTel)
            {
                if (!StringUtil.IsCellPhoneNumber(iptValue))
                {
                    errorTxt += "" + title + "格式错误。\n";
                    return errorTxt;
                }
            }
            if (checkDateConfirm)
            {
                if (!string.IsNullOrEmpty(iptValue) && !StringUtil.IsBirthday(iptValue))
                {
                    errorTxt += string.Format("{0} 格式错误。（示例：{1}）\n", title, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    return errorTxt;
                }
            }

            if (checkLength)
            {
                if (!StringUtil.IsLengthed(iptValue, lenMin, lenMax))
                {
                    if (lenMin == "*")
                    {
                        errorTxt += title + "的长度不能大于" + lenMax;
                    }
                    else if (lenMax == "*")
                    {
                        errorTxt += title + "的长度不能小于" + lenMin;
                    }
                    else
                    {
                        if (lenMin == lenMax)
                        {
                            errorTxt += title + "的长度必须为" + lenMin;
                        }
                        else
                        {
                            errorTxt += title + "的长度不能小于" + lenMin + "并且不能大于" + lenMax;
                        }
                    }
                    return errorTxt + "\n";
                }
            }

            if (checkNumericRange)
            {
                int iValue;
                try
                {
                    iValue = Int32.Parse(iptValue);
                }
                catch
                {
                    return "" + title + "格式错误。(不能小于" + lenMin + "并且不能大于" + lenMax + ")\n";
                }
                int iMin = Converter.ToInt32(lenMin, int.MinValue);
                int iMax = Converter.ToInt32(lenMax, int.MaxValue);

                if (iValue < iMin || iValue > iMax)
                {
                    if (lenMin == "*")
                    {
                        errorTxt += title + "不能大于" + lenMax;
                    }
                    else if (lenMax == "*")
                    {
                        errorTxt += title + "不能小于" + lenMin;
                    }
                    else
                    {
                        if (lenMin == lenMax)
                        {
                            errorTxt += title + "必须等于" + lenMin;
                        }
                        else
                        {
                            errorTxt += title + "不能小于" + lenMin + "并且不能大于" + lenMax;
                        }
                    }
                    return errorTxt + "\n";
                }
            }

            if (checkDecimalRange)
            {
                decimal iValue;
                try
                {
                    iValue = decimal.Parse(iptValue);
                }
                catch
                {
                    return "" + title + "格式错误。\n";
                }
                decimal iMin = Converter.ToDecimal(lenMin, decimal.MinValue);
                decimal iMax = Converter.ToDecimal(lenMax, decimal.MaxValue);

                if (iValue < iMin || iValue > iMax)
                {
                    if (lenMin == "*")
                    {
                        errorTxt += title + "不能大于" + lenMax;
                    }
                    else if (lenMax == "*")
                    {
                        errorTxt += title + "不能小于" + lenMin;
                    }
                    else
                    {
                        if (lenMin == lenMax)
                        {
                            errorTxt += title + "必须等于" + lenMin;
                        }
                        else
                        {
                            errorTxt += title + "不能小于" + lenMin + "并且不能大于" + lenMax;
                        }
                    }
                    return errorTxt + "\n";
                }
            }

            if (checkCHN)
            {
                if (!StringUtil.IsAllCHNChars(iptValue))
                {
                    errorTxt += title + "不是中文字符\n";
                    return errorTxt;
                }
            }

            return "";
        }

    }
}
