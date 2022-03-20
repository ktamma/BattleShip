using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    GameId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MoveByPlayer = table.Column<bool>(type: "bit", nullable: false),
                    Board1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Board2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Width = table.Column<int>(type: "int", nullable: false),
                    Height = table.Column<int>(type: "int", nullable: false),
                    Player1Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Player2Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GameName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.GameId);
                });

            migrationBuilder.CreateTable(
                name: "TouchRules",
                columns: table => new
                {
                    TouchRuleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Rule = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TouchRules", x => x.TouchRuleId);
                });

            migrationBuilder.CreateTable(
                name: "Ships",
                columns: table => new
                {
                    ShipId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Lenght = table.Column<int>(type: "int", nullable: true),
                    Height = table.Column<int>(type: "int", nullable: true),
                    Coordinates = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ships1GameId = table.Column<int>(type: "int", nullable: true),
                    Ships2GameId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ships", x => x.ShipId);
                    table.ForeignKey(
                        name: "FK_Ships_Games_Ships1GameId",
                        column: x => x.Ships1GameId,
                        principalTable: "Games",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ships_Games_Ships2GameId",
                        column: x => x.Ships2GameId,
                        principalTable: "Games",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Configs",
                columns: table => new
                {
                    ConfigId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BoardSizeX = table.Column<int>(type: "int", nullable: false),
                    BoardSizeY = table.Column<int>(type: "int", nullable: false),
                    Player1Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Player2Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TouchRuleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configs", x => x.ConfigId);
                    table.ForeignKey(
                        name: "FK_Configs_TouchRules_TouchRuleId",
                        column: x => x.TouchRuleId,
                        principalTable: "TouchRules",
                        principalColumn: "TouchRuleId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShipConfigs",
                columns: table => new
                {
                    ShipConfigId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ShipSizeY = table.Column<int>(type: "int", nullable: false),
                    ShipSizeX = table.Column<int>(type: "int", nullable: false),
                    ConfigId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipConfigs", x => x.ShipConfigId);
                    table.ForeignKey(
                        name: "FK_ShipConfigs_Configs_ConfigId",
                        column: x => x.ConfigId,
                        principalTable: "Configs",
                        principalColumn: "ConfigId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Configs_TouchRuleId",
                table: "Configs",
                column: "TouchRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipConfigs_ConfigId",
                table: "ShipConfigs",
                column: "ConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_Ships_Ships1GameId",
                table: "Ships",
                column: "Ships1GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Ships_Ships2GameId",
                table: "Ships",
                column: "Ships2GameId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShipConfigs");

            migrationBuilder.DropTable(
                name: "Ships");

            migrationBuilder.DropTable(
                name: "Configs");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "TouchRules");
        }
    }
}
