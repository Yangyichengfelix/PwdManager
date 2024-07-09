using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PwdManager.srv.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Apiusers",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    AzureId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Apiusers", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Coffres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Salt = table.Column<string>(type: "text", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coffres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApiUserCoffres",
                columns: table => new
                {
                    CoffreId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Access = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiUserCoffres", x => new { x.CoffreId, x.UserId });
                    table.ForeignKey(
                        name: "FK_ApiUserCoffres_Apiusers_UserId",
                        column: x => x.UserId,
                        principalTable: "Apiusers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApiUserCoffres_Coffres_CoffreId",
                        column: x => x.CoffreId,
                        principalTable: "Coffres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CoffreLogs",
                columns: table => new
                {
                    CoffreLogId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DateOperation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Operation = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    CoffreName = table.Column<string>(type: "text", nullable: false),
                    CoffreId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoffreLogs", x => x.CoffreLogId);
                    table.ForeignKey(
                        name: "FK_CoffreLogs_Apiusers_UserId",
                        column: x => x.UserId,
                        principalTable: "Apiusers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CoffreLogs_Coffres_CoffreId",
                        column: x => x.CoffreId,
                        principalTable: "Coffres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Entrees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EncryptedLogin = table.Column<string>(type: "text", nullable: false),
                    EncryptedPwd = table.Column<string>(type: "text", nullable: false),
                    EncryptedURL = table.Column<string>(type: "text", nullable: true),
                    IVLogin = table.Column<string>(type: "text", nullable: false),
                    IVPwd = table.Column<string>(type: "text", nullable: false),
                    Icon = table.Column<string>(type: "text", nullable: true),
                    IVUrl = table.Column<string>(type: "text", nullable: true),
                    TagLogin = table.Column<string>(type: "text", nullable: false),
                    TagPwd = table.Column<string>(type: "text", nullable: false),
                    TagUrl = table.Column<string>(type: "text", nullable: true),
                    CoffreId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entrees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Entrees_Coffres_CoffreId",
                        column: x => x.CoffreId,
                        principalTable: "Coffres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntreeLogs",
                columns: table => new
                {
                    EntreeHistoryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EncryptedLogin = table.Column<string>(type: "text", nullable: false),
                    EncryptedPwd = table.Column<string>(type: "text", nullable: false),
                    EncryptedURL = table.Column<string>(type: "text", nullable: true),
                    IVLogin = table.Column<string>(type: "text", nullable: false),
                    IVPwd = table.Column<string>(type: "text", nullable: false),
                    IVUrl = table.Column<string>(type: "text", nullable: true),
                    TagLogin = table.Column<string>(type: "text", nullable: false),
                    TagPwd = table.Column<string>(type: "text", nullable: false),
                    TagUrl = table.Column<string>(type: "text", nullable: true),
                    CoffreId = table.Column<int>(type: "integer", nullable: true),
                    DateOperation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Operation = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    EntreeName = table.Column<string>(type: "text", nullable: false),
                    EntreeId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntreeLogs", x => x.EntreeHistoryId);
                    table.ForeignKey(
                        name: "FK_EntreeLogs_Apiusers_UserId",
                        column: x => x.UserId,
                        principalTable: "Apiusers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntreeLogs_Coffres_CoffreId",
                        column: x => x.CoffreId,
                        principalTable: "Coffres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EntreeLogs_Entrees_EntreeId",
                        column: x => x.EntreeId,
                        principalTable: "Entrees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "Apiusers",
                columns: new[] { "UserId", "AzureId" },
                values: new object[,]
                {
                    { "43c38655-9aa0-48b4-aab1-7cd175500f09", "y" },
                    { "5bda2409-9516-4983-90a3-08363427e744", "r" }
                });

            migrationBuilder.InsertData(
                table: "Coffres",
                columns: new[] { "Id", "Created", "Description", "Modified", "PasswordHash", "Salt", "Title" },
                values: new object[,]
                {
                    { 1, null, "pwd sample", null, "AQAAAAIAAYagAAAAEBJN85RhB6dNhzLF8TIsaQZOAUmG/30o9xcquDlyTuHCehNk1PHbr4O5Qe8r2aDNTQ==", "ets", "sample" },
                    { 2, null, "pwd sample", null, "AQAAAAIAAYagAAAAEPUq1HHrxM3kcxmCLDBW+hTk7L+kkqCJ/kaIGO1rKo/k29p/r1I8nXFNwcQzQ5mimw==", "ets", "sample 2" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiUserCoffres_UserId",
                table: "ApiUserCoffres",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CoffreLogs_CoffreId",
                table: "CoffreLogs",
                column: "CoffreId");

            migrationBuilder.CreateIndex(
                name: "IX_CoffreLogs_UserId",
                table: "CoffreLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EntreeLogs_CoffreId",
                table: "EntreeLogs",
                column: "CoffreId");

            migrationBuilder.CreateIndex(
                name: "IX_EntreeLogs_EntreeId",
                table: "EntreeLogs",
                column: "EntreeId");

            migrationBuilder.CreateIndex(
                name: "IX_EntreeLogs_UserId",
                table: "EntreeLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Entrees_CoffreId",
                table: "Entrees",
                column: "CoffreId");

            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION public.""NotifyLastCoffreLogsChange""()
                 RETURNS trigger
                 LANGUAGE plpgsql
                AS $function$ 
                DECLARE
                  data JSON; notification JSON;
                            BEGIN
                  IF(TG_OP = 'INSERT')     THEN
                    data = row_to_json(NEW);
                    END IF;
                            notification = json_build_object(
                                      'table', TG_TABLE_NAME,
                                      'action', TG_OP,
                                      'data', data);
                            PERFORM pg_notify('lastcoffrelogchange', notification::TEXT);
                            RETURN NEW;
                            END
                $function$
                ;
                    CREATE TRIGGER ""OnLastCoffreLogsChange""
                AFTER INSERT OR DELETE OR UPDATE
                ON public.""CoffreLogs"" 
                FOR EACH ROW
                EXECUTE PROCEDURE public.""NotifyLastCoffreLogsChange""();
                ");
            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION public.""NotifyLastEntreeLogsChange""()
                 RETURNS trigger
                 LANGUAGE plpgsql
                AS $function$ 
                DECLARE
                  data JSON; notification JSON;
                            BEGIN
                  IF(TG_OP = 'INSERT')     THEN
                    data = row_to_json(NEW);
                    END IF;
                            notification = json_build_object(
                                      'table', TG_TABLE_NAME,
                                      'action', TG_OP,
                                      'data', data);
                            PERFORM pg_notify('lastentreelogchange', notification::TEXT);
                            RETURN NEW;
                            END
                $function$
                ;
                    CREATE TRIGGER ""OnLastEntreeLogsChange""
                AFTER INSERT OR DELETE OR UPDATE
                ON public.""EntreeLogs"" 
                FOR EACH ROW
                EXECUTE PROCEDURE public.""NotifyLastEntreeLogsChange""();
                ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiUserCoffres");

            migrationBuilder.DropTable(
                name: "CoffreLogs");

            migrationBuilder.DropTable(
                name: "EntreeLogs");

            migrationBuilder.DropTable(
                name: "Apiusers");

            migrationBuilder.DropTable(
                name: "Entrees");

            migrationBuilder.DropTable(
                name: "Coffres");
        }
    }
}
