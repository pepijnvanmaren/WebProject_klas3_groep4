namespace WebProject_klas3_groep4.models
{
    public class KoperDB : GebruikerDB
    {
        public string BankGegevens { get; set; }

        public string Postcode { get; set; }

        public string Adres { get; set; }
        public List<LotDB> Lists { get; set; }

    }
}