using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Entities;

public class PersonsDbContext:DbContext
{
    public PersonsDbContext(DbContextOptions options):
        base(options)
    {
        
    }
    public DbSet<Person> Persons { get; set; }
    public DbSet<Country> Countries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Country>().ToTable("Countries");
        modelBuilder.Entity<Person>().ToTable("Persons");

        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasOne(p => p.Country)
            .WithMany(c => c.Persons)
            .HasForeignKey(p => p.CountryId);
        });

        string CountriesJson = System.IO.File.ReadAllText("countries.json");
        List<Country> Countries = System.Text.Json.JsonSerializer.Deserialize<List<Country>>(CountriesJson)??new();
        foreach (var country in Countries)
            modelBuilder.Entity<Country>().HasData(country);

        string PersonsJson = System.IO.File.ReadAllText("persons.json");
        List<Person> Persons = System.Text.Json.JsonSerializer.Deserialize<List<Person>>(PersonsJson) ?? new();
        foreach (var person in Persons)
            modelBuilder.Entity<Person>().HasData(person);

    }

    public List<Person> sp_GetAllPersons()
    {
        return Persons.FromSqlRaw("Execute [dbo].[GetAllPersons]").ToList();
    }

    public int sp_InsertPerson(Person person)
    {
        SqlParameter[] parameters = new SqlParameter[] {
        new SqlParameter("@PersonId", person.PersonId),
        new SqlParameter("@PersonName", person.PersonName),
        new SqlParameter("@Email", person.Email),
        new SqlParameter("@DateOfBirth", person.DateOfBirth),
        new SqlParameter("@Gender", person.Gender),
        new SqlParameter("@CountryId", person.CountryId),
        new SqlParameter("@Address", person.Address),
        new SqlParameter("@ReceiveNewsLetters", person.ReceiveNewsLetters)
      };

        return Database.ExecuteSqlRaw("EXECUTE [dbo].[InsertPerson] @PersonId, @PersonName, @Email, @DateOfBirth, @Gender, @CountryId, @Address, @ReceiveNewsLetters", parameters);
    }
}
