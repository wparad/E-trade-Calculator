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
		private static string ETRADE_BASE_URL = "https://us.etrade.com/";

		public static void Main (string[] args)
		{
			Can_Authenticate_With_OAuth();
		}

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
			var baseUrl = new Uri(ETRADE_BASE_URL);

			var client = new RestClient(baseUrl);
			client.Authenticator = OAuth1Authenticator.ForRequestToken(consumerKey, consumerSecret, "oob");
			var request = new RestRequest("oauth/request_token", Method.POST);

			request.AddHeader("Accept", "application/json");

			var response = client.Execute(request);

			if(response.StatusCode != HttpStatusCode.OK)
			{
				Console.WriteLine("{0} {1}: {2}", response.StatusCode, response.StatusDescription, response.Content);
				Environment.Exit(1);
			}
			var qs = HttpUtility.ParseQueryString(response.Content);
			var oauth_token = qs["oauth_token"];
			var oauth_token_secret = qs["oauth_token_secret"];

			request = new RestRequest("e/t/etws/authorize"); //oauth/authorize");
			request.AddParameter("token", oauth_token);
			request.AddParameter("key", consumerKey);
			var url = client.BuildUri(request).ToString();
			Process.Start(url);
			var verifier = "123456"; // <-- Breakpoint here (set verifier in debugger)
			request = new RestRequest("oauth/access_token", Method.POST);
			client.Authenticator = OAuth1Authenticator.ForAccessToken(
				consumerKey, consumerSecret, oauth_token, oauth_token_secret, verifier
			);
			response = client.Execute(request);
			if(response.StatusCode != HttpStatusCode.OK)
			{
				Console.WriteLine("{0} {1}: {2}", response.StatusCode, response.StatusDescription, response.Content);
			}
			qs = HttpUtility.ParseQueryString(response.Content);
			oauth_token = qs["oauth_token"];
			oauth_token_secret = qs["oauth_token_secret"];

			request = new RestRequest("e/t/etws/authorize");
			client.Authenticator = OAuth1Authenticator.ForProtectedResource(
				consumerKey, consumerSecret, oauth_token, oauth_token_secret
			);
			response = client.Execute(request);
			if(response.StatusCode != HttpStatusCode.OK)
			{
				Console.WriteLine("{0} {1}: {2}", response.StatusCode, response.StatusDescription, response.Content);
			}
		}
	}
}
