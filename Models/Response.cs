using System.Dynamic;

namespace ZabbixManager.Models
{
    public class Response
    {
        public string Jsonrpc { get; set; }
        
        public dynamic Result = new ExpandoObject();
        public int Id { get; set; }
    }
}
