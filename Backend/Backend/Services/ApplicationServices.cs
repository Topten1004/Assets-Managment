using Backend.Business.Services;
using Backend.Data.Repositories;

namespace Backend.API.Services
{
    public class ApplicationServices
    {
        public static void AddServices(IServiceCollection services)
        {
            services.AddTransient<IGenericService, GenericService>();

            services.AddTransient<IGenericRepository, GenericRepository>();
        }
    }
}
