using DingdongCall.App_Start;
using DingdongCall.Models;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace DingdongCall
{
    /// <summary>
    /// HandlerPost 的摘要说明
    /// </summary>
    public class HandlerPost : IHttpHandler
    {
        private static string appID = System.Configuration.ConfigurationManager.AppSettings["appId_ytx"];
        private static void CallMobile(string phoneNum)
        {
            string jsonData = "{\"action\":\"callDailBack\",\"src\":\"" + phoneNum + "\",\"dst\":\"" + phoneNum + "\",\"appid\":\"" + appID + "\",\"credit\":\"" + "10" + "\"}";
            //2、云通信平台接口请求URL
            string url = "/call/DailbackCall.wx";
            string result = CommenHelper.SendRequest(url, jsonData);

        }
        static FileLog runLog = new FileLog(AppDomain.CurrentDomain.BaseDirectory + @"/log/runLog.txt");
        public void ProcessRequest(HttpContext context)
        {
            string postStr = string.Empty;
            Stream inputStream = context.Request.InputStream;
            int contentLength = context.Request.ContentLength;
            int offset = 0;
            if (contentLength > 0)
            {
                byte[] buffer = new byte[contentLength];
                for (int i = inputStream.Read(buffer, offset, contentLength - offset); i > 0; i = inputStream.Read(buffer, offset, contentLength - offset))
                {
                    offset += i;
                }
                UTF8Encoding encoding = new UTF8Encoding();
                postStr = encoding.GetString(buffer);
            }
            try
            {
                
                runLog.log(postStr);

                var reqObj = Newtonsoft.Json.JsonConvert.DeserializeObject<DingDongRequest>(postStr);
                Database db = new Database("dingdongDB");

                var toDingDongServer = new DingDongResponse();
                toDingDongServer.versionid = "1.0";
                toDingDongServer.is_end = true;
                toDingDongServer.sequence = reqObj.sequence;
                toDingDongServer.timestamp = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
                toDingDongServer.directive = new Directive();
                //根据用户找手机
                string phone = db.ExecuteScalar<string>("select callPhone from DingDongCall_User where DingDongUserId=@0", reqObj.user.user_id);
                if (!string.IsNullOrEmpty(phone))
                {
                    CallMobile(phone);


                    Directive_items item = new Directive_items();
                    item.content = "主人，已播打电话请注意接听";
                    item.type = "1";
                    toDingDongServer.directive = new Directive();
                    toDingDongServer.directive.directive_items = new List<Directive_items>();
                    toDingDongServer.directive.directive_items.Add(item);

                }
                else
                {
                    Directive_items item = new Directive_items();
                    item.content = "主人，您还没有设置手机号码，请在手机APP应用平台中的小军找手机技能里，进行设置，然后您就可以说“让小军找手机”啦";
                    item.type = "1";
                    toDingDongServer.directive = new Directive();
                    toDingDongServer.directive.directive_items = new List<Directive_items>();
                    toDingDongServer.directive.directive_items.Add(item);
                }



                context.Response.Write (Newtonsoft.Json.JsonConvert.SerializeObject(toDingDongServer));
            }
            catch (Exception e)
            {
                runLog.log("err post:" + e.Message);
                context.Response.Write(e.Message);
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}