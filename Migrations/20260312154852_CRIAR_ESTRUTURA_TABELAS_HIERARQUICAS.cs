using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace New.AI.Chat.Migrations
{
    /// <inheritdoc />
    public partial class CRIAR_ESTRUTURA_TABELAS_HIERARQUICAS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:pg_trgm", ",,")
                .Annotation("Npgsql:PostgresExtension:vector", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:vector", ",,");

            migrationBuilder.CreateTable(
                name: "KDInformation_KDI",
                columns: table => new
                {
                    KDI_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    KDI_FileName = table.Column<string>(type: "text", nullable: false),
                    KDI_Format = table.Column<string>(type: "text", nullable: false),
                    KDI_Size = table.Column<long>(type: "bigint", nullable: false),
                    KDI_ContextText = table.Column<string>(type: "text", nullable: false),
                    KDI_DateFirstInsertion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    KDI_DateLastInsertion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KDInformation_KDI", x => x.KDI_ID);
                });

            migrationBuilder.CreateTable(
                name: "KDLowGranularity_KDLG",
                columns: table => new
                {
                    KDLG_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    KDLG_InformationId = table.Column<Guid>(type: "uuid", nullable: false),
                    KDLG_ContextText = table.Column<string>(type: "text", nullable: false),
                    KDLG_Embedding = table.Column<Vector>(type: "vector(768)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KDLowGranularity_KDLG", x => x.KDLG_ID);
                    table.ForeignKey(
                        name: "FK_KDLowGranularity_KDLG_KDInformation_KDI_KDLG_InformationId",
                        column: x => x.KDLG_InformationId,
                        principalTable: "KDInformation_KDI",
                        principalColumn: "KDI_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KDHighGranularity_KDHG",
                columns: table => new
                {
                    KDHG_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    KDHG_LowGranularityId = table.Column<Guid>(type: "uuid", nullable: false),
                    KDHG_ContentText = table.Column<string>(type: "text", nullable: false),
                    KDHG_Embedding = table.Column<Vector>(type: "vector(768)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KDHighGranularity_KDHG", x => x.KDHG_ID);
                    table.ForeignKey(
                        name: "FK_KDHighGranularity_KDHG_KDLowGranularity_KDLG_KDHG_LowGranul~",
                        column: x => x.KDHG_LowGranularityId,
                        principalTable: "KDLowGranularity_KDLG",
                        principalColumn: "KDLG_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KDHighGranularity_KDHG_KDHG_ContentText",
                table: "KDHighGranularity_KDHG",
                column: "KDHG_ContentText")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_KDHighGranularity_KDHG_KDHG_Embedding",
                table: "KDHighGranularity_KDHG",
                column: "KDHG_Embedding")
                .Annotation("Npgsql:IndexMethod", "hnsw")
                .Annotation("Npgsql:IndexOperators", new[] { "vector_l2_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_KDHighGranularity_KDHG_KDHG_LowGranularityId",
                table: "KDHighGranularity_KDHG",
                column: "KDHG_LowGranularityId");

            migrationBuilder.CreateIndex(
                name: "IX_KDLowGranularity_KDLG_KDLG_ContextText",
                table: "KDLowGranularity_KDLG",
                column: "KDLG_ContextText")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_KDLowGranularity_KDLG_KDLG_Embedding",
                table: "KDLowGranularity_KDLG",
                column: "KDLG_Embedding")
                .Annotation("Npgsql:IndexMethod", "hnsw")
                .Annotation("Npgsql:IndexOperators", new[] { "vector_l2_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_KDLowGranularity_KDLG_KDLG_InformationId",
                table: "KDLowGranularity_KDLG",
                column: "KDLG_InformationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KDHighGranularity_KDHG");

            migrationBuilder.DropTable(
                name: "KDLowGranularity_KDLG");

            migrationBuilder.DropTable(
                name: "KDInformation_KDI");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:vector", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:pg_trgm", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:vector", ",,");
        }
    }
}
