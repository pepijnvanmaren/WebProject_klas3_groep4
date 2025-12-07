using System;
using System.Collections.Generic;
using WebProject_klas3_groep4.DTO;
using WebProject_klas3_groep4.models;

namespace WebProject_klas3_groep4.Tests.TestHelpers
{
    public static class TestSpawner
    {
        public static productDB CreateValidProduct(
            int id = 1,
            int? veilingId = null,
            int? aanvoerderId = null)
        {
            return new productDB
            {
                ID = id,
                Naam = "Test Product",
                Beschrijving = "Beschrijving test",
                MinimalePrijs = 10,
                Hoeveelheid = 5,
                Gewicht = 1.2,
                Potmaat = 12,
                Steellengte = 30,
                Oogstdatum = DateTime.Now,
                Status = VeilingStatus.InWachtrij,
                Foto = null,
                VerkochtePrijs = null,
                AanvoerderId = aanvoerderId,
                VeilingId = veilingId
            };
        }

        public static VeilingDB CreateValidVeiling(
            int id = 1,
            int veilingmeesterId = 1,
            int aantalProducten = 1)
        {
            return new VeilingDB
            {
                ID = id,
                StarTijd = "08:00",
                StartDatum = "2026-01-01",
                KlokLocatie = "Zoetermeer",
                HuidigeSituatieVanVeiling = "Open",
                Bechrijving = "Test veiling",
                AantalProducten = aantalProducten,
                VeilingmeesterId = veilingmeesterId,
                Producten = new List<productDB>()
            };
        }
        public static GebruikerDB CreateValidGebruiker(
                   int id = 1,
                   string email = "test@test.nl",
                   string userName = "TestUser",
                   string phoneNumber = "0612345678",
                   string rol = "Gebruiker")
        {
            return new GebruikerDB
            {
                Id = id,
                UserName = userName,
                Email = email,
                PhoneNumber = phoneNumber,
                Rol = rol,
                EmailConfirmed = true,
                NormalizedEmail = email.ToUpper(),
                NormalizedUserName = userName.ToUpper()
            };
        }

        public static GebruikerCreateDto CreateValidGebruikerCreateDto(
            string email = "newuser@test.nl",
            string userName = "NewUser",
            string password = "Password123!",
            string phoneNumber = "0612345678",
            string rol = "Gebruiker")
        {
            return new GebruikerCreateDto
            {
                UserName = userName,
                Email = email,
                Password = password,
                PhoneNumber = phoneNumber,
                Rol = rol
            };
        }

        public static GebruikerUpdateDto CreateValidGebruikerUpdateDto(
            string email = "updated@test.nl",
            string newPassword = null,
            string userName = "UpdatedUser",
            string phoneNumber = "0687654321")
        {
            return new GebruikerUpdateDto
            {
                UserName = userName,
                Email = email,
                PhoneNumber = phoneNumber,
                NewPassword = newPassword
            };
        }

        public static GebruikerDto CreateValidGebruikerDto(
            int id = 1,
            string email = "test@test.nl",
            string userName = "TestUser",
            string phoneNumber = "0612345678",
            string rol = "Gebruiker")
        {
            return new GebruikerDto
            {
                Id = id,
                UserName = userName,
                Email = email,
                PhoneNumber = phoneNumber,
                Rol = rol
            };
        }

        public static GebruikerDB CreateValidAanvoerder(
                   int id = 1,
                   string email = "test@test.nl",
                   string userName = "TestUser",
                   string phoneNumber = "0612345678",
                   string rol = "Aanvoerder")
        {
            return new GebruikerDB
            {
                Id = id,
                UserName = userName,
                Email = email,
                PhoneNumber = phoneNumber,
                Rol = rol,
                EmailConfirmed = true,
                NormalizedEmail = email.ToUpper(),
                NormalizedUserName = userName.ToUpper(),
                KvkNummer = "12345678",
                NaamVanBedrijf = "Test Bedrijf",
                BedrijfTelefoonnummer = "0701234567",
                BedrijfEmail = "Bedrijf@email"
            };
        }

        public static AanvoerderCreateDto CreateValidAanvoerderCreateDto(
            string email = "newuser@test.nl",
            string userName = "NewUser",
            string password = "Password123!",
            string phoneNumber = "0612345678")
        {
            return new AanvoerderCreateDto
            {
                Adres = "Test Adres",
                KvkNummer = "12345678",
                NaamVanBedrijf = "Test Bedrijf",
                Postcode = "1234AB",
                BedrijfTelefoonnummer = "0701234567",
                BedrijfEmail = "Bedrijf@email",
                UserName = userName,
                Email = email,
                Password = password,
                PhoneNumber = phoneNumber,
            };
        }

        public static AanvoerderUpdateDto CreateValidAanvoerderUpdateDto(
            string email = "newuser@test.nl",
            string userName = "NewUser",
            string password = "Password123!",
            string phoneNumber = "0612345678")
        {
            return new AanvoerderUpdateDto
            {
                Adres = "Updated Adres",
                KvkNummer = "87654321",
                NaamVanBedrijf = "Updated Bedrijf",
                Postcode = "4321BA",
                BedrijfTelefoonnummer = "0707654321",
                BedrijfEmail = "UpdatedBedrijf@email",
                UserName = userName,
                Email = email,
                PhoneNumber = phoneNumber,
            };
        }
    }
}
