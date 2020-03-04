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
        public IActionResult CreateCustomer(CreateCustomerRequest request)
        {
            repository = new CustomerRepositoryImpl();
            return Ok(repository.createCustomer(request));
        }

        
    }
}