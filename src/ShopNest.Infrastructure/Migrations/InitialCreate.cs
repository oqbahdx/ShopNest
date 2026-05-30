using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using ShopNest.Infrastructure.Persistence;

namespace ShopNest.Infrastructure.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260524185229_InitialCreate")]
public class InitialCreate : Migration
{
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.CreateTable("Categories", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uniqueidentifier");
			int? maxLength = 150;
			OperationBuilder<AddColumnOperation> name = table.Column<string>("nvarchar(150)", null, maxLength);
			maxLength = 180;
			OperationBuilder<AddColumnOperation> slug = table.Column<string>("nvarchar(180)", null, maxLength);
			maxLength = 1000;
			OperationBuilder<AddColumnOperation> description = table.Column<string>("nvarchar(1000)", null, maxLength, rowVersion: false, null, nullable: true);
			maxLength = 500;
			return new
			{
				Id = id,
				Name = name,
				Slug = slug,
				Description = description,
				ImageUrl = table.Column<string>("nvarchar(500)", null, maxLength, rowVersion: false, null, nullable: true),
				DisplayOrder = table.Column<int>("int"),
				IsActive = table.Column<bool>("bit"),
				ParentCategoryId = table.Column<Guid>("uniqueidentifier", null, null, rowVersion: false, null, nullable: true),
				IsDeleted = table.Column<bool>("bit"),
				DeletedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
				DeletedBy = table.Column<Guid>("uniqueidentifier", null, null, rowVersion: false, null, nullable: true),
				CreatedAt = table.Column<DateTime>("datetime2"),
				UpdatedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
				CreatedBy = table.Column<Guid>("uniqueidentifier", null, null, rowVersion: false, null, nullable: true),
				UpdatedBy = table.Column<Guid>("uniqueidentifier", null, null, rowVersion: false, null, nullable: true)
			};
		}, null, table =>
		{
			table.PrimaryKey("PK_Categories", x => x.Id);
			table.ForeignKey("FK_Categories_Categories_ParentCategoryId", x => x.ParentCategoryId, "Categories", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
		});
		migrationBuilder.CreateTable("Coupons", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uniqueidentifier");
			int? maxLength = 50;
			OperationBuilder<AddColumnOperation> code = table.Column<string>("nvarchar(50)", null, maxLength);
			maxLength = 300;
			return new
			{
				Id = id,
				Code = code,
				Description = table.Column<string>("nvarchar(300)", null, maxLength, rowVersion: false, null, nullable: true),
				DiscountType = table.Column<int>("int"),
				DiscountValue = table.Column<decimal>("decimal(18,2)"),
				MinimumOrderAmount = table.Column<decimal>("decimal(18,2)"),
				MaximumDiscountAmount = table.Column<decimal>("decimal(18,2)", null, null, rowVersion: false, null, nullable: true),
				UsageLimit = table.Column<int>("int", null, null, rowVersion: false, null, nullable: true),
				UsedCount = table.Column<int>("int"),
				IsOnePerUser = table.Column<bool>("bit"),
				IsActive = table.Column<bool>("bit"),
				ExpiresAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
				CreatedAt = table.Column<DateTime>("datetime2"),
				UpdatedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
				CreatedBy = table.Column<Guid>("uniqueidentifier", null, null, rowVersion: false, null, nullable: true),
				UpdatedBy = table.Column<Guid>("uniqueidentifier", null, null, rowVersion: false, null, nullable: true)
			};
		}, null, table =>
		{
			table.PrimaryKey("PK_Coupons", x => x.Id);
		});
		migrationBuilder.CreateTable("Roles", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uniqueidentifier");
			int? maxLength = 256;
			OperationBuilder<AddColumnOperation> name = table.Column<string>("nvarchar(256)", null, maxLength, rowVersion: false, null, nullable: true);
			maxLength = 256;
			return new
			{
				Id = id,
				Name = name,
				NormalizedName = table.Column<string>("nvarchar(256)", null, maxLength, rowVersion: false, null, nullable: true),
				ConcurrencyStamp = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true)
			};
		}, null, table =>
		{
			table.PrimaryKey("PK_Roles", x => x.Id);
		});
		migrationBuilder.CreateTable("Users", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uniqueidentifier");
			int? maxLength = 100;
			OperationBuilder<AddColumnOperation> firstName = table.Column<string>("nvarchar(100)", null, maxLength);
			maxLength = 100;
			OperationBuilder<AddColumnOperation> lastName = table.Column<string>("nvarchar(100)", null, maxLength);
			maxLength = 500;
			OperationBuilder<AddColumnOperation> avatarUrl = table.Column<string>("nvarchar(500)", null, maxLength, rowVersion: false, null, nullable: true);
			OperationBuilder<AddColumnOperation> isActive = table.Column<bool>("bit");
			OperationBuilder<AddColumnOperation> createdAt = table.Column<DateTime>("datetime2");
			OperationBuilder<AddColumnOperation> updatedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true);
			maxLength = 256;
			OperationBuilder<AddColumnOperation> userName = table.Column<string>("nvarchar(256)", null, maxLength, rowVersion: false, null, nullable: true);
			maxLength = 256;
			OperationBuilder<AddColumnOperation> normalizedUserName = table.Column<string>("nvarchar(256)", null, maxLength, rowVersion: false, null, nullable: true);
			maxLength = 256;
			OperationBuilder<AddColumnOperation> email = table.Column<string>("nvarchar(256)", null, maxLength, rowVersion: false, null, nullable: true);
			maxLength = 256;
			return new
			{
				Id = id,
				FirstName = firstName,
				LastName = lastName,
				AvatarUrl = avatarUrl,
				IsActive = isActive,
				CreatedAt = createdAt,
				UpdatedAt = updatedAt,
				UserName = userName,
				NormalizedUserName = normalizedUserName,
				Email = email,
				NormalizedEmail = table.Column<string>("nvarchar(256)", null, maxLength, rowVersion: false, null, nullable: true),
				EmailConfirmed = table.Column<bool>("bit"),
				PasswordHash = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true),
				SecurityStamp = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true),
				ConcurrencyStamp = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true),
				PhoneNumber = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true),
				PhoneNumberConfirmed = table.Column<bool>("bit"),
				TwoFactorEnabled = table.Column<bool>("bit"),
				LockoutEnd = table.Column<DateTimeOffset>("datetimeoffset", null, null, rowVersion: false, null, nullable: true),
				LockoutEnabled = table.Column<bool>("bit"),
				AccessFailedCount = table.Column<int>("int")
			};
		}, null, table =>
		{
			table.PrimaryKey("PK_Users", x => x.Id);
		});
		migrationBuilder.CreateTable("Products", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uniqueidentifier");
			int? maxLength = 200;
			OperationBuilder<AddColumnOperation> name = table.Column<string>("nvarchar(200)", null, maxLength);
			maxLength = 250;
			OperationBuilder<AddColumnOperation> slug = table.Column<string>("nvarchar(250)", null, maxLength);
			OperationBuilder<AddColumnOperation> description = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true);
			maxLength = 500;
			OperationBuilder<AddColumnOperation> shortDescription = table.Column<string>("nvarchar(500)", null, maxLength, rowVersion: false, null, nullable: true);
			OperationBuilder<AddColumnOperation> price = table.Column<decimal>("decimal(18,2)");
			OperationBuilder<AddColumnOperation> compareAtPrice = table.Column<decimal>("decimal(18,2)", null, null, rowVersion: false, null, nullable: true);
			OperationBuilder<AddColumnOperation> costPrice = table.Column<decimal>("decimal(18,2)", null, null, rowVersion: false, null, nullable: true);
			OperationBuilder<AddColumnOperation> stockQuantity = table.Column<int>("int");
			OperationBuilder<AddColumnOperation> lowStockThreshold = table.Column<int>("int");
			maxLength = 100;
			OperationBuilder<AddColumnOperation> sKU = table.Column<string>("nvarchar(100)", null, maxLength);
			maxLength = 100;
			return new
			{
				Id = id,
				Name = name,
				Slug = slug,
				Description = description,
				ShortDescription = shortDescription,
				Price = price,
				CompareAtPrice = compareAtPrice,
				CostPrice = costPrice,
				StockQuantity = stockQuantity,
				LowStockThreshold = lowStockThreshold,
				SKU = sKU,
				Barcode = table.Column<string>("nvarchar(100)", null, maxLength, rowVersion: false, null, nullable: true),
				Weight = table.Column<decimal>("decimal(10,3)", null, null, rowVersion: false, null, nullable: true),
				CategoryId = table.Column<Guid>("uniqueidentifier"),
				IsFeatured = table.Column<bool>("bit"),
				IsActive = table.Column<bool>("bit"),
				AverageRating = table.Column<decimal>("decimal(3,2)"),
				ReviewCount = table.Column<int>("int"),
				IsDeleted = table.Column<bool>("bit"),
				DeletedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
				DeletedBy = table.Column<Guid>("uniqueidentifier", null, null, rowVersion: false, null, nullable: true),
				CreatedAt = table.Column<DateTime>("datetime2"),
				UpdatedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
				CreatedBy = table.Column<Guid>("uniqueidentifier", null, null, rowVersion: false, null, nullable: true),
				UpdatedBy = table.Column<Guid>("uniqueidentifier", null, null, rowVersion: false, null, nullable: true)
			};
		}, null, table =>
		{
			table.PrimaryKey("PK_Products", x => x.Id);
			table.ForeignKey("FK_Products_Categories_CategoryId", x => x.CategoryId, "Categories", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
		});
		migrationBuilder.CreateTable("RoleClaims", (ColumnsBuilder table) => new
		{
			Id = table.Column<int>("int").Annotation("SqlServer:Identity", "1, 1"),
			RoleId = table.Column<Guid>("uniqueidentifier"),
			ClaimType = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true),
			ClaimValue = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true)
		}, null, table =>
		{
			table.PrimaryKey("PK_RoleClaims", x => x.Id);
			table.ForeignKey("FK_RoleClaims_Roles_RoleId", x => x.RoleId, "Roles", "Id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
		});
		migrationBuilder.CreateTable("Addresses", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uniqueidentifier");
			OperationBuilder<AddColumnOperation> userId = table.Column<Guid>("uniqueidentifier");
			int? maxLength = 150;
			OperationBuilder<AddColumnOperation> fullName = table.Column<string>("nvarchar(150)", null, maxLength);
			maxLength = 30;
			OperationBuilder<AddColumnOperation> phone = table.Column<string>("nvarchar(30)", null, maxLength);
			maxLength = 250;
			OperationBuilder<AddColumnOperation> street = table.Column<string>("nvarchar(250)", null, maxLength);
			maxLength = 100;
			OperationBuilder<AddColumnOperation> city = table.Column<string>("nvarchar(100)", null, maxLength);
			maxLength = 100;
			OperationBuilder<AddColumnOperation> state = table.Column<string>("nvarchar(100)", null, maxLength);
			maxLength = 20;
			OperationBuilder<AddColumnOperation> postalCode = table.Column<string>("nvarchar(20)", null, maxLength);
			maxLength = 100;
			return new
			{
				Id = id,
				UserId = userId,
				FullName = fullName,
				Phone = phone,
				Street = street,
				City = city,
				State = state,
				PostalCode = postalCode,
				Country = table.Column<string>("nvarchar(100)", null, maxLength),
				IsDefault = table.Column<bool>("bit"),
				IsDeleted = table.Column<bool>("bit"),
				DeletedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
				DeletedBy = table.Column<Guid>("uniqueidentifier", null, null, rowVersion: false, null, nullable: true),
				CreatedAt = table.Column<DateTime>("datetime2"),
				UpdatedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
				CreatedBy = table.Column<Guid>("uniqueidentifier", null, null, rowVersion: false, null, nullable: true),
				UpdatedBy = table.Column<Guid>("uniqueidentifier", null, null, rowVersion: false, null, nullable: true)
			};
		}, null, table =>
		{
			table.PrimaryKey("PK_Addresses", x => x.Id);
			table.ForeignKey("FK_Addresses_Users_UserId", x => x.UserId, "Users", "Id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
		});
		migrationBuilder.CreateTable("Carts", (ColumnsBuilder table) => new
		{
			Id = table.Column<Guid>("uniqueidentifier"),
			UserId = table.Column<Guid>("uniqueidentifier"),
			CouponId = table.Column<Guid>("uniqueidentifier", null, null, rowVersion: false, null, nullable: true),
			SubTotal = table.Column<decimal>("decimal(18,2)"),
			DiscountAmount = table.Column<decimal>("decimal(18,2)"),
			ShippingCost = table.Column<decimal>("decimal(18,2)"),
			Total = table.Column<decimal>("decimal(18,2)"),
			CreatedAt = table.Column<DateTime>("datetime2"),
			UpdatedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
			CreatedBy = table.Column<Guid>("uniqueidentifier", null, null, rowVersion: false, null, nullable: true),
			UpdatedBy = table.Column<Guid>("uniqueidentifier", null, null, rowVersion: false, null, nullable: true)
		}, null, table =>
		{
			table.PrimaryKey("PK_Carts", x => x.Id);
			table.ForeignKey("FK_Carts_Coupons_CouponId", x => x.CouponId, "Coupons", "Id", null, ReferentialAction.NoAction, ReferentialAction.SetNull);
			table.ForeignKey("FK_Carts_Users_UserId", x => x.UserId, "Users", "Id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
		});
		migrationBuilder.CreateTable("Notifications", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uniqueidentifier");
			OperationBuilder<AddColumnOperation> userId = table.Column<Guid>("uniqueidentifier");
			int? maxLength = 200;
			OperationBuilder<AddColumnOperation> title = table.Column<string>("nvarchar(200)", null, maxLength);
			maxLength = 1000;
			return new
			{
				Id = id,
				UserId = userId,
				Title = title,
				Message = table.Column<string>("nvarchar(1000)", null, maxLength),
				Type = table.Column<int>("int"),
				IsRead = table.Column<bool>("bit"),
				ReadAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
				Data = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true),
				CreatedAt = table.Column<DateTime>("datetime2")
			};
		}, null, table =>
		{
			table.PrimaryKey("PK_Notifications", x => x.Id);
			table.ForeignKey("FK_Notifications_Users_UserId", x => x.UserId, "Users", "Id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
		});
		migrationBuilder.CreateTable("RefreshTokens", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uniqueidentifier");
			OperationBuilder<AddColumnOperation> userId = table.Column<Guid>("uniqueidentifier");
			int? maxLength = 512;
			OperationBuilder<AddColumnOperation> tokenHash = table.Column<string>("nvarchar(512)", null, maxLength);
			OperationBuilder<AddColumnOperation> expiresAt = table.Column<DateTime>("datetime2");
			OperationBuilder<AddColumnOperation> isRevoked = table.Column<bool>("bit");
			OperationBuilder<AddColumnOperation> revokedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true);
			maxLength = 50;
			OperationBuilder<AddColumnOperation> revokedByIp = table.Column<string>("nvarchar(50)", null, maxLength, rowVersion: false, null, nullable: true);
			maxLength = 512;
			OperationBuilder<AddColumnOperation> replacedByToken = table.Column<string>("nvarchar(512)", null, maxLength, rowVersion: false, null, nullable: true);
			maxLength = 50;
			return new
			{
				Id = id,
				UserId = userId,
				TokenHash = tokenHash,
				ExpiresAt = expiresAt,
				IsRevoked = isRevoked,
				RevokedAt = revokedAt,
				RevokedByIp = revokedByIp,
				ReplacedByToken = replacedByToken,
				CreatedByIp = table.Column<string>("nvarchar(50)", null, maxLength, rowVersion: false, null, nullable: true),
				CreatedAt = table.Column<DateTime>("datetime2")
			};
		}, null, table =>
		{
			table.PrimaryKey("PK_RefreshTokens", x => x.Id);
			table.ForeignKey("FK_RefreshTokens_Users_UserId", x => x.UserId, "Users", "Id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
		});
		migrationBuilder.CreateTable("UserClaims", (ColumnsBuilder table) => new
		{
			Id = table.Column<int>("int").Annotation("SqlServer:Identity", "1, 1"),
			UserId = table.Column<Guid>("uniqueidentifier"),
			ClaimType = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true),
			ClaimValue = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true)
		}, null, table =>
		{
			table.PrimaryKey("PK_UserClaims", x => x.Id);
			table.ForeignKey("FK_UserClaims_Users_UserId", x => x.UserId, "Users", "Id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
		});
		migrationBuilder.CreateTable("UserLogins", (ColumnsBuilder table) => new
		{
			LoginProvider = table.Column<string>("nvarchar(450)"),
			ProviderKey = table.Column<string>("nvarchar(450)"),
			ProviderDisplayName = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true),
			UserId = table.Column<Guid>("uniqueidentifier")
		}, null, table =>
		{
			table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
			table.ForeignKey("FK_UserLogins_Users_UserId", x => x.UserId, "Users", "Id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
		});
		migrationBuilder.CreateTable("UserRoles", (ColumnsBuilder table) => new
		{
			UserId = table.Column<Guid>("uniqueidentifier"),
			RoleId = table.Column<Guid>("uniqueidentifier")
		}, null, table =>
		{
			table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
			table.ForeignKey("FK_UserRoles_Roles_RoleId", x => x.RoleId, "Roles", "Id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
			table.ForeignKey("FK_UserRoles_Users_UserId", x => x.UserId, "Users", "Id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
		});
		migrationBuilder.CreateTable("UserTokens", (ColumnsBuilder table) => new
		{
			UserId = table.Column<Guid>("uniqueidentifier"),
			LoginProvider = table.Column<string>("nvarchar(450)"),
			Name = table.Column<string>("nvarchar(450)"),
			Value = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true)
		}, null, table =>
		{
			table.PrimaryKey("PK_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
			table.ForeignKey("FK_UserTokens_Users_UserId", x => x.UserId, "Users", "Id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
		});
		migrationBuilder.CreateTable("ProductImages", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uniqueidentifier");
			OperationBuilder<AddColumnOperation> productId = table.Column<Guid>("uniqueidentifier");
			int? maxLength = 500;
			OperationBuilder<AddColumnOperation> imageUrl = table.Column<string>("nvarchar(500)", null, maxLength);
			maxLength = 500;
			OperationBuilder<AddColumnOperation> thumbnailUrl = table.Column<string>("nvarchar(500)", null, maxLength, rowVersion: false, null, nullable: true);
			maxLength = 250;
			return new
			{
				Id = id,
				ProductId = productId,
				ImageUrl = imageUrl,
				ThumbnailUrl = thumbnailUrl,
				AltText = table.Column<string>("nvarchar(250)", null, maxLength, rowVersion: false, null, nullable: true),
				DisplayOrder = table.Column<int>("int"),
				IsPrimary = table.Column<bool>("bit")
			};
		}, null, table =>
		{
			table.PrimaryKey("PK_ProductImages", x => x.Id);
			table.ForeignKey("FK_ProductImages_Products_ProductId", x => x.ProductId, "Products", "Id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
		});
		migrationBuilder.CreateTable("Reviews", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uniqueidentifier");
			OperationBuilder<AddColumnOperation> productId = table.Column<Guid>("uniqueidentifier");
			OperationBuilder<AddColumnOperation> userId = table.Column<Guid>("uniqueidentifier");
			OperationBuilder<AddColumnOperation> rating = table.Column<int>("int");
			int? maxLength = 200;
			OperationBuilder<AddColumnOperation> title = table.Column<string>("nvarchar(200)", null, maxLength, rowVersion: false, null, nullable: true);
			maxLength = 2000;
			OperationBuilder<AddColumnOperation> comment = table.Column<string>("nvarchar(2000)", null, maxLength, rowVersion: false, null, nullable: true);
			OperationBuilder<AddColumnOperation> status = table.Column<int>("int");
			OperationBuilder<AddColumnOperation> isVerifiedPurchase = table.Column<bool>("bit");
			maxLength = 500;
			return new
			{
				Id = id,
				ProductId = productId,
				UserId = userId,
				Rating = rating,
				Title = title,
				Comment = comment,
				Status = status,
				IsVerifiedPurchase = isVerifiedPurchase,
				AdminNote = table.Column<string>("nvarchar(500)", null, maxLength, rowVersion: false, null, nullable: true),
				ApprovedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
				ApprovedBy = table.Column<Guid>("uniqueidentifier", null, null, rowVersion: false, null, nullable: true),
				IsDeleted = table.Column<bool>("bit"),
				DeletedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
				DeletedBy = table.Column<Guid>("uniqueidentifier", null, null, rowVersion: false, null, nullable: true),
				CreatedAt = table.Column<DateTime>("datetime2"),
				UpdatedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
				CreatedBy = table.Column<Guid>("uniqueidentifier", null, null, rowVersion: false, null, nullable: true),
				UpdatedBy = table.Column<Guid>("uniqueidentifier", null, null, rowVersion: false, null, nullable: true)
			};
		}, null, table =>
		{
			table.PrimaryKey("PK_Reviews", x => x.Id);
			table.ForeignKey("FK_Reviews_Products_ProductId", x => x.ProductId, "Products", "Id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
			table.ForeignKey("FK_Reviews_Users_UserId", x => x.UserId, "Users", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
		});
		migrationBuilder.CreateTable("Orders", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uniqueidentifier");
			int? maxLength = 50;
			OperationBuilder<AddColumnOperation> orderNumber = table.Column<string>("nvarchar(50)", null, maxLength);
			OperationBuilder<AddColumnOperation> userId = table.Column<Guid>("uniqueidentifier");
			OperationBuilder<AddColumnOperation> status = table.Column<int>("int");
			OperationBuilder<AddColumnOperation> subTotal = table.Column<decimal>("decimal(18,2)");
			OperationBuilder<AddColumnOperation> discountAmount = table.Column<decimal>("decimal(18,2)");
			OperationBuilder<AddColumnOperation> shippingCost = table.Column<decimal>("decimal(18,2)");
			OperationBuilder<AddColumnOperation> taxAmount = table.Column<decimal>("decimal(18,2)");
			OperationBuilder<AddColumnOperation> totalAmount = table.Column<decimal>("decimal(18,2)");
			OperationBuilder<AddColumnOperation> shippingAddressId = table.Column<Guid>("uniqueidentifier");
			OperationBuilder<AddColumnOperation> couponId = table.Column<Guid>("uniqueidentifier", null, null, rowVersion: false, null, nullable: true);
			maxLength = 100;
			OperationBuilder<AddColumnOperation> trackingNumber = table.Column<string>("nvarchar(100)", null, maxLength, rowVersion: false, null, nullable: true);
			maxLength = 500;
			OperationBuilder<AddColumnOperation> notes = table.Column<string>("nvarchar(500)", null, maxLength, rowVersion: false, null, nullable: true);
			maxLength = 500;
			return new
			{
				Id = id,
				OrderNumber = orderNumber,
				UserId = userId,
				Status = status,
				SubTotal = subTotal,
				DiscountAmount = discountAmount,
				ShippingCost = shippingCost,
				TaxAmount = taxAmount,
				TotalAmount = totalAmount,
				ShippingAddressId = shippingAddressId,
				CouponId = couponId,
				TrackingNumber = trackingNumber,
				Notes = notes,
				CancelReason = table.Column<string>("nvarchar(500)", null, maxLength, rowVersion: false, null, nullable: true),
				CancelledAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
				ShippedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
				DeliveredAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
				IsDeleted = table.Column<bool>("bit"),
				DeletedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
				DeletedBy = table.Column<Guid>("uniqueidentifier", null, null, rowVersion: false, null, nullable: true),
				CreatedAt = table.Column<DateTime>("datetime2"),
				UpdatedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
				CreatedBy = table.Column<Guid>("uniqueidentifier", null, null, rowVersion: false, null, nullable: true),
				UpdatedBy = table.Column<Guid>("uniqueidentifier", null, null, rowVersion: false, null, nullable: true)
			};
		}, null, table =>
		{
			table.PrimaryKey("PK_Orders", x => x.Id);
			table.ForeignKey("FK_Orders_Addresses_ShippingAddressId", x => x.ShippingAddressId, "Addresses", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
			table.ForeignKey("FK_Orders_Coupons_CouponId", x => x.CouponId, "Coupons", "Id", null, ReferentialAction.NoAction, ReferentialAction.SetNull);
			table.ForeignKey("FK_Orders_Users_UserId", x => x.UserId, "Users", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
		});
		migrationBuilder.CreateTable("CartItems", (ColumnsBuilder table) => new
		{
			Id = table.Column<Guid>("uniqueidentifier"),
			CartId = table.Column<Guid>("uniqueidentifier"),
			ProductId = table.Column<Guid>("uniqueidentifier"),
			Quantity = table.Column<int>("int"),
			UnitPrice = table.Column<decimal>("decimal(18,2)"),
			TotalPrice = table.Column<decimal>("decimal(18,2)")
		}, null, table =>
		{
			table.PrimaryKey("PK_CartItems", x => x.Id);
			table.ForeignKey("FK_CartItems_Carts_CartId", x => x.CartId, "Carts", "Id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
			table.ForeignKey("FK_CartItems_Products_ProductId", x => x.ProductId, "Products", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
		});
		migrationBuilder.CreateTable("OrderItems", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uniqueidentifier");
			OperationBuilder<AddColumnOperation> orderId = table.Column<Guid>("uniqueidentifier");
			OperationBuilder<AddColumnOperation> productId = table.Column<Guid>("uniqueidentifier");
			int? maxLength = 200;
			OperationBuilder<AddColumnOperation> productName = table.Column<string>("nvarchar(200)", null, maxLength);
			maxLength = 500;
			OperationBuilder<AddColumnOperation> productImageUrl = table.Column<string>("nvarchar(500)", null, maxLength, rowVersion: false, null, nullable: true);
			maxLength = 100;
			return new
			{
				Id = id,
				OrderId = orderId,
				ProductId = productId,
				ProductName = productName,
				ProductImageUrl = productImageUrl,
				ProductSKU = table.Column<string>("nvarchar(100)", null, maxLength),
				UnitPrice = table.Column<decimal>("decimal(18,2)"),
				Quantity = table.Column<int>("int"),
				TotalPrice = table.Column<decimal>("decimal(18,2)")
			};
		}, null, table =>
		{
			table.PrimaryKey("PK_OrderItems", x => x.Id);
			table.ForeignKey("FK_OrderItems_Orders_OrderId", x => x.OrderId, "Orders", "Id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
			table.ForeignKey("FK_OrderItems_Products_ProductId", x => x.ProductId, "Products", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
		});
		migrationBuilder.CreateTable("Payments", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uniqueidentifier");
			OperationBuilder<AddColumnOperation> orderId = table.Column<Guid>("uniqueidentifier");
			OperationBuilder<AddColumnOperation> amount = table.Column<decimal>("decimal(18,2)");
			OperationBuilder<AddColumnOperation> status = table.Column<int>("int");
			OperationBuilder<AddColumnOperation> method = table.Column<int>("int");
			int? maxLength = 200;
			OperationBuilder<AddColumnOperation> stripePaymentIntentId = table.Column<string>("nvarchar(200)", null, maxLength, rowVersion: false, null, nullable: true);
			maxLength = 200;
			OperationBuilder<AddColumnOperation> stripeChargeId = table.Column<string>("nvarchar(200)", null, maxLength, rowVersion: false, null, nullable: true);
			maxLength = 200;
			OperationBuilder<AddColumnOperation> stripeCustomerId = table.Column<string>("nvarchar(200)", null, maxLength, rowVersion: false, null, nullable: true);
			maxLength = 500;
			OperationBuilder<AddColumnOperation> failureReason = table.Column<string>("nvarchar(500)", null, maxLength, rowVersion: false, null, nullable: true);
			OperationBuilder<AddColumnOperation> paidAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true);
			OperationBuilder<AddColumnOperation> refundedAmount = table.Column<decimal>("decimal(18,2)");
			OperationBuilder<AddColumnOperation> refundedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true);
			maxLength = 500;
			return new
			{
				Id = id,
				OrderId = orderId,
				Amount = amount,
				Status = status,
				Method = method,
				StripePaymentIntentId = stripePaymentIntentId,
				StripeChargeId = stripeChargeId,
				StripeCustomerId = stripeCustomerId,
				FailureReason = failureReason,
				PaidAt = paidAt,
				RefundedAmount = refundedAmount,
				RefundedAt = refundedAt,
				RefundReason = table.Column<string>("nvarchar(500)", null, maxLength, rowVersion: false, null, nullable: true),
				CreatedAt = table.Column<DateTime>("datetime2"),
				UpdatedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
				CreatedBy = table.Column<Guid>("uniqueidentifier", null, null, rowVersion: false, null, nullable: true),
				UpdatedBy = table.Column<Guid>("uniqueidentifier", null, null, rowVersion: false, null, nullable: true)
			};
		}, null, table =>
		{
			table.PrimaryKey("PK_Payments", x => x.Id);
			table.ForeignKey("FK_Payments_Orders_OrderId", x => x.OrderId, "Orders", "Id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
		});
		migrationBuilder.CreateIndex("IX_Addresses_UserId", "Addresses", "UserId");
		migrationBuilder.CreateIndex("IX_CartItems_CartId_ProductId", "CartItems", new string[2] { "CartId", "ProductId" }, null, unique: true);
		migrationBuilder.CreateIndex("IX_CartItems_ProductId", "CartItems", "ProductId");
		migrationBuilder.CreateIndex("IX_Carts_CouponId", "Carts", "CouponId");
		migrationBuilder.CreateIndex("IX_Carts_UserId", "Carts", "UserId", null, unique: true);
		migrationBuilder.CreateIndex("IX_Categories_IsActive", "Categories", "IsActive");
		migrationBuilder.CreateIndex("IX_Categories_ParentCategoryId", "Categories", "ParentCategoryId");
		migrationBuilder.CreateIndex("IX_Categories_Slug", "Categories", "Slug", null, unique: true);
		migrationBuilder.CreateIndex("IX_Coupons_Code", "Coupons", "Code", null, unique: true);
		migrationBuilder.CreateIndex("IX_Coupons_ExpiresAt", "Coupons", "ExpiresAt");
		migrationBuilder.CreateIndex("IX_Coupons_IsActive", "Coupons", "IsActive");
		migrationBuilder.CreateIndex("IX_Notifications_CreatedAt", "Notifications", "CreatedAt");
		migrationBuilder.CreateIndex("IX_Notifications_UserId", "Notifications", "UserId");
		migrationBuilder.CreateIndex("IX_Notifications_UserId_IsRead", "Notifications", new string[2] { "UserId", "IsRead" });
		migrationBuilder.CreateIndex("IX_OrderItems_OrderId", "OrderItems", "OrderId");
		migrationBuilder.CreateIndex("IX_OrderItems_ProductId", "OrderItems", "ProductId");
		migrationBuilder.CreateIndex("IX_Orders_CouponId", "Orders", "CouponId");
		migrationBuilder.CreateIndex("IX_Orders_CreatedAt", "Orders", "CreatedAt");
		migrationBuilder.CreateIndex("IX_Orders_OrderNumber", "Orders", "OrderNumber", null, unique: true);
		migrationBuilder.CreateIndex("IX_Orders_ShippingAddressId", "Orders", "ShippingAddressId");
		migrationBuilder.CreateIndex("IX_Orders_Status", "Orders", "Status");
		migrationBuilder.CreateIndex("IX_Orders_UserId", "Orders", "UserId");
		migrationBuilder.CreateIndex("IX_Payments_OrderId", "Payments", "OrderId", null, unique: true);
		migrationBuilder.CreateIndex("IX_Payments_StripePaymentIntentId", "Payments", "StripePaymentIntentId");
		migrationBuilder.CreateIndex("IX_ProductImages_ProductId_DisplayOrder", "ProductImages", new string[2] { "ProductId", "DisplayOrder" });
		migrationBuilder.CreateIndex("IX_Products_CategoryId", "Products", "CategoryId");
		migrationBuilder.CreateIndex("IX_Products_IsActive_IsDeleted_CreatedAt", "Products", new string[3] { "IsActive", "IsDeleted", "CreatedAt" });
		migrationBuilder.CreateIndex("IX_Products_SKU", "Products", "SKU", null, unique: true);
		migrationBuilder.CreateIndex("IX_Products_Slug", "Products", "Slug", null, unique: true);
		migrationBuilder.CreateIndex("IX_RefreshTokens_ExpiresAt", "RefreshTokens", "ExpiresAt");
		migrationBuilder.CreateIndex("IX_RefreshTokens_TokenHash", "RefreshTokens", "TokenHash");
		migrationBuilder.CreateIndex("IX_RefreshTokens_UserId", "RefreshTokens", "UserId");
		migrationBuilder.CreateIndex("IX_Reviews_ProductId", "Reviews", "ProductId");
		migrationBuilder.CreateIndex("IX_Reviews_Status", "Reviews", "Status");
		migrationBuilder.CreateIndex("IX_Reviews_UserId_ProductId", "Reviews", new string[2] { "UserId", "ProductId" }, null, unique: true);
		migrationBuilder.CreateIndex("IX_RoleClaims_RoleId", "RoleClaims", "RoleId");
		migrationBuilder.CreateIndex("RoleNameIndex", "Roles", "NormalizedName", null, unique: true, "[NormalizedName] IS NOT NULL");
		migrationBuilder.CreateIndex("IX_UserClaims_UserId", "UserClaims", "UserId");
		migrationBuilder.CreateIndex("IX_UserLogins_UserId", "UserLogins", "UserId");
		migrationBuilder.CreateIndex("IX_UserRoles_RoleId", "UserRoles", "RoleId");
		migrationBuilder.CreateIndex("EmailIndex", "Users", "NormalizedEmail");
		migrationBuilder.CreateIndex("IX_Users_Email", "Users", "Email", null, unique: true, "[Email] IS NOT NULL");
		migrationBuilder.CreateIndex("UserNameIndex", "Users", "NormalizedUserName", null, unique: true, "[NormalizedUserName] IS NOT NULL");
	}

	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropTable("CartItems");
		migrationBuilder.DropTable("Notifications");
		migrationBuilder.DropTable("OrderItems");
		migrationBuilder.DropTable("Payments");
		migrationBuilder.DropTable("ProductImages");
		migrationBuilder.DropTable("RefreshTokens");
		migrationBuilder.DropTable("Reviews");
		migrationBuilder.DropTable("RoleClaims");
		migrationBuilder.DropTable("UserClaims");
		migrationBuilder.DropTable("UserLogins");
		migrationBuilder.DropTable("UserRoles");
		migrationBuilder.DropTable("UserTokens");
		migrationBuilder.DropTable("Carts");
		migrationBuilder.DropTable("Orders");
		migrationBuilder.DropTable("Products");
		migrationBuilder.DropTable("Roles");
		migrationBuilder.DropTable("Addresses");
		migrationBuilder.DropTable("Coupons");
		migrationBuilder.DropTable("Categories");
		migrationBuilder.DropTable("Users");
	}

	protected override void BuildTargetModel(ModelBuilder modelBuilder)
	{
		modelBuilder.HasAnnotation("ProductVersion", "10.0.8").HasAnnotation("Relational:MaxIdentifierLength", 128);
		modelBuilder.UseIdentityColumns(1L);
		modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole<System.Guid>", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uniqueidentifier");
			b.Property<string>("ConcurrencyStamp").IsConcurrencyToken().HasColumnType("nvarchar(max)");
			b.Property<string>("Name").HasMaxLength(256).HasColumnType("nvarchar(256)");
			b.Property<string>("NormalizedName").HasMaxLength(256).HasColumnType("nvarchar(256)");
			b.HasKey("Id");
			b.HasIndex("NormalizedName").IsUnique().HasDatabaseName("RoleNameIndex")
				.HasFilter("[NormalizedName] IS NOT NULL");
			b.ToTable("Roles", (string?)null);
		});
		modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", delegate(EntityTypeBuilder b)
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<string>("ClaimType").HasColumnType("nvarchar(max)");
			b.Property<string>("ClaimValue").HasColumnType("nvarchar(max)");
			b.Property<Guid>("RoleId").HasColumnType("uniqueidentifier");
			b.HasKey("Id");
			b.HasIndex("RoleId");
			b.ToTable("RoleClaims", (string?)null);
		});
		modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", delegate(EntityTypeBuilder b)
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<string>("ClaimType").HasColumnType("nvarchar(max)");
			b.Property<string>("ClaimValue").HasColumnType("nvarchar(max)");
			b.Property<Guid>("UserId").HasColumnType("uniqueidentifier");
			b.HasKey("Id");
			b.HasIndex("UserId");
			b.ToTable("UserClaims", (string?)null);
		});
		modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", delegate(EntityTypeBuilder b)
		{
			b.Property<string>("LoginProvider").HasColumnType("nvarchar(450)");
			b.Property<string>("ProviderKey").HasColumnType("nvarchar(450)");
			b.Property<string>("ProviderDisplayName").HasColumnType("nvarchar(max)");
			b.Property<Guid>("UserId").HasColumnType("uniqueidentifier");
			b.HasKey("LoginProvider", "ProviderKey");
			b.HasIndex("UserId");
			b.ToTable("UserLogins", (string?)null);
		});
		modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("UserId").HasColumnType("uniqueidentifier");
			b.Property<Guid>("RoleId").HasColumnType("uniqueidentifier");
			b.HasKey("UserId", "RoleId");
			b.HasIndex("RoleId");
			b.ToTable("UserRoles", (string?)null);
		});
		modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("UserId").HasColumnType("uniqueidentifier");
			b.Property<string>("LoginProvider").HasColumnType("nvarchar(450)");
			b.Property<string>("Name").HasColumnType("nvarchar(450)");
			b.Property<string>("Value").HasColumnType("nvarchar(max)");
			b.HasKey("UserId", "LoginProvider", "Name");
			b.ToTable("UserTokens", (string?)null);
		});
		modelBuilder.Entity("ShopNest.Domain.Entities.Address", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uniqueidentifier");
			b.Property<string>("City").IsRequired().HasMaxLength(100)
				.HasColumnType("nvarchar(100)");
			b.Property<string>("Country").IsRequired().HasMaxLength(100)
				.HasColumnType("nvarchar(100)");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<Guid?>("CreatedBy").HasColumnType("uniqueidentifier");
			b.Property<DateTime?>("DeletedAt").HasColumnType("datetime2");
			b.Property<Guid?>("DeletedBy").HasColumnType("uniqueidentifier");
			b.Property<string>("FullName").IsRequired().HasMaxLength(150)
				.HasColumnType("nvarchar(150)");
			b.Property<bool>("IsDefault").HasColumnType("bit");
			b.Property<bool>("IsDeleted").HasColumnType("bit");
			b.Property<string>("Phone").IsRequired().HasMaxLength(30)
				.HasColumnType("nvarchar(30)");
			b.Property<string>("PostalCode").IsRequired().HasMaxLength(20)
				.HasColumnType("nvarchar(20)");
			b.Property<string>("State").IsRequired().HasMaxLength(100)
				.HasColumnType("nvarchar(100)");
			b.Property<string>("Street").IsRequired().HasMaxLength(250)
				.HasColumnType("nvarchar(250)");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.Property<Guid?>("UpdatedBy").HasColumnType("uniqueidentifier");
			b.Property<Guid>("UserId").HasColumnType("uniqueidentifier");
			b.HasKey("Id");
			b.HasIndex("UserId");
			b.ToTable("Addresses");
		});
		modelBuilder.Entity("ShopNest.Domain.Entities.Cart", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uniqueidentifier");
			b.Property<Guid?>("CouponId").HasColumnType("uniqueidentifier");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<Guid?>("CreatedBy").HasColumnType("uniqueidentifier");
			b.Property<decimal>("DiscountAmount").HasColumnType("decimal(18,2)");
			b.Property<decimal>("ShippingCost").HasColumnType("decimal(18,2)");
			b.Property<decimal>("SubTotal").HasColumnType("decimal(18,2)");
			b.Property<decimal>("Total").HasColumnType("decimal(18,2)");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.Property<Guid?>("UpdatedBy").HasColumnType("uniqueidentifier");
			b.Property<Guid>("UserId").HasColumnType("uniqueidentifier");
			b.HasKey("Id");
			b.HasIndex("CouponId");
			b.HasIndex("UserId").IsUnique();
			b.ToTable("Carts");
		});
		modelBuilder.Entity("ShopNest.Domain.Entities.CartItem", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uniqueidentifier");
			b.Property<Guid>("CartId").HasColumnType("uniqueidentifier");
			b.Property<Guid>("ProductId").HasColumnType("uniqueidentifier");
			b.Property<int>("Quantity").HasColumnType("int");
			b.Property<decimal>("TotalPrice").HasColumnType("decimal(18,2)");
			b.Property<decimal>("UnitPrice").HasColumnType("decimal(18,2)");
			b.HasKey("Id");
			b.HasIndex("ProductId");
			b.HasIndex("CartId", "ProductId").IsUnique();
			b.ToTable("CartItems");
		});
		modelBuilder.Entity("ShopNest.Domain.Entities.Category", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uniqueidentifier");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<Guid?>("CreatedBy").HasColumnType("uniqueidentifier");
			b.Property<DateTime?>("DeletedAt").HasColumnType("datetime2");
			b.Property<Guid?>("DeletedBy").HasColumnType("uniqueidentifier");
			b.Property<string>("Description").HasMaxLength(1000).HasColumnType("nvarchar(1000)");
			b.Property<int>("DisplayOrder").HasColumnType("int");
			b.Property<string>("ImageUrl").HasMaxLength(500).HasColumnType("nvarchar(500)");
			b.Property<bool>("IsActive").HasColumnType("bit");
			b.Property<bool>("IsDeleted").HasColumnType("bit");
			b.Property<string>("Name").IsRequired().HasMaxLength(150)
				.HasColumnType("nvarchar(150)");
			b.Property<Guid?>("ParentCategoryId").HasColumnType("uniqueidentifier");
			b.Property<string>("Slug").IsRequired().HasMaxLength(180)
				.HasColumnType("nvarchar(180)");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.Property<Guid?>("UpdatedBy").HasColumnType("uniqueidentifier");
			b.HasKey("Id");
			b.HasIndex("IsActive");
			b.HasIndex("ParentCategoryId");
			b.HasIndex("Slug").IsUnique();
			b.ToTable("Categories");
		});
		modelBuilder.Entity("ShopNest.Domain.Entities.Coupon", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uniqueidentifier");
			b.Property<string>("Code").IsRequired().HasMaxLength(50)
				.HasColumnType("nvarchar(50)");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<Guid?>("CreatedBy").HasColumnType("uniqueidentifier");
			b.Property<string>("Description").HasMaxLength(300).HasColumnType("nvarchar(300)");
			b.Property<int>("DiscountType").HasColumnType("int");
			b.Property<decimal>("DiscountValue").HasColumnType("decimal(18,2)");
			b.Property<DateTime?>("ExpiresAt").HasColumnType("datetime2");
			b.Property<bool>("IsActive").HasColumnType("bit");
			b.Property<bool>("IsOnePerUser").HasColumnType("bit");
			b.Property<decimal?>("MaximumDiscountAmount").HasColumnType("decimal(18,2)");
			b.Property<decimal>("MinimumOrderAmount").HasColumnType("decimal(18,2)");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.Property<Guid?>("UpdatedBy").HasColumnType("uniqueidentifier");
			b.Property<int?>("UsageLimit").HasColumnType("int");
			b.Property<int>("UsedCount").HasColumnType("int");
			b.HasKey("Id");
			b.HasIndex("Code").IsUnique();
			b.HasIndex("ExpiresAt");
			b.HasIndex("IsActive");
			b.ToTable("Coupons");
		});
		modelBuilder.Entity("ShopNest.Domain.Entities.Notification", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uniqueidentifier");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<string>("Data").HasColumnType("nvarchar(max)");
			b.Property<bool>("IsRead").HasColumnType("bit");
			b.Property<string>("Message").IsRequired().HasMaxLength(1000)
				.HasColumnType("nvarchar(1000)");
			b.Property<DateTime?>("ReadAt").HasColumnType("datetime2");
			b.Property<string>("Title").IsRequired().HasMaxLength(200)
				.HasColumnType("nvarchar(200)");
			b.Property<int>("Type").HasColumnType("int");
			b.Property<Guid>("UserId").HasColumnType("uniqueidentifier");
			b.HasKey("Id");
			b.HasIndex("CreatedAt");
			b.HasIndex("UserId");
			b.HasIndex("UserId", "IsRead");
			b.ToTable("Notifications");
		});
		modelBuilder.Entity("ShopNest.Domain.Entities.Order", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uniqueidentifier");
			b.Property<string>("CancelReason").HasMaxLength(500).HasColumnType("nvarchar(500)");
			b.Property<DateTime?>("CancelledAt").HasColumnType("datetime2");
			b.Property<Guid?>("CouponId").HasColumnType("uniqueidentifier");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<Guid?>("CreatedBy").HasColumnType("uniqueidentifier");
			b.Property<DateTime?>("DeletedAt").HasColumnType("datetime2");
			b.Property<Guid?>("DeletedBy").HasColumnType("uniqueidentifier");
			b.Property<DateTime?>("DeliveredAt").HasColumnType("datetime2");
			b.Property<decimal>("DiscountAmount").HasColumnType("decimal(18,2)");
			b.Property<bool>("IsDeleted").HasColumnType("bit");
			b.Property<string>("Notes").HasMaxLength(500).HasColumnType("nvarchar(500)");
			b.Property<string>("OrderNumber").IsRequired().HasMaxLength(50)
				.HasColumnType("nvarchar(50)");
			b.Property<DateTime?>("ShippedAt").HasColumnType("datetime2");
			b.Property<Guid>("ShippingAddressId").HasColumnType("uniqueidentifier");
			b.Property<decimal>("ShippingCost").HasColumnType("decimal(18,2)");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<decimal>("SubTotal").HasColumnType("decimal(18,2)");
			b.Property<decimal>("TaxAmount").HasColumnType("decimal(18,2)");
			b.Property<decimal>("TotalAmount").HasColumnType("decimal(18,2)");
			b.Property<string>("TrackingNumber").HasMaxLength(100).HasColumnType("nvarchar(100)");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.Property<Guid?>("UpdatedBy").HasColumnType("uniqueidentifier");
			b.Property<Guid>("UserId").HasColumnType("uniqueidentifier");
			b.HasKey("Id");
			b.HasIndex("CouponId");
			b.HasIndex("CreatedAt");
			b.HasIndex("OrderNumber").IsUnique();
			b.HasIndex("ShippingAddressId");
			b.HasIndex("Status");
			b.HasIndex("UserId");
			b.ToTable("Orders");
		});
		modelBuilder.Entity("ShopNest.Domain.Entities.OrderItem", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uniqueidentifier");
			b.Property<Guid>("OrderId").HasColumnType("uniqueidentifier");
			b.Property<Guid>("ProductId").HasColumnType("uniqueidentifier");
			b.Property<string>("ProductImageUrl").HasMaxLength(500).HasColumnType("nvarchar(500)");
			b.Property<string>("ProductName").IsRequired().HasMaxLength(200)
				.HasColumnType("nvarchar(200)");
			b.Property<string>("ProductSKU").IsRequired().HasMaxLength(100)
				.HasColumnType("nvarchar(100)");
			b.Property<int>("Quantity").HasColumnType("int");
			b.Property<decimal>("TotalPrice").HasColumnType("decimal(18,2)");
			b.Property<decimal>("UnitPrice").HasColumnType("decimal(18,2)");
			b.HasKey("Id");
			b.HasIndex("OrderId");
			b.HasIndex("ProductId");
			b.ToTable("OrderItems");
		});
		modelBuilder.Entity("ShopNest.Domain.Entities.Payment", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uniqueidentifier");
			b.Property<decimal>("Amount").HasColumnType("decimal(18,2)");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<Guid?>("CreatedBy").HasColumnType("uniqueidentifier");
			b.Property<string>("FailureReason").HasMaxLength(500).HasColumnType("nvarchar(500)");
			b.Property<int>("Method").HasColumnType("int");
			b.Property<Guid>("OrderId").HasColumnType("uniqueidentifier");
			b.Property<DateTime?>("PaidAt").HasColumnType("datetime2");
			b.Property<string>("RefundReason").HasMaxLength(500).HasColumnType("nvarchar(500)");
			b.Property<decimal>("RefundedAmount").HasColumnType("decimal(18,2)");
			b.Property<DateTime?>("RefundedAt").HasColumnType("datetime2");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<string>("StripeChargeId").HasMaxLength(200).HasColumnType("nvarchar(200)");
			b.Property<string>("StripeCustomerId").HasMaxLength(200).HasColumnType("nvarchar(200)");
			b.Property<string>("StripePaymentIntentId").HasMaxLength(200).HasColumnType("nvarchar(200)");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.Property<Guid?>("UpdatedBy").HasColumnType("uniqueidentifier");
			b.HasKey("Id");
			b.HasIndex("OrderId").IsUnique();
			b.HasIndex("StripePaymentIntentId");
			b.ToTable("Payments");
		});
		modelBuilder.Entity("ShopNest.Domain.Entities.Product", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uniqueidentifier");
			b.Property<decimal>("AverageRating").HasColumnType("decimal(3,2)");
			b.Property<string>("Barcode").HasMaxLength(100).HasColumnType("nvarchar(100)");
			b.Property<Guid>("CategoryId").HasColumnType("uniqueidentifier");
			b.Property<decimal?>("CompareAtPrice").HasColumnType("decimal(18,2)");
			b.Property<decimal?>("CostPrice").HasColumnType("decimal(18,2)");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<Guid?>("CreatedBy").HasColumnType("uniqueidentifier");
			b.Property<DateTime?>("DeletedAt").HasColumnType("datetime2");
			b.Property<Guid?>("DeletedBy").HasColumnType("uniqueidentifier");
			b.Property<string>("Description").HasColumnType("nvarchar(max)");
			b.Property<bool>("IsActive").HasColumnType("bit");
			b.Property<bool>("IsDeleted").HasColumnType("bit");
			b.Property<bool>("IsFeatured").HasColumnType("bit");
			b.Property<int>("LowStockThreshold").HasColumnType("int");
			b.Property<string>("Name").IsRequired().HasMaxLength(200)
				.HasColumnType("nvarchar(200)");
			b.Property<decimal>("Price").HasColumnType("decimal(18,2)");
			b.Property<int>("ReviewCount").HasColumnType("int");
			b.Property<string>("SKU").IsRequired().HasMaxLength(100)
				.HasColumnType("nvarchar(100)");
			b.Property<string>("ShortDescription").HasMaxLength(500).HasColumnType("nvarchar(500)");
			b.Property<string>("Slug").IsRequired().HasMaxLength(250)
				.HasColumnType("nvarchar(250)");
			b.Property<int>("StockQuantity").HasColumnType("int");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.Property<Guid?>("UpdatedBy").HasColumnType("uniqueidentifier");
			b.Property<decimal?>("Weight").HasColumnType("decimal(10,3)");
			b.HasKey("Id");
			b.HasIndex("CategoryId");
			b.HasIndex("SKU").IsUnique();
			b.HasIndex("Slug").IsUnique();
			b.HasIndex("IsActive", "IsDeleted", "CreatedAt");
			b.ToTable("Products");
		});
		modelBuilder.Entity("ShopNest.Domain.Entities.ProductImage", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uniqueidentifier");
			b.Property<string>("AltText").HasMaxLength(250).HasColumnType("nvarchar(250)");
			b.Property<int>("DisplayOrder").HasColumnType("int");
			b.Property<string>("ImageUrl").IsRequired().HasMaxLength(500)
				.HasColumnType("nvarchar(500)");
			b.Property<bool>("IsPrimary").HasColumnType("bit");
			b.Property<Guid>("ProductId").HasColumnType("uniqueidentifier");
			b.Property<string>("ThumbnailUrl").HasMaxLength(500).HasColumnType("nvarchar(500)");
			b.HasKey("Id");
			b.HasIndex("ProductId", "DisplayOrder");
			b.ToTable("ProductImages");
		});
		modelBuilder.Entity("ShopNest.Domain.Entities.RefreshToken", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uniqueidentifier");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<string>("CreatedByIp").HasMaxLength(50).HasColumnType("nvarchar(50)");
			b.Property<DateTime>("ExpiresAt").HasColumnType("datetime2");
			b.Property<bool>("IsRevoked").HasColumnType("bit");
			b.Property<string>("ReplacedByToken").HasMaxLength(512).HasColumnType("nvarchar(512)");
			b.Property<DateTime?>("RevokedAt").HasColumnType("datetime2");
			b.Property<string>("RevokedByIp").HasMaxLength(50).HasColumnType("nvarchar(50)");
			b.Property<string>("TokenHash").IsRequired().HasMaxLength(512)
				.HasColumnType("nvarchar(512)");
			b.Property<Guid>("UserId").HasColumnType("uniqueidentifier");
			b.HasKey("Id");
			b.HasIndex("ExpiresAt");
			b.HasIndex("TokenHash");
			b.HasIndex("UserId");
			b.ToTable("RefreshTokens");
		});
		modelBuilder.Entity("ShopNest.Domain.Entities.Review", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uniqueidentifier");
			b.Property<string>("AdminNote").HasMaxLength(500).HasColumnType("nvarchar(500)");
			b.Property<DateTime?>("ApprovedAt").HasColumnType("datetime2");
			b.Property<Guid?>("ApprovedBy").HasColumnType("uniqueidentifier");
			b.Property<string>("Comment").HasMaxLength(2000).HasColumnType("nvarchar(2000)");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<Guid?>("CreatedBy").HasColumnType("uniqueidentifier");
			b.Property<DateTime?>("DeletedAt").HasColumnType("datetime2");
			b.Property<Guid?>("DeletedBy").HasColumnType("uniqueidentifier");
			b.Property<bool>("IsDeleted").HasColumnType("bit");
			b.Property<bool>("IsVerifiedPurchase").HasColumnType("bit");
			b.Property<Guid>("ProductId").HasColumnType("uniqueidentifier");
			b.Property<int>("Rating").HasColumnType("int");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<string>("Title").HasMaxLength(200).HasColumnType("nvarchar(200)");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.Property<Guid?>("UpdatedBy").HasColumnType("uniqueidentifier");
			b.Property<Guid>("UserId").HasColumnType("uniqueidentifier");
			b.HasKey("Id");
			b.HasIndex("ProductId");
			b.HasIndex("Status");
			b.HasIndex("UserId", "ProductId").IsUnique();
			b.ToTable("Reviews");
		});
		modelBuilder.Entity("ShopNest.Application.Common.Identity.AppUser", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uniqueidentifier");
			b.Property<int>("AccessFailedCount").HasColumnType("int");
			b.Property<string>("AvatarUrl").HasMaxLength(500).HasColumnType("nvarchar(500)");
			b.Property<string>("ConcurrencyStamp").IsConcurrencyToken().HasColumnType("nvarchar(max)");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<string>("Email").HasMaxLength(256).HasColumnType("nvarchar(256)");
			b.Property<bool>("EmailConfirmed").HasColumnType("bit");
			b.Property<string>("FirstName").IsRequired().HasMaxLength(100)
				.HasColumnType("nvarchar(100)");
			b.Property<bool>("IsActive").HasColumnType("bit");
			b.Property<string>("LastName").IsRequired().HasMaxLength(100)
				.HasColumnType("nvarchar(100)");
			b.Property<bool>("LockoutEnabled").HasColumnType("bit");
			b.Property<DateTimeOffset?>("LockoutEnd").HasColumnType("datetimeoffset");
			b.Property<string>("NormalizedEmail").HasMaxLength(256).HasColumnType("nvarchar(256)");
			b.Property<string>("NormalizedUserName").HasMaxLength(256).HasColumnType("nvarchar(256)");
			b.Property<string>("PasswordHash").HasColumnType("nvarchar(max)");
			b.Property<string>("PhoneNumber").HasColumnType("nvarchar(max)");
			b.Property<bool>("PhoneNumberConfirmed").HasColumnType("bit");
			b.Property<string>("SecurityStamp").HasColumnType("nvarchar(max)");
			b.Property<bool>("TwoFactorEnabled").HasColumnType("bit");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.Property<string>("UserName").HasMaxLength(256).HasColumnType("nvarchar(256)");
			b.HasKey("Id");
			b.HasIndex("Email").IsUnique().HasFilter("[Email] IS NOT NULL");
			b.HasIndex("NormalizedEmail").HasDatabaseName("EmailIndex");
			b.HasIndex("NormalizedUserName").IsUnique().HasDatabaseName("UserNameIndex")
				.HasFilter("[NormalizedUserName] IS NOT NULL");
			b.ToTable("Users", (string?)null);
		});
		modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", delegate(EntityTypeBuilder b)
		{
			b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<System.Guid>", null).WithMany().HasForeignKey("RoleId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
		});
		modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", delegate(EntityTypeBuilder b)
		{
			b.HasOne("ShopNest.Application.Common.Identity.AppUser", null).WithMany().HasForeignKey("UserId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
		});
		modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", delegate(EntityTypeBuilder b)
		{
			b.HasOne("ShopNest.Application.Common.Identity.AppUser", null).WithMany().HasForeignKey("UserId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
		});
		modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", delegate(EntityTypeBuilder b)
		{
			b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<System.Guid>", null).WithMany().HasForeignKey("RoleId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
			b.HasOne("ShopNest.Application.Common.Identity.AppUser", null).WithMany().HasForeignKey("UserId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
		});
		modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", delegate(EntityTypeBuilder b)
		{
			b.HasOne("ShopNest.Application.Common.Identity.AppUser", null).WithMany().HasForeignKey("UserId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
		});
		modelBuilder.Entity("ShopNest.Domain.Entities.Address", delegate(EntityTypeBuilder b)
		{
			b.HasOne("ShopNest.Application.Common.Identity.AppUser", null).WithMany("Addresses").HasForeignKey("UserId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
		});
		modelBuilder.Entity("ShopNest.Domain.Entities.Cart", delegate(EntityTypeBuilder b)
		{
			b.HasOne("ShopNest.Domain.Entities.Coupon", "Coupon").WithMany("Carts").HasForeignKey("CouponId")
				.OnDelete(DeleteBehavior.SetNull);
			b.HasOne("ShopNest.Application.Common.Identity.AppUser", null).WithOne("Cart").HasForeignKey("ShopNest.Domain.Entities.Cart", "UserId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
			b.Navigation("Coupon");
		});
		modelBuilder.Entity("ShopNest.Domain.Entities.CartItem", delegate(EntityTypeBuilder b)
		{
			b.HasOne("ShopNest.Domain.Entities.Cart", "Cart").WithMany("Items").HasForeignKey("CartId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
			b.HasOne("ShopNest.Domain.Entities.Product", "Product").WithMany("CartItems").HasForeignKey("ProductId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.Navigation("Cart");
			b.Navigation("Product");
		});
		modelBuilder.Entity("ShopNest.Domain.Entities.Category", delegate(EntityTypeBuilder b)
		{
			b.HasOne("ShopNest.Domain.Entities.Category", "ParentCategory").WithMany("SubCategories").HasForeignKey("ParentCategoryId")
				.OnDelete(DeleteBehavior.Restrict);
			b.Navigation("ParentCategory");
		});
		modelBuilder.Entity("ShopNest.Domain.Entities.Notification", delegate(EntityTypeBuilder b)
		{
			b.HasOne("ShopNest.Application.Common.Identity.AppUser", null).WithMany("Notifications").HasForeignKey("UserId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
		});
		modelBuilder.Entity("ShopNest.Domain.Entities.Order", delegate(EntityTypeBuilder b)
		{
			b.HasOne("ShopNest.Domain.Entities.Coupon", "Coupon").WithMany("Orders").HasForeignKey("CouponId")
				.OnDelete(DeleteBehavior.SetNull);
			b.HasOne("ShopNest.Domain.Entities.Address", "ShippingAddress").WithMany("Orders").HasForeignKey("ShippingAddressId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.HasOne("ShopNest.Application.Common.Identity.AppUser", null).WithMany("Orders").HasForeignKey("UserId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.Navigation("Coupon");
			b.Navigation("ShippingAddress");
		});
		modelBuilder.Entity("ShopNest.Domain.Entities.OrderItem", delegate(EntityTypeBuilder b)
		{
			b.HasOne("ShopNest.Domain.Entities.Order", "Order").WithMany("Items").HasForeignKey("OrderId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
			b.HasOne("ShopNest.Domain.Entities.Product", "Product").WithMany("OrderItems").HasForeignKey("ProductId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.Navigation("Order");
			b.Navigation("Product");
		});
		modelBuilder.Entity("ShopNest.Domain.Entities.Payment", delegate(EntityTypeBuilder b)
		{
			b.HasOne("ShopNest.Domain.Entities.Order", "Order").WithOne("Payment").HasForeignKey("ShopNest.Domain.Entities.Payment", "OrderId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
			b.Navigation("Order");
		});
		modelBuilder.Entity("ShopNest.Domain.Entities.Product", delegate(EntityTypeBuilder b)
		{
			b.HasOne("ShopNest.Domain.Entities.Category", "Category").WithMany("Products").HasForeignKey("CategoryId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.Navigation("Category");
		});
		modelBuilder.Entity("ShopNest.Domain.Entities.ProductImage", delegate(EntityTypeBuilder b)
		{
			b.HasOne("ShopNest.Domain.Entities.Product", "Product").WithMany("Images").HasForeignKey("ProductId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
			b.Navigation("Product");
		});
		modelBuilder.Entity("ShopNest.Domain.Entities.RefreshToken", delegate(EntityTypeBuilder b)
		{
			b.HasOne("ShopNest.Application.Common.Identity.AppUser", null).WithMany("RefreshTokens").HasForeignKey("UserId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
		});
		modelBuilder.Entity("ShopNest.Domain.Entities.Review", delegate(EntityTypeBuilder b)
		{
			b.HasOne("ShopNest.Domain.Entities.Product", "Product").WithMany("Reviews").HasForeignKey("ProductId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
			b.HasOne("ShopNest.Application.Common.Identity.AppUser", null).WithMany("Reviews").HasForeignKey("UserId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.Navigation("Product");
		});
		modelBuilder.Entity("ShopNest.Domain.Entities.Address", delegate(EntityTypeBuilder b)
		{
			b.Navigation("Orders");
		});
		modelBuilder.Entity("ShopNest.Domain.Entities.Cart", delegate(EntityTypeBuilder b)
		{
			b.Navigation("Items");
		});
		modelBuilder.Entity("ShopNest.Domain.Entities.Category", delegate(EntityTypeBuilder b)
		{
			b.Navigation("Products");
			b.Navigation("SubCategories");
		});
		modelBuilder.Entity("ShopNest.Domain.Entities.Coupon", delegate(EntityTypeBuilder b)
		{
			b.Navigation("Carts");
			b.Navigation("Orders");
		});
		modelBuilder.Entity("ShopNest.Domain.Entities.Order", delegate(EntityTypeBuilder b)
		{
			b.Navigation("Items");
			b.Navigation("Payment");
		});
		modelBuilder.Entity("ShopNest.Domain.Entities.Product", delegate(EntityTypeBuilder b)
		{
			b.Navigation("CartItems");
			b.Navigation("Images");
			b.Navigation("OrderItems");
			b.Navigation("Reviews");
		});
		modelBuilder.Entity("ShopNest.Application.Common.Identity.AppUser", delegate(EntityTypeBuilder b)
		{
			b.Navigation("Addresses");
			b.Navigation("Cart");
			b.Navigation("Notifications");
			b.Navigation("Orders");
			b.Navigation("RefreshTokens");
			b.Navigation("Reviews");
		});
	}
}
