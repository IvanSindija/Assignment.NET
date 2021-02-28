using PublicAddressBook.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublicAddressBook.Handlers.intf
{
    public interface IContactHandler
    {
        public Task<List<ContactViewModel>> GetContact(int page);
        public Task<ContactViewModel> GetContactById(int id);
        public Task AddContact(ContactViewModel user);

        public Task UpdateContact(ContactViewModel user);

        public Task DeleteContact(int id);

        public Task UpdateContactInfoAsync(int contactId);
    }
}
