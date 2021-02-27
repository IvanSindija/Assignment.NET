using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PublicAddressBook.Dal
{
    public class PublicAddressBookContext : DbContext
    {
        public PublicAddressBookContext(DbContextOptions<PublicAddressBookContext> options)
            : base(options) { }
          public DbSet<Contact> Contacts { get; set; }
        public DbSet<PhoneNumber> PhoneNumbers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contact>()
            .HasIndex(p => new { p.Name, p.Address }).IsUnique();
        }
        
    }

    public class Contact
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public ICollection<PhoneNumber> PhoneNumbers { get; set; }
    }

    public class PhoneNumber
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [ForeignKey("ContactId")]
        public int ContactId { get; set; }
        public Contact Contact { get; set; }
        public string Number { get; set; }
    }
}
