CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) NOT NULL,
    `ProductVersion` varchar(32) NOT NULL,
    PRIMARY KEY (`MigrationId`)
);

START TRANSACTION;
CREATE TABLE `tenants` (
    `Id` bigint NOT NULL AUTO_INCREMENT,
    `Name` varchar(200) NOT NULL,
    `Code` varchar(50) NOT NULL,
    `LegalName` varchar(250) NULL,
    `TaxIdentifierLastFour` varchar(4) NULL,
    `Status` int NOT NULL,
    `TimeZone` varchar(100) NULL,
    `DefaultCurrency` varchar(10) NULL,
    `ContactEmail` varchar(320) NULL,
    `ContactPhone` varchar(50) NULL,
    `ExternalId` char(36) NOT NULL,
    `CreatedAtUtc` datetime(6) NOT NULL,
    `UpdatedAtUtc` datetime(6) NULL,
    `CreatedByUserId` bigint NULL,
    `UpdatedByUserId` bigint NULL,
    PRIMARY KEY (`Id`)
);

CREATE UNIQUE INDEX `IX_tenants_Code` ON `tenants` (`Code`);

CREATE UNIQUE INDEX `IX_tenants_ExternalId` ON `tenants` (`ExternalId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260915111501_InitialBaseline', '10.0.11');

COMMIT;

