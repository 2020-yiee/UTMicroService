using AuthServer.EFModels;
using AuthServer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Repository
{
    public class CustomerRepositoryImpl : ICustomerRepository
    {
        public List<Customer> GetAllCustomers()
        {
            using (var context = new DBUTContext())
            {
                var listCustomers = context.Customer.ToList();
                return listCustomers;
            }
        }

        public Customer getCustomerByUsernameAndPassword(LoginRequestModel model)
        {
            using (var context = new DBUTContext())
            {
                var customer = context.Customer
                    .Where(s => s.UserName == model.name)
                    .Where(s => s.Password == model.password)
                    .FirstOrDefault();
                return customer;
            }
        }
    }
}
