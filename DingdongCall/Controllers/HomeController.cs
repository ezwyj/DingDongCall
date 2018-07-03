using DingdongCall.App_Start;
using DingdongCall.Models;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DingdongCall.Controllers
{
    public class HomeController : Controller
    {
        private static byte[] _key = Convert.FromBase64String(@"uw4FGrtauRGbh2ukh2ZFAA ==");
        private static string Decrypt(string toDecrypt, byte[] key)
        {
            byte[] keyArray = key;
            byte[] inputBuffer = Convert.FromBase64String(toDecrypt);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = rDel.CreateDecryptor();

            byte[] resultArray = cTransform.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);

            return Encoding.UTF8.GetString(resultArray);
        }
        private string GetJsonString()
        {
            string postStr = string.Empty;
            Stream inputStream = Request.InputStream;
            int contentLength = Request.ContentLength;
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
            return postStr;
        }


        /// <summary>
        /// 察看用户配置信息
        /// </summary>
        /// <returns></returns>
        public ActionResult UserProfile(string state)
        {
            state = state.Replace(" ", "+");
            var json = Decrypt(state, _key);
            var entity = Newtonsoft.Json.JsonConvert.DeserializeObject<DingDongOpenRequest>(json);
            ViewBag.userId = entity.userid;
            Database db = new Database("dingdongDB");
            string phone = db.ExecuteScalar<string>("select callPhone from DingDongCall_User where DingDongUserId=@0", entity.userid);
            ViewBag.phone = phone;

            return View();
        }
        public JsonResult SavePhone(string userId, string phone)
        {
            runLog.log("save userId:" + userId + ",mobile:" + phone);
            Database db = new Database("dingdongDB");
            db.BeginTransaction();
            string sqlUserToDo = string.Empty;
            sqlUserToDo = string.Format("Update DingDongCall_User set callphone='{0}',setphoneTime=getdate() where DingDongUserId='{1}'", phone, userId);
            try
            {

                db.Execute(sqlUserToDo);
                db.CompleteTransaction();
                runLog.log(string.Format("save user:{0} phone:{1}", userId, phone));
                return new JsonResult { Data = new { State = true, Msg ="保存成功"}, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            catch (Exception e)
            {
                db.AbortTransaction();
                return new JsonResult { Data = new { State = false, Msg =e.Message}, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
        }
        private static string appID = System.Configuration.ConfigurationManager.AppSettings["appId_ytx"];
        private static void CallMobile(string phoneNum)
        {
            string jsonData = "{\"action\":\"callDailBack\",\"src\":\"" + phoneNum + "\",\"dst\":\"01053189990\",\"appid\":\"" + appID + "\",\"credit\":\"" + "10" + "\"}";
            //2、云通信平台接口请求URL
            string url = "/call/DailbackCall.wx";
            string result = CommenHelper.SendRequest(url, jsonData);
            runLog.log("call :" + result);
        }
        static FileLog runLog = new FileLog(AppDomain.CurrentDomain.BaseDirectory + @"/log/runLog.txt");
        public ContentResult OpenStatus(string state)
        {
            runLog.log(state);
            state = state.Replace(" ", "+");
            var json = Decrypt(state, _key);
            var entity = Newtonsoft.Json.JsonConvert.DeserializeObject<DingDongOpenRequest>(json);
            Database db = new Database("dingdongDB");
            db.BeginTransaction();
            string sqlCheck = string.Format("Select count(0) from DingDongCall_User where DingDongUserId='{0}'",entity.userid);
            var first = db.ExecuteScalar<int>(sqlCheck);
            string sqlUserToDo = string.Empty;
            if (first == 0)
            {
                //新增
                sqlUserToDo = string.Format("Insert into DingDongCall_User (DingDongUserId,Inputtime,status) values ('{0}',getdate(),'{1}')", entity.userid, entity.operation);

            }
            else
            {
                sqlUserToDo = string.Format("Update DingDongCall_User set status='{0}',InputTime=getdate() where DingDongUserId='{1}'", entity.operation, entity.userid);
            }
            try
            {
                
                db.Execute(sqlUserToDo);
                db.CompleteTransaction();
                return Content("0");
            }
            catch(Exception e)
            {
                db.AbortTransaction();
                return Content("1");
            }

        }
        public ActionResult Index()
        {
            return View("");
        }

        [HttpPost]
        public ContentResult Post()
        {
            var json =GetJsonString();
            runLog.log(json);

            var reqObj  = Newtonsoft.Json.JsonConvert.DeserializeObject<DingDongRequest>(json);
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
                CallMobile(phone.Trim());
                Directive_items item = new Directive_items();
                item.content = "已拨打电话请注意接听";
                item.type = "1";
                toDingDongServer.directive = new Directive();
                toDingDongServer.directive.directive_items = new List<Directive_items>();
                toDingDongServer.directive.directive_items.Add(item);

            }
            else
            {
                Directive_items item = new Directive_items();
                item.content = "未找到拨打电话,请先设置电话";
                item.type = "1";
                toDingDongServer.directive = new Directive();
                toDingDongServer.directive.directive_items = new List<Directive_items>();
                toDingDongServer.directive.directive_items.Add(item);
            }
            
            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(toDingDongServer));
        }
    }
}