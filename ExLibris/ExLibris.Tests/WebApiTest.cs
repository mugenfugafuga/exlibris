using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace ExLibris.Tests
{
    [TestClass]
    public class WebApiTest
    {
        [TestMethod]
        public void GetTest()
        {
            using (var client = new HttpClient())
            {
                var response = client.GetAsync("https://httpbin.org/get").Result;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                    Assert.Fail(response.ToString());
                }

                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            }
        }

        [TestMethod]
        public void PostTest()
        {
            using (var client = new HttpClient())
            {
                var parameters = new Dictionary<string, string>()
                {
                    { "foo", "hoge" },
                    { "bar", "fuga1 fuga2" },
                    { "baz", "test" },
                };
                var content = new FormUrlEncodedContent(parameters);

                var response = client.PostAsync("https://httpbin.org/post", content).Result;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                    Assert.Fail(response.ToString());
                }

                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            }
        }

        [TestMethod]
        public void PostTest1()
        {
            using (var client = new HttpClient())
            {
                var content = new StringContent(@"{""v1"":""hoge"", ""v2"":123}");

                var response = client.PostAsync("https://httpbin.org/post", content).Result;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                    Assert.Fail(response.ToString());
                }

                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            }
        }

        [TestMethod]
        public void PostTest2()
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://httpbin.org/post");

                var response = client.SendAsync(request).Result;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                    Assert.Fail(response.ToString());
                }

                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            }
        }

        [TestMethod]
        public void BasicTest()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("aa");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes("aa:bb")));

                var response = client.GetAsync("https://httpbin.org/basic-auth/aa/bb").Result;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                    Assert.Fail(response.ToString());
                }

                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            }
        }

        [TestMethod]
        public void BearerTest()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "hogehgoe");

                var response = client.GetAsync("https://httpbin.org/bearer").Result;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                    Assert.Fail(response.ToString());
                }

                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            }
        }
    }
}
