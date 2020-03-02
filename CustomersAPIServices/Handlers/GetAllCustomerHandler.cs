using CustomersAPIServices.Models.ResponseModels;
using CustomersAPIServices.Queries;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CustomersAPIServices.Handlers
{
    public class GetAllCustomerHandler : IRequestHandler<GetAllCustomerQuery, List<CustomerResponse>>
    {
        public GetAllCustomerHandler()
        {

        }

        public async Task<List<CustomerResponse>> Handle(GetAllCustomerQuery request, CancellationToken cancellationToken)
        {
            var result = new List<CustomerResponse>();
            result.Add(new CustomerResponse("customer 1"));
            result.Add(new CustomerResponse("customer 2"));
            result.Add(new CustomerResponse("customer 3"));
            result.Add(new CustomerResponse("customer 4"));
            result.Add(new CustomerResponse("customer 5"));
            return result;
        }
    }
}
