using CustomersAPIServices.Models.ResponseModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomersAPIServices.Queries
{
    public class GetAllCustomerQuery : IRequest<List<CustomerResponse>>
    {
    }
}
