namespace TvShows.FunctionalTests
{
    using System.Threading.Tasks;
    using Xunit;

    public class TvShowScenarios
       : TvShowScenariosBase
    {
        [Fact]
        public async Task Get_get_all_catalogitems_and_response_ok_status_code()
        {
            using (Microsoft.AspNetCore.TestHost.TestServer server = CreateServer())
            {
                System.Net.Http.HttpResponseMessage response = await server.CreateClient()
                    .GetAsync(Get.Items());

                response.EnsureSuccessStatusCode();
            }
        }

        [Fact]
        public async Task Get_get_paginated_catalog_items_and_response_ok_status_code()
        {
            using (Microsoft.AspNetCore.TestHost.TestServer server = CreateServer())
            {
                const bool paginated = true;
                System.Net.Http.HttpResponseMessage response = await server.CreateClient()
                    .GetAsync(Get.Items(paginated));

                response.EnsureSuccessStatusCode();
            }
        }
    }
}
