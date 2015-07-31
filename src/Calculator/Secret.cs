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

	[JsonObject]
	public class Secret
	{
		[JsonProperty("oauth_consumer_key")]
		public string Key {get; set;}

		[JsonProperty("consumer_secret")]
		public string ConsumerSecret {get; set;}

		[JsonProperty("username")]
		public string Username {get; set;}

		[JsonProperty("password")]
		public string Password {get; set;}
	}
}