using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PublicAddressBook.Dal;
using PublicAddressBook.Handlers.intf;
using PublicAddressBook.Hubs;
using PublicAddressBook.Translators;
using PublicAddressBook.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublicAddressBook.Handlers.impl
{
    public class PublicAddressBookHandler : IPublicAddressBookHandler
    {
        private readonly PublicAddressBookContext dbContext;
        private readonly int pageSize;
        private readonly IHubContext<SignalRHub> hub;
        public PublicAddressBookHandler(PublicAddressBookContext dbContext, IHubContext<SignalRHub> hub)
        {
            this.dbContext = dbContext;
            this.hub = hub;
            pageSize = 10;
        }

        private async Task UpdateContactInfoAsync(int contactId)
        {
            var contact = await dbContext.Contacts.Include(c => c.PhoneNumbers).FirstOrDefaultAsync(c => c.Id == contactId);
            var contactVM = ContactTranslator.TranslateModel(contact);

            hub.Clients.All.SendAsync("UpdateContactInfo", contactVM);
        }

        private async Task DeleteContactInfoAsync(int contactId)
        {
            hub.Clients.All.SendAsync("DeleteContactInfoAsync", contactId);
        }

        private async Task DeletePhoneNumberAsync(int phoneNumberId)
        {
            hub.Clients.All.SendAsync("DeletePhoneNumberAsync", phoneNumberId);
        }

        private async Task UpdatePhoneNumberInfoAsync(int phoneNumberId)
        {
            var phoneNumber = await dbContext.PhoneNumbers.FirstOrDefaultAsync(c => c.Id == phoneNumberId);
            var phoneNumberVM = new PhoneNumberViewModel() { Id=phoneNumber.Id, Number = phoneNumber.Number};

            hub.Clients.All.SendAsync("UpdatePhoneNumberInfoAsync", phoneNumberVM);
        }

        public async Task AddContact(ContactViewModel contactVM)
        {
            try
            {
                var contact = ContactTranslator.Translate(contactVM);
                dbContext.Contacts.Add(contact);
                await dbContext.SaveChangesAsync();
                await UpdateContactInfoAsync(contact.Id);
            }
            catch(Exception ex)
            {
                throw new Exception("Exception during creation of new contact", ex);
            }
        }

        public async Task AddPhoneNumber(int contactId, string number)
        {
            try
            {
                var phoneNumber = new PhoneNumber
                {
                    ContactId = contactId,
                    Number = number
                };
                await dbContext.PhoneNumbers.AddAsync(phoneNumber);
                await dbContext.SaveChangesAsync();
                await UpdateContactInfoAsync(contactId);
            }
            catch(Exception ex)
            {
                throw new Exception("Exception during addition of phone number to contact.", ex);
            }
        }

        public async Task DeleteContact(int id)
        {
            try
            {
                var contact = await dbContext.Contacts.FirstOrDefaultAsync(c => c.Id == id);
                if (contact != null)
                {
                    dbContext.Entry(contact).State = EntityState.Deleted;
                    await dbContext.SaveChangesAsync();
                    await DeleteContactInfoAsync(id);
                }

            }catch(Exception ex)
            {
                throw new Exception("Exception during deletion od contact.", ex);
            }
        }

        public async Task DeletePhoneNumber(int id)
        {
            try
            {
                var phoneNumbers = await dbContext.PhoneNumbers.FirstOrDefaultAsync(c => c.Id == id);
                if (phoneNumbers != null)
                {
                    dbContext.Entry(phoneNumbers).State = EntityState.Deleted;
                    await dbContext.SaveChangesAsync();
                    await DeletePhoneNumberAsync(id);
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Exception during deletion od phone nuber.", ex);
            }
        }

        public async Task<List<ContactViewModel>> GetContact(int page)
        {
            try
            {
                var contactsDb = await dbContext.Contacts
                    .Include(c=>c.PhoneNumbers)
                    .Skip(page * pageSize)
                    .Take(pageSize).ToListAsync();
                return ContactTranslator.TranslateModel(contactsDb);
            }catch(Exception ex)
            {
                throw new Exception("Exception getting contacts.", ex);
            }
        }

        public async Task<ContactViewModel> GetContactById(int id)
        {
            try
            {
                var contactsDb = await dbContext.Contacts
                    .Include(c => c.PhoneNumbers)
                    .FirstOrDefaultAsync(c => c.Id == id);
                return ContactTranslator.TranslateModel(contactsDb);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception getting contact.", ex);
            }
        }

        public async Task UpdateContact(ContactViewModel contactVM)
        {
            try
            {
                if (contactVM.Id.HasValue)
                {
                    var contactDb = dbContext.Contacts.FirstOrDefault(n => n.Id == contactVM.Id);
                    if (contactDb != null)
                    {
                        contactDb.Name = contactVM.Name;
                        contactDb.DateOfBirth = contactVM.DateOfBirth;
                        contactDb.Address = contactVM.Address;
                        await dbContext.SaveChangesAsync();
                        await UpdateContactInfoAsync(contactVM.Id.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception during update of contact.", ex);
            }
        }

        public async Task UpdatePhoneNumber(PhoneNumberViewModel numberVM)
        {
            try
            {
                if (numberVM.Id.HasValue)
                {
                    var numberDb = dbContext.PhoneNumbers.FirstOrDefault(n => n.Id == numberVM.Id);
                    if (numberDb != null)
                    {
                        numberDb.Number = numberVM.Number;
                        await dbContext.SaveChangesAsync();
                        await UpdatePhoneNumberInfoAsync(numberVM.Id.Value);
                    }
                }
            }catch(Exception ex)
            {
                throw new Exception("Exception during update of phone number.", ex);
            }
        }
    }
}
