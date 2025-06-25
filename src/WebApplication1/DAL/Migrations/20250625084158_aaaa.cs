using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class aaaa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompanyClient",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    KrsNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyClient", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Login = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IndividualClient",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Pesel = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndividualClient", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SoftwareSystem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CurrentVersion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PricingType = table.Column<int>(type: "int", nullable: false),
                    SubscriptionPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    UpfrontPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoftwareSystem", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Discount",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    PercentageValue = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    DiscountType = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SoftwareSystemId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Discount_SoftwareSystem_SoftwareSystemId",
                        column: x => x.SoftwareSystemId,
                        principalTable: "SoftwareSystem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Contract",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    ClientType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoftwareSystemId = table.Column<int>(type: "int", nullable: false),
                    SoftwareVersion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SupportYears = table.Column<int>(type: "int", nullable: false),
                    BasePrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FinalPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AppliedDiscountPercentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    AppliedDiscountAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    AppliedDiscountId = table.Column<int>(type: "int", nullable: true),
                    IsReturningClientDiscountApplied = table.Column<bool>(type: "bit", nullable: false),
                    ReturningClientDiscountAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contract", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contract_Discount_AppliedDiscountId",
                        column: x => x.AppliedDiscountId,
                        principalTable: "Discount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Contract_SoftwareSystem_SoftwareSystemId",
                        column: x => x.SoftwareSystemId,
                        principalTable: "SoftwareSystem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContractId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payment_Contract_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contract",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CompanyClient",
                columns: new[] { "Id", "Address", "CompanyName", "Email", "KrsNumber", "PhoneNumber" },
                values: new object[,]
                {
                    { 1, "ul. Długa 1, 00-001 Warszawa", "TechCorp Solutions Sp. z o.o.", "kontakt@techcorp.pl", "1234567890", "+48111222333" },
                    { 2, "ul. Krótka 5, 31-001 Kraków", "Innovation Systems S.A.", "info@innovationsystems.pl", "2345678901", "+48222333444" },
                    { 3, "ul. Morska 15, 81-001 Gdynia", "Digital Future Ltd.", "contact@digitalfuture.com", "3456789012", "+48333444555" }
                });

            migrationBuilder.InsertData(
                table: "Employee",
                columns: new[] { "Id", "Login", "Password", "Role" },
                values: new object[,]
                {
                    { 1, "admin", "0192023a7bbd73250516f069df18b500", 1 },
                    { 2, "john.doe", "482c811da5d5b4bc6d497ffa98491e38", 0 },
                    { 3, "jane.smith", "482c811da5d5b4bc6d497ffa98491e38", 0 }
                });

            migrationBuilder.InsertData(
                table: "IndividualClient",
                columns: new[] { "Id", "Address", "Email", "FirstName", "IsDeleted", "LastName", "Pesel", "PhoneNumber" },
                values: new object[,]
                {
                    { 1, "ul. Marszałkowska 123, 00-001 Warszawa", "jan.kowalski@email.com", "Jan", false, "Kowalski", "80010112345", "+48123456789" },
                    { 2, "ul. Krakowska 456, 31-001 Kraków", "anna.nowak@email.com", "Anna", false, "Nowak", "85051567890", "+48987654321" },
                    { 3, "ul. Gdańska 789, 80-001 Gdańsk", "piotr.wisniewski@email.com", "Piotr", false, "Wiśniewski", "90030398765", "+48555666777" }
                });

            migrationBuilder.InsertData(
                table: "SoftwareSystem",
                columns: new[] { "Id", "Category", "CurrentVersion", "Description", "Name", "PricingType", "SubscriptionPrice", "UpfrontPrice" },
                values: new object[,]
                {
                    { 1, "Finances", "2.1.0", "Advanced financial reporting and analytics platform", "Financial Analytics Pro", 2, 299.99m, 2999.99m },
                    { 2, "Education", "1.8.5", "Comprehensive learning management system", "EduTech Learning Platform", 0, 199.99m, null },
                    { 3, "Business", "3.2.1", "Customer relationship management system", "CRM Business Suite", 1, null, 4999.99m }
                });

            migrationBuilder.InsertData(
                table: "Discount",
                columns: new[] { "Id", "Description", "DiscountType", "EndDate", "IsActive", "Name", "PercentageValue", "SoftwareSystemId", "StartDate" },
                values: new object[,]
                {
                    { 1, "Special discount for new customers", 1, new DateTime(2025, 12, 25, 10, 41, 58, 655, DateTimeKind.Local).AddTicks(4610), true, "Spring Promotion", 15.0m, 1, new DateTime(2025, 5, 26, 10, 41, 58, 655, DateTimeKind.Local).AddTicks(4610) },
                    { 2, "Special pricing for educational institutions", 1, new DateTime(2026, 6, 25, 10, 41, 58, 655, DateTimeKind.Local).AddTicks(4610), true, "Education Discount", 25.0m, 2, new DateTime(2025, 4, 26, 10, 41, 58, 655, DateTimeKind.Local).AddTicks(4610) },
                    { 3, "Volume discount for large enterprises", 1, new DateTime(2026, 3, 25, 10, 41, 58, 655, DateTimeKind.Local).AddTicks(4610), true, "Enterprise Deal", 10.0m, 3, new DateTime(2025, 6, 10, 10, 41, 58, 655, DateTimeKind.Local).AddTicks(4610) }
                });

            migrationBuilder.InsertData(
                table: "Contract",
                columns: new[] { "Id", "AppliedDiscountAmount", "AppliedDiscountId", "AppliedDiscountPercentage", "BasePrice", "ClientId", "ClientType", "EndDate", "FinalPrice", "IsReturningClientDiscountApplied", "ReturningClientDiscountAmount", "SoftwareSystemId", "SoftwareVersion", "StartDate", "Status", "SupportYears" },
                values: new object[,]
                {
                    { 1, 599.99m, 1, 15.0m, 3999.99m, 1, "individual", new DateTime(2025, 7, 15, 10, 41, 58, 655, DateTimeKind.Local).AddTicks(4610), 3059.99m, true, 170.0m, 1, "2.1.0", new DateTime(2025, 6, 30, 10, 41, 58, 655, DateTimeKind.Local).AddTicks(4610), 1, 2 },
                    { 2, 50.0m, 2, 25.0m, 199.99m, 1, "company", new DateTime(2025, 7, 7, 10, 41, 58, 655, DateTimeKind.Local).AddTicks(4610), 149.99m, false, null, 2, "1.8.5", new DateTime(2025, 6, 27, 10, 41, 58, 655, DateTimeKind.Local).AddTicks(4610), 0, 1 },
                    { 3, 699.99m, 3, 10.0m, 6999.99m, 2, "individual", new DateTime(2025, 7, 25, 10, 41, 58, 655, DateTimeKind.Local).AddTicks(4610), 5994.99m, true, 315.0m, 3, "3.2.1", new DateTime(2025, 7, 2, 10, 41, 58, 655, DateTimeKind.Local).AddTicks(4610), 0, 3 },
                    { 4, 449.99m, 1, 15.0m, 2999.99m, 3, "individual", new DateTime(2025, 6, 23, 10, 41, 58, 655, DateTimeKind.Local).AddTicks(4610), 2549.99m, false, null, 1, "2.1.0", new DateTime(2025, 6, 15, 10, 41, 58, 655, DateTimeKind.Local).AddTicks(4610), 0, 1 }
                });

            migrationBuilder.InsertData(
                table: "Payment",
                columns: new[] { "Id", "Amount", "ContractId", "PaymentMethod", "Status" },
                values: new object[,]
                {
                    { 1, 1500.00m, 1, "Credit Card", 1 },
                    { 2, 1559.99m, 1, "Bank Transfer", 1 },
                    { 3, 75.00m, 2, "Credit Card", 1 },
                    { 4, 2000.00m, 3, "Bank Transfer", 1 },
                    { 5, 1000.00m, 4, "Credit Card", 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyClient_Email",
                table: "CompanyClient",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyClient_KrsNumber",
                table: "CompanyClient",
                column: "KrsNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contract_AppliedDiscountId",
                table: "Contract",
                column: "AppliedDiscountId");

            migrationBuilder.CreateIndex(
                name: "IX_Contract_SoftwareSystemId",
                table: "Contract",
                column: "SoftwareSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_Discount_SoftwareSystemId",
                table: "Discount",
                column: "SoftwareSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_Login",
                table: "Employee",
                column: "Login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IndividualClient_Email",
                table: "IndividualClient",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IndividualClient_Pesel",
                table: "IndividualClient",
                column: "Pesel",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payment_ContractId",
                table: "Payment",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_SoftwareSystem_Name",
                table: "SoftwareSystem",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyClient");

            migrationBuilder.DropTable(
                name: "Employee");

            migrationBuilder.DropTable(
                name: "IndividualClient");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "Contract");

            migrationBuilder.DropTable(
                name: "Discount");

            migrationBuilder.DropTable(
                name: "SoftwareSystem");
        }
    }
}
