using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmHelm.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialAgriculturalCore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "farm",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    default_currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    time_zone = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_farm", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "crop",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    crop_code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    scientific_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_crop", x => x.id);
                    table.ForeignKey(
                        name: "FK_crop_farm_farm_id",
                        column: x => x.farm_id,
                        principalTable: "farm",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "farm_location",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    location_code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_farm_location", x => x.id);
                    table.ForeignKey(
                        name: "FK_farm_location_farm_farm_id",
                        column: x => x.farm_id,
                        principalTable: "farm",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "mortality_reason",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mortality_reason", x => x.id);
                    table.CheckConstraint("ck_mortality_reason_display_order_non_negative", "display_order >= 0");
                    table.ForeignKey(
                        name: "FK_mortality_reason_farm_farm_id",
                        column: x => x.farm_id,
                        principalTable: "farm",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "crop_stage",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    crop_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_crop_stage", x => x.id);
                    table.CheckConstraint("ck_crop_stage_display_order_non_negative", "display_order >= 0");
                    table.ForeignKey(
                        name: "FK_crop_stage_crop_crop_id",
                        column: x => x.crop_id,
                        principalTable: "crop",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "variety",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    crop_id = table.Column<Guid>(type: "uuid", nullable: false),
                    variety_code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_variety", x => x.id);
                    table.ForeignKey(
                        name: "FK_variety_crop_crop_id",
                        column: x => x.crop_id,
                        principalTable: "crop",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "batch",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    batch_code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    variety_id = table.Column<Guid>(type: "uuid", nullable: false),
                    location_id = table.Column<Guid>(type: "uuid", nullable: true),
                    planting_date = table.Column<DateOnly>(type: "date", nullable: false),
                    initial_plant_count = table.Column<int>(type: "integer", nullable: false),
                    current_stage_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    individual_tracking_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    removed_date = table.Column<DateOnly>(type: "date", nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_batch", x => x.id);
                    table.CheckConstraint("ck_batch_initial_plant_count_positive", "initial_plant_count > 0");
                    table.ForeignKey(
                        name: "FK_batch_crop_stage_current_stage_id",
                        column: x => x.current_stage_id,
                        principalTable: "crop_stage",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_batch_farm_farm_id",
                        column: x => x.farm_id,
                        principalTable: "farm",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_batch_farm_location_location_id",
                        column: x => x.location_id,
                        principalTable: "farm_location",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_batch_variety_variety_id",
                        column: x => x.variety_id,
                        principalTable: "variety",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "batch_stage_history",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    batch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    stage_id = table.Column<Guid>(type: "uuid", nullable: false),
                    effective_date = table.Column<DateOnly>(type: "date", nullable: false),
                    notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_batch_stage_history", x => x.id);
                    table.ForeignKey(
                        name: "FK_batch_stage_history_batch_batch_id",
                        column: x => x.batch_id,
                        principalTable: "batch",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_batch_stage_history_crop_stage_stage_id",
                        column: x => x.stage_id,
                        principalTable: "crop_stage",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "plant",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    batch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    plant_number = table.Column<int>(type: "integer", nullable: false),
                    plant_code = table.Column<string>(type: "character varying(48)", maxLength: 48, nullable: false),
                    status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plant", x => x.id);
                    table.UniqueConstraint("AK_plant_id_batch_id", x => new { x.id, x.batch_id });
                    table.CheckConstraint("ck_plant_plant_number_positive", "plant_number > 0");
                    table.ForeignKey(
                        name: "FK_plant_batch_batch_id",
                        column: x => x.batch_id,
                        principalTable: "batch",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "mortality_record",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    batch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    plant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    mortality_date = table.Column<DateOnly>(type: "date", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    reason_id = table.Column<Guid>(type: "uuid", nullable: false),
                    notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mortality_record", x => x.id);
                    table.CheckConstraint("ck_mortality_record_quantity_positive", "quantity > 0");
                    table.ForeignKey(
                        name: "FK_mortality_record_batch_batch_id",
                        column: x => x.batch_id,
                        principalTable: "batch",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_mortality_record_mortality_reason_reason_id",
                        column: x => x.reason_id,
                        principalTable: "mortality_reason",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_mortality_record_plant_plant_id_batch_id",
                        columns: x => new { x.plant_id, x.batch_id },
                        principalTable: "plant",
                        principalColumns: new[] { "id", "batch_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_batch_batch_code",
                table: "batch",
                column: "batch_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_batch_current_stage_id",
                table: "batch",
                column: "current_stage_id");

            migrationBuilder.CreateIndex(
                name: "IX_batch_farm_id",
                table: "batch",
                column: "farm_id");

            migrationBuilder.CreateIndex(
                name: "IX_batch_location_id",
                table: "batch",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "IX_batch_planting_date",
                table: "batch",
                column: "planting_date");

            migrationBuilder.CreateIndex(
                name: "IX_batch_status",
                table: "batch",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_batch_variety_id",
                table: "batch",
                column: "variety_id");

            migrationBuilder.CreateIndex(
                name: "IX_batch_stage_history_batch_id",
                table: "batch_stage_history",
                column: "batch_id");

            migrationBuilder.CreateIndex(
                name: "IX_batch_stage_history_effective_date",
                table: "batch_stage_history",
                column: "effective_date");

            migrationBuilder.CreateIndex(
                name: "IX_batch_stage_history_stage_id",
                table: "batch_stage_history",
                column: "stage_id");

            migrationBuilder.CreateIndex(
                name: "IX_crop_crop_code",
                table: "crop",
                column: "crop_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_crop_farm_id",
                table: "crop",
                column: "farm_id");

            migrationBuilder.CreateIndex(
                name: "IX_crop_farm_id_name",
                table: "crop",
                columns: new[] { "farm_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_crop_stage_crop_id",
                table: "crop_stage",
                column: "crop_id");

            migrationBuilder.CreateIndex(
                name: "IX_crop_stage_crop_id_name",
                table: "crop_stage",
                columns: new[] { "crop_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_farm_farm_code",
                table: "farm",
                column: "farm_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_farm_location_farm_id",
                table: "farm_location",
                column: "farm_id");

            migrationBuilder.CreateIndex(
                name: "IX_farm_location_farm_id_name",
                table: "farm_location",
                columns: new[] { "farm_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_farm_location_location_code",
                table: "farm_location",
                column: "location_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_mortality_reason_farm_id",
                table: "mortality_reason",
                column: "farm_id");

            migrationBuilder.CreateIndex(
                name: "IX_mortality_reason_farm_id_name",
                table: "mortality_reason",
                columns: new[] { "farm_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_mortality_record_batch_id",
                table: "mortality_record",
                column: "batch_id");

            migrationBuilder.CreateIndex(
                name: "IX_mortality_record_mortality_date",
                table: "mortality_record",
                column: "mortality_date");

            migrationBuilder.CreateIndex(
                name: "IX_mortality_record_plant_id",
                table: "mortality_record",
                column: "plant_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_mortality_record_plant_id_batch_id",
                table: "mortality_record",
                columns: new[] { "plant_id", "batch_id" });

            migrationBuilder.CreateIndex(
                name: "IX_mortality_record_reason_id",
                table: "mortality_record",
                column: "reason_id");

            migrationBuilder.CreateIndex(
                name: "IX_plant_batch_id",
                table: "plant",
                column: "batch_id");

            migrationBuilder.CreateIndex(
                name: "IX_plant_batch_id_plant_number",
                table: "plant",
                columns: new[] { "batch_id", "plant_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_plant_plant_code",
                table: "plant",
                column: "plant_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_variety_crop_id",
                table: "variety",
                column: "crop_id");

            migrationBuilder.CreateIndex(
                name: "IX_variety_crop_id_name",
                table: "variety",
                columns: new[] { "crop_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_variety_variety_code",
                table: "variety",
                column: "variety_code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "batch_stage_history");

            migrationBuilder.DropTable(
                name: "mortality_record");

            migrationBuilder.DropTable(
                name: "mortality_reason");

            migrationBuilder.DropTable(
                name: "plant");

            migrationBuilder.DropTable(
                name: "batch");

            migrationBuilder.DropTable(
                name: "crop_stage");

            migrationBuilder.DropTable(
                name: "farm_location");

            migrationBuilder.DropTable(
                name: "variety");

            migrationBuilder.DropTable(
                name: "crop");

            migrationBuilder.DropTable(
                name: "farm");
        }
    }
}
