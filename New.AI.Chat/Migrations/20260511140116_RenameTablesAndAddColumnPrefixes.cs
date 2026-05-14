using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace New.AI.Chat.Migrations
{
    /// <inheritdoc />
    public partial class RenameTablesAndAddColumnPrefixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuthenticationLogs",
                table: "AuthenticationLogs");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "Users_USR");

            migrationBuilder.RenameTable(
                name: "AuthenticationLogs",
                newName: "AuthenticationLogs_AL");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Users_USR",
                newName: "USR_USERNAME");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Users_USR",
                newName: "USR_UPDATED_AT");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "Users_USR",
                newName: "USR_PASSWORD_HASH");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Users_USR",
                newName: "USR_IS_ACTIVE");

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "Users_USR",
                newName: "USR_FULL_NAME");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Users_USR",
                newName: "USR_EMAIL");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Users_USR",
                newName: "USR_CREATED_AT");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Users_USR",
                newName: "USR_ID");

            migrationBuilder.RenameIndex(
                name: "IX_Users_Username",
                table: "Users_USR",
                newName: "IX_Users_USR_USR_USERNAME");

            migrationBuilder.RenameIndex(
                name: "IX_Users_IsActive",
                table: "Users_USR",
                newName: "IX_Users_USR_USR_IS_ACTIVE");

            migrationBuilder.RenameIndex(
                name: "IX_Users_Email",
                table: "Users_USR",
                newName: "IX_Users_USR_USR_EMAIL");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "AuthenticationLogs_AL",
                newName: "AL_USERNAME");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "AuthenticationLogs_AL",
                newName: "AL_USER_ID");

            migrationBuilder.RenameColumn(
                name: "TokenExpiresAt",
                table: "AuthenticationLogs_AL",
                newName: "AL_TOKEN_EXPIRES_AT");

            migrationBuilder.RenameColumn(
                name: "Token",
                table: "AuthenticationLogs_AL",
                newName: "AL_TOKEN");

            migrationBuilder.RenameColumn(
                name: "LoginDateTime",
                table: "AuthenticationLogs_AL",
                newName: "AL_LOGIN_DATE_TIME");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "AuthenticationLogs_AL",
                newName: "AL_ID");

            migrationBuilder.RenameIndex(
                name: "IX_AuthenticationLogs_UserId",
                table: "AuthenticationLogs_AL",
                newName: "IX_AuthenticationLogs_AL_AL_USER_ID");

            migrationBuilder.RenameIndex(
                name: "IX_AuthenticationLogs_LoginDateTime",
                table: "AuthenticationLogs_AL",
                newName: "IX_AuthenticationLogs_AL_AL_LOGIN_DATE_TIME");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users_USR",
                table: "Users_USR",
                column: "USR_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuthenticationLogs_AL",
                table: "AuthenticationLogs_AL",
                column: "AL_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_AuthenticationLogs_AL_Users_USR_AL_USER_ID",
                table: "AuthenticationLogs_AL",
                column: "AL_USER_ID",
                principalTable: "Users_USR",
                principalColumn: "USR_ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthenticationLogs_AL_Users_USR_AL_USER_ID",
                table: "AuthenticationLogs_AL");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users_USR",
                table: "Users_USR");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuthenticationLogs_AL",
                table: "AuthenticationLogs_AL");

            migrationBuilder.RenameTable(
                name: "Users_USR",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "AuthenticationLogs_AL",
                newName: "AuthenticationLogs");

            migrationBuilder.RenameColumn(
                name: "USR_USERNAME",
                table: "Users",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "USR_UPDATED_AT",
                table: "Users",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "USR_PASSWORD_HASH",
                table: "Users",
                newName: "PasswordHash");

            migrationBuilder.RenameColumn(
                name: "USR_IS_ACTIVE",
                table: "Users",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "USR_FULL_NAME",
                table: "Users",
                newName: "FullName");

            migrationBuilder.RenameColumn(
                name: "USR_EMAIL",
                table: "Users",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "USR_CREATED_AT",
                table: "Users",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "USR_ID",
                table: "Users",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_Users_USR_USR_USERNAME",
                table: "Users",
                newName: "IX_Users_Username");

            migrationBuilder.RenameIndex(
                name: "IX_Users_USR_USR_IS_ACTIVE",
                table: "Users",
                newName: "IX_Users_IsActive");

            migrationBuilder.RenameIndex(
                name: "IX_Users_USR_USR_EMAIL",
                table: "Users",
                newName: "IX_Users_Email");

            migrationBuilder.RenameColumn(
                name: "AL_USER_ID",
                table: "AuthenticationLogs",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "AL_USERNAME",
                table: "AuthenticationLogs",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "AL_TOKEN_EXPIRES_AT",
                table: "AuthenticationLogs",
                newName: "TokenExpiresAt");

            migrationBuilder.RenameColumn(
                name: "AL_TOKEN",
                table: "AuthenticationLogs",
                newName: "Token");

            migrationBuilder.RenameColumn(
                name: "AL_LOGIN_DATE_TIME",
                table: "AuthenticationLogs",
                newName: "LoginDateTime");

            migrationBuilder.RenameColumn(
                name: "AL_ID",
                table: "AuthenticationLogs",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_AuthenticationLogs_AL_AL_USER_ID",
                table: "AuthenticationLogs",
                newName: "IX_AuthenticationLogs_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AuthenticationLogs_AL_AL_LOGIN_DATE_TIME",
                table: "AuthenticationLogs",
                newName: "IX_AuthenticationLogs_LoginDateTime");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuthenticationLogs",
                table: "AuthenticationLogs",
                column: "Id");
        }
    }
}
