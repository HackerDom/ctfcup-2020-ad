using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using QueenOfHearts.CoreLibrary.Models;
using QueenOfHearts.CoreLibrary.Serialization;
using QueenOfHearts.ExecutionService.Configuration;

namespace QueenOfHearts.ExecutionService
{
    internal interface IVictimsStorageClient
    {
        Task<IEnumerable<string>> GetVictims();
        Task<Victim> GetVictim(string victimName);
        Task PutVictim(Victim victim);
    }

    internal class VictimsStorageClient : IVictimsStorageClient
    {
        private readonly HttpClient _httpClient;
        private readonly ISettingsProvider _settingsProvider;

        public VictimsStorageClient(ISettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;
            _httpClient = new HttpClient();
        }

        public async Task<IEnumerable<string>> GetVictims()
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, GetUri("victims"));

            var result = await _httpClient.SendAsync(httpRequestMessage);
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadFromJsonAsync<string[]>();
        }

        public async Task<Victim> GetVictim(string victimName)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get,
                GetUri("victim"))
            {
                Content = new StringContent(new GetVictimRequest
                {
                    VictimName = victimName
                }.ToJson())
            };

            var result = await _httpClient.SendAsync(httpRequestMessage);
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadFromJsonAsync<Victim>();
        }

        public async Task PutVictim(Victim victim)
        {
            var result =
                await _httpClient.PutAsync(GetUri("victim"), new StringContent(victim.ToJson()));
            result.EnsureSuccessStatusCode();
        }

        private string GetUri(string victims)
        {
            var builder = new UriBuilder
            {
                Host = _settingsProvider.ApplicationSettings.VictimStorageAddress,
                Path = victims,
                Port = _settingsProvider.ApplicationSettings.VictimStoragePort
            };
            var requestUri = builder.ToString();
            return requestUri;
        }

        private class GetVictimRequest
        {
            public string VictimName { get; set; }
        }
    }
}