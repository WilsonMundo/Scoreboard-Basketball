using AngularApp1.Server.Application.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AngularApp1.Server
{
    public class ResponseService
    {
        public ActionResult CreateResponse<T>(ResultAPI<T> data)
        {

            switch (data.Code)
            {
                case StatusHttpResponse.OK:
                    if (data == null || (data is ResultAPI<object?> resultApi && resultApi.Result == null))
                        return new OkResult();
                    else
                        return new OkObjectResult(data.Result);

                case StatusHttpResponse.Created:
                    if (data == null || (data is ResultAPI<object?> apiresult && apiresult.Result == null))
                        throw new ArgumentException("null response in code 201");
                    else
                        return new ObjectResult(data.Result)
                        {
                            StatusCode = (int)HttpStatusCode.Created
                        };
                case StatusHttpResponse.NoContent:
                    return new NoContentResult();
                case StatusHttpResponse.BadRequest:
                    return new BadRequestObjectResult(data.Message);
                case StatusHttpResponse.InternalServerError:
                    return new ObjectResult(data.Message)
                    {
                        StatusCode = (int)HttpStatusCode.InternalServerError
                    };
                case StatusHttpResponse.Unauthorized:
                    return new UnauthorizedObjectResult(data.Message);
                case StatusHttpResponse.NotFound:
                    return new NotFoundObjectResult(data.Message);
                case StatusHttpResponse.Conflict:
                    return new ConflictObjectResult(data.Message);
                default:
                    return new ObjectResult(data.Message)
                    {
                        StatusCode = (int)HttpStatusCode.InternalServerError
                    };


            }

        }
    }
}
