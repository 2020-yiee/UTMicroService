using CustomersAPIServices.Commands;
using CustomersAPIServices.Models.ResponseModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CustomersAPIServices.Handlers
{
    public class CreateCustomerHandler : IRequestHandler<CreateCustomerCommand, CustomerResponse>
    {
        public CreateCustomerHandler()
        {
        }

        public async Task<CustomerResponse> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            return new CustomerResponse(request.name);
        }
    }
}
