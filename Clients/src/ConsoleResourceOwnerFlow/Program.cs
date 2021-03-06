﻿using Clients;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConsoleResourceOwnerFlow
{
    public class Program
    {
        public static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();

        static async Task MainAsync()
        {
            Console.Title = "Console ResourceOwner Flow";

            var response = await RequestTokenAsync();
            response.Show();

            Console.ReadLine();
            await CallServiceAsync(response.AccessToken);
        }

        static async Task<TokenResponse> RequestTokenAsync()
        {
            var client = new TokenClient(
                Constants.TokenEndpoint,
                "roclient",
                "secret");

            // idsrv supports additional non-standard parameters 
            // that get passed through to the user service
            var optional = new
            {
                acr_values = "tenant:custom_account_store1 foo bar quux"
            };

            return await client.RequestResourceOwnerPasswordAsync("bob", "bob", "api1 api2.read_only", optional);
        }

        static async Task CallServiceAsync(string token)
        {
            var baseAddress = Constants.AspNetWebApiSampleApi;

            var client = new HttpClient
            {
                BaseAddress = new Uri(baseAddress)
            };

            client.SetBearerToken(token);
            var response = await client.GetStringAsync("identity");

            "\n\nService claims:".ConsoleGreen();
            Console.WriteLine(JArray.Parse(response));
        }
    }
}