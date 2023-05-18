using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HumanResourcesDepartment.Migrations
{
    /// <inheritdoc />
    public partial class FirstMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganizationalUnits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EstablishmentYear = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationalUnits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BirthYear = table.Column<int>(type: "int", nullable: false),
                    EmploymentYear = table.Column<int>(type: "int", nullable: false),
                    Salary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_OrganizationalUnits_UnitId",
                        column: x => x.UnitId,
                        principalTable: "OrganizationalUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "OrganizationalUnits",
                columns: new[] { "Id", "EstablishmentYear", "Name" },
                values: new object[,]
                {
                    { 1, 2010, "Administracija" },
                    { 2, 2012, "Računovodstvo" },
                    { 3, 2013, "Razvoj" }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "BirthYear", "EmploymentYear", "FullName", "Role", "Salary", "UnitId" },
                values: new object[,]
                {
                    { 1, 1980, 2010, "Pera Peric", "Direktor", 3000m, 1 },
                    { 2, 1985, 2011, "Mika Mikic", "Sekretar", 1000m, 1 },
                    { 3, 1981, 2012, "Iva Ivic", "Računovođa", 2000m, 2 },
                    { 4, 1982, 2013, "Zika Zikic", "Inženjer", 2500m, 3 },
                    { 5, 1984, 2014, "Ana Anic", "Inženjer", 2500m, 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_UnitId",
                table: "Employees",
                column: "UnitId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "OrganizationalUnits");
        }
    }
}
