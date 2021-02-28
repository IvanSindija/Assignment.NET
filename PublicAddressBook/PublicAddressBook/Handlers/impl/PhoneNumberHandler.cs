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
    public class PhoneNumberHandler : IPhoneNumberHandler
    {
        private readonly PublicAddressBookContext dbContext;
        private readonly int pageSize;
        private readonly IHubContext<SignalRHub> hub;
        public readonly IContactHandler _contactHandler;
        public PhoneNumberHandler(PublicAddressBookContext dbContext, IHubContext<SignalRHub> hub, IContactHandler contactHandler)
        {
            this.dbContext = dbContext;
            this.hub = hub;
            this._contactHandler = contactHandler;
            pageSize = 10;
        }

        public async Task<List<PhoneNumberViewModel>> GetPhoneNumber(int page)
        {
            try
            {
                var contactsDb = await dbContext.PhoneNumbers
                    .Skip(page * pageSize)
                    .Take(pageSize).ToListAsync();
                return PhoneNumberTranslator.Translate(contactsDb);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception getting phone numbers.", ex);
            }
        }

        public async Task<PhoneNumberViewModel> GetPhoneNumberById(int id)
        {
            try
            {
                var contactsDb = await dbContext.PhoneNumbers
                    .FirstOrDefaultAsync(c => c.Id == id);
                return PhoneNumberTranslator.Translate(contactsDb);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception getting phone number.", ex);
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
            }
            catch (Exception ex)
            {
                throw new Exception("Exception during update of phone number.", ex);
            }
        }

        private async Task DeletePhoneNumberAsync(int phoneNumberId)
        {
            await hub.Clients.All.SendAsync("DeletePhoneNumberAsync", phoneNumberId);
        }

        private async Task UpdatePhoneNumberInfoAsync(int phoneNumberId)
        {
            var phoneNumber = await dbContext.PhoneNumbers.FirstOrDefaultAsync(c => c.Id == phoneNumberId);
            var phoneNumberVM = new PhoneNumberViewModel() { Id = phoneNumber.Id, Number = phoneNumber.Number };

            await hub.Clients.All.SendAsync("UpdatePhoneNumberInfoAsync", phoneNumberVM);
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
                await _contactHandler.UpdateContactInfoAsync(contactId);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception during addition of phone number to contact.", ex);
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
    }
}
