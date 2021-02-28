using PublicAddressBook.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublicAddressBook.Handlers.intf
{
    public interface IPhoneNumberHandler
    {
        public Task<List<PhoneNumberViewModel>> GetPhoneNumber(int page);
        public Task<PhoneNumberViewModel> GetPhoneNumberById(int phoneNumberId);
        public Task AddPhoneNumber(int contactId, string number);

        public Task UpdatePhoneNumber(PhoneNumberViewModel number);
        public Task DeletePhoneNumber(int id);
    }
}
