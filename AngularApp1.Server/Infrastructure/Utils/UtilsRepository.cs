using AngularApp1.Server.Domain.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AngularApp1.Server.Infrastructure.Utils
{
    public class UtilsRepository : IUtilsRepository
    {
        private readonly RegisterDBContext _dbContext;

        public UtilsRepository(RegisterDBContext context)
        {
            this._dbContext = context;
        }
        public long getSequence(string tableName)
        {
            var inputParam = new SqlParameter("@NombreTabla", SqlDbType.VarChar)
            {
                Value = tableName,

            };

            var outputParam = new SqlParameter
            {
                ParameterName = "@SiguienteValor",
                SqlDbType = SqlDbType.BigInt,
                Direction = ParameterDirection.Output
            };

            _dbContext.Database.ExecuteSqlRaw("EXEC nextValueTable @NombreTabla, @SiguienteValor OUT", inputParam, outputParam);

            var result = (long)outputParam.Value;

            return result;
        }
    }
}
