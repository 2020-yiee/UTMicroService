using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomersAPIServices.Commands;
using CustomersAPIServices.Models.RequestModels;
using CustomersAPIServices.Models.ResponseModels;
using CustomersAPIServices.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomersAPIServices.Controllers
{
    [Route("api/[controller]")]
    public class CustomersController : Controller
    {
        private readonly IMediator _mediator;

        public CustomersController(Mediator mediator)
        {
            _mediator = mediator; 
        }


        [HttpGet]
        [Authorize( Roles = "admin")]
        public async Task<IActionResult> GetAsync()
        {
            var query = new GetAllCustomerQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
            //return new string[] { "Catcher Wong", "James Li", Request.Host.Port.ToString() };
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var query = new GetCustomerByIdQuery(id);
            var result = await _mediator.Send(query);
            return result != null ? (IActionResult) Ok(result) : NotFound();
            //return $"Catcher Wong - {id} - {Request.Host.Port}";
        }


        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerCommand createCustomerCommand)
        {
            var result = await _mediator.Send(createCustomerCommand);
            return result != null ? (IActionResult)Ok(result) : NotFound();
            //return $"Catcher Wong - {id} - {Request.Host.Port}";
        }
    }
}