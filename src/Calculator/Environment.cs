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
	public class Environment
	{
		public string OauthEndpoint {get; set;}
		public string AuthorizeEndpoint {get; set;}
		public string ResourceEndpoint {get; set;}
		public bool Test {get; set;}
	}
	
}
