using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizLib_Model.Models;

namespace WizLib_DataAccess.FluentConfig
{
    public class FluentBookAuthorConfig : IEntityTypeConfiguration<Fluent_BookAuthor>
    {
        public void Configure(EntityTypeBuilder<Fluent_BookAuthor> modelBuilder)
        {
            // Book Author
            // Fluent - Many-to-Many relation between book and author
            modelBuilder.HasKey(ba => new { ba.Author_Id, ba.Book_Id });
            modelBuilder
                .HasOne(b => b.Fluent_Book)
                .WithMany(b => b.Fluent_BookAuthors).HasForeignKey(b => b.Book_Id);
            modelBuilder
                .HasOne(b => b.Fluent_Author)
                .WithMany(b => b.Fluent_BookAuthor).HasForeignKey(b => b.Author_Id);
        }
    }
}
