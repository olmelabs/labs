using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.APIGatewayEvents;

using DebuggingExample;
using Amazon.XRay.Recorder.Core;

namespace DebuggingExample.Tests
{
    public class FunctionTest
    {
        public FunctionTest()
        {
        }

        [Fact]
        public void TetGetMethod()
        {
            TestLambdaContext context;
            APIGatewayProxyRequest request;
            APIGatewayProxyResponse response;

            Functions functions = new Functions();

            request = new APIGatewayProxyRequest();
            context = new TestLambdaContext();

            AWSXRayRecorder.Instance.BeginSegment("TestSegment");
            response = functions.Get(request, context);
            AWSXRayRecorder.Instance.EndSegment();

            Assert.Equal(200, response.StatusCode);
        }
    }
}
