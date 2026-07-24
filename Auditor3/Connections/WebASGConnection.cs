/*
 * Auditor3 :: Connections :: WebASGConnection
 * 
 * This class defines the web client connection to the ASG REST API.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;

using static Auditor3.Locations.Locations;

namespace Auditor3.Connections {
    internal static class WebASGConnection {
        private const string BASE_CORP = "https://c3ha.avaya.com/conninfo/rest/asg/CorruptionAudit";
        private const string BASE_VDI = "https://c3ha.PCI.avaya.com/conninfo/rest/asg/CorruptionAudit";

        // Method for getting an ASG response
        internal static string GetResponse(string details, string username = "init") {
            try {
                var challenge = Regex.Match(details, @"(Challenge: )([-0-9]+)").Groups[2].Value;
                var product = Regex.Match(details, @"(Product ID: )([0-9a-zA-Z]+)").Groups[2].Value;

                var url = $"{CURRENT().WebASGURL}/{product}/{username}/{challenge}/0";
                var credsArray = Encoding.ASCII.GetBytes("CorruptionAudit:Avaya123");

                var client = new HttpClient();
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(credsArray));

                var response = client.GetAsync("").Result;
                var asg = "";

                if (response.IsSuccessStatusCode) {
                    asg = response.Content.ReadAsStringAsync().Result;
                } 
                else {
                    Globals.GUI.AddStatus($"WEBASG FAILED - Code: {(int)response.StatusCode}  Reason: {response.ReasonPhrase}");
                }

                client.Dispose();
                return asg;
            } catch (Exception error) {
                Globals.GUI.Error("WebASG exception", error);
                return "";
            }
        }
    }
}
