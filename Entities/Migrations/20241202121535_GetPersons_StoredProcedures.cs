using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class GetPersons_StoredProcedures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sp_GetAllPersons = @" 
                Create Procedure [dbo].[GetAllPersons]
                As Begin
                Select * from [dbo].[Persons]
                end";

            migrationBuilder.Sql(sp_GetAllPersons);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string sp_GetAllPersons = @" 
                drop Procedure [dbo].[GetAllPersons]";

            migrationBuilder.Sql(sp_GetAllPersons);
        }
    }
}
