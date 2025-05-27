using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BE.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDataSeedInMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"));

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"));

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"));

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"));

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"));

            migrationBuilder.DeleteData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 5);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Owners",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "John Doe" },
                    { 2, "Jane Smith" },
                    { 3, "Michael Johnson" },
                    { 4, "Emily Davis" },
                    { 5, "David Brown" }
                });

            migrationBuilder.InsertData(
                table: "Cars",
                columns: new[] { "Id", "Brand", "Color", "Model", "Name", "OwnerId", "Price", "Year" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "Audi", "Gray", "Sedan", "A4", 4, 30000, 2018 },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "Jeep", "Green", "SUV", "Wrangler", 4, 45000, 2022 },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "Honda", "White", "Sedan", "Accord", 5, 27000, 2020 },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "Ford", "Blue", "Truck", "Ranger", 5, 32000, 2021 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Toyota", "White", "Sedan", "Corolla", 1, 20000, 2020 },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "Honda", "Black", "Sedan", "Civic", 1, 22000, 2021 },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), "Tesla", "Red", "Electric", "Model 3", 2, 35000, 2022 },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), "Ford", "Blue", "Coupe", "Mustang", 2, 40000, 2019 },
                    { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), "Toyota", "Silver", "Sedan", "Camry", 3, 25000, 2021 },
                    { new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"), "BMW", "Black", "SUV", "X5", 3, 55000, 2020 }
                });
        }
    }
}
