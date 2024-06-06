using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MagicVilla_API.Migrations
{
    /// <inheritdoc />
    public partial class AlimentarTablaVilla : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Villas",
                columns: new[] { "Id", "Amenidad", "Detalle", "FechaActualizacion", "FechaCreacion", "ImagenUrl", "MetrosCuadrados", "Nombre", "Ocupantes", "Tarifa" },
                values: new object[,]
                {
                    { 1, "", "Detalle de la Villa...", new DateTime(2024, 6, 6, 4, 39, 42, 247, DateTimeKind.Local).AddTicks(9941), new DateTime(2024, 6, 6, 4, 39, 42, 247, DateTimeKind.Local).AddTicks(9928), "", 50, "Villa Real", 5, 200.0 },
                    { 2, "", "Detalle de la Villa...", new DateTime(2024, 6, 6, 4, 39, 42, 247, DateTimeKind.Local).AddTicks(9944), new DateTime(2024, 6, 6, 4, 39, 42, 247, DateTimeKind.Local).AddTicks(9944), "", 40, "Premium Vista a la Piscina", 4, 150.0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
