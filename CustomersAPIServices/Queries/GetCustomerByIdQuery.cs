using CustomersAPIServices.Models.ResponseModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomersAPIServices.Queries
{
    public class GetCustomerByIdQuery : IRequest<CustomerResponse>
    {
        public int id { get; set; }

        public GetCustomerByIdQuery(int id)
        {
            this.id = id;
        }

        public GetCustomerByIdQuery()
        {
        }
    }
}
