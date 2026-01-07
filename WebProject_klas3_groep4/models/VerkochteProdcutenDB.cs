using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebProject_klas3_groep4.models;
namespace WebProject_klas3_groep4
{
    public class VerkochteProdcutenDB
    {
        [Key]
        public int ID { get; set; }
        public int HoeveelHeid { get; set; }
        public double VerkochtePrijs { get; set; }
        public DateTime VerkoopDatum { get; set; }
        // Foreign Key naar Product
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public productDB? Product { get; set; }
        // Foreign Key naar Koper
        public int KoperId { get; set; }
        public GebruikerDB? Koper { get; set; }
    }
}