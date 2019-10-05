using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;

namespace GitHubAPI
{
    class Program
    {
        static readonly HttpClient _client = new HttpClient();
        static string username = string.Empty;

        static void Main(string[] args)
        {
            Console.Write("*** .NET Core GitHub API using HttpClient ***\n\n"
                + "This app is intended to take a GitHub username, list data from "
                + "their public repositories and make the data available for download as an Excel file.\n\n"
                + "Enter a GitHub username: ");

            username = Console.ReadLine().ToLower();

            var userRepos = GetGitHubRepoDataByUsername(username).Result;

            foreach (var repo in userRepos)
            {
                Console.WriteLine(repo);
            }

            Console.Write("\n\nPress 'D' to download this data as an Excel file.\n"
                + "Press any other key to exit: ");

            if (string.Equals(Console.ReadLine(), "D", StringComparison.OrdinalIgnoreCase))
            {
                Download(userRepos);
            }
        }

        static async Task<List<RepositoryModel>> GetGitHubRepoDataByUsername(string username)
        {
            const string uri = "https://api.github.com/users/";

            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/vnd.github.43+json"));
            _client.DefaultRequestHeaders.Add("User-Agent", username);

            var serializer = new DataContractJsonSerializer(typeof(List<RepositoryModel>));
            var responseStrm = _client.GetStreamAsync($"{uri}{username}/repos");

            return serializer.ReadObject(await responseStrm) as List<RepositoryModel>;
        }

        static void Download(List<RepositoryModel> repos)
        {
            byte[] fileData;
            var path = @"D:\development\" + username + "_RepoData.xlsx";

            using (var pkg = new ExcelPackage())
            {
                var wrksht = pkg.Workbook.Worksheets.Add("RepoData");

                for (int i = 1; i < repos.Count; i++)
                {
                    if (repos != null)
                    {
                        wrksht.Cells[i, 1].Value = repos[i].Name;
                        wrksht.Cells[i, 2].Value = repos[i].HtmlUrl;
                        wrksht.Cells[i, 3].Value = repos[i].Description;
                        wrksht.Cells[i, 4].Value = repos[i].DefaultBranch;
                        wrksht.Cells[i, 5].Value = repos[i].LastPushDate.ToLongDateString();
                    }
                }
                fileData = pkg.GetAsByteArray();
            }
            File.WriteAllBytes(path, fileData);
        }
    }
}
