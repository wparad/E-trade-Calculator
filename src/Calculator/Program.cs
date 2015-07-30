using Newtonsoft;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Authenticators.OAuth;
using RestSharp.Contrib;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace Calculator
{
	class Program
	{
		public static void Main (string[] args)
		{
			Can_Authenticate_With_OAuth();
		}

		private static string OAUTH_ENDPOINT = "https://etws.etrade.com/oauth";
		//private static string OAUTH_ENDPOINT = "https://etwssandbox.etrade.com/oauth";

		[JsonObject]
		public class Secret
		{
			[JsonProperty("oauth_consumer_key")]
			public string Key {get; set;}

			[JsonProperty("consumer_secret")]
			public string ConsumerSecret {get; set;}
		}
		public static void Can_Authenticate_With_OAuth()
		{
			var secret = JsonConvert.DeserializeObject<Secret>(File.ReadAllText(Path.Combine("/home/warren/Desktop/git_repos/e-trade-calculator/secrets", "secrets.json")));
			var consumerKey = secret.Key;
			var consumerSecret = secret.ConsumerSecret;

			var oauth_client = new RestClient(OAUTH_ENDPOINT);
			oauth_client.Authenticator = OAuth1Authenticator.ForRequestToken(consumerKey, consumerSecret, "oob");
			var request = new RestRequest("request_token", Method.POST);

			request.AddHeader("Accept", "application/json");

			var response = oauth_client.Execute(request);

			if(response.StatusCode != HttpStatusCode.OK)
			{
				Console.WriteLine("{0} {1}: {2}", response.StatusCode, response.StatusDescription, response.Content);
				Environment.Exit(1);
			}
			var qs = HttpUtility.ParseQueryString(response.Content);
			var oauth_token = qs["oauth_token"];
			var oauth_token_secret = qs["oauth_token_secret"];

			var authorize_client = new RestClient("https://us.etrade.com/");
			request = new RestRequest("e/t/etws/authorize");
			request.AddParameter("token", oauth_token);
			request.AddParameter("key", consumerKey);
			var url = authorize_client.BuildUri(request).ToString();
			Process.Start(url);
			var verifier = "123456"; // <-- Breakpoint here (set verifier in debugger)
			request = new RestRequest("access_token", Method.POST);
			oauth_client.Authenticator = OAuth1Authenticator.ForAccessToken(
				consumerKey, consumerSecret, oauth_token, oauth_token_secret, verifier
			);
			response = oauth_client.Execute(request);
			if(response.StatusCode != HttpStatusCode.OK)
			{
				Console.WriteLine("{0} {1}: {2}", response.StatusCode, response.StatusDescription, response.Content);
				return;
			}
			qs = HttpUtility.ParseQueryString(response.Content);
			oauth_token = qs["oauth_token"];
			oauth_token_secret = qs["oauth_token_secret"];

			var resource_client = new RestClient("https://etrade.com/market/rest/");
			//var resource_client = new RestClient("https://etwssandbox.etrade.com/market/sandbox/rest/");
			request = new RestRequest("quote/AAPL");
			resource_client.Authenticator = OAuth1Authenticator.ForProtectedResource(
				consumerKey, consumerSecret, oauth_token, oauth_token_secret
			);

			request.AddHeader("Accept", "application/json");
			response = resource_client.Execute(request);
			if(response.StatusCode != HttpStatusCode.OK)
			{
				Console.WriteLine("{0} {1}: {2}", response.StatusCode, response.StatusDescription, response.Content);
			}
		}
	}
}
