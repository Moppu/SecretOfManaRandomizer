using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.util
{
    public class UpdateCheck
    {
        private const string USER = "Moppu";
        private const string REPO = "SecretOfManaRandomizer";
        private static readonly string updateCheckUrl = $"https://api.github.com/repos/{USER}/{REPO}/releases/latest";
        public static readonly string updateDownloadUrl = $"https://github.com/{USER}/{REPO}/releases/latest";

        public static async Task<bool> checkForNewVersion()
        {
            // try to grab the latest release form GitHub and use the tag as latest version number
            try
            {
                string responseBody;
                // get the latest release
                using (HttpClient client = new HttpClient())
                {
                    //GitHub API requires a UserAgent to be set
                    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Blerf", "1.0"));

                    HttpResponseMessage response = await client.GetAsync(updateCheckUrl);
                    response.EnsureSuccessStatusCode();
                    responseBody = await response.Content.ReadAsStringAsync();
                }

                // parse the JSON response
                using (JsonDocument doc = JsonDocument.Parse(responseBody))
                {
                    JsonElement root = doc.RootElement;
                    string tagName = root.GetProperty("tag_name").GetString();
                    if (string.IsNullOrEmpty(tagName))
                    {
                        return false;
                    }

                    // throw the version string into Version class to compare
                    // note: the version string needs to have 2-4 numeric parts to work, just 1 won't parse (i.e. v2 would fail)
                    Version currentVersion = new Version(RomGenerator.VERSION_NUMBER);
                    Version latestVersion = new Version(tagName.Replace("v", ""));
                    return currentVersion < latestVersion;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}