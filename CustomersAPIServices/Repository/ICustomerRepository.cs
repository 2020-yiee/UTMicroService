using CustomersAPIServices.EFModels;
using CustomersAPIServices.Models.RequestModels;
using CustomersAPIServices.Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomersAPIServices.Repository
{
    public interface ICustomerRepository
    {
        Customer createCustomer(CreateCustomerRequest customer);
        IEnumerable<CustomerResponse> getAllCustomers();
        CustomerResponse getCustomer(GetCustomerRequest request);

        Boolean deleteCustomer(int customerId);
        Boolean updateCustomer(UpdateCustomerRequest request);

    }
}
