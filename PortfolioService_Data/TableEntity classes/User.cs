using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;

namespace PortfolioServiceStorage {
    public class User : TableEntity {
        public User(string email) {
            this.PartitionKey = "UserPartition";
            this.RowKey = email;
        }

        public User() { }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public string PasswordHash { get; set; }
    }
}
