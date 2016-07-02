using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace RemoteControl
{
    public partial class CommandsHandler
    {
        private Windows.Networking.HostName _host_ip;
        private AjaxHelper _ajax_client;
        public CommandsHandler()
        {

            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            string ip_string = "";
            try
            {
                ip_string = localSettings.Values["ip_address"].ToString();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("NO ITEM: "+e.ToString());
                ip_string = "172.16.0.112";
            }
            System.Diagnostics.Debug.WriteLine("IP: "+ip_string.ToString());
            _host_ip = new Windows.Networking.HostName(ip_string);
            _ajax_client = new AjaxHelper(ip_string);
        }

        public void set_ip(string ip)
        {
            bool ok = false;
            try
            {
                _host_ip = new Windows.Networking.HostName(ip);
                _ajax_client = new AjaxHelper(ip);
                ok = true;
            }
            catch (ArgumentException e)
            {
                //System.Diagnostics.Debug.WriteLine(e.ToString());
            }

            if (ok)
            {
                Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                localSettings.Values["ip_address"] = ip;
                System.Diagnostics.Debug.WriteLine("IP saved: " + ip);
            }

        }
        public void play()
        { _ajax_client.SendGet("start"); }

        public void pause()
        { _ajax_client.SendGet("pause"); }

        public void next()
        { _ajax_client.SendGet("next"); }

        public void previous()
        { _ajax_client.SendGet("prev"); }
        public void clear()
        { _ajax_client.SendGet("clear"); }

        public void search_and_play(string song_search_string)
        {
            string s = String.Join("+", song_search_string.Split());
            _ajax_client.SendGet("play/" + s);
        }

        public async Task<JsonArray> get_playlist()
        {
            JsonObject root = await _ajax_client.GetJSON("list");
            JsonArray arr = root["list"].GetArray();
            return arr;
        }

        public void play_radio(string station_number)
        { _ajax_client.SendGet("radio/" + station_number); }

        public void play_id(string id)
        { _ajax_client.SendGet("playid/" + id); }

        public void volume_down()
        { _ajax_client.SendGet("volume/-"); }
        public void volume_mute()
        { _ajax_client.SendGet("volume/mute"); }
        public void volume_up()
        { _ajax_client.SendGet("volume/+"); }

        public void send_key(string keys)
        {
            _ajax_client.SendGet("keys/" + keys);
        }
        private async Task<string> send_and_receive(string command, int port = 6600, bool endline = true)
        {
            if (endline)
            {
                command += Environment.NewLine;
            }

            System.Text.StringBuilder strBuilder = new StringBuilder();
            using (StreamSocket clientSocket = new StreamSocket())
            {
                await clientSocket.ConnectAsync(_host_ip, port.ToString());
                using (DataWriter writer = new DataWriter(clientSocket.OutputStream))
                {
                    writer.WriteString(command);
                    await writer.StoreAsync();
                    writer.DetachStream();
                }
                using (DataReader reader = new DataReader(clientSocket.InputStream))
                {
                    reader.InputStreamOptions = InputStreamOptions.Partial;
                    await reader.LoadAsync(8192);
                    while (reader.UnconsumedBufferLength > 0)
                    {
                        string s = reader.ReadString(reader.UnconsumedBufferLength);

                        strBuilder.Append(s);
                        await reader.LoadAsync(8192);
                    }
                    reader.DetachStream();
                }
            }
            return (strBuilder.ToString());
        }

        private async void send_and_forget(string command, int port = 6600, bool endline = true)
        {
            if (endline)
            {
                command += Environment.NewLine;
            }

            using (StreamSocket clientSocket = new StreamSocket())
            {
                await clientSocket.ConnectAsync(_host_ip, port.ToString());
                using (DataWriter writer = new DataWriter(clientSocket.OutputStream))
                {
                    writer.WriteString(command);
                    await writer.StoreAsync();
                    writer.DetachStream();
                }

            }
        }
    }
}
