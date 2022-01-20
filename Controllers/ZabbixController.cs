using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ZabbixManager.Domain;
using ZabbixManager.Models;

namespace ZabbixManager.Controllers
{
    [Route("api/[controller]")]
    public class ZabbixController : ControllerBase
    {

        private const string ZabbixApiUrl = "https://zabbix.my.ru/api_jsonrpc.php";
        private readonly ILogger<ZabbixController> _logger;
        private readonly AuthUser _conf;

        public ZabbixController(ILogger<ZabbixController> logger, IOptions<AuthUser> options)
        {
            _logger = logger;
            _conf = options.Value;
        }

        [HttpGet]
        [Route("hosts")]
        public dynamic GetHostsZabbix()
        {
            _logger.LogInformation("Старт запроса  по хостам к заббикс");

            Zabbix zabbix = new Zabbix(_conf.Login, _conf.Password, ZabbixApiUrl);
            zabbix.Login();
            Response responseObj = zabbix.GetObjectResponse(
                "host.get",
                new
                {
                    output = new[] { "extend" }
                }
                );
            zabbix.Logout();

            _logger.LogInformation("Конец запроса по хостам к заббикс");

  
            return responseObj.Result;
        }


        [HttpGet]
        [Route("triggers")]
        public dynamic GetTriggersZabbix()
        {
            _logger.LogInformation("Старт запроса по триггерам к заббикс");

            Zabbix zabbix = new Zabbix(_conf.Login, _conf.Password, ZabbixApiUrl);
            zabbix.Login();
            Response responseObj = zabbix.GetObjectResponse(
                "trigger.get",
                new
                {
                    output = new[] { "extend" }
                }
            );
            zabbix.Logout();

            _logger.LogInformation("Конец запроса по триггерам к заббикс");


            return responseObj.Result;
        }


        [HttpGet]
        [Route("events")]
        public dynamic GetEventsZabbix()
        {
            _logger.LogInformation("Старт запроса по событиям к заббикс");

            Zabbix zabbix = new Zabbix(_conf.Login, _conf.Password, ZabbixApiUrl);
            zabbix.Login();
            Response responseObj = zabbix.GetObjectResponse(
                "event.get",
                new
                {
                    output = new[] { "extend" }
                }
            );
            zabbix.Logout();

            _logger.LogInformation("Конец запроса по событиям к заббикс");


            return responseObj.Result;
        }
    }
}
