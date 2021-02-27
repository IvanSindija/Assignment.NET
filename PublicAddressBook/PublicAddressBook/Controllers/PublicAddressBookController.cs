using Microsoft.AspNetCore.Mvc;
using PublicAddressBook.Handlers.impl;
using PublicAddressBook.Handlers.intf;
using PublicAddressBook.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PublicAddressBook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicAddressBookController : ControllerBase
    {
        private readonly IPublicAddressBookHandler publicAddressBookHandler;
        public PublicAddressBookController(IPublicAddressBookHandler publicAddressBookHandler)
        {
            this.publicAddressBookHandler = publicAddressBookHandler;
        }
        // GET: api/<PublicAddressBookControlle>
        [HttpGet]
        public async Task<IEnumerable<ContactViewModel>> Get(int? page)
        {
            return await publicAddressBookHandler.GetContact(page??0);
        }

        // GET api/<PublicAddressBookControlle>/5
        [HttpGet("{id}")]
        public async Task<ContactViewModel> Get(int id)
        {
            return await publicAddressBookHandler.GetContactById(id);
        }

        // POST api/<PublicAddressBookControlle>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<PublicAddressBookControlle>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<PublicAddressBookControlle>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
