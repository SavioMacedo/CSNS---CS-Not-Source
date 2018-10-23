using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json; 

namespace Api
{
    public class ApiTest
    {
        public long UserId {get;set;}
        public long Id {get;set;}
        public string Title {get;set;}
        public bool Completed {get;set;}
    }

    public class ApiExecute
    {
        public static async Task<long> GetID() {
            var client = new HttpClient();
            ApiTest apiTest = new ApiTest(){ Id = 3};

            var response = await client.GetAsync("https://jsonplaceholder.typicode.com/todos/1");

            if(response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                apiTest = JsonConvert.DeserializeObject<ApiTest>(data);
            }

            return apiTest.Id;
        }
    }
}