using PublicAddressBook.Dal;
using PublicAddressBook.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublicAddressBook.Translators
{
    public class ContactTranslator
    {
        public static Contact Translate(ContactViewModel vm)
        {
            if (vm == null)
            {
                throw new Exception("Contact is null");
            }
            else
            {
                var dbModel = new Contact()
                {
                    Name = vm.Name,
                    Address = vm.Address,
                    DateOfBirth = vm.DateOfBirth
                };
                if (vm.PhoneNumbers != null && vm.PhoneNumbers.Count > 0)
                {
                    dbModel.PhoneNumbers = new List<PhoneNumber>();
                    foreach (var number in vm.PhoneNumbers)
                    {
                        dbModel.PhoneNumbers.Add(new PhoneNumber() { Number = number.Number });
                    }
                }

                return dbModel;
            }
        }

        public static ContactViewModel TranslateModel(Contact contactDb)
        {
            if (contactDb != null)
            {
                var contactsVM = new ContactViewModel()
                {
                    Id = contactDb.Id,
                    Address = contactDb.Address,
                    DateOfBirth = contactDb.DateOfBirth,
                    Name = contactDb.Name
                };
                if (contactDb.PhoneNumbers != null && contactDb.PhoneNumbers.Count > 0)
                {
                    contactsVM.PhoneNumbers = new List<PhoneNumberViewModel>();
                    foreach (var phoneNumber in contactDb.PhoneNumbers)
                    {
                        contactsVM.PhoneNumbers.Add(new PhoneNumberViewModel()
                        {
                            Id = phoneNumber.Id,
                            Number = phoneNumber.Number
                        });
                    }
                }

                return contactsVM;
            }
            else
            {
                return new ContactViewModel();
            }
        }

        public static List<ContactViewModel> TranslateModel(List<Contact> contactsDb)
        {
            if (contactsDb != null && contactsDb.Count > 0)
            {
                var contactsVMList = new List<ContactViewModel>();
                foreach (var contact in contactsDb)
                {
                    var contactsVM = TranslateModel(contact);
                    contactsVMList.Add(contactsVM);
                }

                return contactsVMList;
            }
            else
            {
                return new List<ContactViewModel>();
            }
        }
    }
}
