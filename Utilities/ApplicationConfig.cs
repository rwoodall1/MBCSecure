using System.Configuration;

namespace Utilities
{
    public static class ApplicationConfig
    {
        private static readonly object LockObject = new object();
        private static volatile string _APIUsername;
        private static volatile string _APIPassword;


        public static string APIUsername
        {
            get
            {
                if (_APIUsername != null)
                {
                    return _APIUsername;
                }
                lock (LockObject)
                {
                    _APIUsername = ConfigurationManager.AppSettings["APIUsername"];
                }
                return _APIUsername;
            }
        }

        public static string APIPassword
        {
            get
            {
                if (_APIPassword != null)
                {
                    return _APIPassword;
                }
                lock (LockObject)
                {
                    _APIPassword = ConfigurationManager.AppSettings["APIPassword"];
                }
                return _APIPassword;
            }
        }
    }
}
