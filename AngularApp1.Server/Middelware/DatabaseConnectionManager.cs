namespace AngularApp1.Server.Middelware
{
    public class DatabaseConnectionManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DatabaseConnectionManager(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string ValidateConnectionString(string baseConnectionString, string databaseName)
        {
            string conexion = string.Empty;

            if (!string.IsNullOrEmpty(baseConnectionString))
            {
                conexion = baseConnectionString.Replace("{XXXXX}", databaseName);
            }
           
            return conexion;
        }

    }
}
