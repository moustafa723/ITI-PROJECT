BEGIN TRANSACTION;
DROP TABLE IF EXISTS "Addresses";
CREATE TABLE "Addresses" (
	"Id"	INTEGER NOT NULL,
	"UserId"	TEXT NOT NULL,
	"Label"	TEXT NOT NULL,
	"Line1"	TEXT NOT NULL,
	"Line2"	TEXT NOT NULL,
	"City"	TEXT NOT NULL,
	"State"	TEXT NOT NULL,
	"PostalCode"	TEXT NOT NULL,
	"Country"	TEXT NOT NULL,
	"ContactName"	TEXT NOT NULL,
	"Phone"	TEXT NOT NULL,
	"IsDefault"	INTEGER NOT NULL,
	CONSTRAINT "PK_Addresses" PRIMARY KEY("Id" AUTOINCREMENT)
);
DROP TABLE IF EXISTS "CartItems";
CREATE TABLE "CartItems" (
	"Id"	INTEGER NOT NULL,
	"CartId"	INTEGER NOT NULL,
	"Price"	TEXT NOT NULL,
	"ProductId"	INTEGER NOT NULL,
	"Quantity"	INTEGER NOT NULL,
	CONSTRAINT "PK_CartItems" PRIMARY KEY("Id" AUTOINCREMENT),
	CONSTRAINT "FK_CartItems_Carts_CartId" FOREIGN KEY("CartId") REFERENCES "Carts"("Id") ON DELETE CASCADE,
	CONSTRAINT "FK_CartItems_Products_ProductId" FOREIGN KEY("ProductId") REFERENCES "Products"("Id") ON DELETE CASCADE
);
DROP TABLE IF EXISTS "Carts";
CREATE TABLE "Carts" (
	"Id"	INTEGER NOT NULL,
	"UserId"	TEXT,
	CONSTRAINT "PK_Carts" PRIMARY KEY("Id" AUTOINCREMENT)
);
DROP TABLE IF EXISTS "Categories";
CREATE TABLE "Categories" (
	"Id"	INTEGER NOT NULL,
	"Back_Color"	TEXT NOT NULL,
	"Name"	TEXT NOT NULL,
	"PhotoPath"	TEXT,
	CONSTRAINT "PK_Categories" PRIMARY KEY("Id" AUTOINCREMENT)
);
DROP TABLE IF EXISTS "OrderItems";
CREATE TABLE "OrderItems" (
	"Id"	INTEGER NOT NULL,
	"OrderId"	INTEGER NOT NULL,
	"Price"	TEXT NOT NULL,
	"ProductId"	INTEGER NOT NULL,
	"Quantity"	INTEGER NOT NULL,
	CONSTRAINT "PK_OrderItems" PRIMARY KEY("Id" AUTOINCREMENT),
	CONSTRAINT "FK_OrderItems_Orders_OrderId" FOREIGN KEY("OrderId") REFERENCES "Orders"("Id") ON DELETE CASCADE,
	CONSTRAINT "FK_OrderItems_Products_ProductId" FOREIGN KEY("ProductId") REFERENCES "Products"("Id") ON DELETE CASCADE
);
DROP TABLE IF EXISTS "Orders";
CREATE TABLE "Orders" (
	"Id"	INTEGER NOT NULL,
	"AddressId"	INTEGER,
	"CreatedAt"	TEXT NOT NULL,
	"CustomerName"	TEXT NOT NULL,
	"Email"	TEXT NOT NULL,
	"TotalAmount"	TEXT NOT NULL,
	"UserId"	TEXT NOT NULL,
	CONSTRAINT "PK_Orders" PRIMARY KEY("Id" AUTOINCREMENT),
	CONSTRAINT "FK_Orders_Addresses_AddressId" FOREIGN KEY("AddressId") REFERENCES "Addresses"("Id")
);
DROP TABLE IF EXISTS "Payments";
CREATE TABLE "Payments" (
	"Id"	INTEGER NOT NULL,
	"Amount"	TEXT NOT NULL,
	"Date"	TEXT NOT NULL,
	"OrderId"	INTEGER NOT NULL,
	CONSTRAINT "PK_Payments" PRIMARY KEY("Id" AUTOINCREMENT),
	CONSTRAINT "FK_Payments_Orders_OrderId" FOREIGN KEY("OrderId") REFERENCES "Orders"("Id") ON DELETE CASCADE
);
DROP TABLE IF EXISTS "Products";
CREATE TABLE "Products" (
	"Id"	INTEGER NOT NULL,
	"Alts"	TEXT,
	"Badge"	TEXT,
	"CategoryId"	INTEGER NOT NULL,
	"Color"	TEXT,
	"Images"	TEXT NOT NULL,
	"In_stock"	INTEGER NOT NULL,
	"Name"	TEXT NOT NULL,
	"OldPrice"	TEXT,
	"Price"	TEXT NOT NULL,
	"Rating"	REAL NOT NULL,
	"Review"	INTEGER NOT NULL,
	"Size"	TEXT,
	CONSTRAINT "PK_Products" PRIMARY KEY("Id" AUTOINCREMENT),
	CONSTRAINT "FK_Products_Categories_CategoryId" FOREIGN KEY("CategoryId") REFERENCES "Categories"("Id") ON DELETE RESTRICT
);
DROP TABLE IF EXISTS "__EFMigrationsHistory";
CREATE TABLE "__EFMigrationsHistory" (
	"MigrationId"	TEXT NOT NULL,
	"ProductVersion"	TEXT NOT NULL,
	CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY("MigrationId")
);
DROP TABLE IF EXISTS "__EFMigrationsLock";
CREATE TABLE "__EFMigrationsLock" (
	"Id"	INTEGER NOT NULL,
	"Timestamp"	TEXT NOT NULL,
	CONSTRAINT "PK___EFMigrationsLock" PRIMARY KEY("Id")
);
INSERT INTO "Addresses" ("Id","UserId","Label","Line1","Line2","City","State","PostalCode","Country","ContactName","Phone","IsDefault") VALUES (4,'001e44bc-4041-4786-a48a-e7be59b552b4','Home','ffff','ffffh','Mansoura','Mee','10001','Egypt','moustafa mouhamed elgazzar','01061996466',0);
INSERT INTO "Addresses" ("Id","UserId","Label","Line1","Line2","City","State","PostalCode","Country","ContactName","Phone","IsDefault") VALUES (5,'001e44bc-4041-4786-a48a-e7be59b552b4','office','ffff','ffffh','Mansoura','Me','10001','Egypt','moustafa mouhamed elgazzar','01061996466',1);
INSERT INTO "Addresses" ("Id","UserId","Label","Line1","Line2","City","State","PostalCode","Country","ContactName","Phone","IsDefault") VALUES (6,'14f3173b-59c5-440c-b3bf-17beb1ca50bb','Home','ffffاتات','ffffh','Mansoura','Meeقبللىاا','10001','Egypt','moustafa mouhamed elgazzar','01061996466',1);
INSERT INTO "Addresses" ("Id","UserId","Label","Line1","Line2","City","State","PostalCode","Country","ContactName","Phone","IsDefault") VALUES (7,'14f3173b-59c5-440c-b3bf-17beb1ca50bb','office','ffff','ffffh','Mansoura','Mee','10001','Egypt','moustafa mouhamed elgazzar','01061996466',0);
INSERT INTO "Carts" ("Id","UserId") VALUES (45,'14f3173b-59c5-440c-b3bf-17beb1ca50bb');
INSERT INTO "Categories" ("Id","Back_Color","Name","PhotoPath") VALUES (1,'cat-men','Men''s Wear','/uploads/f4d0f3bb-3c7e-43b1-b9d0-3ae3d36b7723.webp');
INSERT INTO "Categories" ("Id","Back_Color","Name","PhotoPath") VALUES (2,'cat-cosmetics','Women''s Wear','/uploads/4d388334-f192-48d4-93a2-af81a964589c.webp');
INSERT INTO "Categories" ("Id","Back_Color","Name","PhotoPath") VALUES (3,'cat-kids','Kids'' Clothing','/uploads/6eca097d-f1e2-415d-86a7-16c6ae7f0d13.jpeg');
INSERT INTO "Categories" ("Id","Back_Color","Name","PhotoPath") VALUES (4,'cat-accessories','Accessories','/uploads/0b4af4e0-31c2-4f52-9039-c8efdb13bc3f.webp');
INSERT INTO "OrderItems" ("Id","OrderId","Price","ProductId","Quantity") VALUES (9,8,'95.0',1,1);
INSERT INTO "Orders" ("Id","AddressId","CreatedAt","CustomerName","Email","TotalAmount","UserId") VALUES (8,6,'2025-09-12 09:44:23.4201199','moustafa','admin1@stylehub.com','0.0','14f3173b-59c5-440c-b3bf-17beb1ca50bb');
INSERT INTO "Products" ("Id","Alts","Badge","CategoryId","Color","Images","In_stock","Name","OldPrice","Price","Rating","Review","Size") VALUES (1,'Men''s Collection','New',1,'new','["/uploads/cc394981-58e8-4c59-83cd-e2639094672b.webp","/uploads/ba57537c-016a-4651-9749-aa51d8b30096.webp"]',1,'Elit Consectetur',NULL,'95.0',4.8,28,'L');
INSERT INTO "Products" ("Id","Alts","Badge","CategoryId","Color","Images","In_stock","Name","OldPrice","Price","Rating","Review","Size") VALUES (2,'Tempor Incididunt',NULL,2,NULL,'["/uploads/e15847c8-88ef-4d37-baf8-237805da06c3.jpg","/uploads/30354c9d-bddc-46d0-8699-d796672f880b.jpg","/uploads/03801f48-1ea1-471c-a467-56e54a8203e1.jpg","/uploads/54747027-38fd-4885-9023-629690c974f4.jpg","/uploads/2b3b4bfa-9c33-45f6-ba88-5693203dfbc6.jpg","/uploads/d6812991-6c0d-441e-8cfa-7a12db7bf9d8.jpg"]',0,'Tempor Incididunt',NULL,'129.0',4.8,42,'M');
INSERT INTO "Products" ("Id","Alts","Badge","CategoryId","Color","Images","In_stock","Name","OldPrice","Price","Rating","Review","Size") VALUES (3,'string','string',1,'string','[]',1,'stringshgs','0.0','0.0',0.0,0,'string');
INSERT INTO "Products" ("Id","Alts","Badge","CategoryId","Color","Images","In_stock","Name","OldPrice","Price","Rating","Review","Size") VALUES (4,'x','x',1,'x','[]',1,'x','10.0','10.0',10.0,10,'x');
INSERT INTO "Products" ("Id","Alts","Badge","CategoryId","Color","Images","In_stock","Name","OldPrice","Price","Rating","Review","Size") VALUES (5,'string','string',1,'string','[]',1,'string','0.0','0.0',0.0,0,'string');
INSERT INTO "Products" ("Id","Alts","Badge","CategoryId","Color","Images","In_stock","Name","OldPrice","Price","Rating","Review","Size") VALUES (6,'string','string',2,'string','[]',1,'string','0.0','0.0',0.0,0,'string');
INSERT INTO "Products" ("Id","Alts","Badge","CategoryId","Color","Images","In_stock","Name","OldPrice","Price","Rating","Review","Size") VALUES (7,'Men''s Collection','New',3,'new','[]',0,'Elit Consectetur',NULL,'95.0',4.8,28,'L');
INSERT INTO "__EFMigrationsHistory" ("MigrationId","ProductVersion") VALUES ('20250904094209_initialcreate','9.0.8');
INSERT INTO "__EFMigrationsHistory" ("MigrationId","ProductVersion") VALUES ('20250904095814_secondcreate','9.0.8');
INSERT INTO "__EFMigrationsHistory" ("MigrationId","ProductVersion") VALUES ('20250905082627_v1','9.0.8');
INSERT INTO "__EFMigrationsHistory" ("MigrationId","ProductVersion") VALUES ('20250906115720_v2','9.0.8');
INSERT INTO "__EFMigrationsHistory" ("MigrationId","ProductVersion") VALUES ('20250906131649_v3','9.0.8');
INSERT INTO "__EFMigrationsHistory" ("MigrationId","ProductVersion") VALUES ('20250907052401_v4','9.0.8');
INSERT INTO "__EFMigrationsHistory" ("MigrationId","ProductVersion") VALUES ('20250908082323_vb','9.0.8');
INSERT INTO "__EFMigrationsHistory" ("MigrationId","ProductVersion") VALUES ('20250908093813_vf','9.0.8');
INSERT INTO "__EFMigrationsHistory" ("MigrationId","ProductVersion") VALUES ('20250908151904_UpdateProductImagesConverter','9.0.8');
INSERT INTO "__EFMigrationsHistory" ("MigrationId","ProductVersion") VALUES ('20250908160848_UpdateProductImagesConverter1','9.0.8');
INSERT INTO "__EFMigrationsHistory" ("MigrationId","ProductVersion") VALUES ('20250908161510_UpdateProductImagesConverter12','9.0.8');
INSERT INTO "__EFMigrationsHistory" ("MigrationId","ProductVersion") VALUES ('20250909232742_NJ','9.0.8');
INSERT INTO "__EFMigrationsHistory" ("MigrationId","ProductVersion") VALUES ('20250908125641_v5','9.0.8');
INSERT INTO "__EFMigrationsHistory" ("MigrationId","ProductVersion") VALUES ('20250909093139_va','9.0.8');
INSERT INTO "__EFMigrationsHistory" ("MigrationId","ProductVersion") VALUES ('20250910152118_Init','9.0.8');
INSERT INTO "__EFMigrationsHistory" ("MigrationId","ProductVersion") VALUES ('20250912070504_VB54','9.0.8');
INSERT INTO "__EFMigrationsHistory" ("MigrationId","ProductVersion") VALUES ('20250912091503_VB548','9.0.8');
DROP INDEX IF EXISTS "IX_CartItems_CartId_ProductId";
CREATE UNIQUE INDEX "IX_CartItems_CartId_ProductId" ON "CartItems" (
	"CartId",
	"ProductId"
);
DROP INDEX IF EXISTS "IX_CartItems_ProductId";
CREATE INDEX "IX_CartItems_ProductId" ON "CartItems" (
	"ProductId"
);
DROP INDEX IF EXISTS "IX_Carts_UserId";
CREATE UNIQUE INDEX "IX_Carts_UserId" ON "Carts" (
	"UserId"
);
DROP INDEX IF EXISTS "IX_OrderItems_OrderId";
CREATE INDEX "IX_OrderItems_OrderId" ON "OrderItems" (
	"OrderId"
);
DROP INDEX IF EXISTS "IX_OrderItems_ProductId";
CREATE INDEX "IX_OrderItems_ProductId" ON "OrderItems" (
	"ProductId"
);
DROP INDEX IF EXISTS "IX_Orders_AddressId";
CREATE INDEX "IX_Orders_AddressId" ON "Orders" (
	"AddressId"
);
DROP INDEX IF EXISTS "IX_Payments_OrderId";
CREATE INDEX "IX_Payments_OrderId" ON "Payments" (
	"OrderId"
);
DROP INDEX IF EXISTS "IX_Products_CategoryId";
CREATE INDEX "IX_Products_CategoryId" ON "Products" (
	"CategoryId"
);
COMMIT;
