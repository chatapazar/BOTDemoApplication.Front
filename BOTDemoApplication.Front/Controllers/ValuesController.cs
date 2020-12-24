using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BOTDemoApplication.Front.Model;
using BOTDemoApplication.Front.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BOTDemoApplication.Front.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {

        private readonly ILogger<ValuesController> _logger;
        private readonly HttpClient httpClient = new HttpClient();

        public ValuesController(ILogger<ValuesController> logger)
        {
            this._logger = logger;
        }

        [HttpGet]
        public ActionResult<string> Get()
        {
            return StatusCode((int)HttpStatusCode.OK, "Hello from front");
        }

        // GET api/values/information
        [HttpGet("information")]
        public ActionResult<string> GenerateInformation()
        {
            DateTime startInvocationTime = DateTime.Now;
            LoggingUtilities.LogInvocationInformation(_logger, HttpContext, "InformationParameter", HttpStatusCode.OK.ToString(), startInvocationTime: startInvocationTime);
            return StatusCode((int)HttpStatusCode.OK, "Front - Success, 200");
        }

        // GET api/values/warning
        [HttpGet("warning")]
        public ActionResult<string> GenerateWarning()
        {
            DateTime startInvocationTime = DateTime.Now;
            LoggingUtilities.LogInvocationWarning(_logger, HttpContext, "WarningParameter", HttpStatusCode.BadRequest.ToString(), new Exception() { }, startInvocationTime: startInvocationTime);
            return StatusCode((int)HttpStatusCode.BadRequest, "Front - Warning, 400");
        }

        // GET api/values/warning
        [HttpGet("error")]
        public ActionResult<string> GenerateError()
        {
            DateTime startInvocationTime = DateTime.Now;
            LoggingUtilities.LogInvocationError(_logger, HttpContext, "ErrorParameter", HttpStatusCode.InternalServerError.ToString(), new Exception() { }, startInvocationTime: startInvocationTime);
            return StatusCode((int)HttpStatusCode.InternalServerError, "Front - Error, 500");
        }

        // POST api/values/front
        [HttpPost("front")]
        public ActionResult<ValuesResponseModel> PostFront(ValuesRequestModel frontInput)
        {
            DateTime startInvocationTime = DateTime.Now;
            ValuesResponseModel output = new ValuesResponseModel()
            {
                output = frontInput.input
            };
            LoggingUtilities.LogInvocationInformation(_logger, HttpContext, JsonConvert.SerializeObject(frontInput), HttpStatusCode.OK.ToString(), startInvocationTime: startInvocationTime);
            return output;
        }

        // POST api/values/front
        [HttpPost("scaling")]
        public ActionResult<ValuesResponseModel> PostScaling(ValuesRequestModel frontInput)
        {
            DateTime startInvocationTime = DateTime.Now;
            int i;
            //int j = 1000000000;
            string iteration_count = Environment.GetEnvironmentVariable("ITERATION_COUNT");
            int j = string.IsNullOrEmpty(iteration_count) ? 10000000 : Int32.Parse(iteration_count);

            string[] temp = new string[j];

            
            for (i=0; i<j ;i++)
            {
                temp[i] = ((j << i) + (j >> i)).ToString();
            }

            ValuesResponseModel output = new ValuesResponseModel()
            {
                output = frontInput.input
            };
            LoggingUtilities.LogInvocationInformation(_logger, HttpContext, JsonConvert.SerializeObject(frontInput), HttpStatusCode.OK.ToString(), startInvocationTime: startInvocationTime);
            return output;
        }


        // POST api/values/front
        [HttpPost("fronttoback")]
        public ActionResult<ValuesResponseModel> PostFrontToBack(ValuesRequestModel frontInput)
        {
            DateTime startInvocationTime = DateTime.Now;
            ValuesResponseModel output = new ValuesResponseModel();
            //ValuesResponseModel output = new ValuesResponseModel()
            //{
            //    output = frontInput.input
            //};
            string jsonInString = JsonConvert.SerializeObject(frontInput);

            string backendURL = Environment.GetEnvironmentVariable("BACKEND_URL");

            if(String.IsNullOrEmpty(backendURL))
            {
                LoggingUtilities.LogInvocationWarning(_logger, HttpContext, JsonConvert.SerializeObject(frontInput), HttpStatusCode.BadRequest.ToString(), new Exception() { }, startInvocationTime: startInvocationTime);
                return StatusCode((int)HttpStatusCode.BadRequest, "Please set \"BACKEND_URL\" in environment variable, 400");
            }

            var backendResponse = httpClient.PostAsync(backendURL, new StringContent(jsonInString, Encoding.UTF8, "application/json")).Result;

            
            if(backendResponse.StatusCode == HttpStatusCode.OK)
            {
                output = JsonConvert.DeserializeObject<ValuesResponseModel>(backendResponse.Content.ReadAsStringAsync().Result);
            }
            else
            {
                LoggingUtilities.LogInvocationWarning(_logger, HttpContext, JsonConvert.SerializeObject(frontInput), HttpStatusCode.BadRequest.ToString(), new Exception() { }, startInvocationTime: startInvocationTime);
                //return StatusCode((int)HttpStatusCode.BadRequest, JsonConvert.SerializeObject(backendResponse) +  ", 400");
                return StatusCode((int)HttpStatusCode.BadRequest, backendResponse);
            }
            
            LoggingUtilities.LogInvocationInformation(_logger, HttpContext, JsonConvert.SerializeObject(frontInput), HttpStatusCode.OK.ToString(), startInvocationTime: startInvocationTime);
            return output;
        }

    }
}
