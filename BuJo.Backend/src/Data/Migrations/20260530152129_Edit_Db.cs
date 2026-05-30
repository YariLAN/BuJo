using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuJo.Data.Migrations
{
    /// <inheritdoc />
    public partial class Edit_Db : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_sent_reminder",
                table: "tasks",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "reminder_at",
                table: "tasks",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "start_date_time",
                table: "tasks",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_sent_reminder",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "reminder_at",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "start_date_time",
                table: "tasks");
        }
    }
}
