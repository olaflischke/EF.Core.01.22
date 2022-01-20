using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NorthwindDal.Migrations
{
    public partial class TimeStampForOrders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "TimeStamp",
                table: "Orders",
                type: "rowversion",
                rowVersion: true,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeStamp",
                table: "Orders");
        }
    }
}
