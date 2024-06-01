using TopUpManager.API.ExceptionHandler;
using TopUpManager.Common.Configs;
using TopUpManager.Common.Interfaces.DataAccess;
using TopUpManager.Common.Interfaces.Services;
using TopUpManager.DataAccess.Extensions;
using TopUpManager.DataAccess.Repositories;
using TopUpManager.Services.Services;

namespace TopUpManager.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IUserRepo, UserRepo>();
            builder.Services.AddScoped<IBeneficiaryService, BeneficiaryService>();
            builder.Services.AddScoped<IBeneficiaryRepo, BeneficiaryRepo>();
            builder.Services.AddScoped<ITopUpService, TopUpService>();
            builder.Services.AddScoped<ITopUpTransactionRepo, TopUpTransactionRepo>();
            builder.Services.AddScoped<IExternalFinanceService, ExternalFinanceService>();
            builder.Services.AddHttpClient<ExternalFinanceService>();

            //Add logging
            builder.Services.AddLogging(configure => configure.AddConsole());

            // Add global exception handler
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            //Add Db Context
            builder.Services.AddDBService(builder.Configuration.GetConnectionString("DbConnectionString"));

            //Add configurations
            builder.Services.Configure<Configurations>(builder.Configuration.GetSection("Configurations"));

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseExceptionHandler();

            app.MapControllers();

            app.Run();
        }
    }
}
