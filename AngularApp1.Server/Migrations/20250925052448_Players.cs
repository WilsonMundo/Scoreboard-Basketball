using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AngularApp1.Server.Migrations
{
    /// <inheritdoc />
    public partial class Players : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuarterScore_Games_GameId",
                table: "QuarterScore");

            migrationBuilder.DropForeignKey(
                name: "FK_QuarterScore_Team_TeamId",
                table: "QuarterScore");

            migrationBuilder.DropForeignKey(
                name: "FK_Team_Games_GameId",
                table: "Team");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Team",
                table: "Team");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuarterScore",
                table: "QuarterScore");

            migrationBuilder.RenameTable(
                name: "Team",
                newName: "Teams");

            migrationBuilder.RenameTable(
                name: "QuarterScore",
                newName: "QuarterScores");

            migrationBuilder.RenameIndex(
                name: "IX_Team_GameId",
                table: "Teams",
                newName: "IX_Teams_GameId");

            migrationBuilder.RenameIndex(
                name: "IX_QuarterScore_TeamId",
                table: "QuarterScores",
                newName: "IX_QuarterScores_TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_QuarterScore_GameId_TeamId_QuarterNumber",
                table: "QuarterScores",
                newName: "IX_QuarterScores_GameId_TeamId_QuarterNumber");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Teams",
                table: "Teams",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuarterScores",
                table: "QuarterScores",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Number = table.Column<int>(type: "int", nullable: true),
                    Position = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HeightMeters = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: true),
                    Age = table.Column<int>(type: "int", nullable: true),
                    Nationality = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TeamCatalogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LogoUrl = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamCatalogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GameRosters",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<long>(type: "bigint", nullable: false),
                    TeamId = table.Column<long>(type: "bigint", nullable: false),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameRosters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameRosters_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameRosters_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameRosters_TeamCatalogs_TeamId",
                        column: x => x.TeamId,
                        principalTable: "TeamCatalogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameTeams",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<long>(type: "bigint", nullable: false),
                    TeamId = table.Column<long>(type: "bigint", nullable: false),
                    IsHome = table.Column<bool>(type: "bit", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Fouls = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameTeams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameTeams_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameTeams_TeamCatalogs_TeamId",
                        column: x => x.TeamId,
                        principalTable: "TeamCatalogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameRosters_GameId_TeamId_PlayerId",
                table: "GameRosters",
                columns: new[] { "GameId", "TeamId", "PlayerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameRosters_PlayerId",
                table: "GameRosters",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_GameRosters_TeamId",
                table: "GameRosters",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_GameTeams_GameId_TeamId",
                table: "GameTeams",
                columns: new[] { "GameId", "TeamId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameTeams_TeamId",
                table: "GameTeams",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamCatalogs_Name",
                table: "TeamCatalogs",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_QuarterScores_Games_GameId",
                table: "QuarterScores",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuarterScores_Teams_TeamId",
                table: "QuarterScores",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Games_GameId",
                table: "Teams",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuarterScores_Games_GameId",
                table: "QuarterScores");

            migrationBuilder.DropForeignKey(
                name: "FK_QuarterScores_Teams_TeamId",
                table: "QuarterScores");

            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Games_GameId",
                table: "Teams");

            migrationBuilder.DropTable(
                name: "GameRosters");

            migrationBuilder.DropTable(
                name: "GameTeams");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "TeamCatalogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Teams",
                table: "Teams");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuarterScores",
                table: "QuarterScores");

            migrationBuilder.RenameTable(
                name: "Teams",
                newName: "Team");

            migrationBuilder.RenameTable(
                name: "QuarterScores",
                newName: "QuarterScore");

            migrationBuilder.RenameIndex(
                name: "IX_Teams_GameId",
                table: "Team",
                newName: "IX_Team_GameId");

            migrationBuilder.RenameIndex(
                name: "IX_QuarterScores_TeamId",
                table: "QuarterScore",
                newName: "IX_QuarterScore_TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_QuarterScores_GameId_TeamId_QuarterNumber",
                table: "QuarterScore",
                newName: "IX_QuarterScore_GameId_TeamId_QuarterNumber");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Team",
                table: "Team",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuarterScore",
                table: "QuarterScore",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuarterScore_Games_GameId",
                table: "QuarterScore",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuarterScore_Team_TeamId",
                table: "QuarterScore",
                column: "TeamId",
                principalTable: "Team",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Team_Games_GameId",
                table: "Team",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
