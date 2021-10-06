using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace Massoterapia.Integration.Tests
{
    public class IntegrationBase
    {

        private HttpRequestMessage request;
        private HttpClient client ;

        public IntegrationBase (HttpRequestMessage _request, HttpClient _client)
        {
            this.request = _request;
            this.client = _client;
        }

        //private HttpResponseMessage PostResult(BlogInputModel blogInputModelToUpdate)
        public HttpResponseMessage PostResult(object blogInputModelToUpdate)
        {
            var json = JsonConvert.SerializeObject(blogInputModelToUpdate);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = client.PostAsync(request.RequestUri.AbsolutePath, request.Content).ConfigureAwait(false);
            var responseInfo = response.GetAwaiter().GetResult();
            return responseInfo;   
        }


        //private HttpResponseMessage PutResult(BlogInputModel blogInputModelToUpdate)
        public HttpResponseMessage PutResult(object blogInputModelToUpdate)
        {
            var json = JsonConvert.SerializeObject(blogInputModelToUpdate);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = client.PutAsync(request.RequestUri.AbsolutePath, request.Content).ConfigureAwait(false);
            var responseInfo = response.GetAwaiter().GetResult();
            return responseInfo;
        }        
    }
}