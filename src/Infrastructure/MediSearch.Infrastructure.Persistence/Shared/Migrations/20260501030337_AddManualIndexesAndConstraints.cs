using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediSearch.Infrastructure.Persistence.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddManualIndexesAndConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_users_normalized_username",
                table: "users",
                column: "normalized_username",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_users_company_id_last_name_first_name",
                table: "users",
                columns: ["company_id", "last_name", "first_name"],
                filter: "company_id IS NOT NULL"
            );

            migrationBuilder.CreateIndex(
                name: "ix_chat_room_participants_agent",
                table: "chat_room_participants",
                columns: ["participant_type_id", "participant_id"]
            );

            migrationBuilder.CreateIndex(
                name: "ix_chat_room_participants_room_agent",
                table: "chat_room_participants",
                columns: ["chat_room_id", "participant_type_id", "participant_id"],
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_messages_sender_type_id_sender_id",
                table: "messages",
                columns: ["sender_type_id", "sender_id"]
            );

            migrationBuilder.CreateIndex(
                name: "ix_messages_media_content_asset_key",
                table: "messages",
                column: "media_content_asset_key",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_company_favorites_company_favoriter",
                table: "company_favorites",
                columns: ["company_id", "favoriter_type_id", "favoriter_id"],
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_company_favorites_favoriter_created_at",
                table: "company_favorites",
                columns: ["favoriter_type_id", "favoriter_id", "created_at"]
            );

            migrationBuilder.CreateIndex(
                name: "ix_product_favorites_product_favoriter",
                table: "product_favorites",
                columns: ["product_id", "favoriter_type_id", "favoriter_id"],
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_product_favorites_favoriter_created_at",
                table: "product_favorites",
                columns: ["favoriter_type_id", "favoriter_id", "created_at"]
            );

            migrationBuilder.AddForeignKey(
                name: "fk_products_classification_categories_category_id",
                table: "products_classification_categories",
                column: "category_id",
                principalTable: "classification_categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_products_classification_categories_category_id",
                table: "products_classification_categories"
            );

            migrationBuilder.DropIndex(
                name: "ix_product_favorites_favoriter_created_at",
                table: "product_favorites"
            );

            migrationBuilder.DropIndex(
                name: "ix_product_favorites_product_favoriter",
                table: "product_favorites"
            );

            migrationBuilder.DropIndex(
                name: "ix_company_favorites_favoriter_created_at",
                table: "company_favorites"
            );

            migrationBuilder.DropIndex(
                name: "ix_company_favorites_company_favoriter",
                table: "company_favorites"
            );

            migrationBuilder.DropIndex(
                name: "ix_messages_media_content_asset_key",
                table: "messages"
            );

            migrationBuilder.DropIndex(
                name: "ix_messages_sender_type_id_sender_id",
                table: "messages"
            );

            migrationBuilder.DropIndex(
                name: "ix_chat_room_participants_room_agent",
                table: "chat_room_participants"
            );

            migrationBuilder.DropIndex(
                name: "ix_chat_room_participants_agent",
                table: "chat_room_participants"
            );

            migrationBuilder.DropIndex(
                name: "ix_users_company_id_last_name_first_name",
                table: "users"
            );

            migrationBuilder.DropIndex(name: "ix_users_normalized_username", table: "users");
        }
    }
}
