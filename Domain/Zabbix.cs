using Newtonsoft.Json;
using System;
using System.Dynamic;
using System.IO;
using System.Net;
using ZabbixManager.Models;
using static System.String;

namespace ZabbixManager.Domain
{
    public class Zabbix
    {
        private const string JsonRpcVersion = "2.0";
        private readonly string _user;
        private readonly string _password;
        private readonly string _zabbixUrl;
        private readonly string _basicAuth;
        private string _auth;
        public bool LoggedOn;

        public Zabbix(string user, string password, string zabbixUrl, bool basicAuth = false)
        {
            _user = user;
            _password = password;
            _zabbixUrl = zabbixUrl;
            if (basicAuth) _basicAuth = Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(_user + ":" + _password));
            _auth = null;
        }

        #region Login/Logout

        public void Login()
        {
            dynamic userAuth = new ExpandoObject();
            userAuth.user = _user;
            userAuth.password = _password;
            Response zbxResponse = GetObjectResponse("user.login", userAuth);
            _auth = zbxResponse.Result;
            if (!IsNullOrEmpty(_auth)) LoggedOn = true;
        }

        public bool Logout()
        {
            Response zbxResponse = GetObjectResponse("user.logout", new string[] { });
            var result = zbxResponse.Result;
            return result;
        }

        #endregion

        #region Request/Response

        public string JsonResponse(string method, object parameters)
        {
            Request zbxRequest = new Request(JsonRpcVersion, method, 1, _auth, parameters);
            string jsonParams = JsonConvert.SerializeObject(zbxRequest);
            return SendRequest(jsonParams);
        }


        public Response GetObjectResponse(string method, object parameters)
        {
            Request zbxRequest = new Request(JsonRpcVersion, method, 1, _auth, parameters);
            string jsonParams = JsonConvert.SerializeObject(zbxRequest);
            return CastToResponse(SendRequest(jsonParams));
        }


        private static Response CastToResponse(string json)
        {
            Response zbxResponse = JsonConvert.DeserializeObject<Response>(json);
            return zbxResponse;
        }

        private string SendRequest(string jsonParams)
        {
            WebRequest request = WebRequest.Create(_zabbixUrl);
            if (_basicAuth != null) request.Headers.Add("Authorization", "Basic " + _basicAuth);
            request.ContentType = "application/json-rpc";
            request.Method = "POST";
            string jsonResult;

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(jsonParams);
                streamWriter.Flush();
                streamWriter.Close();
            }

            WebResponse response = request.GetResponse();
            using (var streamReader = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException()))
            {
                jsonResult = streamReader.ReadToEnd();
            }

            return jsonResult;
        }

        #endregion
    }
}
