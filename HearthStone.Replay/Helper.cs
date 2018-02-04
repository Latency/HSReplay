using System;
using System.Collections.Specialized;
using System.Web;

namespace HSReplay
{
	internal static class Helper
	{
		public static string BuildUrl(string url, NameValueCollection parameters)
		{
			if(parameters == null || !parameters.HasKeys())
				return url;
			var uriBuilder = new UriBuilder(url);
			var query = HttpUtility.ParseQueryString(uriBuilder.Query);
			query.Add(parameters);
			uriBuilder.Query = query.ToString();
			return uriBuilder.Uri.ToString();
		}
	}
}
