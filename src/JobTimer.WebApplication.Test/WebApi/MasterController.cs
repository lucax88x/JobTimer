using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using NUnit.Framework;

namespace JobTimer.WebApplication.Test.WebApi
{
    public class MasterControllerTest
    {
        private HttpClient _client;
        private IDisposable _app;
        private const string BaseAddress = "http://localhost:9000/";

        //move to setupfixture in order to have one httpclient for all api controller calls
        [SetUp]
        public void Setup()
        {
            _app = WebApp.Start<Startup>(url: BaseAddress);
            _client = new HttpClient();
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _app.Dispose();
        }

        [Test]
        public void get_bundles()
        {            
            var response = _client.GetAsync(BaseAddress + "api/master/getbundles").Result;

            Console.WriteLine(response);
            Console.WriteLine(response.Content.ReadAsStringAsync().Result);

            Console.ReadLine();
        }
    }
}
