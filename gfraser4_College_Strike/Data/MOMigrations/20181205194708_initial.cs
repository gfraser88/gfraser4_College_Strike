using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace gfraser4_College_Strike.Data.MOMigrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "CS");

            migrationBuilder.CreateTable(
                name: "Assignments",
                schema: "CS",
                columns: table => new
                {
                    CreatedBy = table.Column<string>(maxLength: 256, nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(maxLength: 256, nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AssignmentName = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assignments", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Positions",
                schema: "CS",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Members",
                schema: "CS",
                columns: table => new
                {
                    CreatedBy = table.Column<string>(maxLength: 256, nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(maxLength: 256, nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FirstName = table.Column<string>(maxLength: 50, nullable: false),
                    LastName = table.Column<string>(maxLength: 50, nullable: false),
                    Phone = table.Column<long>(nullable: false),
                    eMail = table.Column<string>(maxLength: 255, nullable: false),
                    AssignmentID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Members_Assignments_AssignmentID",
                        column: x => x.AssignmentID,
                        principalSchema: "CS",
                        principalTable: "Assignments",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MemberPositions",
                schema: "CS",
                columns: table => new
                {
                    PositionID = table.Column<int>(nullable: false),
                    MemberID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberPositions", x => new { x.PositionID, x.MemberID });
                    table.ForeignKey(
                        name: "FK_MemberPositions_Members_MemberID",
                        column: x => x.MemberID,
                        principalSchema: "CS",
                        principalTable: "Members",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MemberPositions_Positions_PositionID",
                        column: x => x.PositionID,
                        principalSchema: "CS",
                        principalTable: "Positions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Shifts",
                schema: "CS",
                columns: table => new
                {
                    CreatedBy = table.Column<string>(maxLength: 256, nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(maxLength: 256, nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ShiftDate = table.Column<DateTime>(nullable: false),
                    AssignmentID = table.Column<int>(nullable: false),
                    MemberID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shifts", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Shifts_Assignments_AssignmentID",
                        column: x => x.AssignmentID,
                        principalSchema: "CS",
                        principalTable: "Assignments",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Shifts_Members_MemberID",
                        column: x => x.MemberID,
                        principalSchema: "CS",
                        principalTable: "Members",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_AssignmentName",
                schema: "CS",
                table: "Assignments",
                column: "AssignmentName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MemberPositions_MemberID",
                schema: "CS",
                table: "MemberPositions",
                column: "MemberID");

            migrationBuilder.CreateIndex(
                name: "IX_Members_AssignmentID",
                schema: "CS",
                table: "Members",
                column: "AssignmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Members_eMail",
                schema: "CS",
                table: "Members",
                column: "eMail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Positions_Title",
                schema: "CS",
                table: "Positions",
                column: "Title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_AssignmentID",
                schema: "CS",
                table: "Shifts",
                column: "AssignmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_MemberID_ShiftDate",
                schema: "CS",
                table: "Shifts",
                columns: new[] { "MemberID", "ShiftDate" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MemberPositions",
                schema: "CS");

            migrationBuilder.DropTable(
                name: "Shifts",
                schema: "CS");

            migrationBuilder.DropTable(
                name: "Positions",
                schema: "CS");

            migrationBuilder.DropTable(
                name: "Members",
                schema: "CS");

            migrationBuilder.DropTable(
                name: "Assignments",
                schema: "CS");
        }
    }
}
