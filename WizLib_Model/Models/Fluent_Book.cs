using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WizLib_Model.Models
{
    public class Fluent_Book
    {

        public int Book_Id { get; set; }


        public string Title { get; set; }

        public string ISBN { get; set; }


        public double Price { get; set; }

    }
}