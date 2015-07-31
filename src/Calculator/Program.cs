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
			
		public static void Can_Authenticate_With_OAuth()
		{
			var secret = JsonConvert.DeserializeObject<Secret>(File.ReadAllText(Path.Combine("/home/warren/Desktop/git_repos/e-trade-calculator/secrets", "secrets.json")));
			var consumerKey = secret.Key;
			var consumerSecret = secret.ConsumerSecret;

			var oauth_client = new RestClient(EnvironmentProvider.Sandbox.OauthEndpoint);
			oauth_client.Authenticator = OAuth1Authenticator.ForRequestToken(consumerKey, consumerSecret, "oob");
			var request = new RestRequest(false ? "" : "request_token", Method.POST);

			request.AddHeader("Accept", "application/json");
			var response = oauth_client.Execute(request);

			if(response.StatusCode != HttpStatusCode.OK)
			{
				Console.WriteLine("{0} {1}: {2}", response.StatusCode, response.StatusDescription, response.Content);
				System.Environment.Exit(1);
			}
			var qs = HttpUtility.ParseQueryString(response.Content);
			var oauth_token = qs["oauth_token"];
			var oauth_token_secret = qs["oauth_token_secret"];

			var authorize_client = new RestClient(EnvironmentProvider.Sandbox.AuthorizeEndpoint);
			request = new RestRequest(false ? "" : "e/t/etws/authorize");
			//

			request.AddParameter("token", oauth_token);
			request.AddParameter("key", consumerKey);
			var url = authorize_client.BuildUri(request).ToString();
			Process.Start(url);

			response = authorize_client.Execute(request);
			if(response.StatusCode != HttpStatusCode.OK)
			{
				Console.WriteLine("{0} {1}: {2}", response.StatusCode, response.StatusDescription, response.Content);
				return;
			}

			request = new RestRequest(string.Format("e/t/etws/login.fcc?USER={0}&PASSWORD={1}&Target={2}", secret.Username, secret.Password,
				System.Uri.EscapeDataString(string.Format("/e/t/etws/authorize?token={0}&key={1}", oauth_token, consumerKey))));
			//var url = authorize_client.BuildUri(request).ToString();
			//Process.Start(url);

			response = authorize_client.Execute(request);
			if(response.StatusCode != HttpStatusCode.OK)
			{
				Console.WriteLine("{0} {1}: {2}", response.StatusCode, response.StatusDescription, response.Content);
				return;
			}

			request = new RestRequest(false ? "" : "e/t/etws/TradingAPICustomerInfo");
			response = authorize_client.Execute(request);
			if(response.StatusCode != HttpStatusCode.OK)
			{
				Console.WriteLine("{0} {1}: {2}", response.StatusCode, response.StatusDescription, response.Content);
				return;
			}

			var verifier = "C4H2T"; // <-- Breakpoint here (set verifier in debugger)
			request = new RestRequest(true ? "" : "access_token", Method.POST);
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

			var resource_client = new RestClient(EnvironmentProvider.Test.ResourceEndpoint);
			request = new RestRequest(true ? "" : "quote/AAPL");
			
			resource_client.Authenticator = OAuth1Authenticator.ForProtectedResource(
				consumerKey, consumerSecret, oauth_token, oauth_token_secret
			);

			request.AddHeader("Accept", "application/json");
			response = resource_client.Execute(request);
			if(response.StatusCode != HttpStatusCode.OK)
			{
				Console.WriteLine("{0} {1}: {2}", response.StatusCode, response.StatusDescription, response.Content);
			}

			request = new RestRequest(true ? "" : "revoke_access_token", Method.POST);
			oauth_client.Authenticator = OAuth1Authenticator.ForAccessToken(
				consumerKey, consumerSecret, oauth_token, oauth_token_secret
			);
			response = oauth_client.Execute(request);
			if(response.StatusCode != HttpStatusCode.OK)
			{
				Console.WriteLine("{0} {1}: {2}", response.StatusCode, response.StatusDescription, response.Content);
				return;
			}
			Console.WriteLine("Done");
		}
	}
}
