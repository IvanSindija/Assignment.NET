﻿CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

CREATE TABLE "Contacts" (
    "Id" integer NOT NULL GENERATED BY DEFAULT AS IDENTITY,
    "Name" text NULL,
    "DateOfBirth" timestamp without time zone NOT NULL,
    "Address" text NULL,
    CONSTRAINT "PK_Contacts" PRIMARY KEY ("Id")
);

CREATE TABLE "PhoneNumbers" (
    "Id" integer NOT NULL GENERATED BY DEFAULT AS IDENTITY,
    "ContactId" integer NOT NULL,
    "Number" text NULL,
    CONSTRAINT "PK_PhoneNumbers" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_PhoneNumbers_Contacts_ContactId" FOREIGN KEY ("ContactId") REFERENCES "Contacts" ("Id") ON DELETE CASCADE
);

CREATE UNIQUE INDEX "IX_Contacts_Name_Address" ON "Contacts" ("Name", "Address");

CREATE INDEX "IX_PhoneNumbers_ContactId" ON "PhoneNumbers" ("ContactId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20210227115508_InitialCreate', '5.0.3');

COMMIT;

