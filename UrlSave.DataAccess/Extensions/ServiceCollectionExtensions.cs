using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlSave.Application.Interfaces;
using UrlSave.DataAccess.Contexts;

namespace UrlSave.DataAccess.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLinkDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ILinkContext, LinkContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }
}
