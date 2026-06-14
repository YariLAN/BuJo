using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuJo.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_habit_log_habits_habit_id",
                table: "habit_log");

            migrationBuilder.DropPrimaryKey(
                name: "pk_habit_log",
                table: "habit_log");

            migrationBuilder.DropIndex(
                name: "ix_habit_log_habit_id",
                table: "habit_log");

            migrationBuilder.RenameTable(
                name: "habit_log",
                newName: "habit_logs");

            migrationBuilder.AlterColumn<bool>(
                name: "is_completed",
                table: "habit_logs",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddPrimaryKey(
                name: "pk_habit_logs",
                table: "habit_logs",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_habit_logs_date",
                table: "habit_logs",
                column: "date");

            migrationBuilder.CreateIndex(
                name: "ix_habit_logs_habit_id_date",
                table: "habit_logs",
                columns: new[] { "habit_id", "date" });

            migrationBuilder.AddForeignKey(
                name: "fk_habit_logs_habits_habit_id",
                table: "habit_logs",
                column: "habit_id",
                principalTable: "habits",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_habit_logs_habits_habit_id",
                table: "habit_logs");

            migrationBuilder.DropPrimaryKey(
                name: "pk_habit_logs",
                table: "habit_logs");

            migrationBuilder.DropIndex(
                name: "ix_habit_logs_date",
                table: "habit_logs");

            migrationBuilder.DropIndex(
                name: "ix_habit_logs_habit_id_date",
                table: "habit_logs");

            migrationBuilder.RenameTable(
                name: "habit_logs",
                newName: "habit_log");

            migrationBuilder.AlterColumn<bool>(
                name: "is_completed",
                table: "habit_log",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "pk_habit_log",
                table: "habit_log",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_habit_log_habit_id",
                table: "habit_log",
                column: "habit_id");

            migrationBuilder.AddForeignKey(
                name: "fk_habit_log_habits_habit_id",
                table: "habit_log",
                column: "habit_id",
                principalTable: "habits",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
