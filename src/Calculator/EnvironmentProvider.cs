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

	public static class EnvironmentProvider
	{
		public static Environment Test = new Environment
		{
			OauthEndpoint = "http://requestb.in/11lxc8p1",
			AuthorizeEndpoint = "http://requestb.in/11lxc8p1",
			ResourceEndpoint = "http://requestb.in/11lxc8p1",
			Test = true
		};
		public static Environment Sandbox = new Environment
		{
			OauthEndpoint = "https://etwssandbox.etrade.com/oauth/",
			AuthorizeEndpoint = "https://us.etrade.com/",
			ResourceEndpoint = "https://etwssandbox.etrade.com/market/sandbox/rest/",
			Test = false
		};
		public static Environment Production = new Environment
		{
			OauthEndpoint = "hhttps://etws.etrade.com/oauth",
			AuthorizeEndpoint = "https://us.etrade.com/",
			ResourceEndpoint = "https://etrade.com/market/rest/",
			Test = false
		};
	}
	
}
