using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NetCoreDBConnection.Classes;
using NetCoreDBConnection.Model;
using NetCoreDBConnection.Util;
using Npgsql;
using static NetCoreDBConnection.Classes.Constants;

namespace NetCoreDBConnection.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            var ret = EmployeeUtil.GetEmployee();
            try
            {
                if (ret.Status.Code == ResCode.Success)
                    return Ok(new { result = ret });
                else
                    return BadRequest(new { result = ret });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Unauthorized(new { result = ret });
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] EmployeeModel model)
        {
            var ret = EmployeeUtil.CreateEmployee(model);

            try
            {
                if (ret.Status.Code == ResCode.Success)
                    return Ok(new { result = ret });
                else
                    return BadRequest(new { result = ret });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Unauthorized(new { result = ret });
            }
        }

        [HttpPost("Bulk")]
        public IActionResult Post([FromBody] List<EmployeeModel> model)
        {
            var ret = EmployeeUtil.CreateBulkEmployee(model);
            return Ok(new { result = ret });
        }
    }
}