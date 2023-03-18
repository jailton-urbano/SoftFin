using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace SoftFin.API.App_Start
{
    /// <summary>
    /// Configura o Swagger para aceitar Authorizacao por Authorization no header
    /// </summary>
    public class AddRequiredHeaderParameter : IOperationFilter
    {

        /// <summary>
        /// Aplicar configuração de authorização no header
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<IParameter>();
            operation.Parameters.Add(new NonBodyParameter
            {
                Name = "Authorization",
                In = "header",
                Description = "JWT Token",
                Required = true,
                Type = "string"
            });
        }
    }
}