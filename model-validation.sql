CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) NOT NULL,
    `ProductVersion` varchar(32) NOT NULL,
    PRIMARY KEY (`MigrationId`)
);

START TRANSACTION;
CREATE TABLE `tenants` (
    `id` bigint unsigned NOT NULL AUTO_INCREMENT,
    `tenant_name` varchar(200) NOT NULL,
    `tenant_code` varchar(50) NOT NULL,
    `legal_name` varchar(250) NULL,
    `tax_identifier` varchar(50) NULL,
    `status` enum('TRIAL','ACTIVE','SUSPENDED','INACTIVE') NOT NULL,
    `timezone` varchar(100) NOT NULL,
    `DefaultCurrency` longtext NULL,
    `contact_email` varchar(254) NULL,
    `contact_phone` varchar(30) NULL,
    `tenant_uuid` char(36) NOT NULL,
    `created_at` datetime(6) NOT NULL,
    `updated_at` datetime(6) NULL,
    `CreatedByUserId` bigint NULL,
    `UpdatedByUserId` bigint NULL,
    PRIMARY KEY (`id`)
);

CREATE TABLE `organizations` (
    `id` bigint unsigned NOT NULL AUTO_INCREMENT,
    `tenant_id` bigint unsigned NOT NULL,
    `organization_code` varchar(50) NOT NULL,
    `organization_name` varchar(200) NOT NULL,
    `legal_name` varchar(250) NULL,
    `organization_type` enum('MEDICAL_GROUP','HOSPITAL','HEALTH_SYSTEM','PRACTICE','OTHER') NOT NULL,
    `tax_id` varchar(50) NULL,
    `npi` varchar(10) NULL,
    `phone` varchar(30) NULL,
    `fax` varchar(30) NULL,
    `email` varchar(254) NULL,
    `website` varchar(500) NULL,
    `status` enum('ACTIVE','INACTIVE') NOT NULL,
    `organization_uuid` char(36) NOT NULL,
    `created_at` datetime(6) NOT NULL,
    `updated_at` datetime(6) NOT NULL,
    `CreatedByUserId` bigint NULL,
    `UpdatedByUserId` bigint NULL,
    PRIMARY KEY (`id`),
    CONSTRAINT `fk_organizations_tenant_id_1` FOREIGN KEY (`tenant_id`) REFERENCES `tenants` (`id`) ON DELETE RESTRICT
);

CREATE TABLE `patients` (
    `id` bigint unsigned NOT NULL AUTO_INCREMENT,
    `tenant_id` bigint unsigned NOT NULL,
    `patient_id` varchar(50) NOT NULL,
    `mrn` varchar(100) NOT NULL,
    `account_number` varchar(100) NOT NULL,
    `first_name` varchar(100) NOT NULL,
    `middle_name` varchar(100) NULL,
    `last_name` varchar(100) NOT NULL,
    `preferred_name` varchar(100) NULL,
    `date_of_birth` date NOT NULL,
    `gender` enum('UNKNOWN','MALE','FEMALE','NON_BINARY','OTHER') NOT NULL,
    `patient_status` enum('ACTIVE','INACTIVE','DECEASED','MERGED') NOT NULL,
    `registration_date` date NULL,
    `last_visit_date` date NULL,
    `patient_class` enum('OUTPATIENT','INPATIENT','OBSERVATION','EMERGENCY','OTHER') NOT NULL,
    `portal_access` enum('DISABLED','ENABLED','PENDING') NOT NULL,
    `communication_consent` enum('PENDING','GRANTED','DECLINED') NOT NULL,
    `preferred_pharmacy` varchar(250) NULL,
    `referral_source` enum('PHYSICIAN','HOSPITAL','SELF','INSURANCE','EMPLOYER','OTHER') NULL,
    `financial_assistance` enum('NONE','PENDING','APPROVED','DECLINED') NOT NULL,
    `notes` text NULL,
    `patient_uuid` char(36) NOT NULL,
    `created_at` datetime(6) NOT NULL,
    `updated_at` datetime(6) NOT NULL,
    `CreatedByUserId` bigint NULL,
    `UpdatedByUserId` bigint NULL,
    PRIMARY KEY (`id`),
    CONSTRAINT `fk_patients_tenant_id_1` FOREIGN KEY (`tenant_id`) REFERENCES `tenants` (`id`) ON DELETE RESTRICT
);

CREATE TABLE `payers` (
    `id` bigint unsigned NOT NULL AUTO_INCREMENT,
    `tenant_id` bigint unsigned NOT NULL,
    `payer_code` varchar(50) NOT NULL,
    `payer_name` varchar(200) NOT NULL,
    `payer_id` varchar(50) NULL,
    `payer_type` enum('COMMERCIAL','MEDICARE','MEDICAID','TRICARE','WORKERS_COMP','OTHER') NOT NULL,
    `phone` varchar(30) NULL,
    `website` varchar(500) NULL,
    `status` enum('ACTIVE','INACTIVE') NOT NULL,
    `payer_uuid` char(36) NOT NULL,
    `created_at` datetime(6) NOT NULL,
    `updated_at` datetime(6) NOT NULL,
    `CreatedByUserId` bigint NULL,
    `UpdatedByUserId` bigint NULL,
    PRIMARY KEY (`id`),
    CONSTRAINT `fk_payers_tenant_id_1` FOREIGN KEY (`tenant_id`) REFERENCES `tenants` (`id`) ON DELETE RESTRICT
);

CREATE TABLE `providers` (
    `id` bigint unsigned NOT NULL AUTO_INCREMENT,
    `tenant_id` bigint unsigned NOT NULL,
    `organization_id` bigint unsigned NOT NULL,
    `provider_code` varchar(50) NOT NULL,
    `first_name` varchar(100) NOT NULL,
    `middle_name` varchar(100) NULL,
    `last_name` varchar(100) NOT NULL,
    `credential` varchar(50) NULL,
    `npi` varchar(10) NOT NULL,
    `specialty` varchar(150) NULL,
    `department` varchar(150) NULL,
    `phone` varchar(30) NULL,
    `email` varchar(254) NULL,
    `status` enum('ACTIVE','INACTIVE') NOT NULL,
    `provider_uuid` char(36) NOT NULL,
    `created_at` datetime(6) NOT NULL,
    `updated_at` datetime(6) NOT NULL,
    `CreatedByUserId` bigint NULL,
    `UpdatedByUserId` bigint NULL,
    PRIMARY KEY (`id`),
    CONSTRAINT `fk_providers_organization_id_2` FOREIGN KEY (`organization_id`) REFERENCES `organizations` (`id`) ON DELETE RESTRICT,
    CONSTRAINT `fk_providers_tenant_id_1` FOREIGN KEY (`tenant_id`) REFERENCES `tenants` (`id`) ON DELETE RESTRICT
);

CREATE INDEX `idx_organizations_organization_name` ON `organizations` (`organization_name`);

CREATE INDEX `idx_organizations_status` ON `organizations` (`status`);

CREATE INDEX `idx_organizations_tenant_id` ON `organizations` (`tenant_id`);

CREATE UNIQUE INDEX `uk_organizations_1` ON `organizations` (`tenant_id`, `organization_code`);

CREATE UNIQUE INDEX `uk_organizations_2` ON `organizations` (`tenant_id`, `npi`);

CREATE UNIQUE INDEX `uk_organizations_organization_uuid` ON `organizations` (`organization_uuid`);

CREATE INDEX `idx_patients_date_of_birth` ON `patients` (`date_of_birth`);

CREATE INDEX `idx_patients_first_name` ON `patients` (`first_name`);

CREATE INDEX `idx_patients_last_name` ON `patients` (`last_name`);

CREATE INDEX `idx_patients_patient_class` ON `patients` (`patient_class`);

CREATE INDEX `idx_patients_patient_status` ON `patients` (`patient_status`);

CREATE INDEX `idx_patients_tenant_id` ON `patients` (`tenant_id`);

CREATE UNIQUE INDEX `uk_patients_1` ON `patients` (`tenant_id`, `patient_id`);

CREATE UNIQUE INDEX `uk_patients_2` ON `patients` (`tenant_id`, `mrn`);

CREATE UNIQUE INDEX `uk_patients_3` ON `patients` (`tenant_id`, `account_number`);

CREATE UNIQUE INDEX `uk_patients_patient_uuid` ON `patients` (`patient_uuid`);

CREATE INDEX `idx_payers_payer_id` ON `payers` (`payer_id`);

CREATE INDEX `idx_payers_payer_name` ON `payers` (`payer_name`);

CREATE INDEX `idx_payers_status` ON `payers` (`status`);

CREATE INDEX `idx_payers_tenant_id` ON `payers` (`tenant_id`);

CREATE UNIQUE INDEX `uk_payers_1` ON `payers` (`tenant_id`, `payer_code`);

CREATE UNIQUE INDEX `uk_payers_payer_uuid` ON `payers` (`payer_uuid`);

CREATE INDEX `idx_providers_last_name` ON `providers` (`last_name`);

CREATE INDEX `idx_providers_organization_id` ON `providers` (`organization_id`);

CREATE INDEX `idx_providers_specialty` ON `providers` (`specialty`);

CREATE INDEX `idx_providers_status` ON `providers` (`status`);

CREATE INDEX `idx_providers_tenant_id` ON `providers` (`tenant_id`);

CREATE UNIQUE INDEX `uk_providers_1` ON `providers` (`tenant_id`, `provider_code`);

CREATE UNIQUE INDEX `uk_providers_2` ON `providers` (`tenant_id`, `npi`);

CREATE UNIQUE INDEX `uk_providers_provider_uuid` ON `providers` (`provider_uuid`);

CREATE UNIQUE INDEX `IX_tenants_tax_identifier` ON `tenants` (`tax_identifier`);

CREATE UNIQUE INDEX `IX_tenants_tenant_code` ON `tenants` (`tenant_code`);

CREATE INDEX `IX_tenants_tenant_name` ON `tenants` (`tenant_name`);

CREATE UNIQUE INDEX `IX_tenants_tenant_uuid` ON `tenants` (`tenant_uuid`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260921073829_ModelValidation', '10.0.11');

COMMIT;

