using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace BOTDemoApplication.Front.Utilities
{
    public static class LoggingUtilities
    {
        public static void LogInvocationInformation<T>(ILogger<T> _logger, HttpContext httpContext, string parameterJson, string actionResult, [CallerMemberName] string callingMethod = "", DateTime? startInvocationTime = null)
        {
            string applicationName = (httpContext == null || httpContext.Items["ApplicationName"] == null) ? "Unknown" : httpContext.Items["ApplicationName"].ToString();
            string sourceIP = GetRequestIP(httpContext);
            string httpMethod = httpContext.Request.Method;
            string invokeURL = GetFullPathWithQueryString(httpContext);

            using (LogContext.PushProperty("Method", callingMethod))
            {
                if (startInvocationTime != null)
                {
                    DateTime endInvocationTime = DateTime.Now;
                    double elapseTimeInMilliseconds = (endInvocationTime - startInvocationTime).Value.TotalMilliseconds;
                    _logger.LogInformation("{Application} ({SourceIP}) invoke {HTTPMethod} {URL} with parameter {Parameter} in {ElapseTime} milliseconds, The result is {ActionResult}", applicationName, sourceIP, httpMethod, invokeURL, parameterJson, elapseTimeInMilliseconds, actionResult);
                }
                else
                {
                    _logger.LogInformation("{Application} ({SourceIP}) invoke {HTTPMethod} {URL} with parameter {Parameter}, The result is {ActionResult}", applicationName, sourceIP, httpMethod, invokeURL, parameterJson, actionResult);
                }
            }
        }


        public static void LogInvocationError<T>(ILogger<T> _logger, HttpContext httpContext, string parameterJson, string actionResult, Exception exception, [CallerMemberName] string callingMethod = "", DateTime? startInvocationTime = null)
        {
            string applicationName = (httpContext == null || httpContext.Items["ApplicationName"] == null) ? "Unknown" : httpContext.Items["ApplicationName"].ToString();
            string sourceIP = GetRequestIP(httpContext);
            string httpMethod = httpContext.Request.Method;
            string invokeURL = GetFullPathWithQueryString(httpContext);

            using (LogContext.PushProperty("Method", callingMethod))
            {
                if (startInvocationTime != null)
                {
                    DateTime endInvocationTime = DateTime.Now;
                    double elapseTimeInMilliseconds = (endInvocationTime - startInvocationTime).Value.TotalMilliseconds;
                    _logger.LogError(exception, "{Application} ({SourceIP}) invoke {HTTPMethod} {URL} with parameter {Parameter} in {ElapseTime} milliseconds, The result is {ActionResult}", applicationName, sourceIP, httpMethod, invokeURL, parameterJson, elapseTimeInMilliseconds, actionResult);
                }
                else
                {
                    _logger.LogError(exception, "{Application} ({SourceIP}) invoke {HTTPMethod} {URL} with parameter {Parameter}, The result is {ActionResult}", applicationName, sourceIP, httpMethod, invokeURL, parameterJson, actionResult);
                }
            }
        }


        public static void LogInvocationWarning<T>(ILogger<T> _logger, HttpContext httpContext, string parameterJson, string actionResult, Exception exception, [CallerMemberName] string callingMethod = "", DateTime? startInvocationTime = null)
        {
            string applicationName = (httpContext == null || httpContext.Items["ApplicationName"] == null) ? "Unknown" : httpContext.Items["ApplicationName"].ToString();
            string sourceIP = GetRequestIP(httpContext);
            string httpMethod = httpContext.Request.Method;
            string invokeURL = GetFullPathWithQueryString(httpContext);

            using (LogContext.PushProperty("Method", callingMethod))
            {
                if (startInvocationTime != null)
                {
                    DateTime endInvocationTime = DateTime.Now;
                    double elapseTimeInMilliseconds = (endInvocationTime - startInvocationTime).Value.TotalMilliseconds;
                    //_logger.LogWarning(exception.ToString(), "{Application} ({SourceIP}) invoke {HTTPMethod} {URL} with parameter {Parameter} in {ElapseTime} milliseconds, The result is {ActionResult}", applicationName, sourceIP, httpMethod, invokeURL, parameterJson, elapseTimeInMilliseconds, actionResult);
                    _logger.LogWarning(exception, "{Application} ({SourceIP}) invoke {HTTPMethod} {URL} with parameter {Parameter} in {ElapseTime} milliseconds, The result is {ActionResult}", applicationName, sourceIP, httpMethod, invokeURL, parameterJson, elapseTimeInMilliseconds, actionResult);
                }
                else
                {
                    _logger.LogWarning(exception, "{Application} ({SourceIP}) invoke {HTTPMethod} {URL} with parameter {Parameter}, The result is {ActionResult}", applicationName, sourceIP, httpMethod, invokeURL, parameterJson, actionResult);
                }
            }
        }

        public static void LogInvocationWarning<T>(ILogger<T> _logger, HttpContext httpContext, string parameterJson, string actionResult, object reason, [CallerMemberName] string callingMethod = "", DateTime? startInvocationTime = null)
        {
            string applicationName = (httpContext == null || httpContext.Items["ApplicationName"] == null) ? "Unknown" : httpContext.Items["ApplicationName"].ToString();
            string sourceIP = GetRequestIP(httpContext);
            string httpMethod = httpContext.Request.Method;
            string invokeURL = GetFullPathWithQueryString(httpContext);

            using (LogContext.PushProperty("Method", callingMethod))
            {
                if (startInvocationTime != null)
                {
                    DateTime endInvocationTime = DateTime.Now;
                    double elapseTimeInMilliseconds = (endInvocationTime - startInvocationTime).Value.TotalMilliseconds;
                    _logger.LogWarning("{Application} ({SourceIP}) invoke {HTTPMethod} {URL} with parameter {Parameter} in {ElapseTime} milliseconds, The result is {ActionResult} with reason: {Reason}", applicationName, sourceIP, httpMethod, invokeURL, parameterJson, elapseTimeInMilliseconds, actionResult, JsonConvert.SerializeObject(reason));
                }
                else
                {
                    _logger.LogWarning("{Application} ({SourceIP}) invoke {HTTPMethod} {URL} with parameter {Parameter}, The result is {ActionResult} with reason: {Reason}", applicationName, sourceIP, httpMethod, invokeURL, parameterJson, actionResult, JsonConvert.SerializeObject(reason));
                }
            }
        }

        public static void LogTrace<T>(ILogger<T> _logger, HttpContext httpContext, string parameterJson, [CallerMemberName] string callingMethod = "")
        {
            string applicationName = (httpContext == null || httpContext.Items["ApplicationName"] == null) ? "Unknown" : httpContext.Items["ApplicationName"].ToString();
            string sourceIP = GetRequestIP(httpContext);
            //string httpMethod = httpContext.Request.Method;
            //string invokeURL = GetFullPathWithQueryString(httpContext);

            using (LogContext.PushProperty("Method", callingMethod))
            {
                _logger.LogTrace("has been invoked by {Application} ({SourceIP}) with parameter {Parameter}", applicationName, sourceIP, parameterJson);
            }
        }

        #region Get Client IP Address

        //https://stackoverflow.com/questions/28664686/how-do-i-get-client-ip-address-in-asp-net-core
        private static string GetRequestIP(HttpContext httpContext, bool tryUseXForwardHeader = true)
        {
            string ip = null;


            if (tryUseXForwardHeader)
            {
                ip = GetHeaderValueAs<string>(httpContext, "X-Forwarded-For").SplitCsv().FirstOrDefault();
            }

            // RemoteIpAddress is always null in DNX RC1 Update1 (bug).
            if (ip.IsNullOrWhitespace() && httpContext?.Connection?.RemoteIpAddress != null)
            {
                ip = httpContext.Connection.RemoteIpAddress.ToString();
            }

            if (ip.IsNullOrWhitespace())
            {
                ip = GetHeaderValueAs<string>(httpContext, "REMOTE_ADDR");
            }

            // _httpContextAccessor.HttpContext?.Request?.Host this is the local host.

            if (ip.IsNullOrWhitespace())
            {
                throw new Exception("ไม่สามารถระบุ IP ของผู้ใช้งานได้");
            }

            return ip;
        }

        private static T GetHeaderValueAs<T>(HttpContext httpContext, string headerName)
        {
            StringValues values;

            if (httpContext?.Request?.Headers?.TryGetValue(headerName, out values) ?? false)
            {
                string rawValues = values.ToString();   // writes out as Csv when there are multiple.

                if (!rawValues.IsNullOrWhitespace())
                    return (T)Convert.ChangeType(values.ToString(), typeof(T));
            }
            return default(T);
        }

        private static List<string> SplitCsv(this string csvList, bool nullOrWhitespaceInputReturnsNull = false)
        {
            if (string.IsNullOrWhiteSpace(csvList))
                return nullOrWhitespaceInputReturnsNull ? null : new List<string>();

            return csvList
                .TrimEnd(',')
                .Split(',')
                .AsEnumerable<string>()
                .Select(s => s.Trim())
                .ToList();
        }

        private static bool IsNullOrWhitespace(this string s)
        {
            return String.IsNullOrWhiteSpace(s);
        }

        #endregion

        #region Get Full Path with Query String

        private static string GetFullPathWithQueryString(HttpContext httpContext)
        {
            string Host = "";
            string Path = "";
            string QueryString = "";

            Host = httpContext.Request.Host.ToString();

            if (httpContext.Request.Path.HasValue)
            {
                Path = httpContext.Request.Path.Value;
            }

            if (httpContext.Request.QueryString.HasValue)
            {
                QueryString = httpContext.Request.QueryString.Value;
            }

            return Host + Path + QueryString;
        }

        #endregion

    }
}
