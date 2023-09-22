using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskTrackerServer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCardFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cards_Statuses_StatusId",
                table: "cards");

            migrationBuilder.DropForeignKey(
                name: "FK_cards_Users_LastUserModifiedId",
                table: "cards");

            migrationBuilder.DropForeignKey(
                name: "FK_cards_Users_UserCreatedId",
                table: "cards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_cards",
                table: "cards");

            migrationBuilder.RenameTable(
                name: "cards",
                newName: "Cards");

            migrationBuilder.RenameIndex(
                name: "IX_cards_UserCreatedId",
                table: "Cards",
                newName: "IX_Cards_UserCreatedId");

            migrationBuilder.RenameIndex(
                name: "IX_cards_StatusId",
                table: "Cards",
                newName: "IX_Cards_StatusId");

            migrationBuilder.RenameIndex(
                name: "IX_cards_LastUserModifiedId",
                table: "Cards",
                newName: "IX_Cards_LastUserModifiedId");

            migrationBuilder.AddColumn<int>(
                name: "AssigneeId",
                table: "Cards",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cards",
                table: "Cards",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_AssigneeId",
                table: "Cards",
                column: "AssigneeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_Statuses_StatusId",
                table: "Cards",
                column: "StatusId",
                principalTable: "Statuses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_Users_AssigneeId",
                table: "Cards",
                column: "AssigneeId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_Users_LastUserModifiedId",
                table: "Cards",
                column: "LastUserModifiedId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_Users_UserCreatedId",
                table: "Cards",
                column: "UserCreatedId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cards_Statuses_StatusId",
                table: "Cards");

            migrationBuilder.DropForeignKey(
                name: "FK_Cards_Users_AssigneeId",
                table: "Cards");

            migrationBuilder.DropForeignKey(
                name: "FK_Cards_Users_LastUserModifiedId",
                table: "Cards");

            migrationBuilder.DropForeignKey(
                name: "FK_Cards_Users_UserCreatedId",
                table: "Cards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cards",
                table: "Cards");

            migrationBuilder.DropIndex(
                name: "IX_Cards_AssigneeId",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "AssigneeId",
                table: "Cards");

            migrationBuilder.RenameTable(
                name: "Cards",
                newName: "cards");

            migrationBuilder.RenameIndex(
                name: "IX_Cards_UserCreatedId",
                table: "cards",
                newName: "IX_cards_UserCreatedId");

            migrationBuilder.RenameIndex(
                name: "IX_Cards_StatusId",
                table: "cards",
                newName: "IX_cards_StatusId");

            migrationBuilder.RenameIndex(
                name: "IX_Cards_LastUserModifiedId",
                table: "cards",
                newName: "IX_cards_LastUserModifiedId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_cards",
                table: "cards",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_cards_Statuses_StatusId",
                table: "cards",
                column: "StatusId",
                principalTable: "Statuses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_cards_Users_LastUserModifiedId",
                table: "cards",
                column: "LastUserModifiedId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_cards_Users_UserCreatedId",
                table: "cards",
                column: "UserCreatedId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
