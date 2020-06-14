using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using HtmlAgilityPack;

namespace quest_planner
{
  class HTTPRequester
  {
    public struct HTMLResponse
    {
      public HtmlDocument Document;
      public bool Succeeded;
    }

    static readonly HttpClient http = new HttpClient();

    public static async Task<HTMLResponse> GetURL(string url)
    {
      try
      {
        HttpResponseMessage response = await http.GetAsync(url);
        response.EnsureSuccessStatusCode();

        string contents = await response.Content.ReadAsStringAsync();
        
        HTMLResponse hr;
        hr.Document = new HtmlDocument();
        hr.Document.LoadHtml(contents);

        hr.Succeeded = true;

        return hr;
      }
      catch (HttpRequestException e)
      {
        Console.Error.WriteLine("Invalid HTTP response from URL '{0}';\nMessage: {1} ", url, e.Message);

        HTMLResponse hr;
        hr.Document = null;
        hr.Succeeded = false;

        return hr;
      }
    }

    public static HTMLResponse GetURLBlocking(string url)
    {
      Task<HTMLResponse> hr = GetURL(url);
      hr.Wait(15000);

      return hr.Result;
    }
  }
}
