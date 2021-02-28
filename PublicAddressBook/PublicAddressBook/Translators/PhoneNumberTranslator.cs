using PublicAddressBook.Dal;
using PublicAddressBook.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublicAddressBook.Translators
{
    public static class PhoneNumberTranslator
    {
        public static PhoneNumberViewModel Translate(PhoneNumber phoneNumber)
        {
            if(phoneNumber != null)
            {
                return new PhoneNumberViewModel()
                {
                    Id = phoneNumber.Id,
                    Number = phoneNumber.Number,
                    ContactId = phoneNumber.ContactId
                };
            }
            else
            {
                throw new Exception("Db model is null");
            }
        }

        public static List<PhoneNumberViewModel> Translate(List<PhoneNumber> phoneNumbers)
        {
            if (phoneNumbers != null)
            {
                var phoneNumbersViewModel = new List<PhoneNumberViewModel>();
                foreach(var phoneNumber in phoneNumbers)
                {
                    phoneNumbersViewModel.Add(Translate(phoneNumber));
                }
                return phoneNumbersViewModel;
            }
            else
            {
                throw new Exception("Db model is null");
            }
        }
    }
}
