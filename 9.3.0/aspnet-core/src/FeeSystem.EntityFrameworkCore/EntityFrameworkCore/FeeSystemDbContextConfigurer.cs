using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace FeeSystem.EntityFrameworkCore
{
    public static class FeeSystemDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<FeeSystemDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<FeeSystemDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}
