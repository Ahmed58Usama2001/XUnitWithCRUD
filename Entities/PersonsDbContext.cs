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

        string CountriesJson = System.IO.File.ReadAllText("countries.json");
        List<Country> Countries = System.Text.Json.JsonSerializer.Deserialize<List<Country>>(CountriesJson)??new();
        foreach (var country in Countries)
            modelBuilder.Entity<Country>().HasData(country);

        string PersonsJson = System.IO.File.ReadAllText("persons.json");
        List<Person> Persons = System.Text.Json.JsonSerializer.Deserialize<List<Person>>(PersonsJson) ?? new();
        foreach (var person in Persons)
            modelBuilder.Entity<Person>().HasData(person);




    }
}
