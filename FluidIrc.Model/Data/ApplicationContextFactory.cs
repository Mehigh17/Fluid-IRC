namespace FluidIrc.Model.Data
{
    public class ApplicationContextFactory : IApplicationContextFactory
    {
        public IApplicationContext CreateApplicationContext()
        {
            return new ApplicationDbContext();
        }
    }
}
