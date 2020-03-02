using CustomersAPIServices.Models.RequestModels;
using CustomersAPIServices.Models.ResponseModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomersAPIServices.Commands
{
    public class CreateCustomerCommand : IRequest<CustomerResponse>
    {
        public String name { get; set; }

    }
}
