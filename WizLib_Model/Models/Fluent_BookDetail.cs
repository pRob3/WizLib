﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizLib_Model.Models
{
    public class Fluent_BookDetail
    {
        
        public int BookDetail_Id { get; set; }

        public string NumberOfChapters { get; set; }

        public string NumberOfPages { get; set; }

        public double Weight { get; set; }

    }
}