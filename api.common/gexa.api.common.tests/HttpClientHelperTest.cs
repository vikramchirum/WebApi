using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace api.common.tests
{
    [TestClass]
    public class HttpClientHelperTest
    {
        Api_Settings_Base Settings;

        [TestInitialize]
        public void TestInitialize()
        {
            Settings = new Api_Settings_Base();
            Settings.Api_Url = "http://localhost:3000/api/";
            Settings.Api_Url = "http://docsqa.gexaenergy.com/";
        }

        [TestMethod]
        public void TestHttpClientHelperPATCH()
        {
            var patchBody = new JObject();
            patchBody.Add("Test_Date", DateTime.Now);

            HttpClientHelper.InitializeHttpClient(Settings);
            var request = HttpClientHelper.GetHttpClientRequest<JObject>(Settings, "channels/5b1854839dff3621808446ba", null, patchBody);
            var ss = HttpClientHelper.Patch<JObject, JObject>(request).Result;
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestEFL()
        {
            var body = JObject.Parse(Resource1.EFL_Body);
            HttpClientHelper.InitializeHttpClient(Settings);
            var request = HttpClientHelper.GetHttpClientRequest<dynamic>(Settings, "Get/EFLdata", "lang=en", body);
            var res = HttpClientHelper.GetFileContent_POST<dynamic>(request).Result;
            Assert.AreEqual(res.StatusCode, System.Net.HttpStatusCode.OK);
        }
    }
}