﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NFluent;
using NUnit.Framework;
using WireMock.ResponseBuilders;

namespace WireMock.Net.Tests
{
    [TestFixture]
    public class ResponseTests
    {
        [Test]
        public async Task Response_ProvideResponse_Handlebars_UrlPathVerb()
        {
            // given
            string bodyAsString = "abc";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "POST", body, bodyAsString);

            var response = Response.Create()
                .WithBody("test {{request.url}} {{request.path}} {{request.method}}")
                .WithTransformer();
            
            // act
            var responseMessage = await response.ProvideResponse(request);

            // then
            Check.That(responseMessage.Body).Equals("test http://localhost/foo /foo post");
        }

        [Test]
        public async Task Response_ProvideResponse_Handlebars_Query()
        {
            // given
            string bodyAsString = "abc";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo?a=1&a=2&b=5"), "POST", body, bodyAsString);

            var response = Response.Create()
                .WithBody("test keya={{request.query.a}} idx={{request.query.a.[0]}} idx={{request.query.a.[1]}} keyb={{request.query.b}}")
                .WithTransformer();

            // act
            var responseMessage = await response.ProvideResponse(request);

            // then
            Check.That(responseMessage.Body).Equals("test keya=1 idx=1 idx=2 keyb=5");
        }

        [Test]
        public async Task Response_ProvideResponse_Handlebars_Headers()
        {
            // given
            string bodyAsString = "abc";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "POST", body, bodyAsString, new Dictionary<string, string> { { "Content-Type", "text/plain" } });

            var response = Response.Create().WithHeader("x", "{{request.headers.Content-Type}}").WithBody("test").WithTransformer();

            // act
            var responseMessage = await response.ProvideResponse(request);

            // then
            Check.That(responseMessage.Body).Equals("test");
            Check.That(responseMessage.Headers).Contains(new KeyValuePair<string,string>("x", "text/plain"));
        }
    }
}