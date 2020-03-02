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
    public class GetCustomerByIdHandler : IRequestHandler<GetCustomerByIdQuery, CustomerResponse>
    {
        public async Task<CustomerResponse> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
        {
            return new CustomerResponse("customer" + request.id);
        }
    }
}
