using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OrcShackApi.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<OrcShackApiContext>
    {
        public OrcShackApiContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<OrcShackApiContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=Test;User Id=Abhijit;Password=Winter@2024;Trusted_Connection=True;MultipleActiveResultSets=true");

            return new OrcShackApiContext(optionsBuilder.Options);
        }
    }
}
