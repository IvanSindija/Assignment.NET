using Microsoft.AspNetCore.Mvc;
using PublicAddressBook.Handlers.impl;
using PublicAddressBook.Handlers.intf;
using PublicAddressBook.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublicAddressBook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicAddressBookController : ControllerBase
    {
        private readonly IContactHandler _contactHandler;
        private readonly IPhoneNumberHandler _phoneNumberHandler;
        public PublicAddressBookController(IContactHandler contactHandler, IPhoneNumberHandler phoneNumberHandler)
        {
            this._contactHandler = contactHandler;
            this._phoneNumberHandler = phoneNumberHandler;
        }
        [HttpGet]
        [Route("Contact")]
        public async Task<IEnumerable<ContactViewModel>> Get(int? page)
        {
            return await _contactHandler.GetContact(page??0);
        }

        [HttpGet]
        [Route("Contact/{id}")]
        public async Task<ContactViewModel> ContactGet(int id)
        {
            return await _contactHandler.GetContactById(id);
        }

        [HttpPost]
        [Route("AddContact")]
        public async Task AddContactPost([FromBody] ContactViewModel value)
        {
            await _contactHandler.AddContact(value);
        }

        [HttpPost]
        [Route("UpdateContact")]
        public async Task UpdateContactPost([FromBody] ContactViewModel value)
        {
            await _contactHandler.UpdateContact(value);
        }

        [HttpPost]
        [Route("DeleteContact")]
        public async Task DeleteContactPost([FromBody] int contactId)
        {
            await _contactHandler.DeleteContact(contactId);
        }

        [HttpGet]
        [Route("PhoneNumber")]
        public async Task<IEnumerable<ContactViewModel>> PhoneNumberGet(int? page)
        {
            return await _contactHandler.GetContact(page ?? 0);
        }

        [HttpGet]
        [Route("PhoneNumber/{id}")]
        public async Task<PhoneNumberViewModel> GetPhoneNumber(int id)
        {
            return await _phoneNumberHandler.GetPhoneNumberById(id);
        }

        [HttpPost]
        [Route("AddPhoneNumber")]
        public async Task AddPhoneNumbertPost([FromBody] PhoneNumberViewModel value)
        {
            if (value.ContactId.HasValue)
            {
                await _phoneNumberHandler.AddPhoneNumber(value.ContactId.Value, value.Number);
            }
        }

        [HttpPost]
        [Route("UpdatePhoneNumber")]
        public async Task UpdateContactPost([FromBody] PhoneNumberViewModel value)
        {
            await _phoneNumberHandler.UpdatePhoneNumber(value);
        }

        [HttpPost]
        [Route("DeletePhoneNumber")]
        public async Task DeletePhoneNumberPost([FromBody] int phoneNumberId)
        {
            await _phoneNumberHandler.DeletePhoneNumber(phoneNumberId);
        }
    }
}
