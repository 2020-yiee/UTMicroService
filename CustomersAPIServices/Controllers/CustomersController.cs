using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomersAPIServices.Models.RequestModels;
using CustomersAPIServices.Models.ResponseModels;
using CustomersAPIServices.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomersAPIServices.Controllers
{
    [Route("api/[controller]")]
    public class CustomersController : Controller
    {
        ICustomerRepository repository;
        [Authorize]
        [HttpGet]
        public IActionResult GetAllCustomers()
        {
            repository = new CustomerRepositoryImpl();
            return Ok(repository.getAllCustomers());
        }

        [Authorize]
        [HttpPost("customer")]
        public IActionResult getCustomer(GetCustomerRequest request)
        {
            repository = new CustomerRepositoryImpl();
            return Ok(repository.getCustomer(request));
        }

        [HttpPost("create")]
        public IActionResult CreateCustomer( [FromBody] CreateCustomerRequest request)
        {
            repository = new CustomerRepositoryImpl();
            var customer = repository.createCustomer(request);

            return customer !=null? Ok() : StatusCode(400);
        }

        [HttpPost("delete")]
        public IActionResult deleteCustomer([FromBody] int customerId)
        {
            repository = new CustomerRepositoryImpl();
            return Ok(repository.deleteCustomer(customerId));
        }

        [HttpPost("update")]
        public IActionResult updateCustomer([FromBody] UpdateCustomerRequest request)
        {
            repository = new CustomerRepositoryImpl();
            return Ok(repository.updateCustomer(request));
        }

        
    }
}