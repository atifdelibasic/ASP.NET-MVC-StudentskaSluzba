﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace StudentskaSluzba.Migrations
{
    public partial class editforeignkey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Student_GodinaStudja_GodinaStudijaId",
                table: "Student");

            migrationBuilder.DropIndex(
                name: "IX_Student_GodinaStudijaId",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "GodinaStudijaId",
                table: "Student");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GodinaStudijaId",
                table: "Student",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Student_GodinaStudijaId",
                table: "Student",
                column: "GodinaStudijaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Student_GodinaStudja_GodinaStudijaId",
                table: "Student",
                column: "GodinaStudijaId",
                principalTable: "GodinaStudja",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
