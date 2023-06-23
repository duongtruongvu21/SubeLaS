namespace SubelaServer
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            log4net.Config.XmlConfigurator.Configure(new FileInfo("log4net.config"));

            World world = new World();
            world.Init();

            Console.WriteLine(123);

            Application.Run(new SubelaServer());
        }
    }
}