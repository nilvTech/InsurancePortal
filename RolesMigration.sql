START TRANSACTION;
CREATE TABLE `if_eob_records` (
    `id` bigint unsigned NOT NULL AUTO_INCREMENT,
    `tenant_id` bigint unsigned NOT NULL,
    `payer_id` bigint unsigned NOT NULL,
    `patient_id` bigint unsigned NULL,
    `claim_id` bigint unsigned NULL,
    `document_number` varchar(100) NULL,
    `file_name` varchar(255) NULL,
    `content_type` varchar(150) NULL,
    `storage_reference` varchar(1000) NULL,
    `file_size_bytes` bigint NULL,
    `received_at_utc` datetime(6) NOT NULL,
    `statement_date_utc` datetime(6) NULL,
    `status` varchar(30) NOT NULL,
    `notes` varchar(2000) NULL,
    `eob_guid` char(36) NOT NULL,
    `created_at_utc` datetime(6) NOT NULL,
    `updated_at_utc` datetime(6) NULL,
    `CreatedByUserId` bigint NULL,
    `UpdatedByUserId` bigint NULL,
    PRIMARY KEY (`id`)
);

CREATE TABLE `if_era_records` (
    `id` bigint unsigned NOT NULL AUTO_INCREMENT,
    `tenant_id` bigint unsigned NOT NULL,
    `payer_id` bigint unsigned NOT NULL,
    `trace_number` varchar(100) NULL,
    `check_number` varchar(100) NULL,
    `total_payment_amount` decimal(18,2) NOT NULL,
    `payment_date_utc` datetime(6) NULL,
    `received_at_utc` datetime(6) NOT NULL,
    `file_name` varchar(255) NULL,
    `storage_reference` varchar(1000) NULL,
    `transaction_control_number` varchar(100) NULL,
    `import_status` varchar(30) NOT NULL,
    `error_message` varchar(4000) NULL,
    `raw_content` longtext NULL,
    `era_guid` char(36) NOT NULL,
    `created_at_utc` datetime(6) NOT NULL,
    `updated_at_utc` datetime(6) NULL,
    `CreatedByUserId` bigint NULL,
    `UpdatedByUserId` bigint NULL,
    PRIMARY KEY (`id`)
);

CREATE TABLE `if_payment_adjustments` (
    `id` bigint unsigned NOT NULL AUTO_INCREMENT,
    `tenant_id` bigint unsigned NOT NULL,
    `payment_id` bigint unsigned NOT NULL,
    `payment_allocation_id` bigint unsigned NULL,
    `claim_id` bigint unsigned NULL,
    `adjustment_code` varchar(30) NOT NULL,
    `adjustment_group_code` varchar(30) NULL,
    `description` varchar(500) NOT NULL,
    `adjustment_amount` decimal(18,2) NOT NULL,
    `adjustment_date_utc` datetime(6) NOT NULL,
    `status` varchar(30) NOT NULL,
    `notes` varchar(2000) NULL,
    `adjustment_guid` char(36) NOT NULL,
    `created_at_utc` datetime(6) NOT NULL,
    `updated_at_utc` datetime(6) NULL,
    `CreatedByUserId` bigint NULL,
    `UpdatedByUserId` bigint NULL,
    PRIMARY KEY (`id`)
);

CREATE TABLE `if_payment_reconciliations` (
    `id` bigint unsigned NOT NULL AUTO_INCREMENT,
    `tenant_id` bigint unsigned NOT NULL,
    `reconciliation_number` varchar(100) NOT NULL,
    `period_start_utc` datetime(6) NOT NULL,
    `period_end_utc` datetime(6) NOT NULL,
    `started_at_utc` datetime(6) NOT NULL,
    `completed_at_utc` datetime(6) NULL,
    `expected_amount` decimal(18,2) NOT NULL,
    `actual_amount` decimal(18,2) NOT NULL,
    `difference_amount` decimal(18,2) NOT NULL,
    `payments_processed` int NOT NULL,
    `payments_matched` int NOT NULL,
    `payments_unmatched` int NOT NULL,
    `status` varchar(30) NOT NULL,
    `summary` varchar(4000) NULL,
    `error_message` varchar(4000) NULL,
    `reconciliation_guid` char(36) NOT NULL,
    `created_at_utc` datetime(6) NOT NULL,
    `updated_at_utc` datetime(6) NULL,
    `CreatedByUserId` bigint NULL,
    `UpdatedByUserId` bigint NULL,
    PRIMARY KEY (`id`)
);

CREATE TABLE `if_payments` (
    `id` bigint unsigned NOT NULL AUTO_INCREMENT,
    `tenant_id` bigint unsigned NOT NULL,
    `payment_number` varchar(100) NOT NULL,
    `payer_id` bigint unsigned NOT NULL,
    `patient_id` bigint unsigned NULL,
    `patient_insurance_id` bigint unsigned NULL,
    `check_number` varchar(100) NULL,
    `trace_number` varchar(100) NULL,
    `payment_method` varchar(30) NOT NULL,
    `payment_amount` decimal(18,2) NOT NULL,
    `allocated_amount` decimal(18,2) NOT NULL,
    `unallocated_amount` decimal(18,2) NOT NULL,
    `currency` varchar(3) NOT NULL,
    `payment_date_utc` datetime(6) NOT NULL,
    `received_date_utc` datetime(6) NULL,
    `posted_at_utc` datetime(6) NULL,
    `reversed_at_utc` datetime(6) NULL,
    `status` varchar(30) NOT NULL,
    `notes` varchar(2000) NULL,
    `payment_guid` char(36) NOT NULL,
    `created_at_utc` datetime(6) NOT NULL,
    `updated_at_utc` datetime(6) NULL,
    `CreatedByUserId` bigint NULL,
    `UpdatedByUserId` bigint NULL,
    PRIMARY KEY (`id`),
    CONSTRAINT `FK_if_payments_payers_payer_id` FOREIGN KEY (`payer_id`) REFERENCES `payers` (`id`) ON DELETE RESTRICT
);

CREATE TABLE `if_permissions` (
    `Id` bigint NOT NULL AUTO_INCREMENT,
    `permission_guid` char(36) NOT NULL,
    `tenant_id` bigint unsigned NOT NULL,
    `code` varchar(100) NOT NULL,
    `name` varchar(150) NOT NULL,
    `description` text NULL,
    `is_active` tinyint(1) NOT NULL,
    `created_at_utc` datetime(6) NOT NULL,
    `updated_at_utc` datetime(6) NULL,
    `CreatedByUserId` bigint NULL,
    `UpdatedByUserId` bigint NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_if_permissions_tenants_tenant_id` FOREIGN KEY (`tenant_id`) REFERENCES `tenants` (`id`) ON DELETE RESTRICT
);

CREATE TABLE `if_roles` (
    `Id` bigint NOT NULL AUTO_INCREMENT,
    `role_guid` char(36) NOT NULL,
    `tenant_id` bigint unsigned NOT NULL,
    `name` varchar(100) NOT NULL,
    `description` text NULL,
    `is_active` tinyint(1) NOT NULL,
    `created_at_utc` datetime(6) NOT NULL,
    `updated_at_utc` datetime(6) NULL,
    `CreatedByUserId` bigint NULL,
    `UpdatedByUserId` bigint NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_if_roles_tenants_tenant_id` FOREIGN KEY (`tenant_id`) REFERENCES `tenants` (`id`) ON DELETE RESTRICT
);

CREATE TABLE `if_payment_allocations` (
    `id` bigint unsigned NOT NULL AUTO_INCREMENT,
    `tenant_id` bigint unsigned NOT NULL,
    `payment_id` bigint unsigned NOT NULL,
    `claim_id` bigint unsigned NOT NULL,
    `claim_line_id` bigint unsigned NULL,
    `allocated_amount` decimal(18,2) NOT NULL,
    `allocation_date_utc` datetime(6) NOT NULL,
    `status` varchar(30) NOT NULL,
    `notes` varchar(2000) NULL,
    `allocation_guid` char(36) NOT NULL,
    `created_at_utc` datetime(6) NOT NULL,
    `updated_at_utc` datetime(6) NULL,
    `CreatedByUserId` bigint NULL,
    `UpdatedByUserId` bigint NULL,
    PRIMARY KEY (`id`),
    CONSTRAINT `FK_if_payment_allocations_claims_claim_id` FOREIGN KEY (`claim_id`) REFERENCES `claims` (`id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_if_payment_allocations_if_payments_payment_id` FOREIGN KEY (`payment_id`) REFERENCES `if_payments` (`id`) ON DELETE CASCADE
);

CREATE TABLE `if_refunds` (
    `id` bigint unsigned NOT NULL AUTO_INCREMENT,
    `tenant_id` bigint unsigned NOT NULL,
    `payment_id` bigint unsigned NOT NULL,
    `refund_number` varchar(100) NOT NULL,
    `refund_amount` decimal(18,2) NOT NULL,
    `reason` varchar(1000) NOT NULL,
    `status` varchar(30) NOT NULL,
    `requested_at_utc` datetime(6) NOT NULL,
    `requested_by_user_id` bigint unsigned NULL,
    `reviewed_at_utc` datetime(6) NULL,
    `reviewed_by_user_id` bigint unsigned NULL,
    `review_notes` varchar(2000) NULL,
    `processed_at_utc` datetime(6) NULL,
    `refund_guid` char(36) NOT NULL,
    `created_at_utc` datetime(6) NOT NULL,
    `updated_at_utc` datetime(6) NULL,
    `CreatedByUserId` bigint NULL,
    `UpdatedByUserId` bigint NULL,
    PRIMARY KEY (`id`),
    CONSTRAINT `FK_if_refunds_if_payments_payment_id` FOREIGN KEY (`payment_id`) REFERENCES `if_payments` (`id`) ON DELETE RESTRICT
);

CREATE TABLE `if_role_permissions` (
    `Id` bigint NOT NULL AUTO_INCREMENT,
    `tenant_id` bigint unsigned NOT NULL,
    `role_id` bigint NOT NULL,
    `permission_id` bigint NOT NULL,
    `ExternalId` char(36) NOT NULL,
    `created_at_utc` datetime(6) NOT NULL,
    `updated_at_utc` datetime(6) NULL,
    `CreatedByUserId` bigint NULL,
    `UpdatedByUserId` bigint NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_if_role_permissions_if_permissions_permission_id` FOREIGN KEY (`permission_id`) REFERENCES `if_permissions` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_if_role_permissions_if_roles_role_id` FOREIGN KEY (`role_id`) REFERENCES `if_roles` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_if_role_permissions_tenants_tenant_id` FOREIGN KEY (`tenant_id`) REFERENCES `tenants` (`id`) ON DELETE RESTRICT
);

CREATE UNIQUE INDEX `IX_if_eob_records_eob_guid` ON `if_eob_records` (`eob_guid`);

CREATE INDEX `IX_if_eob_records_tenant_id_claim_id` ON `if_eob_records` (`tenant_id`, `claim_id`);

CREATE INDEX `IX_if_eob_records_tenant_id_payer_id_received_at_utc` ON `if_eob_records` (`tenant_id`, `payer_id`, `received_at_utc`);

CREATE UNIQUE INDEX `IX_if_era_records_era_guid` ON `if_era_records` (`era_guid`);

CREATE INDEX `IX_if_era_records_tenant_id_payer_id_received_at_utc` ON `if_era_records` (`tenant_id`, `payer_id`, `received_at_utc`);

CREATE INDEX `IX_if_era_records_tenant_id_trace_number` ON `if_era_records` (`tenant_id`, `trace_number`);

CREATE UNIQUE INDEX `IX_if_payment_adjustments_adjustment_guid` ON `if_payment_adjustments` (`adjustment_guid`);

CREATE INDEX `IX_if_payment_adjustments_tenant_id_claim_id` ON `if_payment_adjustments` (`tenant_id`, `claim_id`);

CREATE INDEX `IX_if_payment_adjustments_tenant_id_payment_id` ON `if_payment_adjustments` (`tenant_id`, `payment_id`);

CREATE UNIQUE INDEX `IX_if_payment_allocations_allocation_guid` ON `if_payment_allocations` (`allocation_guid`);

CREATE INDEX `IX_if_payment_allocations_claim_id` ON `if_payment_allocations` (`claim_id`);

CREATE INDEX `IX_if_payment_allocations_payment_id` ON `if_payment_allocations` (`payment_id`);

CREATE INDEX `IX_if_payment_allocations_tenant_id_claim_id` ON `if_payment_allocations` (`tenant_id`, `claim_id`);

CREATE INDEX `IX_if_payment_allocations_tenant_id_payment_id` ON `if_payment_allocations` (`tenant_id`, `payment_id`);

CREATE UNIQUE INDEX `IX_if_payment_reconciliations_reconciliation_guid` ON `if_payment_reconciliations` (`reconciliation_guid`);

CREATE UNIQUE INDEX `IX_if_payment_reconciliations_tenant_id_reconciliation_number` ON `if_payment_reconciliations` (`tenant_id`, `reconciliation_number`);

CREATE INDEX `IX_if_payment_reconciliations_tenant_id_started_at_utc` ON `if_payment_reconciliations` (`tenant_id`, `started_at_utc`);

CREATE INDEX `IX_if_payments_payer_id` ON `if_payments` (`payer_id`);

CREATE UNIQUE INDEX `IX_if_payments_payment_guid` ON `if_payments` (`payment_guid`);

CREATE INDEX `IX_if_payments_tenant_id_payer_id_payment_date_utc` ON `if_payments` (`tenant_id`, `payer_id`, `payment_date_utc`);

CREATE UNIQUE INDEX `IX_if_payments_tenant_id_payment_number` ON `if_payments` (`tenant_id`, `payment_number`);

CREATE INDEX `IX_if_payments_tenant_id_status` ON `if_payments` (`tenant_id`, `status`);

CREATE UNIQUE INDEX `IX_if_permissions_permission_guid` ON `if_permissions` (`permission_guid`);

CREATE UNIQUE INDEX `IX_if_permissions_tenant_id_code` ON `if_permissions` (`tenant_id`, `code`);

CREATE INDEX `IX_if_refunds_payment_id` ON `if_refunds` (`payment_id`);

CREATE UNIQUE INDEX `IX_if_refunds_refund_guid` ON `if_refunds` (`refund_guid`);

CREATE INDEX `IX_if_refunds_tenant_id_payment_id` ON `if_refunds` (`tenant_id`, `payment_id`);

CREATE UNIQUE INDEX `IX_if_refunds_tenant_id_refund_number` ON `if_refunds` (`tenant_id`, `refund_number`);

CREATE INDEX `IX_if_refunds_tenant_id_status` ON `if_refunds` (`tenant_id`, `status`);

CREATE INDEX `IX_if_role_permissions_permission_id` ON `if_role_permissions` (`permission_id`);

CREATE INDEX `IX_if_role_permissions_role_id` ON `if_role_permissions` (`role_id`);

CREATE UNIQUE INDEX `IX_if_role_permissions_tenant_id_role_id_permission_id` ON `if_role_permissions` (`tenant_id`, `role_id`, `permission_id`);

CREATE UNIQUE INDEX `IX_if_roles_role_guid` ON `if_roles` (`role_guid`);

CREATE UNIQUE INDEX `IX_if_roles_tenant_id_name` ON `if_roles` (`tenant_id`, `name`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20261119081838_AddPaymentsModule', '10.0.11');

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20261119082333_AddRolesAndPermissions', '10.0.11');

COMMIT;

