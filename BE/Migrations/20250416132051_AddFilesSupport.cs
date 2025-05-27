using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BE.Migrations
{
    /// <inheritdoc />
    public partial class AddFilesSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cars",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StoredFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Data = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoredFiles", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Cars",
                columns: new[] { "Id", "Brand", "Color", "Model", "Name", "Price", "Year" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "Audi", "Gray", "Sedan", "A4", 30000, 2018 },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "Jeep", "Green", "SUV", "Wrangler", 45000, 2022 },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "Honda", "White", "Sedan", "Accord", 27000, 2020 },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "Ford", "Blue", "Truck", "Ranger", 32000, 2021 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Toyota", "White", "Sedan", "Corolla", 20000, 2020 },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "Honda", "Black", "Sedan", "Civic", 22000, 2021 },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), "Tesla", "Red", "Electric", "Model 3", 35000, 2022 },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), "Ford", "Blue", "Coupe", "Mustang", 40000, 2019 },
                    { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), "Toyota", "Silver", "Sedan", "Camry", 25000, 2021 },
                    { new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"), "BMW", "Black", "SUV", "X5", 55000, 2020 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cars");

            migrationBuilder.DropTable(
                name: "StoredFiles");
        }
    }
}
