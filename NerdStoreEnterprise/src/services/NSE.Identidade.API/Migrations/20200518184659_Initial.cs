using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace NSE.Identidade.API.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "identityrole",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<string>(nullable: false),
                    name = table.Column<string>(maxLength: 256, nullable: true),
                    normalized_name = table.Column<string>(maxLength: 256, nullable: true),
                    concurrency_stamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "identityuser",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<string>(nullable: false),
                    user_name = table.Column<string>(maxLength: 256, nullable: true),
                    normalized_user_name = table.Column<string>(maxLength: 256, nullable: true),
                    email = table.Column<string>(maxLength: 256, nullable: true),
                    normalized_email = table.Column<string>(maxLength: 256, nullable: true),
                    email_confirmed = table.Column<bool>(nullable: false),
                    password_hash = table.Column<string>(nullable: true),
                    security_stamp = table.Column<string>(nullable: true),
                    concurrency_stamp = table.Column<string>(nullable: true),
                    phone_number = table.Column<string>(nullable: true),
                    phone_number_confirmed = table.Column<bool>(nullable: false),
                    two_factor_enabled = table.Column<bool>(nullable: false),
                    lockout_end = table.Column<DateTimeOffset>(nullable: true),
                    lockout_enabled = table.Column<bool>(nullable: false),
                    access_failed_count = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "identityroleclaim<string>",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_id = table.Column<string>(nullable: false),
                    claim_type = table.Column<string>(nullable: true),
                    claim_value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_role_claims", x => x.id);
                    table.ForeignKey(
                        name: "fk_role_claims_asp_net_roles_identity_role_id",
                        column: x => x.role_id,
                        principalSchema: "public",
                        principalTable: "identityrole",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "identityuserclaim<string>",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<string>(nullable: false),
                    claim_type = table.Column<string>(nullable: true),
                    claim_value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_claims", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_claims_asp_net_users_identity_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "identityuser",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "identityuserlogin<string>",
                schema: "public",
                columns: table => new
                {
                    login_provider = table.Column<string>(maxLength: 128, nullable: false),
                    provider_key = table.Column<string>(maxLength: 128, nullable: false),
                    provider_display_name = table.Column<string>(nullable: true),
                    user_id = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_logins", x => new { x.login_provider, x.provider_key });
                    table.ForeignKey(
                        name: "fk_user_logins_asp_net_users_identity_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "identityuser",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "identityuserrole<string>",
                schema: "public",
                columns: table => new
                {
                    user_id = table.Column<string>(nullable: false),
                    role_id = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_roles", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "fk_user_roles_asp_net_roles_identity_role_id",
                        column: x => x.role_id,
                        principalSchema: "public",
                        principalTable: "identityrole",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_roles_asp_net_users_identity_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "identityuser",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "identityusertoken<string>",
                schema: "public",
                columns: table => new
                {
                    user_id = table.Column<string>(nullable: false),
                    login_provider = table.Column<string>(maxLength: 128, nullable: false),
                    name = table.Column<string>(maxLength: 128, nullable: false),
                    value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_tokens", x => new { x.user_id, x.login_provider, x.name });
                    table.ForeignKey(
                        name: "fk_user_tokens_asp_net_users_identity_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "identityuser",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "public",
                table: "identityrole",
                column: "normalized_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_role_claims_role_id",
                schema: "public",
                table: "identityroleclaim<string>",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "public",
                table: "identityuser",
                column: "normalized_email");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "public",
                table: "identityuser",
                column: "normalized_user_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_claims_user_id",
                schema: "public",
                table: "identityuserclaim<string>",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_logins_user_id",
                schema: "public",
                table: "identityuserlogin<string>",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_roles_role_id",
                schema: "public",
                table: "identityuserrole<string>",
                column: "role_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "identityroleclaim<string>",
                schema: "public");

            migrationBuilder.DropTable(
                name: "identityuserclaim<string>",
                schema: "public");

            migrationBuilder.DropTable(
                name: "identityuserlogin<string>",
                schema: "public");

            migrationBuilder.DropTable(
                name: "identityuserrole<string>",
                schema: "public");

            migrationBuilder.DropTable(
                name: "identityusertoken<string>",
                schema: "public");

            migrationBuilder.DropTable(
                name: "identityrole",
                schema: "public");

            migrationBuilder.DropTable(
                name: "identityuser",
                schema: "public");
        }
    }
}
