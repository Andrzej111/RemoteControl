using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace RemoteControl
{
    class AjaxHelper
    {
        private HttpClient client;
        private Windows.Networking.HostName _host_base;

        public AjaxHelper(string host)
        {
            client = new HttpClient();
            _host_base = new Windows.Networking.HostName(host);
        }

        public async Task<JsonObject> GetJSON(string uri)
        {
            HttpResponseMessage response = await client.GetAsync("http://" + _host_base.ToString() + '/' + uri);
            var jsonString = await response.Content.ReadAsStringAsync();
            JsonObject root = JsonValue.Parse(jsonString).GetObject();
            return root;
        }

        public void SendGet(string uri)
        {
            client.GetAsync("http://" + _host_base.ToString() + '/' + uri);
        }

    }
}
