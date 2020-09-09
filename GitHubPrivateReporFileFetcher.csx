using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.IO;
using System.Text;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info($"C# HTTP trigger proxy function processed a request. RequestUri={req.RequestUri}");

    // parse query parameter
    string GitHubUri = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "githuburi", true) == 0)
        .Value;
    string GitHubAccessToken = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "githubaccesstoken", true) == 0)
        .Value;
    log.Info($"GitHubPrivateRepoFileFecher function is trying to get file content from {GitHubUri}");

    Encoding outputencoding = Encoding.GetEncoding("ASCII");
    var ProxyResponse = new HttpResponseMessage();
    HttpStatusCode statuscode = new HttpStatusCode();

    if (GitHubUri == null)
    {
        statuscode = HttpStatusCode.BadRequest;
        StringContent errorcontent =new StringContent("Please pass the GitHub raw file content URI (raw.githubusercontent.com) in the request URI string", outputencoding);
        ProxyResponse = req.CreateResponse(statuscode, errorcontent);
        ProxyResponse.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
        ProxyResponse.Content = errorcontent;
    } else if (GitHubAccessToken == null) {
        statuscode = HttpStatusCode.BadRequest;
        StringContent errorcontent =new StringContent("Please pass the GitHub personal access token in the request URI string", outputencoding);
        ProxyResponse = req.CreateResponse(statuscode, errorcontent);
        ProxyResponse.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
        ProxyResponse.Content = errorcontent;
    } else {
        string strAuthHeader = "token " + GitHubAccessToken;
        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3.raw");
        client.DefaultRequestHeaders.Add("Authorization", strAuthHeader);
        HttpResponseMessage response = await client.GetAsync(GitHubUri);
        ProxyResponse = response;
    }
    return ProxyResponse;
}