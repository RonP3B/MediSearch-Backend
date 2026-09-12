using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediSearch.Infrastructure.Persistence.Shared.Migrations
{
    /// <summary>
    /// Gives every text value that needs case-insensitive uniqueness a normalized companion
    /// column, the way <c>username</c> / <c>normalized_username</c> already worked, and moves
    /// the unique indexes onto it. The original column now keeps the casing the user typed
    /// instead of being lower-cased on the way in.
    /// </summary>
    /// <inheritdoc />
    public partial class NormalizeUniqueTextColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Each normalized column is added as nullable, backfilled, and only then made
            // required: the product classification catalog is seeded by an earlier migration,
            // so these tables are never guaranteed to be empty when this runs.
            migrationBuilder.AddColumn<string>(
                name: "normalized_email",
                table: "users",
                type: "character varying(320)",
                maxLength: 320,
                nullable: true
            );

            migrationBuilder.Sql(
                @"UPDATE users SET normalized_email = lower(email);"
            );

            migrationBuilder.AlterColumn<string>(
                name: "normalized_email",
                table: "users",
                type: "character varying(320)",
                maxLength: 320,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(320)",
                oldMaxLength: 320,
                oldNullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "normalized_name",
                table: "companies",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true
            );

            migrationBuilder.Sql(
                @"UPDATE companies SET normalized_name = lower(name);"
            );

            migrationBuilder.AlterColumn<string>(
                name: "normalized_name",
                table: "companies",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "normalized_email",
                table: "companies",
                type: "character varying(320)",
                maxLength: 320,
                nullable: true
            );

            migrationBuilder.Sql(
                @"UPDATE companies SET normalized_email = lower(email);"
            );

            migrationBuilder.AlterColumn<string>(
                name: "normalized_email",
                table: "companies",
                type: "character varying(320)",
                maxLength: 320,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(320)",
                oldMaxLength: 320,
                oldNullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "normalized_name",
                table: "products",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true
            );

            migrationBuilder.Sql(
                @"UPDATE products SET normalized_name = lower(name);"
            );

            migrationBuilder.AlterColumn<string>(
                name: "normalized_name",
                table: "products",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "normalized_name",
                table: "product_classifications",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true
            );

            migrationBuilder.Sql(
                @"UPDATE product_classifications SET normalized_name = lower(name);"
            );

            migrationBuilder.AlterColumn<string>(
                name: "normalized_name",
                table: "product_classifications",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150,
                oldNullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "normalized_name",
                table: "classification_categories",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true
            );

            migrationBuilder.Sql(
                @"UPDATE classification_categories SET normalized_name = lower(name);"
            );

            migrationBuilder.AlterColumn<string>(
                name: "normalized_name",
                table: "classification_categories",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150,
                oldNullable: true
            );

            migrationBuilder.DropIndex(name: "ix_users_email", table: "users");
            migrationBuilder.DropIndex(name: "ix_users_company_id_email", table: "users");
            migrationBuilder.DropIndex(name: "ix_companies_name", table: "companies");
            migrationBuilder.DropIndex(name: "ix_companies_email", table: "companies");
            migrationBuilder.DropIndex(name: "ix_products_company_id_name", table: "products");
            migrationBuilder.DropIndex(name: "ix_product_classifications_name", table: "product_classifications");
            migrationBuilder.DropIndex(name: "ix_classification_categories_classification_id_name", table: "classification_categories");

            migrationBuilder.CreateIndex(
                name: "ix_users_normalized_email",
                table: "users",
                column: "normalized_email",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_users_company_id_normalized_email",
                table: "users",
                columns: ["company_id", "normalized_email"],
                filter: "company_id IS NOT NULL"
            );

            migrationBuilder.CreateIndex(
                name: "ix_companies_normalized_name",
                table: "companies",
                column: "normalized_name",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_companies_normalized_email",
                table: "companies",
                column: "normalized_email",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_products_company_id_normalized_name",
                table: "products",
                columns: ["company_id", "normalized_name"],
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_product_classifications_normalized_name",
                table: "product_classifications",
                column: "normalized_name",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_classification_categories_classification_id_normalized_name",
                table: "classification_categories",
                columns: ["classification_id", "normalized_name"],
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "ix_users_normalized_email", table: "users");
            migrationBuilder.DropIndex(name: "ix_users_company_id_normalized_email", table: "users");
            migrationBuilder.DropIndex(name: "ix_companies_normalized_name", table: "companies");
            migrationBuilder.DropIndex(name: "ix_companies_normalized_email", table: "companies");
            migrationBuilder.DropIndex(name: "ix_products_company_id_normalized_name", table: "products");
            migrationBuilder.DropIndex(name: "ix_product_classifications_normalized_name", table: "product_classifications");
            migrationBuilder.DropIndex(name: "ix_classification_categories_classification_id_normalized_name", table: "classification_categories");

            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                table: "users",
                column: "email",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_users_company_id_email",
                table: "users",
                columns: ["company_id", "email"],
                filter: "company_id IS NOT NULL"
            );

            migrationBuilder.CreateIndex(
                name: "ix_companies_name",
                table: "companies",
                column: "name",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_companies_email",
                table: "companies",
                column: "email",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_products_company_id_name",
                table: "products",
                columns: ["company_id", "name"],
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_product_classifications_name",
                table: "product_classifications",
                column: "name",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_classification_categories_classification_id_name",
                table: "classification_categories",
                columns: ["classification_id", "name"],
                unique: true
            );

            migrationBuilder.DropColumn(name: "normalized_email", table: "users");
            migrationBuilder.DropColumn(name: "normalized_name", table: "companies");
            migrationBuilder.DropColumn(name: "normalized_email", table: "companies");
            migrationBuilder.DropColumn(name: "normalized_name", table: "products");
            migrationBuilder.DropColumn(name: "normalized_name", table: "product_classifications");
            migrationBuilder.DropColumn(name: "normalized_name", table: "classification_categories");
        }
    }
}
