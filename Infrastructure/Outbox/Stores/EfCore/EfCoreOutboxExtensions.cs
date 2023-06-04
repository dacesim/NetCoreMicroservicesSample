﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Infrastructure.Outbox.Stores.EfCore
{
    public static class EfCoreOutboxExtensions
    {
        public static IServiceCollection AddEfCoreOutboxStore(this IServiceCollection services, Action<DbContextOptionsBuilder> dbContextOptions)
        {
            services.AddDbContext<EfCoreOutboxContext>(dbContextOptions);
            services.AddSingleton<IOutboxStore, EfCoreOutboxStore>();

            return services;
        }
    }
}
