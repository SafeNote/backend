﻿CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230117145650_Init') THEN
    CREATE TABLE "DataProtectionKeys" (
        "Id" integer GENERATED BY DEFAULT AS IDENTITY,
        "FriendlyName" text NULL,
        "Xml" text NULL,
        CONSTRAINT "PK_DataProtectionKeys" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230117145650_Init') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20230117145650_Init', '7.0.2');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230118025008_AddedNoteType') THEN
    CREATE TABLE "Notes" (
        "Id" text NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "ModifiedAt" timestamp with time zone NOT NULL,
        "DataBundle" jsonb NOT NULL,
        CONSTRAINT "PK_Notes" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230118025008_AddedNoteType') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20230118025008_AddedNoteType', '7.0.2');
    END IF;
END $EF$;
COMMIT;

