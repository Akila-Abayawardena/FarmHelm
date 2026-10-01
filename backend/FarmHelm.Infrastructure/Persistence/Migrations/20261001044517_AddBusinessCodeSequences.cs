using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmHelm.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBusinessCodeSequences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "batch_code_seq");

            migrationBuilder.CreateSequence(
                name: "crop_code_seq");

            migrationBuilder.CreateSequence(
                name: "farm_code_seq");

            migrationBuilder.CreateSequence(
                name: "farm_location_code_seq");

            migrationBuilder.CreateSequence(
                name: "variety_code_seq");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(
                name: "batch_code_seq");

            migrationBuilder.DropSequence(
                name: "crop_code_seq");

            migrationBuilder.DropSequence(
                name: "farm_code_seq");

            migrationBuilder.DropSequence(
                name: "farm_location_code_seq");

            migrationBuilder.DropSequence(
                name: "variety_code_seq");
        }
    }
}
