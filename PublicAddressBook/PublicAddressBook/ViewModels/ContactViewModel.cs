using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublicAddressBook.ViewModels
{
    public class ContactViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public List<PhoneNumberViewModel> PhoneNumbers { get;set;}
    }
}
