using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvoiceManager.Models
{
    public class Invoice
    {
        private DateTime _currentTime;
        private DateTime _date;
        private DateTime _dueDate;

        public Invoice()
        {
            _currentTime = DateTime.Now;
            _date = DateTime.Now; ;
            _dueDate = DateTime.Now;
        }

        public int ID { get; set; }
        public string Contact { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date")]
        public DateTime Date
        {
            get
            {
                return this._date;
            }
            set
            {
                this._date = value;
            }

        }
        public string Type { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Due Date")]
        public DateTime DueDate
        {
            get
            {
                return this._dueDate;
            }
            set
            {
                this._dueDate = value;
            }

        }

        public string Status { get; set; }
    }
}