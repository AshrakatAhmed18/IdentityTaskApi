using autheticationpart.Data.models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace autheticationpart.Data.context
{
    public class companyContext :IdentityDbContext<Employee>
    {
        public companyContext(DbContextOptions<companyContext> options)
            :base(options)
        {

        }
    }
}
