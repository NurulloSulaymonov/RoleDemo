using System.Net;

namespace Domain.Wrapper;

public class Response<T>
{
    public int StatusCode { get; set; }
    public T Data { get; set; }
    public string Message { get; set; }
    public Response(HttpStatusCode statusCode, string message)
    {
        StatusCode = (int)statusCode;
        Message = message;
    }
       public Response(T data)
       {
           StatusCode = 200;
            Data = data;
       }
   
}