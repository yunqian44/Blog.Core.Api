using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Demo.Core.Api.Core.Common;
using Demo.Core.Api.Model;
using Newtonsoft.Json;
using Demo.Core.Api.Data.LogHelper;

namespace Demo.Core.Api.WebApi.App_Start
{
    public class ApiErrorHandleFilter : IFilterMetadata, IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            // if (context.ExceptionHandled == false)
            // {
            //     if (context.Exception is RequestException)
            //     {
            //         var ex = context.Exception as RequestException;
            //         var errorMessage = ex.ErrorMessage;
            //         var result = new HttpResult(ex.ErrorStatus, errorMessage);
            //         context.Result = new ContentResult
            //         {

            //             Content = JsonConvert.SerializeObject(result),
            //             StatusCode = StatusCodes.Status200OK,
            //             ContentType = "text/html;charset=utf-8"

            //         };
            //     }
            //     LogLock.OutSql2Log("ApiErrorHandleFilter", new string[] { context.Exception.Message });
            // }
            // context.ExceptionHandled = true; //异常已处理了
        }
    }
}