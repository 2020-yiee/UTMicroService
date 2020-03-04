using CustomersAPIServices.EFModels;
using CustomersAPIServices.Models.RequestModels;
using CustomersAPIServices.Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomersAPIServices.Repository
{
    public class CustomerRepositoryImpl : ICustomerRepository
    {
        private DBUTContext context = new DBUTContext();
        public Customer createCustomer(CreateCustomerRequest customer)
        {
            Customer addcustomer = new Customer();
            addcustomer.Email = customer.email;
            addcustomer.FullName = customer.fullname;
            addcustomer.UserName = customer.userName;
            addcustomer.Password = Hashing.HashPassword(customer.password);
            addcustomer.Role = "user";
            try
            {
                context.Customer.Add(addcustomer);
                context.SaveChanges();
                return addcustomer;
            }
            catch (Exception)
            {

                return null;
            }
        }

        public bool deleteCustomer(int customerId)
        {
            Customer customer = context.Customer.Where(s => s.CustomerId == customerId)
                .FirstOrDefault();
            try
            {
                context.Customer.Remove(customer);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public IEnumerable<CustomerResponse> getAllCustomers()
        {
            List<CustomerResponse> result = new List<CustomerResponse>();
            List<Customer> customers = context.Customer.ToList();
            foreach (var customer in customers)
            {
                result.Add(new CustomerResponse(customer.CustomerId,customer.UserName, customer.FullName, customer.Email, customer.Role));
            }
            return result;
        }

        public CustomerResponse getCustomer(GetCustomerRequest request)
        {
            Customer customer = context.Customer.Where(s => s.CustomerId == request.customerId)
                .Where(s => s.Email == request.email).FirstOrDefault();

            return new CustomerResponse(customer.CustomerId,customer.UserName,customer.FullName,customer.Email,customer.Role);
        }

        public bool updateCustomer(UpdateCustomerRequest request)
        {
            Customer customer = context.Customer.Where(s => s.CustomerId == request.customerId)
                .FirstOrDefault();
            customer.FullName = request.fullname;
            customer.Email = request.email;
            customer.Role = request.role;
            try
            {
                context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
