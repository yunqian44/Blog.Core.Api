using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Demo.Core.Api.Core.Common;
using Demo.Core.Api.Model;
using Newtonsoft.Json;
using Demo.Core.Api.Data.LogHelper;
using Microsoft.AspNetCore.Hosting;

namespace Demo.Core.Api.WebApi.App_Start
{
    /// <summary>
    /// 全局异常错误日志
    /// </summary>
    public class ApiErrorHandleFilter : IFilterMetadata, IExceptionFilter
    {
        private readonly IHostingEnvironment _env;
        private readonly ILoggerHelper _loggerHelper;
        public ApiErrorHandleFilter(IHostingEnvironment env, ILoggerHelper loggerHelper)
        {
            _env = env;
            _loggerHelper = loggerHelper;
        }

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


            var json = new JsonErrorResponse();
            json.Message = context.Exception.Message;//错误信息
            if (_env.IsDevelopment())
            {
                json.DevelopmentMessage = context.Exception.StackTrace;//堆栈信息
            }
            context.Result = new InternalServerErrorObjectResult(json);

            //采用log4net 进行错误日志记录
            _loggerHelper.Error(json.Message, WriteLog(json.Message, context.Exception));
        }

        /// <summary>
        /// 自定义返回格式
        /// </summary>
        /// <param name="throwMsg"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public string WriteLog(string throwMsg, Exception ex)
        {
            return string.Format("【自定义错误】：{0} \r\n【异常类型】：{1} \r\n【异常信息】：{2} \r\n【堆栈调用】：{3}", new object[] { throwMsg,
                ex.GetType().Name, ex.Message, ex.StackTrace });
        }

    }
    public class InternalServerErrorObjectResult : ObjectResult
    {
        public InternalServerErrorObjectResult(object value) : base(value)
        {
            StatusCode = StatusCodes.Status500InternalServerError;
        }
    }

    //返回错误信息
    public class JsonErrorResponse
    {
        /// <summary>
        /// 生产环境的消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 开发环境的消息
        /// </summary>
        public string DevelopmentMessage { get; set; }
    }
}