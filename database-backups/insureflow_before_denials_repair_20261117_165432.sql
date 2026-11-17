-- MySQL dump 10.13  Distrib 8.0.46, for Win64 (x86_64)
--
-- Host: localhost    Database: insureflow_db
-- ------------------------------------------------------
-- Server version	8.0.46

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `__efmigrationshistory`
--

DROP TABLE IF EXISTS `__efmigrationshistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) NOT NULL,
  `ProductVersion` varchar(32) NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `__efmigrationshistory`
--

LOCK TABLES `__efmigrationshistory` WRITE;
/*!40000 ALTER TABLE `__efmigrationshistory` DISABLE KEYS */;
INSERT INTO `__efmigrationshistory` VALUES ('20260923111102_InitialBaseline','10.0.12'),('20261022100000_AlignClaimsSchema','10.0.12'),('20261103071528_AddClaimHistory','10.0.12'),('20261103105944_AlignClaimRejectionSchema','10.0.11'),('20261103112042_DiagnoseClaimRejectionModelDiff','10.0.11');
/*!40000 ALTER TABLE `__efmigrationshistory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `appeal_attachments`
--

DROP TABLE IF EXISTS `appeal_attachments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `appeal_attachments` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `tenant_id` bigint unsigned NOT NULL,
  `appeal_id` bigint unsigned NOT NULL,
  `file_name` varchar(255) NOT NULL,
  `storage_key` varchar(500) NOT NULL,
  `content_type` varchar(100) DEFAULT NULL,
  `file_size` bigint unsigned DEFAULT NULL,
  `uploaded_at` datetime(6) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `idx_appeal_attachments_tenant_id` (`tenant_id`),
  KEY `idx_appeal_attachments_appeal_id` (`appeal_id`),
  CONSTRAINT `fk_appeal_attachments_appeal_id_1` FOREIGN KEY (`appeal_id`) REFERENCES `appeals` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `appeal_attachments`
--

LOCK TABLES `appeal_attachments` WRITE;
/*!40000 ALTER TABLE `appeal_attachments` DISABLE KEYS */;
INSERT INTO `appeal_attachments` VALUES (1,1,1,'appeal_clinical_documentation.pdf','appeals/2026/APL-2026-0001/clinical_documentation.pdf','application/pdf',512000,'2026-09-05 13:30:00.000000'),(2,2,2,'benefit_policy.pdf','appeals/2026/APL-2026-0002/benefit_policy.pdf','application/pdf',310000,'2026-09-06 09:45:00.000000');
/*!40000 ALTER TABLE `appeal_attachments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `appeal_decisions`
--

DROP TABLE IF EXISTS `appeal_decisions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `appeal_decisions` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `tenant_id` bigint unsigned NOT NULL,
  `appeal_id` bigint unsigned NOT NULL,
  `decision` varchar(30) NOT NULL DEFAULT 'PENDING',
  `decision_reason` varchar(1000) DEFAULT NULL,
  `approved_amount` decimal(12,2) DEFAULT NULL,
  `decision_at` datetime(6) NOT NULL,
  `decided_by_user_id` bigint unsigned DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `idx_appeal_decisions_tenant_id` (`tenant_id`),
  KEY `idx_appeal_decisions_appeal_id` (`appeal_id`),
  KEY `idx_appeal_decisions_decision_at` (`decision_at`),
  CONSTRAINT `fk_appeal_decisions_appeal_id_1` FOREIGN KEY (`appeal_id`) REFERENCES `appeals` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `appeal_decisions`
--

LOCK TABLES `appeal_decisions` WRITE;
/*!40000 ALTER TABLE `appeal_decisions` DISABLE KEYS */;
INSERT INTO `appeal_decisions` VALUES (1,1,1,'PENDING','Payer review in progress.',NULL,'2026-09-05 14:30:00.000000',3),(2,2,2,'PENDING','Awaiting submission.',NULL,'2026-09-06 10:15:00.000000',4);
/*!40000 ALTER TABLE `appeal_decisions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `appeal_documentation`
--

DROP TABLE IF EXISTS `appeal_documentation`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `appeal_documentation` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `tenant_id` bigint unsigned NOT NULL,
  `appeal_id` bigint unsigned NOT NULL,
  `document_type` varchar(50) NOT NULL DEFAULT 'CLINICAL',
  `document_text` text NOT NULL,
  `created_at` datetime(6) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `idx_appeal_documentation_tenant_id` (`tenant_id`),
  KEY `idx_appeal_documentation_appeal_id` (`appeal_id`),
  CONSTRAINT `fk_appeal_documentation_appeal_id_1` FOREIGN KEY (`appeal_id`) REFERENCES `appeals` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `appeal_documentation`
--

LOCK TABLES `appeal_documentation` WRITE;
/*!40000 ALTER TABLE `appeal_documentation` DISABLE KEYS */;
INSERT INTO `appeal_documentation` VALUES (1,1,1,'CLINICAL','Progress notes and clinical findings supporting medical necessity were included in the appeal package.','2026-09-05 13:15:00.000000'),(2,2,2,'POLICY','Benefit policy excerpt submitted for payer coverage review.','2026-09-06 09:30:00.000000');
/*!40000 ALTER TABLE `appeal_documentation` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `appeal_external_reviews`
--

DROP TABLE IF EXISTS `appeal_external_reviews`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `appeal_external_reviews` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `tenant_id` bigint unsigned NOT NULL,
  `appeal_id` bigint unsigned NOT NULL,
  `review_organization` varchar(255) NOT NULL,
  `status` varchar(30) NOT NULL DEFAULT 'REQUESTED',
  `requested_date` date DEFAULT NULL,
  `decision_date` date DEFAULT NULL,
  `decision` varchar(30) DEFAULT NULL,
  `notes` text,
  PRIMARY KEY (`id`),
  KEY `idx_appeal_external_reviews_tenant_id` (`tenant_id`),
  KEY `idx_appeal_external_reviews_appeal_id` (`appeal_id`),
  CONSTRAINT `fk_appeal_external_reviews_appeal_id_1` FOREIGN KEY (`appeal_id`) REFERENCES `appeals` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `appeal_external_reviews`
--

LOCK TABLES `appeal_external_reviews` WRITE;
/*!40000 ALTER TABLE `appeal_external_reviews` DISABLE KEYS */;
INSERT INTO `appeal_external_reviews` VALUES (1,1,1,'Independent Clinical Review Organization','REQUESTED','2026-09-06',NULL,NULL,'External review requested if payer first-level appeal is unfavorable.'),(2,2,2,'State Independent Review Organization','REQUESTED','2026-09-07',NULL,NULL,'External review option documented.');
/*!40000 ALTER TABLE `appeal_external_reviews` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `appeal_history`
--

DROP TABLE IF EXISTS `appeal_history`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `appeal_history` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `tenant_id` bigint unsigned NOT NULL,
  `appeal_id` bigint unsigned NOT NULL,
  `from_status` varchar(30) NOT NULL,
  `to_status` varchar(30) NOT NULL,
  `note` varchar(1000) DEFAULT NULL,
  `changed_at` datetime(6) NOT NULL,
  `changed_by_user_id` bigint unsigned DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `idx_appeal_history_tenant_id` (`tenant_id`),
  KEY `idx_appeal_history_appeal_id` (`appeal_id`),
  KEY `idx_appeal_history_changed_at` (`changed_at`),
  CONSTRAINT `fk_appeal_history_appeal_id_1` FOREIGN KEY (`appeal_id`) REFERENCES `appeals` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `appeal_history`
--

LOCK TABLES `appeal_history` WRITE;
/*!40000 ALTER TABLE `appeal_history` DISABLE KEYS */;
INSERT INTO `appeal_history` VALUES (1,1,1,'DRAFT','SUBMITTED','Appeal package submitted to payer.','2026-09-05 14:00:00.000000',3),(2,2,2,'DRAFT','DRAFT','Appeal package created and pending documentation.','2026-09-06 10:00:00.000000',4);
/*!40000 ALTER TABLE `appeal_history` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `appeal_notifications`
--

DROP TABLE IF EXISTS `appeal_notifications`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `appeal_notifications` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `tenant_id` bigint unsigned NOT NULL,
  `appeal_id` bigint unsigned NOT NULL,
  `notification_type` varchar(30) NOT NULL DEFAULT 'DEADLINE',
  `recipient` varchar(255) NOT NULL,
  `status` varchar(30) NOT NULL DEFAULT 'PENDING',
  `scheduled_at` datetime(6) NOT NULL,
  `sent_at` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `idx_appeal_notifications_tenant_id` (`tenant_id`),
  KEY `idx_appeal_notifications_status` (`status`),
  KEY `idx_appeal_notifications_scheduled_at` (`scheduled_at`),
  KEY `fk_appeal_notifications_appeal_id_1` (`appeal_id`),
  CONSTRAINT `fk_appeal_notifications_appeal_id_1` FOREIGN KEY (`appeal_id`) REFERENCES `appeals` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `appeal_notifications`
--

LOCK TABLES `appeal_notifications` WRITE;
/*!40000 ALTER TABLE `appeal_notifications` DISABLE KEYS */;
INSERT INTO `appeal_notifications` VALUES (1,1,1,'DEADLINE','rcm@insureflow.example','PENDING','2026-11-15 09:00:00.000000',NULL),(2,2,2,'DEADLINE','rcm@northstar.example','PENDING','2026-11-13 09:00:00.000000',NULL);
/*!40000 ALTER TABLE `appeal_notifications` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `appeals`
--

DROP TABLE IF EXISTS `appeals`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `appeals` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `appeal_guid` char(36) NOT NULL,
  `tenant_id` bigint unsigned NOT NULL,
  `denial_id` bigint unsigned NOT NULL,
  `claim_id` bigint unsigned NOT NULL,
  `patient_id` bigint unsigned NOT NULL,
  `payer_id` bigint unsigned NOT NULL,
  `appeal_number` varchar(60) NOT NULL,
  `appeal_level` varchar(30) NOT NULL DEFAULT 'FIRST_LEVEL',
  `reason` varchar(50) NOT NULL DEFAULT 'MEDICAL_NECESSITY',
  `status` varchar(30) NOT NULL DEFAULT 'DRAFT',
  `submission_date` date NOT NULL,
  `deadline` date DEFAULT NULL,
  `rationale` text,
  `amount_appealed` decimal(12,2) DEFAULT NULL,
  `decision` varchar(30) DEFAULT NULL,
  `decision_date` date DEFAULT NULL,
  `decision_reason` varchar(1000) DEFAULT NULL,
  `created_at` datetime(6) NOT NULL,
  `updated_at` datetime(6) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_appeals_appeal_guid` (`appeal_guid`),
  UNIQUE KEY `uk_appeals_1` (`tenant_id`,`appeal_number`),
  KEY `idx_appeals_tenant_id` (`tenant_id`),
  KEY `idx_appeals_status` (`status`),
  KEY `idx_appeals_deadline` (`deadline`),
  KEY `fk_appeals_denial_id_1` (`denial_id`),
  KEY `fk_appeals_claim_id_2` (`claim_id`),
  KEY `fk_appeals_patient_id_3` (`patient_id`),
  KEY `fk_appeals_payer_id_4` (`payer_id`),
  CONSTRAINT `fk_appeals_claim_id_2` FOREIGN KEY (`claim_id`) REFERENCES `claims` (`id`),
  CONSTRAINT `fk_appeals_denial_id_1` FOREIGN KEY (`denial_id`) REFERENCES `denials` (`id`),
  CONSTRAINT `fk_appeals_patient_id_3` FOREIGN KEY (`patient_id`) REFERENCES `patients` (`id`),
  CONSTRAINT `fk_appeals_payer_id_4` FOREIGN KEY (`payer_id`) REFERENCES `payers` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `appeals`
--

LOCK TABLES `appeals` WRITE;
/*!40000 ALTER TABLE `appeals` DISABLE KEYS */;
INSERT INTO `appeals` VALUES (1,'af000000-0000-4000-8000-000000000001',1,1,2,2,2,'APL-2026-0001','FIRST_LEVEL','MEDICAL_NECESSITY','SUBMITTED','2026-09-05','2026-11-30','Clinical documentation supports the medical necessity of the billed service.',450.00,NULL,NULL,NULL,'2026-09-05 14:00:00.000000','2026-09-05 14:00:00.000000'),(2,'af000000-0000-4000-8000-000000000002',2,2,3,3,3,'APL-2026-0002','FIRST_LEVEL','COVERAGE','DRAFT','2026-09-06','2026-11-28','Requesting review of benefit coverage and payer policy.',80.00,NULL,NULL,NULL,'2026-09-06 10:00:00.000000','2026-09-06 10:00:00.000000');
/*!40000 ALTER TABLE `appeals` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `authorization_appeals`
--

DROP TABLE IF EXISTS `authorization_appeals`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `authorization_appeals` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `tenant_id` bigint unsigned NOT NULL,
  `authorization_id` bigint unsigned NOT NULL,
  `reason` varchar(1000) NOT NULL,
  `notes` text,
  `status` varchar(30) NOT NULL DEFAULT 'SUBMITTED',
  `submitted_at` datetime(6) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `idx_authorization_appeals_tenant_authorization` (`tenant_id`,`authorization_id`),
  KEY `idx_authorization_appeals_submitted_at` (`submitted_at`),
  KEY `fk_authorization_appeals_authorization` (`authorization_id`),
  CONSTRAINT `fk_authorization_appeals_authorization` FOREIGN KEY (`authorization_id`) REFERENCES `authorizations` (`id`) ON DELETE CASCADE,
  CONSTRAINT `fk_authorization_appeals_tenant` FOREIGN KEY (`tenant_id`) REFERENCES `tenants` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `authorization_appeals`
--

LOCK TABLES `authorization_appeals` WRITE;
/*!40000 ALTER TABLE `authorization_appeals` DISABLE KEYS */;
/*!40000 ALTER TABLE `authorization_appeals` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `authorization_diagnoses`
--

DROP TABLE IF EXISTS `authorization_diagnoses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `authorization_diagnoses` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `tenant_id` bigint unsigned NOT NULL,
  `authorization_id` bigint unsigned NOT NULL,
  `icd10_code` varchar(20) NOT NULL,
  `description` varchar(255) DEFAULT NULL,
  `sequence` int NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  KEY `idx_authorization_diagnoses_tenant_id` (`tenant_id`),
  KEY `idx_authorization_diagnoses_authorization_id` (`authorization_id`),
  CONSTRAINT `fk_authorization_diagnoses_authorization_id_1` FOREIGN KEY (`authorization_id`) REFERENCES `authorizations` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `authorization_diagnoses`
--

LOCK TABLES `authorization_diagnoses` WRITE;
/*!40000 ALTER TABLE `authorization_diagnoses` DISABLE KEYS */;
INSERT INTO `authorization_diagnoses` VALUES (1,1,1,'E11.9','Type 2 diabetes mellitus without complications',1),(2,1,2,'I10','Essential hypertension',1);
/*!40000 ALTER TABLE `authorization_diagnoses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `authorization_documents`
--

DROP TABLE IF EXISTS `authorization_documents`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `authorization_documents` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `tenant_id` bigint unsigned NOT NULL,
  `authorization_id` bigint unsigned NOT NULL,
  `file_name` varchar(255) NOT NULL,
  `storage_key` varchar(500) NOT NULL,
  `content_type` varchar(100) DEFAULT NULL,
  `file_size` bigint unsigned DEFAULT NULL,
  `document_type` varchar(40) NOT NULL DEFAULT 'SUPPORTING',
  `uploaded_at` datetime(6) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `idx_authorization_documents_tenant_authorization` (`tenant_id`,`authorization_id`),
  KEY `fk_authorization_documents_authorization` (`authorization_id`),
  CONSTRAINT `fk_authorization_documents_authorization` FOREIGN KEY (`authorization_id`) REFERENCES `authorizations` (`id`) ON DELETE CASCADE,
  CONSTRAINT `fk_authorization_documents_tenant` FOREIGN KEY (`tenant_id`) REFERENCES `tenants` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `authorization_documents`
--

LOCK TABLES `authorization_documents` WRITE;
/*!40000 ALTER TABLE `authorization_documents` DISABLE KEYS */;
/*!40000 ALTER TABLE `authorization_documents` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `authorization_history`
--

DROP TABLE IF EXISTS `authorization_history`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `authorization_history` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `tenant_id` bigint unsigned NOT NULL,
  `authorization_id` bigint unsigned NOT NULL,
  `from_status` varchar(30) NOT NULL,
  `to_status` varchar(30) NOT NULL,
  `note` varchar(1000) DEFAULT NULL,
  `changed_at` datetime(6) NOT NULL,
  `changed_by_user_id` bigint unsigned DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `idx_authorization_history_tenant_id` (`tenant_id`),
  KEY `idx_authorization_history_authorization_id` (`authorization_id`),
  KEY `idx_authorization_history_changed_at` (`changed_at`),
  CONSTRAINT `fk_authorization_history_authorization_id_1` FOREIGN KEY (`authorization_id`) REFERENCES `authorizations` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `authorization_history`
--

LOCK TABLES `authorization_history` WRITE;
/*!40000 ALTER TABLE `authorization_history` DISABLE KEYS */;
INSERT INTO `authorization_history` VALUES (1,1,1,'SUBMITTED','APPROVED','Payer approved requested services.','2026-07-28 14:00:00.000000',2),(2,1,2,'DRAFT','PENDING','Authorization submitted to payer.','2026-08-26 10:05:00.000000',2);
/*!40000 ALTER TABLE `authorization_history` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `authorization_notes`
--

DROP TABLE IF EXISTS `authorization_notes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `authorization_notes` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `tenant_id` bigint unsigned NOT NULL,
  `authorization_id` bigint unsigned NOT NULL,
  `note_type` varchar(30) NOT NULL DEFAULT 'GENERAL',
  `note_text` text NOT NULL,
  `created_at` datetime(6) NOT NULL,
  `created_by_user_id` bigint unsigned DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `idx_authorization_notes_tenant_id` (`tenant_id`),
  KEY `idx_authorization_notes_authorization_id` (`authorization_id`),
  KEY `idx_authorization_notes_created_at` (`created_at`),
  CONSTRAINT `fk_authorization_notes_authorization_id_1` FOREIGN KEY (`authorization_id`) REFERENCES `authorizations` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `authorization_notes`
--

LOCK TABLES `authorization_notes` WRITE;
/*!40000 ALTER TABLE `authorization_notes` DISABLE KEYS */;
INSERT INTO `authorization_notes` VALUES (1,1,1,'CLINICAL','Clinical documentation attached and reviewed.','2026-07-27 15:00:00.000000',2),(2,1,2,'GENERAL','Awaiting payer medical review.','2026-08-26 10:10:00.000000',2);
/*!40000 ALTER TABLE `authorization_notes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `authorization_services`
--

DROP TABLE IF EXISTS `authorization_services`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `authorization_services` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `tenant_id` bigint unsigned NOT NULL,
  `authorization_id` bigint unsigned NOT NULL,
  `cpt_hcpcs_code` varchar(20) NOT NULL,
  `modifier` varchar(20) DEFAULT NULL,
  `diagnosis_code` varchar(20) DEFAULT NULL,
  `units_requested` int NOT NULL DEFAULT '1',
  `units_approved` int DEFAULT NULL,
  `requested_amount` decimal(12,2) DEFAULT NULL,
  `approved_amount` decimal(12,2) DEFAULT NULL,
  `status` varchar(30) NOT NULL DEFAULT 'REQUESTED',
  PRIMARY KEY (`id`),
  KEY `idx_authorization_services_tenant_id` (`tenant_id`),
  KEY `idx_authorization_services_authorization_id` (`authorization_id`),
  CONSTRAINT `fk_authorization_services_authorization_id_1` FOREIGN KEY (`authorization_id`) REFERENCES `authorizations` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `authorization_services`
--

LOCK TABLES `authorization_services` WRITE;
/*!40000 ALTER TABLE `authorization_services` DISABLE KEYS */;
INSERT INTO `authorization_services` VALUES (1,1,1,'99213',NULL,'E11.9',4,4,600.00,480.00,'APPROVED'),(2,1,2,'99214',NULL,'I10',3,NULL,750.00,NULL,'REQUESTED');
/*!40000 ALTER TABLE `authorization_services` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `authorizations`
--

DROP TABLE IF EXISTS `authorizations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `authorizations` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `authorization_guid` char(36) NOT NULL,
  `tenant_id` bigint unsigned NOT NULL,
  `patient_id` bigint unsigned NOT NULL,
  `patient_insurance_id` bigint unsigned NOT NULL,
  `payer_id` bigint unsigned NOT NULL,
  `provider_id` bigint unsigned NOT NULL,
  `authorization_number` varchar(60) NOT NULL,
  `request_type` varchar(30) NOT NULL DEFAULT 'STANDARD',
  `status` varchar(30) NOT NULL DEFAULT 'DRAFT',
  `priority` varchar(20) NOT NULL DEFAULT 'ROUTINE',
  `place_of_service` varchar(10) NOT NULL DEFAULT '11',
  `requested_from` date NOT NULL,
  `requested_to` date NOT NULL,
  `decision_date` date DEFAULT NULL,
  `payer_reference` varchar(100) DEFAULT NULL,
  `clinical_summary` text,
  `decision_reason` varchar(1000) DEFAULT NULL,
  `created_at` datetime(6) NOT NULL,
  `updated_at` datetime(6) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_authorizations_authorization_guid` (`authorization_guid`),
  UNIQUE KEY `uk_authorizations_tenant_id` (`tenant_id`),
  UNIQUE KEY `uk_authorizations_tenant_number` (`tenant_id`,`authorization_number`),
  KEY `idx_authorizations_patient_id` (`patient_id`),
  KEY `idx_authorizations_status` (`status`),
  KEY `idx_authorizations_priority` (`priority`),
  KEY `idx_authorizations_created_at` (`created_at`),
  KEY `fk_authorizations_patient_insurance_id_3` (`patient_insurance_id`),
  KEY `fk_authorizations_payer_id_4` (`payer_id`),
  KEY `fk_authorizations_provider_id_5` (`provider_id`),
  CONSTRAINT `fk_authorizations_patient_id_2` FOREIGN KEY (`patient_id`) REFERENCES `patients` (`id`),
  CONSTRAINT `fk_authorizations_patient_insurance_id_3` FOREIGN KEY (`patient_insurance_id`) REFERENCES `patient_insurances` (`id`),
  CONSTRAINT `fk_authorizations_payer_id_4` FOREIGN KEY (`payer_id`) REFERENCES `payers` (`id`),
  CONSTRAINT `fk_authorizations_provider_id_5` FOREIGN KEY (`provider_id`) REFERENCES `providers` (`id`),
  CONSTRAINT `fk_authorizations_tenant_id_1` FOREIGN KEY (`tenant_id`) REFERENCES `tenants` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `authorizations`
--

LOCK TABLES `authorizations` WRITE;
/*!40000 ALTER TABLE `authorizations` DISABLE KEYS */;
INSERT INTO `authorizations` VALUES (1,'ac000000-0000-4000-8000-000000000001',1,1,1,1,1,'AUTH-2026-0001','STANDARD','APPROVED','ROUTINE','11','2026-08-01','2026-10-31','2026-07-28','AET-AUTH-001','Primary care follow-up and chronic disease management.','Medical necessity criteria met.','2026-07-25 09:00:00.000000','2026-07-28 14:00:00.000000');
/*!40000 ALTER TABLE `authorizations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `claim_attachments`
--

DROP TABLE IF EXISTS `claim_attachments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `claim_attachments` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `tenant_id` bigint unsigned NOT NULL,
  `claim_id` bigint unsigned NOT NULL,
  `file_name` varchar(255) NOT NULL,
  `storage_key` varchar(500) NOT NULL,
  `content_type` varchar(100) DEFAULT NULL,
  `file_size` bigint unsigned DEFAULT NULL,
  `document_type` varchar(40) NOT NULL DEFAULT 'SUPPORTING',
  `uploaded_at` datetime(6) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `idx_claim_attachments_tenant_claim` (`tenant_id`,`claim_id`),
  KEY `fk_claim_attachments_claim` (`claim_id`),
  CONSTRAINT `fk_claim_attachments_claim` FOREIGN KEY (`claim_id`) REFERENCES `claims` (`id`) ON DELETE CASCADE,
  CONSTRAINT `fk_claim_attachments_tenant` FOREIGN KEY (`tenant_id`) REFERENCES `tenants` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `claim_attachments`
--

LOCK TABLES `claim_attachments` WRITE;
/*!40000 ALTER TABLE `claim_attachments` DISABLE KEYS */;
/*!40000 ALTER TABLE `claim_attachments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `claim_diagnoses`
--

DROP TABLE IF EXISTS `claim_diagnoses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `claim_diagnoses` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `tenant_id` bigint unsigned NOT NULL,
  `claim_id` bigint unsigned NOT NULL,
  `icd10_code` varchar(20) NOT NULL,
  `description` varchar(500) DEFAULT NULL,
  `sequence` int NOT NULL,
  PRIMARY KEY (`id`),
  KEY `idx_claim_diagnoses_tenant_claim_sequence` (`tenant_id`,`claim_id`,`sequence`),
  KEY `fk_claim_diagnoses_claim` (`claim_id`),
  CONSTRAINT `fk_claim_diagnoses_claim` FOREIGN KEY (`claim_id`) REFERENCES `claims` (`id`) ON DELETE CASCADE,
  CONSTRAINT `fk_claim_diagnoses_tenant` FOREIGN KEY (`tenant_id`) REFERENCES `tenants` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `claim_diagnoses`
--

LOCK TABLES `claim_diagnoses` WRITE;
/*!40000 ALTER TABLE `claim_diagnoses` DISABLE KEYS */;
/*!40000 ALTER TABLE `claim_diagnoses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `claim_history`
--

DROP TABLE IF EXISTS `claim_history`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `claim_history` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `tenant_id` bigint unsigned NOT NULL,
  `claim_id` bigint unsigned NOT NULL,
  `from_status` varchar(30) NOT NULL,
  `to_status` varchar(30) NOT NULL,
  `note` text,
  `changed_at` datetime(6) NOT NULL,
  `changed_by_user_id` bigint unsigned DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `ix_claim_history_claim_id_id` (`claim_id`,`id`),
  KEY `ix_claim_history_tenant_claim_changed_at` (`tenant_id`,`claim_id`,`changed_at`),
  CONSTRAINT `fk_claim_history_claim_id_1` FOREIGN KEY (`claim_id`) REFERENCES `claims` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `claim_history`
--

LOCK TABLES `claim_history` WRITE;
/*!40000 ALTER TABLE `claim_history` DISABLE KEYS */;
INSERT INTO `claim_history` VALUES (1,1,1,'SUBMITTED','PAID','ERA payment posted.','2026-08-28 12:00:00.000000',2),(2,1,2,'SUBMITTED','DENIED','Payer denied claim.','2026-09-01 09:00:00.000000',2),(3,2,3,'DRAFT','SUBMITTED','Claim transmitted electronically.','2026-08-19 10:00:00.000000',4);
/*!40000 ALTER TABLE `claim_history` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `claim_history_backup_20261103`
--

DROP TABLE IF EXISTS `claim_history_backup_20261103`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `claim_history_backup_20261103` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `tenant_id` bigint unsigned NOT NULL,
  `claim_id` bigint unsigned NOT NULL,
  `from_status` varchar(30) NOT NULL,
  `to_status` varchar(30) NOT NULL,
  `note` varchar(1000) DEFAULT NULL,
  `changed_at` datetime(6) NOT NULL,
  `changed_by_user_id` bigint unsigned DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `idx_claim_history_tenant_id` (`tenant_id`),
  KEY `idx_claim_history_claim_id` (`claim_id`),
  KEY `idx_claim_history_changed_at` (`changed_at`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `claim_history_backup_20261103`
--

LOCK TABLES `claim_history_backup_20261103` WRITE;
/*!40000 ALTER TABLE `claim_history_backup_20261103` DISABLE KEYS */;
INSERT INTO `claim_history_backup_20261103` VALUES (1,1,1,'SUBMITTED','PAID','ERA payment posted.','2026-08-28 12:00:00.000000',2),(2,1,2,'SUBMITTED','DENIED','Payer denied claim.','2026-09-01 09:00:00.000000',2),(3,2,3,'DRAFT','SUBMITTED','Claim transmitted electronically.','2026-08-19 10:00:00.000000',4);
/*!40000 ALTER TABLE `claim_history_backup_20261103` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `claim_lines`
--

DROP TABLE IF EXISTS `claim_lines`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `claim_lines` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `tenant_id` bigint unsigned NOT NULL,
  `claim_id` bigint unsigned NOT NULL,
  `line_number` int NOT NULL,
  `cpt_hcpcs_code` varchar(20) NOT NULL,
  `modifier1` varchar(10) DEFAULT NULL,
  `modifier2` varchar(10) DEFAULT NULL,
  `diagnosis_pointer` varchar(50) DEFAULT NULL,
  `charge_amount` decimal(12,2) NOT NULL DEFAULT '0.00',
  `units` int NOT NULL DEFAULT '1',
  `allowed_amount` decimal(12,2) DEFAULT NULL,
  `paid_amount` decimal(12,2) NOT NULL DEFAULT '0.00',
  `patient_responsibility` decimal(12,2) NOT NULL DEFAULT '0.00',
  `status` varchar(30) NOT NULL DEFAULT 'PENDING',
  `denial_code` varchar(20) DEFAULT NULL,
  `claim_line_uuid` char(36) NOT NULL,
  `service_date` date NOT NULL,
  `modifier3` varchar(10) DEFAULT NULL,
  `modifier4` varchar(10) DEFAULT NULL,
  `place_of_service` varchar(10) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_claim_lines_claim_line_number` (`claim_id`,`line_number`),
  UNIQUE KEY `uk_claim_lines_claim_line_uuid` (`claim_line_uuid`),
  KEY `idx_claim_lines_tenant_id` (`tenant_id`),
  CONSTRAINT `fk_claim_lines_claim_id_1` FOREIGN KEY (`claim_id`) REFERENCES `claims` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `claim_lines`
--

LOCK TABLES `claim_lines` WRITE;
/*!40000 ALTER TABLE `claim_lines` DISABLE KEYS */;
INSERT INTO `claim_lines` VALUES (1,1,1,1,'99213',NULL,NULL,'1',250.00,1,180.00,144.00,36.00,'PAID',NULL,'776168a0-cecc-11f1-8711-9840bb39a8b7','2026-08-20',NULL,NULL,NULL);
/*!40000 ALTER TABLE `claim_lines` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `claim_rejections`
--

DROP TABLE IF EXISTS `claim_rejections`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `claim_rejections` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `tenant_id` bigint unsigned NOT NULL,
  `claim_id` bigint unsigned NOT NULL,
  `rejection_code` varchar(30) NOT NULL,
  `rejection_category` varchar(50) DEFAULT NULL,
  `description` varchar(1000) NOT NULL,
  `status` varchar(30) NOT NULL DEFAULT 'OPEN',
  `rejected_at` datetime(6) NOT NULL,
  `resolved_at` datetime(6) DEFAULT NULL,
  `resolution_note` varchar(1000) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `idx_claim_rejections_tenant_id` (`tenant_id`),
  KEY `idx_claim_rejections_status` (`status`),
  KEY `idx_claim_rejections_rejected_at` (`rejected_at`),
  KEY `fk_claim_rejections_claim_id_1` (`claim_id`),
  CONSTRAINT `fk_claim_rejections_claim_id_1` FOREIGN KEY (`claim_id`) REFERENCES `claims` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `claim_rejections`
--

LOCK TABLES `claim_rejections` WRITE;
/*!40000 ALTER TABLE `claim_rejections` DISABLE KEYS */;
INSERT INTO `claim_rejections` VALUES (1,1,2,'N290','CLINICAL','Missing medical necessity documentation.','OPEN','2026-08-27 09:00:00.000000',NULL,NULL);
/*!40000 ALTER TABLE `claim_rejections` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `claim_responses`
--

DROP TABLE IF EXISTS `claim_responses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `claim_responses` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `tenant_id` bigint unsigned NOT NULL,
  `claim_id` bigint unsigned NOT NULL,
  `response_type` varchar(30) NOT NULL DEFAULT 'ACK',
  `payer_claim_number` varchar(100) DEFAULT NULL,
  `status_code` varchar(30) DEFAULT NULL,
  `status_message` varchar(1000) DEFAULT NULL,
  `raw_response` json DEFAULT NULL,
  `received_at` datetime(6) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `idx_claim_responses_tenant_id` (`tenant_id`),
  KEY `idx_claim_responses_claim_id` (`claim_id`),
  KEY `idx_claim_responses_received_at` (`received_at`),
  CONSTRAINT `fk_claim_responses_claim_id_1` FOREIGN KEY (`claim_id`) REFERENCES `claims` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `claim_responses`
--

LOCK TABLES `claim_responses` WRITE;
/*!40000 ALTER TABLE `claim_responses` DISABLE KEYS */;
INSERT INTO `claim_responses` VALUES (1,1,1,'ERA','AET-CLM-90001','1','Processed and paid.',NULL,'2026-08-28 11:45:00.000000'),(2,1,2,'ERA','BCB-CLM-90002','4','Denied - medical necessity documentation required.',NULL,'2026-09-01 08:50:00.000000'),(3,2,3,'ACK','UHC-CLM-90003','A0','Claim accepted for processing.',NULL,'2026-08-19 10:02:00.000000');
/*!40000 ALTER TABLE `claim_responses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `claims`
--

DROP TABLE IF EXISTS `claims`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `claims` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `claim_guid` char(36) NOT NULL,
  `tenant_id` bigint unsigned NOT NULL,
  `patient_id` bigint unsigned NOT NULL,
  `patient_insurance_id` bigint unsigned NOT NULL,
  `payer_id` bigint unsigned NOT NULL,
  `billing_provider_id` bigint unsigned NOT NULL,
  `rendering_provider_id` bigint unsigned DEFAULT NULL,
  `claim_number` varchar(60) NOT NULL,
  `claim_type` varchar(30) NOT NULL DEFAULT 'PROFESSIONAL',
  `frequency_code` varchar(5) NOT NULL DEFAULT '1',
  `status` varchar(30) NOT NULL DEFAULT 'DRAFT',
  `service_from` date NOT NULL,
  `service_to` date NOT NULL,
  `submission_date` date DEFAULT NULL,
  `total_charge` decimal(12,2) NOT NULL DEFAULT '0.00',
  `total_allowed` decimal(12,2) NOT NULL DEFAULT '0.00',
  `total_paid` decimal(12,2) NOT NULL DEFAULT '0.00',
  `patient_responsibility` decimal(12,2) NOT NULL DEFAULT '0.00',
  `balance` decimal(12,2) NOT NULL DEFAULT '0.00',
  `payer_claim_number` varchar(100) DEFAULT NULL,
  `rejection_reason` varchar(1000) DEFAULT NULL,
  `created_at` datetime(6) NOT NULL,
  `updated_at` datetime(6) NOT NULL,
  `submission_reference` varchar(100) DEFAULT NULL,
  `submitted_at` datetime(6) DEFAULT NULL,
  `last_status_checked_at` datetime(6) DEFAULT NULL,
  `notes` text,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_claims_claim_guid` (`claim_guid`),
  UNIQUE KEY `uk_claims_1` (`tenant_id`,`claim_number`),
  KEY `idx_claims_tenant_id` (`tenant_id`),
  KEY `idx_claims_patient_id` (`patient_id`),
  KEY `idx_claims_status` (`status`),
  KEY `idx_claims_created_at` (`created_at`),
  KEY `fk_claims_patient_insurance_id_3` (`patient_insurance_id`),
  KEY `fk_claims_payer_id_4` (`payer_id`),
  KEY `fk_claims_billing_provider_id_5` (`billing_provider_id`),
  KEY `fk_claims_rendering_provider_id_6` (`rendering_provider_id`),
  CONSTRAINT `fk_claims_billing_provider_id_5` FOREIGN KEY (`billing_provider_id`) REFERENCES `providers` (`id`),
  CONSTRAINT `fk_claims_patient_id_2` FOREIGN KEY (`patient_id`) REFERENCES `patients` (`id`),
  CONSTRAINT `fk_claims_patient_insurance_id_3` FOREIGN KEY (`patient_insurance_id`) REFERENCES `patient_insurances` (`id`),
  CONSTRAINT `fk_claims_payer_id_4` FOREIGN KEY (`payer_id`) REFERENCES `payers` (`id`),
  CONSTRAINT `fk_claims_rendering_provider_id_6` FOREIGN KEY (`rendering_provider_id`) REFERENCES `providers` (`id`),
  CONSTRAINT `fk_claims_tenant_id_1` FOREIGN KEY (`tenant_id`) REFERENCES `tenants` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `claims`
--

LOCK TABLES `claims` WRITE;
/*!40000 ALTER TABLE `claims` DISABLE KEYS */;
INSERT INTO `claims` VALUES (1,'ad000000-0000-4000-8000-000000000001',1,1,1,1,1,1,'CLM-2026-0001','PROFESSIONAL','1','PAID','2026-08-20','2026-08-20','2026-08-21',250.00,180.00,144.00,36.00,0.00,'AET-CLM-90001',NULL,'2026-08-20 17:00:00.000000','2026-08-28 12:00:00.000000',NULL,NULL,NULL,NULL),(2,'ad000000-0000-4000-8000-000000000002',1,2,2,2,1,2,'CLM-2026-0002','PROFESSIONAL','1','DENIED','2026-08-25','2026-08-25','2026-08-26',450.00,0.00,0.00,450.00,450.00,'BCB-CLM-90002','Medical necessity documentation required.','2026-08-25 17:00:00.000000','2026-09-01 09:00:00.000000',NULL,NULL,NULL,NULL),(3,'ad000000-0000-4000-8000-000000000003',2,3,3,3,3,3,'CLM-2026-0003','PROFESSIONAL','1','SUBMITTED','2026-08-18','2026-08-18','2026-08-19',300.00,220.00,0.00,0.00,300.00,'UHC-CLM-90003',NULL,'2026-08-18 17:00:00.000000','2026-08-19 10:00:00.000000',NULL,NULL,NULL,NULL);
/*!40000 ALTER TABLE `claims` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `denial_activity`
--

DROP TABLE IF EXISTS `denial_activity`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `denial_activity` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `tenant_id` bigint unsigned NOT NULL,
  `denial_id` bigint unsigned NOT NULL,
  `activity_type` varchar(30) NOT NULL DEFAULT 'NOTE',
  `activity_text` text NOT NULL,
  `activity_at` datetime(6) NOT NULL,
  `user_id` bigint unsigned DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `idx_denial_activity_tenant_id` (`tenant_id`),
  KEY `idx_denial_activity_denial_id` (`denial_id`),
  KEY `idx_denial_activity_activity_at` (`activity_at`),
  CONSTRAINT `fk_denial_activity_denial_id_1` FOREIGN KEY (`denial_id`) REFERENCES `denials` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `denial_activity`
--

LOCK TABLES `denial_activity` WRITE;
/*!40000 ALTER TABLE `denial_activity` DISABLE KEYS */;
INSERT INTO `denial_activity` VALUES (1,1,1,'NOTE','Reviewed payer denial and initiated appeal workflow.','2026-09-02 10:00:00.000000',3),(2,2,2,'ASSIGNMENT','Denial assigned to RCM administrator.','2026-08-30 11:00:00.000000',4);
/*!40000 ALTER TABLE `denial_activity` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `denial_attachments`
--

DROP TABLE IF EXISTS `denial_attachments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `denial_attachments` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `tenant_id` bigint unsigned NOT NULL,
  `denial_id` bigint unsigned NOT NULL,
  `file_name` varchar(255) NOT NULL,
  `storage_key` varchar(500) NOT NULL,
  `content_type` varchar(100) DEFAULT NULL,
  `file_size` bigint unsigned DEFAULT NULL,
  `uploaded_at` datetime(6) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `idx_denial_attachments_tenant_id` (`tenant_id`),
  KEY `idx_denial_attachments_denial_id` (`denial_id`),
  CONSTRAINT `fk_denial_attachments_denial_id_1` FOREIGN KEY (`denial_id`) REFERENCES `denials` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `denial_attachments`
--

LOCK TABLES `denial_attachments` WRITE;
/*!40000 ALTER TABLE `denial_attachments` DISABLE KEYS */;
INSERT INTO `denial_attachments` VALUES (1,1,1,'clinical_notes.pdf','denials/2026/DEN-2026-0001/clinical_notes.pdf','application/pdf',245760,'2026-09-03 09:00:00.000000'),(2,2,2,'remittance.pdf','denials/2026/DEN-2026-0002/remittance.pdf','application/pdf',128000,'2026-08-31 09:00:00.000000');
/*!40000 ALTER TABLE `denial_attachments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `denial_payer_responses`
--

DROP TABLE IF EXISTS `denial_payer_responses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `denial_payer_responses` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `tenant_id` bigint unsigned NOT NULL,
  `denial_id` bigint unsigned NOT NULL,
  `response_code` varchar(50) NOT NULL,
  `response_text` varchar(1000) NOT NULL,
  `payer_reference` varchar(100) DEFAULT NULL,
  `received_at` datetime(6) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `idx_denial_payer_responses_tenant_id` (`tenant_id`),
  KEY `idx_denial_payer_responses_denial_id` (`denial_id`),
  KEY `idx_denial_payer_responses_received_at` (`received_at`),
  CONSTRAINT `fk_denial_payer_responses_denial_id_1` FOREIGN KEY (`denial_id`) REFERENCES `denials` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `denial_payer_responses`
--

LOCK TABLES `denial_payer_responses` WRITE;
/*!40000 ALTER TABLE `denial_payer_responses` DISABLE KEYS */;
INSERT INTO `denial_payer_responses` VALUES (1,1,1,'DENY','Medical necessity criteria not met based on submitted documentation.','BCB-DEN-001','2026-09-01 08:50:00.000000'),(2,2,2,'PR204','Service excluded from member benefit.','UHC-DEN-002','2026-08-30 09:45:00.000000');
/*!40000 ALTER TABLE `denial_payer_responses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `denial_resolutions`
--

DROP TABLE IF EXISTS `denial_resolutions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `denial_resolutions` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `tenant_id` bigint unsigned NOT NULL,
  `denial_id` bigint unsigned NOT NULL,
  `resolution_type` varchar(50) NOT NULL DEFAULT 'PENDING',
  `resolution_note` varchar(1000) DEFAULT NULL,
  `recovered_amount` decimal(12,2) DEFAULT NULL,
  `resolved_at` datetime(6) NOT NULL,
  `resolved_by_user_id` bigint unsigned DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `idx_denial_resolutions_tenant_id` (`tenant_id`),
  KEY `idx_denial_resolutions_denial_id` (`denial_id`),
  KEY `idx_denial_resolutions_resolved_at` (`resolved_at`),
  CONSTRAINT `fk_denial_resolutions_denial_id_1` FOREIGN KEY (`denial_id`) REFERENCES `denials` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `denial_resolutions`
--

LOCK TABLES `denial_resolutions` WRITE;
/*!40000 ALTER TABLE `denial_resolutions` DISABLE KEYS */;
INSERT INTO `denial_resolutions` VALUES (1,1,1,'APPEALED','First-level appeal submitted.',0.00,'2026-09-05 14:00:00.000000',3),(2,2,2,'PENDING','Awaiting internal review.',0.00,'2026-08-30 12:00:00.000000',4);
/*!40000 ALTER TABLE `denial_resolutions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `denials`
--

DROP TABLE IF EXISTS `denials`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `denials` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `denial_guid` char(36) NOT NULL,
  `tenant_id` bigint unsigned NOT NULL,
  `claim_id` bigint unsigned NOT NULL,
  `claim_line_id` bigint unsigned DEFAULT NULL,
  `patient_id` bigint unsigned NOT NULL,
  `payer_id` bigint unsigned NOT NULL,
  `denial_number` varchar(60) NOT NULL,
  `denial_code` varchar(30) NOT NULL,
  `denial_reason` varchar(1000) NOT NULL,
  `denial_category` varchar(50) NOT NULL DEFAULT 'OTHER',
  `status` varchar(30) NOT NULL DEFAULT 'OPEN',
  `denied_amount` decimal(12,2) NOT NULL DEFAULT '0.00',
  `denial_date` date NOT NULL,
  `appeal_deadline` date DEFAULT NULL,
  `assigned_to_user_id` bigint unsigned DEFAULT NULL,
  `resolution` varchar(1000) DEFAULT NULL,
  `created_at` datetime(6) NOT NULL,
  `updated_at` datetime(6) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_denials_denial_guid` (`denial_guid`),
  UNIQUE KEY `uk_denials_1` (`tenant_id`,`denial_number`),
  KEY `idx_denials_tenant_id` (`tenant_id`),
  KEY `idx_denials_claim_id` (`claim_id`),
  KEY `idx_denials_status` (`status`),
  KEY `idx_denials_appeal_deadline` (`appeal_deadline`),
  KEY `fk_denials_claim_line_id_2` (`claim_line_id`),
  KEY `fk_denials_patient_id_3` (`patient_id`),
  KEY `fk_denials_payer_id_4` (`payer_id`),
  CONSTRAINT `fk_denials_claim_id_1` FOREIGN KEY (`claim_id`) REFERENCES `claims` (`id`),
  CONSTRAINT `fk_denials_claim_line_id_2` FOREIGN KEY (`claim_line_id`) REFERENCES `claim_lines` (`id`),
  CONSTRAINT `fk_denials_patient_id_3` FOREIGN KEY (`patient_id`) REFERENCES `patients` (`id`),
  CONSTRAINT `fk_denials_payer_id_4` FOREIGN KEY (`payer_id`) REFERENCES `payers` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `denials`
--

LOCK TABLES `denials` WRITE;
/*!40000 ALTER TABLE `denials` DISABLE KEYS */;
INSERT INTO `denials` VALUES (1,'ae000000-0000-4000-8000-000000000001',1,2,2,2,2,'DEN-2026-0001','CO50','These are non-covered services because this is not deemed a medical necessity by the payer.','MEDICAL_NECESSITY','APPEAL_IN_PROGRESS',450.00,'2026-09-01','2026-11-30',3,'Appeal submitted with clinical documentation.','2026-09-01 09:10:00.000000','2026-09-05 14:00:00.000000'),(2,'ae000000-0000-4000-8000-000000000002',2,3,NULL,3,3,'DEN-2026-0002','PR204','Service not covered under current benefit plan.','COVERAGE','OPEN',80.00,'2026-08-30','2026-11-28',4,NULL,'2026-08-30 10:00:00.000000','2026-08-30 10:00:00.000000');
/*!40000 ALTER TABLE `denials` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `eligibility_benefits`
--

DROP TABLE IF EXISTS `eligibility_benefits`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `eligibility_benefits` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `tenant_id` bigint unsigned NOT NULL,
  `eligibility_response_id` bigint unsigned NOT NULL,
  `category` varchar(100) NOT NULL,
  `service_type_code` varchar(20) NOT NULL,
  `description` varchar(255) NOT NULL,
  `coverage_level` varchar(50) DEFAULT NULL,
  `copay` decimal(12,2) DEFAULT NULL,
  `coinsurance_percent` decimal(5,2) DEFAULT NULL,
  `deductible` decimal(12,2) DEFAULT NULL,
  `remaining` decimal(12,2) DEFAULT NULL,
  `status` varchar(30) NOT NULL DEFAULT 'ACTIVE',
  PRIMARY KEY (`id`),
  KEY `idx_eligibility_benefits_tenant_id` (`tenant_id`),
  KEY `idx_eligibility_benefits_eligibility_response_id` (`eligibility_response_id`),
  CONSTRAINT `fk_eligibility_benefits_eligibility_response_id_1` FOREIGN KEY (`eligibility_response_id`) REFERENCES `eligibility_responses` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `eligibility_benefits`
--

LOCK TABLES `eligibility_benefits` WRITE;
/*!40000 ALTER TABLE `eligibility_benefits` DISABLE KEYS */;
INSERT INTO `eligibility_benefits` VALUES (1,1,1,'Primary Care','98','Office visit benefit','IN_NETWORK',30.00,20.00,1500.00,800.00,'ACTIVE'),(2,1,2,'Specialist','97','Specialist office visit benefit','IN_NETWORK',25.00,10.00,1000.00,450.00,'ACTIVE'),(3,2,3,'Primary Care','98','Office visit benefit','IN_NETWORK',35.00,20.00,2000.00,1200.00,'ACTIVE');
/*!40000 ALTER TABLE `eligibility_benefits` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `eligibility_history`
--

DROP TABLE IF EXISTS `eligibility_history`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `eligibility_history` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `tenant_id` bigint unsigned NOT NULL,
  `eligibility_request_id` bigint unsigned NOT NULL,
  `from_status` varchar(30) NOT NULL,
  `to_status` varchar(30) NOT NULL,
  `reason` varchar(500) DEFAULT NULL,
  `changed_at` datetime(6) NOT NULL,
  `changed_by_user_id` bigint unsigned DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `idx_eligibility_history_tenant_id` (`tenant_id`),
  KEY `idx_eligibility_history_eligibility_request_id` (`eligibility_request_id`),
  KEY `idx_eligibility_history_changed_at` (`changed_at`),
  CONSTRAINT `fk_eligibility_history_eligibility_request_id_1` FOREIGN KEY (`eligibility_request_id`) REFERENCES `eligibility_requests` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `eligibility_history`
--

LOCK TABLES `eligibility_history` WRITE;
/*!40000 ALTER TABLE `eligibility_history` DISABLE KEYS */;
INSERT INTO `eligibility_history` VALUES (1,1,1,'PENDING','COMPLETED','Payer response received','2026-08-20 10:00:03.000000',2),(2,1,2,'PENDING','COMPLETED','Payer response received','2026-08-25 11:00:04.000000',2),(3,2,3,'PENDING','COMPLETED','Payer response received','2026-08-18 09:30:05.000000',4);
/*!40000 ALTER TABLE `eligibility_history` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `eligibility_requests`
--

DROP TABLE IF EXISTS `eligibility_requests`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `eligibility_requests` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `request_guid` char(36) NOT NULL,
  `tenant_id` bigint unsigned NOT NULL,
  `patient_id` bigint unsigned NOT NULL,
  `patient_insurance_id` bigint unsigned NOT NULL,
  `payer_id` bigint unsigned NOT NULL,
  `request_number` varchar(50) NOT NULL,
  `request_type` varchar(30) NOT NULL DEFAULT 'MANUAL',
  `status` varchar(30) NOT NULL DEFAULT 'PENDING',
  `source` varchar(30) NOT NULL DEFAULT 'API',
  `requested_at` datetime(6) NOT NULL,
  `completed_at` datetime(6) DEFAULT NULL,
  `error_code` varchar(50) DEFAULT NULL,
  `error_message` varchar(500) DEFAULT NULL,
  `trace_id` varchar(100) DEFAULT NULL,
  `raw_request` json DEFAULT NULL,
  `created_at` datetime(6) NOT NULL,
  `updated_at` datetime(6) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_eligibility_requests_request_guid` (`request_guid`),
  UNIQUE KEY `uk_eligibility_requests_tenant_id` (`tenant_id`),
  UNIQUE KEY `uk_eligibility_requests_request_number` (`request_number`),
  KEY `idx_eligibility_requests_patient_id` (`patient_id`),
  KEY `idx_eligibility_requests_status` (`status`),
  KEY `idx_eligibility_requests_requested_at` (`requested_at`),
  KEY `fk_eligibility_requests_patient_insurance_id_3` (`patient_insurance_id`),
  KEY `fk_eligibility_requests_payer_id_4` (`payer_id`),
  CONSTRAINT `fk_eligibility_requests_patient_id_2` FOREIGN KEY (`patient_id`) REFERENCES `patients` (`id`),
  CONSTRAINT `fk_eligibility_requests_patient_insurance_id_3` FOREIGN KEY (`patient_insurance_id`) REFERENCES `patient_insurances` (`id`),
  CONSTRAINT `fk_eligibility_requests_payer_id_4` FOREIGN KEY (`payer_id`) REFERENCES `payers` (`id`),
  CONSTRAINT `fk_eligibility_requests_tenant_id_1` FOREIGN KEY (`tenant_id`) REFERENCES `tenants` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `eligibility_requests`
--

LOCK TABLES `eligibility_requests` WRITE;
/*!40000 ALTER TABLE `eligibility_requests` DISABLE KEYS */;
INSERT INTO `eligibility_requests` VALUES (1,'aa000000-0000-4000-8000-000000000001',1,1,1,1,'ELG-2026-0001','REAL_TIME','COMPLETED','API','2026-08-20 10:00:00.000000','2026-08-20 10:00:03.000000',NULL,NULL,'TRC-ELG-0001',NULL,'2026-08-20 10:00:00.000000','2026-08-20 10:00:03.000000'),(3,'aa000000-0000-4000-8000-000000000003',2,3,3,3,'ELG-2026-0003','REAL_TIME','COMPLETED','API','2026-08-18 09:30:00.000000','2026-08-18 09:30:05.000000',NULL,NULL,'TRC-ELG-0003',NULL,'2026-08-18 09:30:00.000000','2026-08-18 09:30:05.000000');
/*!40000 ALTER TABLE `eligibility_requests` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `eligibility_responses`
--

DROP TABLE IF EXISTS `eligibility_responses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `eligibility_responses` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `response_guid` char(36) NOT NULL,
  `tenant_id` bigint unsigned NOT NULL,
  `eligibility_request_id` bigint unsigned NOT NULL,
  `status` varchar(30) NOT NULL DEFAULT 'VERIFIED',
  `payer_reference` varchar(100) DEFAULT NULL,
  `coverage_status` varchar(30) DEFAULT NULL,
  `effective_date` date DEFAULT NULL,
  `termination_date` date DEFAULT NULL,
  `deductible_individual` decimal(12,2) DEFAULT NULL,
  `deductible_remaining` decimal(12,2) DEFAULT NULL,
  `copay` decimal(12,2) DEFAULT NULL,
  `coinsurance_percent` decimal(5,2) DEFAULT NULL,
  `response_message` varchar(1000) DEFAULT NULL,
  `raw_response` json DEFAULT NULL,
  `responded_at` datetime(6) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_eligibility_responses_response_guid` (`response_guid`),
  UNIQUE KEY `uk_eligibility_responses_eligibility_request_id` (`eligibility_request_id`),
  KEY `idx_eligibility_responses_tenant_id` (`tenant_id`),
  KEY `idx_eligibility_responses_status` (`status`),
  CONSTRAINT `fk_eligibility_responses_eligibility_request_id_2` FOREIGN KEY (`eligibility_request_id`) REFERENCES `eligibility_requests` (`id`) ON DELETE CASCADE,
  CONSTRAINT `fk_eligibility_responses_tenant_id_1` FOREIGN KEY (`tenant_id`) REFERENCES `tenants` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `eligibility_responses`
--

LOCK TABLES `eligibility_responses` WRITE;
/*!40000 ALTER TABLE `eligibility_responses` DISABLE KEYS */;
INSERT INTO `eligibility_responses` VALUES (1,'ab000000-0000-4000-8000-000000000001',1,1,'VERIFIED','AET-ELG-001','ACTIVE','2026-01-01',NULL,1500.00,800.00,30.00,20.00,'Coverage active and eligible.',NULL,'2026-08-20 10:00:03.000000'),(2,'ab000000-0000-4000-8000-000000000002',1,2,'VERIFIED','BCB-ELG-002','ACTIVE','2026-01-01',NULL,1000.00,450.00,25.00,10.00,'Coverage active and eligible.',NULL,'2026-08-25 11:00:04.000000'),(3,'ab000000-0000-4000-8000-000000000003',2,3,'VERIFIED','UHC-ELG-003','ACTIVE','2026-01-01',NULL,2000.00,1200.00,35.00,20.00,'Coverage active and eligible.',NULL,'2026-08-18 09:30:05.000000');
/*!40000 ALTER TABLE `eligibility_responses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `insurance_plans`
--

DROP TABLE IF EXISTS `insurance_plans`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `insurance_plans` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `insurance_plan_uuid` char(36) NOT NULL,
  `tenant_id` bigint unsigned NOT NULL,
  `payer_id` bigint unsigned NOT NULL,
  `plan_code` varchar(100) NOT NULL,
  `plan_name` varchar(250) NOT NULL,
  `plan_type` enum('COMMERCIAL','MEDICARE','MEDICAID','TRICARE','SELF_PAY','OTHER') NOT NULL DEFAULT 'COMMERCIAL',
  `network_type` enum('HMO','PPO','EPO','POS','OPEN','OTHER') DEFAULT NULL,
  `status` enum('ACTIVE','INACTIVE') NOT NULL DEFAULT 'ACTIVE',
  `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `updated_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_insurance_plans_insurance_plan_uuid` (`insurance_plan_uuid`),
  UNIQUE KEY `uk_insurance_plans_1` (`tenant_id`,`plan_code`),
  KEY `idx_insurance_plans_tenant_id` (`tenant_id`),
  KEY `idx_insurance_plans_payer_id` (`payer_id`),
  KEY `idx_insurance_plans_plan_name` (`plan_name`),
  KEY `idx_insurance_plans_status` (`status`),
  CONSTRAINT `fk_insurance_plans_payer_id_2` FOREIGN KEY (`payer_id`) REFERENCES `payers` (`id`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `fk_insurance_plans_tenant_id_1` FOREIGN KEY (`tenant_id`) REFERENCES `tenants` (`id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `insurance_plans`
--

LOCK TABLES `insurance_plans` WRITE;
/*!40000 ALTER TABLE `insurance_plans` DISABLE KEYS */;
INSERT INTO `insurance_plans` VALUES (1,'60000000-0000-4000-8000-000000000001',1,1,'AETNA-PPO-100','Aetna Choice PPO','COMMERCIAL','PPO','ACTIVE','2026-09-09 23:08:26.065459','2026-09-09 23:08:26.065459'),(2,'60000000-0000-4000-8000-000000000002',1,2,'BCBS-HMO-200','Blue Cross Blue Shield HMO','COMMERCIAL','HMO','ACTIVE','2026-09-09 23:08:26.065459','2026-09-09 23:08:26.065459'),(3,'60000000-0000-4000-8000-000000000003',2,3,'UHC-PPO-300','UnitedHealthcare Choice Plus PPO','COMMERCIAL','PPO','ACTIVE','2026-09-09 23:08:26.065459','2026-09-09 23:08:26.065459');
/*!40000 ALTER TABLE `insurance_plans` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `locations`
--

DROP TABLE IF EXISTS `locations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `locations` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `location_uuid` char(36) NOT NULL,
  `tenant_id` bigint unsigned NOT NULL,
  `organization_id` bigint unsigned NOT NULL,
  `location_code` varchar(50) NOT NULL,
  `location_name` varchar(200) NOT NULL,
  `location_type` enum('CLINIC','HOSPITAL','OFFICE','FACILITY','OTHER') NOT NULL DEFAULT 'CLINIC',
  `address_line_1` varchar(250) NOT NULL,
  `address_line_2` varchar(250) DEFAULT NULL,
  `city` varchar(100) NOT NULL,
  `state` char(2) NOT NULL,
  `postal_code` varchar(20) NOT NULL,
  `county` varchar(100) DEFAULT NULL,
  `phone` varchar(30) DEFAULT NULL,
  `fax` varchar(30) DEFAULT NULL,
  `email` varchar(254) DEFAULT NULL,
  `timezone` varchar(100) NOT NULL DEFAULT 'America/New_York',
  `is_primary` tinyint NOT NULL DEFAULT '0',
  `is_active` tinyint NOT NULL DEFAULT '1',
  `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `updated_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_locations_location_uuid` (`location_uuid`),
  UNIQUE KEY `uk_locations_1` (`organization_id`,`location_code`),
  KEY `idx_locations_tenant_id` (`tenant_id`),
  KEY `idx_locations_organization_id` (`organization_id`),
  KEY `idx_locations_city` (`city`),
  KEY `idx_locations_state` (`state`),
  KEY `idx_locations_is_active` (`is_active`),
  CONSTRAINT `fk_locations_organization_id_2` FOREIGN KEY (`organization_id`) REFERENCES `organizations` (`id`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `fk_locations_tenant_id_1` FOREIGN KEY (`tenant_id`) REFERENCES `tenants` (`id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `locations`
--

LOCK TABLES `locations` WRITE;
/*!40000 ALTER TABLE `locations` DISABLE KEYS */;
INSERT INTO `locations` VALUES (1,'70000000-0000-4000-8000-000000000001',1,1,'NYC01','Sunrise Midtown Clinic','CLINIC','100 Madison Avenue',NULL,'New York','NY','10016','New York','+1-212-555-1200',NULL,'midtown@sunrise.example','America/New_York',1,1,'2026-09-09 23:08:26.312648','2026-09-09 23:08:26.312648'),(2,'70000000-0000-4000-8000-000000000002',1,1,'NYC02','Sunrise Downtown Billing Office','OFFICE','200 Broadway',NULL,'New York','NY','10038','New York','+1-212-555-1201',NULL,'billing@sunrise.example','America/New_York',0,1,'2026-09-09 23:08:26.312648','2026-09-09 23:08:26.312648'),(3,'70000000-0000-4000-8000-000000000003',2,2,'CHI01','NorthStar Chicago Clinic','CLINIC','500 W Madison Street',NULL,'Chicago','IL','60661','Cook','+1-312-555-1202',NULL,'chicago@northstar.example','America/Chicago',1,1,'2026-09-09 23:08:26.312648','2026-09-09 23:08:26.312648');
/*!40000 ALTER TABLE `locations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `organizations`
--

DROP TABLE IF EXISTS `organizations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `organizations` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `organization_uuid` char(36) NOT NULL,
  `tenant_id` bigint unsigned NOT NULL,
  `organization_code` varchar(50) NOT NULL,
  `organization_name` varchar(200) NOT NULL,
  `legal_name` varchar(250) DEFAULT NULL,
  `organization_type` enum('MEDICAL_GROUP','HOSPITAL','HEALTH_SYSTEM','PRACTICE','OTHER') NOT NULL DEFAULT 'MEDICAL_GROUP',
  `tax_id` varchar(50) DEFAULT NULL,
  `npi` varchar(10) DEFAULT NULL,
  `phone` varchar(30) DEFAULT NULL,
  `fax` varchar(30) DEFAULT NULL,
  `email` varchar(254) DEFAULT NULL,
  `website` varchar(500) DEFAULT NULL,
  `status` enum('ACTIVE','INACTIVE') NOT NULL DEFAULT 'ACTIVE',
  `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `updated_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_organizations_organization_uuid` (`organization_uuid`),
  UNIQUE KEY `uk_organizations_1` (`tenant_id`,`organization_code`),
  UNIQUE KEY `uk_organizations_2` (`tenant_id`,`npi`),
  KEY `idx_organizations_tenant_id` (`tenant_id`),
  KEY `idx_organizations_organization_name` (`organization_name`),
  KEY `idx_organizations_status` (`status`),
  CONSTRAINT `fk_organizations_tenant_id_1` FOREIGN KEY (`tenant_id`) REFERENCES `tenants` (`id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `organizations`
--

LOCK TABLES `organizations` WRITE;
/*!40000 ALTER TABLE `organizations` DISABLE KEYS */;
INSERT INTO `organizations` VALUES (1,'7f3a9c21-6b44-4e8d-9a12-5c7e81d4b630',1,'IFMG','Sunrise Medical Group','Sunrise Medical Group LLC','MEDICAL_GROUP','12-3456789','1234567890','+1-212-555-1000',NULL,'contact@sunrise.example',NULL,'ACTIVE','2026-09-09 23:08:25.693722','2026-09-24 19:22:14.000000'),(2,'8e4b7d32-7c55-4f9e-a213-6d8f92e5c741',2,'NSHP','NorthStar Health Partners','NorthStar Health Partners Inc','HEALTH_SYSTEM','98-7654321','2234567890','+1-312-555-2000',NULL,'contact@northstar.example',NULL,'ACTIVE','2026-09-09 23:08:25.693722','2026-09-24 19:23:41.000000'),(3,'4b31fc28-0481-466d-901c-d813b29a321f',1,'ORG001','Demo Medical Group Updated','Demo Medical Group Updated LLC','MEDICAL_GROUP',NULL,NULL,'555-0200','555-0201','contact@demomedicalgroup.com','https://demomedicalgroup.com','ACTIVE','2026-09-24 11:28:56.841312','2026-09-24 11:34:55.994321');
/*!40000 ALTER TABLE `organizations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `patient_addresses`
--

DROP TABLE IF EXISTS `patient_addresses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `patient_addresses` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `patient_address_uuid` char(36) NOT NULL,
  `tenant_id` bigint unsigned NOT NULL,
  `patient_id` bigint unsigned NOT NULL,
  `address_type` enum('RESIDENTIAL','MAILING','TEMPORARY','OTHER') NOT NULL DEFAULT 'RESIDENTIAL',
  `address_line_1` varchar(250) NOT NULL,
  `address_line_2` varchar(250) DEFAULT NULL,
  `city` varchar(100) NOT NULL,
  `state` char(2) NOT NULL,
  `postal_code` varchar(20) NOT NULL,
  `postal_code_plus4` varchar(10) DEFAULT NULL,
  `county` varchar(100) DEFAULT NULL,
  `is_primary` tinyint NOT NULL DEFAULT '1',
  `is_active` tinyint NOT NULL DEFAULT '1',
  `effective_from` date DEFAULT NULL,
  `effective_to` date DEFAULT NULL,
  `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `updated_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_patient_addresses_patient_address_uuid` (`patient_address_uuid`),
  KEY `idx_patient_addresses_tenant_id` (`tenant_id`),
  KEY `idx_patient_addresses_patient_id` (`patient_id`),
  KEY `idx_patient_addresses_city` (`city`),
  KEY `idx_patient_addresses_state` (`state`),
  KEY `idx_patient_addresses_is_active` (`is_active`),
  CONSTRAINT `fk_patient_addresses_patient_id_2` FOREIGN KEY (`patient_id`) REFERENCES `patients` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_patient_addresses_tenant_id_1` FOREIGN KEY (`tenant_id`) REFERENCES `tenants` (`id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `patient_addresses`
--

LOCK TABLES `patient_addresses` WRITE;
/*!40000 ALTER TABLE `patient_addresses` DISABLE KEYS */;
INSERT INTO `patient_addresses` VALUES (1,'a0000000-0000-4000-8000-000000000001',1,1,'RESIDENTIAL','25 Park Avenue',NULL,'New York','NY','10016',NULL,'New York',1,1,'2026-01-10',NULL,'2026-09-09 23:08:26.708824','2026-09-09 23:08:26.708824'),(2,'a0000000-0000-4000-8000-000000000002',1,2,'RESIDENTIAL','80 Lexington Avenue',NULL,'New York','NY','10021',NULL,'New York',1,1,'2026-02-05',NULL,'2026-09-09 23:08:26.708824','2026-09-09 23:08:26.708824'),(3,'a0000000-0000-4000-8000-000000000003',2,3,'RESIDENTIAL','410 N Michigan Avenue',NULL,'Chicago','IL','60611',NULL,'Cook',1,1,'2026-03-12',NULL,'2026-09-09 23:08:26.708824','2026-09-09 23:08:26.708824');
/*!40000 ALTER TABLE `patient_addresses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `patient_contacts`
--

DROP TABLE IF EXISTS `patient_contacts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `patient_contacts` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `patient_contact_uuid` char(36) NOT NULL,
  `tenant_id` bigint unsigned NOT NULL,
  `patient_id` bigint unsigned NOT NULL,
  `primary_phone` varchar(30) DEFAULT NULL,
  `secondary_phone` varchar(30) DEFAULT NULL,
  `email` varchar(254) DEFAULT NULL,
  `preferred_contact_method` enum('PHONE','EMAIL','SMS','MAIL','PORTAL','OTHER') NOT NULL DEFAULT 'PHONE',
  `preferred_language` varchar(100) NOT NULL DEFAULT 'English',
  `interpreter_required` tinyint NOT NULL DEFAULT '0',
  `is_primary` tinyint NOT NULL DEFAULT '1',
  `is_active` tinyint NOT NULL DEFAULT '1',
  `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `updated_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_patient_contacts_patient_contact_uuid` (`patient_contact_uuid`),
  KEY `idx_patient_contacts_tenant_id` (`tenant_id`),
  KEY `idx_patient_contacts_patient_id` (`patient_id`),
  KEY `idx_patient_contacts_primary_phone` (`primary_phone`),
  KEY `idx_patient_contacts_email` (`email`),
  CONSTRAINT `fk_patient_contacts_patient_id_2` FOREIGN KEY (`patient_id`) REFERENCES `patients` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_patient_contacts_tenant_id_1` FOREIGN KEY (`tenant_id`) REFERENCES `tenants` (`id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `patient_contacts`
--

LOCK TABLES `patient_contacts` WRITE;
/*!40000 ALTER TABLE `patient_contacts` DISABLE KEYS */;
INSERT INTO `patient_contacts` VALUES (1,'b0000000-0000-4000-8000-000000000001',1,1,'+1-212-555-2001',NULL,'james.miller@example.com','PHONE','English',0,1,1,'2026-09-09 23:08:26.840379','2026-09-09 23:08:26.840379'),(2,'b0000000-0000-4000-8000-000000000002',1,2,'+1-212-555-2002',NULL,'maria.garcia@example.com','EMAIL','English',0,1,1,'2026-09-09 23:08:26.840379','2026-09-09 23:08:26.840379'),(3,'b0000000-0000-4000-8000-000000000003',2,3,'+1-312-555-2003',NULL,'thomas.clark@example.com','PHONE','English',0,1,1,'2026-09-09 23:08:26.840379','2026-09-09 23:08:26.840379');
/*!40000 ALTER TABLE `patient_contacts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `patient_emergency_contacts`
--

DROP TABLE IF EXISTS `patient_emergency_contacts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `patient_emergency_contacts` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `emergency_contact_uuid` char(36) NOT NULL,
  `tenant_id` bigint unsigned NOT NULL,
  `patient_id` bigint unsigned NOT NULL,
  `first_name` varchar(100) NOT NULL,
  `last_name` varchar(100) NOT NULL,
  `relationship` enum('SPOUSE','PARENT','CHILD','SIBLING','PARTNER','FRIEND','OTHER') NOT NULL,
  `phone` varchar(30) NOT NULL,
  `alternate_phone` varchar(30) DEFAULT NULL,
  `email` varchar(254) DEFAULT NULL,
  `priority_order` tinyint unsigned NOT NULL DEFAULT '1',
  `is_primary` tinyint NOT NULL DEFAULT '1',
  `is_active` tinyint NOT NULL DEFAULT '1',
  `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `updated_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_patient_emergency_contacts_emergency_contact_uuid` (`emergency_contact_uuid`),
  KEY `idx_patient_emergency_contacts_tenant_id` (`tenant_id`),
  KEY `idx_patient_emergency_contacts_patient_id` (`patient_id`),
  CONSTRAINT `fk_patient_emergency_contacts_patient_id_2` FOREIGN KEY (`patient_id`) REFERENCES `patients` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_patient_emergency_contacts_tenant_id_1` FOREIGN KEY (`tenant_id`) REFERENCES `tenants` (`id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `patient_emergency_contacts`
--

LOCK TABLES `patient_emergency_contacts` WRITE;
/*!40000 ALTER TABLE `patient_emergency_contacts` DISABLE KEYS */;
INSERT INTO `patient_emergency_contacts` VALUES (1,'c0000000-0000-4000-8000-000000000001',1,1,'Linda','Miller','SPOUSE','+1-212-555-2101',NULL,NULL,1,1,1,'2026-09-09 23:08:26.944130','2026-09-09 23:08:26.944130'),(2,'c0000000-0000-4000-8000-000000000002',1,2,'Carlos','Garcia','SPOUSE','+1-212-555-2102',NULL,NULL,1,1,1,'2026-09-09 23:08:26.944130','2026-09-09 23:08:26.944130'),(3,'c0000000-0000-4000-8000-000000000003',2,3,'Susan','Clark','SPOUSE','+1-312-555-2103',NULL,NULL,1,1,1,'2026-09-09 23:08:26.944130','2026-09-09 23:08:26.944130');
/*!40000 ALTER TABLE `patient_emergency_contacts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `patient_guarantors`
--

DROP TABLE IF EXISTS `patient_guarantors`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `patient_guarantors` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `guarantor_uuid` char(36) NOT NULL,
  `tenant_id` bigint unsigned NOT NULL,
  `patient_id` bigint unsigned NOT NULL,
  `guarantor_account_id` varchar(100) NOT NULL,
  `relationship` enum('SELF','SPOUSE','PARENT','GUARDIAN','OTHER') NOT NULL DEFAULT 'SELF',
  `first_name` varchar(100) NOT NULL,
  `last_name` varchar(100) NOT NULL,
  `phone` varchar(30) DEFAULT NULL,
  `email` varchar(254) DEFAULT NULL,
  `is_primary` tinyint NOT NULL DEFAULT '1',
  `is_active` tinyint NOT NULL DEFAULT '1',
  `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `updated_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_patient_guarantors_guarantor_uuid` (`guarantor_uuid`),
  UNIQUE KEY `uk_patient_guarantors_1` (`tenant_id`,`guarantor_account_id`),
  KEY `idx_patient_guarantors_tenant_id` (`tenant_id`),
  KEY `idx_patient_guarantors_patient_id` (`patient_id`),
  CONSTRAINT `fk_patient_guarantors_patient_id_2` FOREIGN KEY (`patient_id`) REFERENCES `patients` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_patient_guarantors_tenant_id_1` FOREIGN KEY (`tenant_id`) REFERENCES `tenants` (`id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `patient_guarantors`
--

LOCK TABLES `patient_guarantors` WRITE;
/*!40000 ALTER TABLE `patient_guarantors` DISABLE KEYS */;
INSERT INTO `patient_guarantors` VALUES (1,'d0000000-0000-4000-8000-000000000001',1,1,'GUA10001','SELF','James','Miller','+1-212-555-2001','james.miller@example.com',1,1,'2026-09-09 23:08:27.011832','2026-09-09 23:08:27.011832'),(2,'d0000000-0000-4000-8000-000000000002',1,2,'GUA10002','SELF','Maria','Garcia','+1-212-555-2002','maria.garcia@example.com',1,1,'2026-09-09 23:08:27.011832','2026-09-09 23:08:27.011832'),(3,'d0000000-0000-4000-8000-000000000003',2,3,'GUA20001','SELF','Thomas','Clark','+1-312-555-2003','thomas.clark@example.com',1,1,'2026-09-09 23:08:27.011832','2026-09-09 23:08:27.011832');
/*!40000 ALTER TABLE `patient_guarantors` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `patient_identifiers`
--

DROP TABLE IF EXISTS `patient_identifiers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `patient_identifiers` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `patient_identifier_uuid` char(36) NOT NULL,
  `tenant_id` bigint unsigned NOT NULL,
  `patient_id` bigint unsigned NOT NULL,
  `identifier_type` enum('MRN','SSN','DRIVER_LICENSE','PASSPORT','MEDICARE_ID','MEDICAID_ID','EXTERNAL_ID','OTHER') NOT NULL,
  `identifier_token` varchar(500) DEFAULT NULL,
  `identifier_hash` char(64) DEFAULT NULL,
  `external_identifier` varchar(250) DEFAULT NULL,
  `source_system` varchar(150) DEFAULT NULL,
  `is_primary` tinyint NOT NULL DEFAULT '0',
  `is_active` tinyint NOT NULL DEFAULT '1',
  `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `updated_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_patient_identifiers_patient_identifier_uuid` (`patient_identifier_uuid`),
  KEY `idx_patient_identifiers_tenant_id` (`tenant_id`),
  KEY `idx_patient_identifiers_patient_id` (`patient_id`),
  KEY `idx_patient_identifiers_identifier_type` (`identifier_type`),
  KEY `idx_patient_identifiers_identifier_hash` (`identifier_hash`),
  KEY `idx_patient_identifiers_source_system` (`source_system`),
  CONSTRAINT `fk_patient_identifiers_patient_id_2` FOREIGN KEY (`patient_id`) REFERENCES `patients` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_patient_identifiers_tenant_id_1` FOREIGN KEY (`tenant_id`) REFERENCES `tenants` (`id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `patient_identifiers`
--

LOCK TABLES `patient_identifiers` WRITE;
/*!40000 ALTER TABLE `patient_identifiers` DISABLE KEYS */;
INSERT INTO `patient_identifiers` VALUES (1,'e0000000-0000-4000-8000-000000000001',1,1,'MRN','MRN10001','aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa','EXT10001','Sunrise Demo',1,1,'2026-09-09 23:08:27.143258','2026-09-09 23:08:27.143258'),(2,'e0000000-0000-4000-8000-000000000002',1,2,'MRN','MRN10002','bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb','EXT10002','Sunrise Demo',1,1,'2026-09-09 23:08:27.143258','2026-09-09 23:08:27.143258'),(3,'e0000000-0000-4000-8000-000000000003',2,3,'MRN','MRN20001','cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc','EXT20001','Sunrise Demo',1,1,'2026-09-09 23:08:27.143258','2026-09-09 23:08:27.143258');
/*!40000 ALTER TABLE `patient_identifiers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `patient_insurances`
--

DROP TABLE IF EXISTS `patient_insurances`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `patient_insurances` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `patient_insurance_uuid` char(36) NOT NULL,
  `tenant_id` bigint unsigned NOT NULL,
  `patient_id` bigint unsigned NOT NULL,
  `payer_id` bigint unsigned NOT NULL,
  `insurance_plan_id` bigint unsigned DEFAULT NULL,
  `insurance_type` enum('COMMERCIAL','MEDICARE','MEDICAID','TRICARE','SELF_PAY','OTHER') NOT NULL DEFAULT 'COMMERCIAL',
  `coverage_priority` enum('PRIMARY','SECONDARY','TERTIARY') NOT NULL DEFAULT 'PRIMARY',
  `insurance_status` enum('PENDING_VERIFICATION','ACTIVE','INACTIVE','TERMINATED','EXPIRED') NOT NULL DEFAULT 'PENDING_VERIFICATION',
  `member_id` varchar(100) NOT NULL,
  `group_number` varchar(100) DEFAULT NULL,
  `subscriber_id` varchar(100) DEFAULT NULL,
  `subscriber_first_name` varchar(100) DEFAULT NULL,
  `subscriber_last_name` varchar(100) DEFAULT NULL,
  `subscriber_relationship` enum('SELF','SPOUSE','CHILD','PARENT','OTHER') NOT NULL DEFAULT 'SELF',
  `effective_date` date DEFAULT NULL,
  `termination_date` date DEFAULT NULL,
  `payer_id_display` varchar(50) DEFAULT NULL,
  `eligibility_status` enum('NOT_VERIFIED','VERIFIED','INACTIVE','ERROR') NOT NULL DEFAULT 'NOT_VERIFIED',
  `last_verified_at` datetime(6) DEFAULT NULL,
  `is_active` tinyint NOT NULL DEFAULT '1',
  `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `updated_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_patient_insurances_patient_insurance_uuid` (`patient_insurance_uuid`),
  KEY `idx_patient_insurances_tenant_id` (`tenant_id`),
  KEY `idx_patient_insurances_patient_id` (`patient_id`),
  KEY `idx_patient_insurances_payer_id` (`payer_id`),
  KEY `idx_patient_insurances_insurance_plan_id` (`insurance_plan_id`),
  KEY `idx_patient_insurances_coverage_priority` (`coverage_priority`),
  KEY `idx_patient_insurances_insurance_status` (`insurance_status`),
  KEY `idx_patient_insurances_member_id` (`member_id`),
  KEY `idx_patient_insurances_eligibility_status` (`eligibility_status`),
  CONSTRAINT `fk_patient_insurances_insurance_plan_id_4` FOREIGN KEY (`insurance_plan_id`) REFERENCES `insurance_plans` (`id`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `fk_patient_insurances_patient_id_2` FOREIGN KEY (`patient_id`) REFERENCES `patients` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_patient_insurances_payer_id_3` FOREIGN KEY (`payer_id`) REFERENCES `payers` (`id`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `fk_patient_insurances_tenant_id_1` FOREIGN KEY (`tenant_id`) REFERENCES `tenants` (`id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `patient_insurances`
--

LOCK TABLES `patient_insurances` WRITE;
/*!40000 ALTER TABLE `patient_insurances` DISABLE KEYS */;
INSERT INTO `patient_insurances` VALUES (1,'f0000000-0000-4000-8000-000000000001',1,1,1,1,'COMMERCIAL','PRIMARY','ACTIVE','AETNA100001','GRP100','AETNA100001','James','Miller','SELF','2026-01-01',NULL,'60054','VERIFIED','2026-08-20 10:00:00.000000',1,'2026-09-09 23:08:27.246133','2026-09-09 23:08:27.246133'),(2,'f0000000-0000-4000-8000-000000000002',1,2,2,2,'COMMERCIAL','PRIMARY','ACTIVE','BCBS100002','GRP200','BCBS100002','Maria','Garcia','SELF','2026-01-01',NULL,'12345','VERIFIED','2026-08-25 11:00:00.000000',1,'2026-09-09 23:08:27.246133','2026-09-09 23:08:27.246133'),(3,'f0000000-0000-4000-8000-000000000003',2,3,3,3,'COMMERCIAL','PRIMARY','ACTIVE','UHC200003','GRP300','UHC200003','Thomas','Clark','SELF','2026-01-01',NULL,'87726','VERIFIED','2026-08-18 09:30:00.000000',1,'2026-09-09 23:08:27.246133','2026-09-09 23:08:27.246133');
/*!40000 ALTER TABLE `patient_insurances` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `patient_provider_assignments`
--

DROP TABLE IF EXISTS `patient_provider_assignments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `patient_provider_assignments` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `assignment_uuid` char(36) NOT NULL,
  `tenant_id` bigint unsigned NOT NULL,
  `patient_id` bigint unsigned NOT NULL,
  `provider_id` bigint unsigned NOT NULL,
  `location_id` bigint unsigned NOT NULL,
  `department` varchar(150) DEFAULT NULL,
  `care_team` varchar(200) DEFAULT NULL,
  `assignment_type` enum('PRIMARY','SECONDARY','REFERRING','CONSULTING','OTHER') NOT NULL DEFAULT 'PRIMARY',
  `effective_from` date NOT NULL,
  `effective_to` date DEFAULT NULL,
  `is_primary` tinyint NOT NULL DEFAULT '1',
  `is_active` tinyint NOT NULL DEFAULT '1',
  `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `updated_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_patient_provider_assignments_assignment_uuid` (`assignment_uuid`),
  KEY `idx_patient_provider_assignments_tenant_id` (`tenant_id`),
  KEY `idx_patient_provider_assignments_patient_id` (`patient_id`),
  KEY `idx_patient_provider_assignments_provider_id` (`provider_id`),
  KEY `idx_patient_provider_assignments_location_id` (`location_id`),
  KEY `idx_patient_provider_assignments_is_active` (`is_active`),
  CONSTRAINT `fk_patient_provider_assignments_location_id_4` FOREIGN KEY (`location_id`) REFERENCES `locations` (`id`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `fk_patient_provider_assignments_patient_id_2` FOREIGN KEY (`patient_id`) REFERENCES `patients` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_patient_provider_assignments_provider_id_3` FOREIGN KEY (`provider_id`) REFERENCES `providers` (`id`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `fk_patient_provider_assignments_tenant_id_1` FOREIGN KEY (`tenant_id`) REFERENCES `tenants` (`id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `patient_provider_assignments`
--

LOCK TABLES `patient_provider_assignments` WRITE;
/*!40000 ALTER TABLE `patient_provider_assignments` DISABLE KEYS */;
INSERT INTO `patient_provider_assignments` VALUES (1,'a1000000-0000-4000-8000-000000000001',1,1,1,1,'Primary Care','Team A','PRIMARY','2026-01-10',NULL,1,1,'2026-09-09 23:08:27.328623','2026-09-09 23:08:27.328623'),(2,'a1000000-0000-4000-8000-000000000002',1,2,2,1,'Cardiology','Team B','PRIMARY','2026-02-05',NULL,1,1,'2026-09-09 23:08:27.328623','2026-09-09 23:08:27.328623'),(3,'a1000000-0000-4000-8000-000000000003',2,3,3,3,'Primary Care','Team C','PRIMARY','2026-03-12',NULL,1,1,'2026-09-09 23:08:27.328623','2026-09-09 23:08:27.328623');
/*!40000 ALTER TABLE `patient_provider_assignments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `patients`
--

DROP TABLE IF EXISTS `patients`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `patients` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `patient_uuid` char(36) NOT NULL,
  `tenant_id` bigint unsigned NOT NULL,
  `patient_id` varchar(50) NOT NULL,
  `mrn` varchar(100) NOT NULL,
  `account_number` varchar(100) NOT NULL,
  `first_name` varchar(100) NOT NULL,
  `middle_name` varchar(100) DEFAULT NULL,
  `last_name` varchar(100) NOT NULL,
  `preferred_name` varchar(100) DEFAULT NULL,
  `date_of_birth` date NOT NULL,
  `gender` enum('UNKNOWN','MALE','FEMALE','NON_BINARY','OTHER') NOT NULL DEFAULT 'UNKNOWN',
  `patient_status` enum('ACTIVE','INACTIVE','DECEASED','MERGED') NOT NULL DEFAULT 'ACTIVE',
  `registration_date` date DEFAULT NULL,
  `last_visit_date` date DEFAULT NULL,
  `patient_class` enum('OUTPATIENT','INPATIENT','OBSERVATION','EMERGENCY','OTHER') DEFAULT 'OUTPATIENT',
  `portal_access` enum('DISABLED','ENABLED','PENDING') NOT NULL DEFAULT 'DISABLED',
  `communication_consent` enum('PENDING','GRANTED','DECLINED') NOT NULL DEFAULT 'PENDING',
  `preferred_pharmacy` varchar(250) DEFAULT NULL,
  `referral_source` enum('PHYSICIAN','HOSPITAL','SELF','INSURANCE','EMPLOYER','OTHER') DEFAULT NULL,
  `financial_assistance` enum('NONE','PENDING','APPROVED','DECLINED') NOT NULL DEFAULT 'NONE',
  `notes` text,
  `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `updated_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_patients_patient_uuid` (`patient_uuid`),
  UNIQUE KEY `uk_patients_1` (`tenant_id`,`patient_id`),
  UNIQUE KEY `uk_patients_2` (`tenant_id`,`mrn`),
  UNIQUE KEY `uk_patients_3` (`tenant_id`,`account_number`),
  KEY `idx_patients_tenant_id` (`tenant_id`),
  KEY `idx_patients_first_name` (`first_name`),
  KEY `idx_patients_last_name` (`last_name`),
  KEY `idx_patients_date_of_birth` (`date_of_birth`),
  KEY `idx_patients_patient_status` (`patient_status`),
  KEY `idx_patients_patient_class` (`patient_class`),
  CONSTRAINT `fk_patients_tenant_id_1` FOREIGN KEY (`tenant_id`) REFERENCES `tenants` (`id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `patients`
--

LOCK TABLES `patients` WRITE;
/*!40000 ALTER TABLE `patients` DISABLE KEYS */;
INSERT INTO `patients` VALUES (1,'90000000-0000-4000-8000-000000000001',1,'PAT0001','MRN10001','ACC10001','James','A','Miller','Jim','1980-04-15','MALE','ACTIVE','2026-01-10','2026-08-20','OUTPATIENT','ENABLED','GRANTED','CVS Pharmacy','PHYSICIAN','NONE','Demo patient for commercial insurance workflow.','2026-09-09 23:08:26.532744','2026-09-09 23:08:26.532744'),(2,'90000000-0000-4000-8000-000000000002',1,'PAT0002','MRN10002','ACC10002','Maria','L','Garcia','Maria','1975-09-22','FEMALE','ACTIVE','2026-02-05','2026-08-25','OUTPATIENT','ENABLED','GRANTED','Walgreens','SELF','NONE',NULL,'2026-09-09 23:08:26.532744','2026-09-09 23:08:26.532744'),(3,'90000000-0000-4000-8000-000000000003',2,'PAT0003','MRN20001','ACC20001','Thomas','R','Clark','Tom','1968-11-03','MALE','ACTIVE','2026-03-12','2026-08-18','OUTPATIENT','ENABLED','GRANTED','CVS Pharmacy','HOSPITAL','PENDING',NULL,'2026-09-09 23:08:26.532744','2026-09-09 23:08:26.532744'),(4,'030c4436-c86e-40a5-838c-80d11229ca37',1,'PAT004','MRN004','ACC004','Robert','James','Anderson','Rob','1985-06-15','MALE','ACTIVE','2026-09-28',NULL,'OUTPATIENT','DISABLED','PENDING','Sunrise Pharmacy','PHYSICIAN','NONE','Initial patient registration.','2026-09-28 14:26:52.020194','2026-09-28 14:26:52.023735');
/*!40000 ALTER TABLE `patients` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `payers`
--

DROP TABLE IF EXISTS `payers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `payers` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `payer_uuid` char(36) NOT NULL,
  `tenant_id` bigint unsigned NOT NULL,
  `payer_code` varchar(50) NOT NULL,
  `payer_name` varchar(200) NOT NULL,
  `payer_id` varchar(50) DEFAULT NULL,
  `payer_type` enum('COMMERCIAL','MEDICARE','MEDICAID','TRICARE','WORKERS_COMP','OTHER') NOT NULL DEFAULT 'COMMERCIAL',
  `phone` varchar(30) DEFAULT NULL,
  `website` varchar(500) DEFAULT NULL,
  `status` enum('ACTIVE','INACTIVE') NOT NULL DEFAULT 'ACTIVE',
  `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `updated_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_payers_payer_uuid` (`payer_uuid`),
  UNIQUE KEY `uk_payers_1` (`tenant_id`,`payer_code`),
  KEY `idx_payers_tenant_id` (`tenant_id`),
  KEY `idx_payers_payer_name` (`payer_name`),
  KEY `idx_payers_payer_id` (`payer_id`),
  KEY `idx_payers_status` (`status`),
  CONSTRAINT `fk_payers_tenant_id_1` FOREIGN KEY (`tenant_id`) REFERENCES `tenants` (`id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `payers`
--

LOCK TABLES `payers` WRITE;
/*!40000 ALTER TABLE `payers` DISABLE KEYS */;
INSERT INTO `payers` VALUES (1,'30000000-0000-4000-8000-000000000001',1,'AETNA','Aetna','60054','COMMERCIAL','+1-800-872-3862','https://www.aetna.com','ACTIVE','2026-09-09 23:08:25.795625','2026-09-09 23:08:25.795625'),(2,'30000000-0000-4000-8000-000000000002',1,'BCBS','Blue Cross Blue Shield','12345','COMMERCIAL','+1-800-262-2583','https://www.bcbs.com','ACTIVE','2026-09-09 23:08:25.795625','2026-09-09 23:08:25.795625'),(3,'30000000-0000-4000-8000-000000000003',2,'UHC','UnitedHealthcare','87726','COMMERCIAL','+1-800-328-5979','https://www.uhc.com','ACTIVE','2026-09-09 23:08:25.795625','2026-09-09 23:08:25.795625'),(4,'a1bf309e-2a3e-494d-87cb-9707249adc72',1,'PAY001','Blue Cross Blue Shield','BCBS001','COMMERCIAL','800-555-1000','https://example.com','ACTIVE','2026-09-28 13:44:12.815892','2026-09-28 13:44:12.817646');
/*!40000 ALTER TABLE `payers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `payment_allocations`
--

DROP TABLE IF EXISTS `payment_allocations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `payment_allocations` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `tenant_id` bigint unsigned NOT NULL,
  `payment_id` bigint unsigned NOT NULL,
  `claim_id` bigint unsigned DEFAULT NULL,
  `patient_id` bigint unsigned DEFAULT NULL,
  `amount` decimal(12,2) NOT NULL DEFAULT '0.00',
  `allocation_type` varchar(30) NOT NULL DEFAULT 'CLAIM',
  `allocated_at` datetime(6) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `idx_payment_allocations_tenant_id` (`tenant_id`),
  KEY `idx_payment_allocations_payment_id` (`payment_id`),
  KEY `fk_payment_allocations_claim_id_2` (`claim_id`),
  KEY `fk_payment_allocations_patient_id_3` (`patient_id`),
  CONSTRAINT `fk_payment_allocations_claim_id_2` FOREIGN KEY (`claim_id`) REFERENCES `claims` (`id`),
  CONSTRAINT `fk_payment_allocations_patient_id_3` FOREIGN KEY (`patient_id`) REFERENCES `patients` (`id`),
  CONSTRAINT `fk_payment_allocations_payment_id_1` FOREIGN KEY (`payment_id`) REFERENCES `payments` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `payment_allocations`
--

LOCK TABLES `payment_allocations` WRITE;
/*!40000 ALTER TABLE `payment_allocations` DISABLE KEYS */;
INSERT INTO `payment_allocations` VALUES (1,1,1,1,1,144.00,'CLAIM','2026-08-28 12:00:00.000000'),(2,2,2,3,3,220.00,'CLAIM','2026-09-02 10:00:00.000000');
/*!40000 ALTER TABLE `payment_allocations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `payment_history`
--

DROP TABLE IF EXISTS `payment_history`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `payment_history` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `tenant_id` bigint unsigned NOT NULL,
  `payment_id` bigint unsigned NOT NULL,
  `from_status` varchar(30) NOT NULL,
  `to_status` varchar(30) NOT NULL,
  `note` varchar(1000) DEFAULT NULL,
  `changed_at` datetime(6) NOT NULL,
  `changed_by_user_id` bigint unsigned DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `idx_payment_history_tenant_id` (`tenant_id`),
  KEY `idx_payment_history_payment_id` (`payment_id`),
  KEY `idx_payment_history_changed_at` (`changed_at`),
  CONSTRAINT `fk_payment_history_payment_id_1` FOREIGN KEY (`payment_id`) REFERENCES `payments` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `payment_history`
--

LOCK TABLES `payment_history` WRITE;
/*!40000 ALTER TABLE `payment_history` DISABLE KEYS */;
INSERT INTO `payment_history` VALUES (1,1,1,'RECEIVED','POSTED','ERA reconciled and posted.','2026-08-28 12:00:00.000000',2),(2,2,2,'RECEIVED','RECEIVED','Payment received; pending posting.','2026-09-02 10:00:00.000000',4);
/*!40000 ALTER TABLE `payment_history` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `payment_lines`
--

DROP TABLE IF EXISTS `payment_lines`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `payment_lines` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `tenant_id` bigint unsigned NOT NULL,
  `payment_id` bigint unsigned NOT NULL,
  `claim_id` bigint unsigned DEFAULT NULL,
  `claim_line_id` bigint unsigned DEFAULT NULL,
  `charge_amount` decimal(12,2) NOT NULL DEFAULT '0.00',
  `allowed_amount` decimal(12,2) NOT NULL DEFAULT '0.00',
  `paid_amount` decimal(12,2) NOT NULL DEFAULT '0.00',
  `deductible_amount` decimal(12,2) NOT NULL DEFAULT '0.00',
  `coinsurance_amount` decimal(12,2) NOT NULL DEFAULT '0.00',
  `copay_amount` decimal(12,2) NOT NULL DEFAULT '0.00',
  `patient_responsibility` decimal(12,2) NOT NULL DEFAULT '0.00',
  `adjustment_group_code` varchar(10) DEFAULT NULL,
  `adjustment_reason_code` varchar(10) DEFAULT NULL,
  `adjustment_amount` decimal(12,2) NOT NULL DEFAULT '0.00',
  PRIMARY KEY (`id`),
  KEY `idx_payment_lines_tenant_id` (`tenant_id`),
  KEY `idx_payment_lines_payment_id` (`payment_id`),
  KEY `fk_payment_lines_claim_id_2` (`claim_id`),
  KEY `fk_payment_lines_claim_line_id_3` (`claim_line_id`),
  CONSTRAINT `fk_payment_lines_claim_id_2` FOREIGN KEY (`claim_id`) REFERENCES `claims` (`id`),
  CONSTRAINT `fk_payment_lines_claim_line_id_3` FOREIGN KEY (`claim_line_id`) REFERENCES `claim_lines` (`id`),
  CONSTRAINT `fk_payment_lines_payment_id_1` FOREIGN KEY (`payment_id`) REFERENCES `payments` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `payment_lines`
--

LOCK TABLES `payment_lines` WRITE;
/*!40000 ALTER TABLE `payment_lines` DISABLE KEYS */;
INSERT INTO `payment_lines` VALUES (1,1,1,1,1,250.00,180.00,144.00,0.00,36.00,0.00,36.00,'CO','45',70.00),(2,2,2,3,NULL,300.00,220.00,220.00,0.00,0.00,0.00,0.00,'CO','45',80.00);
/*!40000 ALTER TABLE `payment_lines` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `payments`
--

DROP TABLE IF EXISTS `payments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `payments` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `payment_guid` char(36) NOT NULL,
  `tenant_id` bigint unsigned NOT NULL,
  `payer_id` bigint unsigned NOT NULL,
  `payment_number` varchar(60) NOT NULL,
  `payment_type` varchar(30) NOT NULL DEFAULT 'ERA',
  `status` varchar(30) NOT NULL DEFAULT 'POSTED',
  `payment_date` date NOT NULL,
  `check_number` varchar(100) DEFAULT NULL,
  `trace_number` varchar(100) DEFAULT NULL,
  `payment_amount` decimal(12,2) NOT NULL DEFAULT '0.00',
  `posted_amount` decimal(12,2) NOT NULL DEFAULT '0.00',
  `unapplied_amount` decimal(12,2) NOT NULL DEFAULT '0.00',
  `source_file` varchar(500) DEFAULT NULL,
  `created_at` datetime(6) NOT NULL,
  `updated_at` datetime(6) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_payments_payment_guid` (`payment_guid`),
  UNIQUE KEY `uk_payments_1` (`tenant_id`,`payment_number`),
  KEY `idx_payments_tenant_id` (`tenant_id`),
  KEY `idx_payments_status` (`status`),
  KEY `idx_payments_payment_date` (`payment_date`),
  KEY `fk_payments_payer_id_2` (`payer_id`),
  CONSTRAINT `fk_payments_payer_id_2` FOREIGN KEY (`payer_id`) REFERENCES `payers` (`id`),
  CONSTRAINT `fk_payments_tenant_id_1` FOREIGN KEY (`tenant_id`) REFERENCES `tenants` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `payments`
--

LOCK TABLES `payments` WRITE;
/*!40000 ALTER TABLE `payments` DISABLE KEYS */;
INSERT INTO `payments` VALUES (1,'b1000000-0000-4000-8000-000000000001',1,1,'PMT-2026-0001','ERA','POSTED','2026-08-28','CHK-AET-1001','TRC-AET-1001',144.00,144.00,0.00,'era_20260828_aetna_001.edi','2026-08-28 11:50:00.000000','2026-08-28 12:00:00.000000'),(2,'b1000000-0000-4000-8000-000000000002',2,3,'PMT-2026-0002','ERA','RECEIVED','2026-09-02','CHK-UHC-2001','TRC-UHC-2001',220.00,0.00,220.00,'era_20260902_uhc_001.edi','2026-09-02 10:00:00.000000','2026-09-02 10:00:00.000000');
/*!40000 ALTER TABLE `payments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `permissions`
--

DROP TABLE IF EXISTS `permissions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `permissions` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `permission_uuid` char(36) NOT NULL,
  `permission_code` varchar(150) NOT NULL,
  `permission_name` varchar(200) NOT NULL,
  `module` varchar(100) NOT NULL,
  `action` varchar(100) NOT NULL,
  `description` varchar(500) DEFAULT NULL,
  `is_active` tinyint NOT NULL DEFAULT '1',
  `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `updated_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_permissions_permission_uuid` (`permission_uuid`),
  UNIQUE KEY `uk_permissions_permission_code` (`permission_code`),
  KEY `idx_permissions_module` (`module`),
  KEY `idx_permissions_action` (`action`),
  KEY `idx_permissions_is_active` (`is_active`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `permissions`
--

LOCK TABLES `permissions` WRITE;
/*!40000 ALTER TABLE `permissions` DISABLE KEYS */;
INSERT INTO `permissions` VALUES (1,'10000000-0000-4000-8000-000000000001','PATIENT_VIEW','View Patients','PATIENT','VIEW','View patient records',1,'2026-09-09 23:08:25.609757','2026-09-09 23:08:25.609757'),(2,'10000000-0000-4000-8000-000000000002','CLAIM_VIEW','View Claims','CLAIMS','VIEW','View claims and claim status',1,'2026-09-09 23:08:25.609757','2026-09-09 23:08:25.609757'),(3,'10000000-0000-4000-8000-000000000003','CLAIM_MANAGE','Manage Claims','CLAIMS','MANAGE','Create and manage claims',1,'2026-09-09 23:08:25.609757','2026-09-09 23:08:25.609757'),(4,'10000000-0000-4000-8000-000000000004','APPEAL_MANAGE','Manage Appeals','APPEALS','MANAGE','Manage denial appeals',1,'2026-09-09 23:08:25.609757','2026-09-09 23:08:25.609757');
/*!40000 ALTER TABLE `permissions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `providers`
--

DROP TABLE IF EXISTS `providers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `providers` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `provider_uuid` char(36) NOT NULL,
  `tenant_id` bigint unsigned NOT NULL,
  `organization_id` bigint unsigned NOT NULL,
  `provider_code` varchar(50) NOT NULL,
  `first_name` varchar(100) NOT NULL,
  `middle_name` varchar(100) DEFAULT NULL,
  `last_name` varchar(100) NOT NULL,
  `credential` varchar(50) DEFAULT NULL,
  `npi` varchar(10) NOT NULL,
  `specialty` varchar(150) DEFAULT NULL,
  `department` varchar(150) DEFAULT NULL,
  `phone` varchar(30) DEFAULT NULL,
  `email` varchar(254) DEFAULT NULL,
  `status` enum('ACTIVE','INACTIVE') NOT NULL DEFAULT 'ACTIVE',
  `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `updated_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_providers_provider_uuid` (`provider_uuid`),
  UNIQUE KEY `uk_providers_1` (`tenant_id`,`provider_code`),
  UNIQUE KEY `uk_providers_2` (`tenant_id`,`npi`),
  KEY `idx_providers_tenant_id` (`tenant_id`),
  KEY `idx_providers_organization_id` (`organization_id`),
  KEY `idx_providers_last_name` (`last_name`),
  KEY `idx_providers_specialty` (`specialty`),
  KEY `idx_providers_status` (`status`),
  CONSTRAINT `fk_providers_organization_id_2` FOREIGN KEY (`organization_id`) REFERENCES `organizations` (`id`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `fk_providers_tenant_id_1` FOREIGN KEY (`tenant_id`) REFERENCES `tenants` (`id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `providers`
--

LOCK TABLES `providers` WRITE;
/*!40000 ALTER TABLE `providers` DISABLE KEYS */;
INSERT INTO `providers` VALUES (1,'f47ac10b-58cc-4372-a567-0e02b2c3d479',1,1,'PRV001','Robert',NULL,'Wilson','MD','1111111111','Internal Medicine','Primary Care','+1-212-555-1301','robert.wilson@sunrise.example','ACTIVE','2026-09-09 23:08:26.428088','2026-09-24 19:27:38.000000'),(2,'9b1deb4d-3b7d-4bad-9bdd-2b0d7b3f4c61',1,1,'PRV002','Jennifer',NULL,'Taylor','MD','2222222222','Cardiology','Cardiology','+1-212-555-1302','jennifer.taylor@sunrise.example','ACTIVE','2026-09-09 23:08:26.428088','2026-09-24 19:27:38.000000'),(3,'c9bf9e57-1685-4c89-bafb-ff5af830be8a',2,2,'NSP001','Daniel',NULL,'Anderson','DO','3333333333','Family Medicine','Primary Care','+1-312-555-1303','daniel.anderson@northstar.example','ACTIVE','2026-09-09 23:08:26.428088','2026-09-24 19:27:38.000000'),(4,'6ba7b810-9dad-41d1-80b4-00c04fd430c8',1,1,'PRV004','John','Michael','Smith','MD','1234567891','Internal Medicine','Primary Care','555-0300','john.smith@sunrise.example','ACTIVE','2026-09-24 13:38:48.901035','2026-09-24 19:27:38.000000');
/*!40000 ALTER TABLE `providers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `role_permissions`
--

DROP TABLE IF EXISTS `role_permissions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `role_permissions` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `role_id` bigint unsigned NOT NULL,
  `permission_id` bigint unsigned NOT NULL,
  `granted_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_role_permissions_role_id` (`role_id`),
  UNIQUE KEY `uk_role_permissions_permission_id` (`permission_id`),
  CONSTRAINT `fk_role_permissions_permission_id_2` FOREIGN KEY (`permission_id`) REFERENCES `permissions` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_role_permissions_role_id_1` FOREIGN KEY (`role_id`) REFERENCES `roles` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `role_permissions`
--

LOCK TABLES `role_permissions` WRITE;
/*!40000 ALTER TABLE `role_permissions` DISABLE KEYS */;
INSERT INTO `role_permissions` VALUES (1,1,1,'2026-09-09 23:08:27.418833'),(2,2,2,'2026-09-09 23:08:27.418833');
/*!40000 ALTER TABLE `role_permissions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `roles`
--

DROP TABLE IF EXISTS `roles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `roles` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `role_uuid` char(36) NOT NULL,
  `tenant_id` bigint unsigned DEFAULT NULL,
  `role_code` varchar(100) NOT NULL,
  `role_name` varchar(150) NOT NULL,
  `description` varchar(500) DEFAULT NULL,
  `is_system_role` tinyint NOT NULL DEFAULT '0',
  `is_active` tinyint NOT NULL DEFAULT '1',
  `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `updated_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_roles_role_uuid` (`role_uuid`),
  UNIQUE KEY `uk_roles_1` (`tenant_id`,`role_code`),
  KEY `idx_roles_tenant_id` (`tenant_id`),
  KEY `idx_roles_is_active` (`is_active`),
  CONSTRAINT `fk_roles_tenant_id_1` FOREIGN KEY (`tenant_id`) REFERENCES `tenants` (`id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `roles`
--

LOCK TABLES `roles` WRITE;
/*!40000 ALTER TABLE `roles` DISABLE KEYS */;
INSERT INTO `roles` VALUES (1,'40000000-0000-4000-8000-000000000001',1,'ADMIN','Tenant Administrator','Full tenant administration',0,1,'2026-09-09 23:08:25.894322','2026-09-09 23:08:25.894322'),(2,'40000000-0000-4000-8000-000000000002',1,'BILLING','Billing Specialist','Claims and payment operations',0,1,'2026-09-09 23:08:25.894322','2026-09-09 23:08:25.894322'),(3,'40000000-0000-4000-8000-000000000003',1,'RCM_MANAGER','RCM Manager','Revenue cycle management oversight',0,1,'2026-09-09 23:08:25.894322','2026-09-09 23:08:25.894322');
/*!40000 ALTER TABLE `roles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tenants`
--

DROP TABLE IF EXISTS `tenants`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tenants` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `tenant_uuid` char(36) NOT NULL,
  `tenant_code` varchar(50) NOT NULL,
  `tenant_name` varchar(200) NOT NULL,
  `legal_name` varchar(250) DEFAULT NULL,
  `tax_identifier` varchar(50) DEFAULT NULL,
  `status` enum('TRIAL','ACTIVE','SUSPENDED','INACTIVE') NOT NULL DEFAULT 'TRIAL',
  `subscription_plan` enum('STARTER','PROFESSIONAL','ENTERPRISE') NOT NULL DEFAULT 'STARTER',
  `timezone` varchar(100) NOT NULL DEFAULT 'America/New_York',
  `contact_first_name` varchar(100) DEFAULT NULL,
  `contact_last_name` varchar(100) DEFAULT NULL,
  `contact_email` varchar(254) DEFAULT NULL,
  `contact_phone` varchar(30) DEFAULT NULL,
  `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `updated_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_tenants_tenant_uuid` (`tenant_uuid`),
  UNIQUE KEY `uk_tenants_tenant_code` (`tenant_code`),
  UNIQUE KEY `uk_tenants_tax_identifier` (`tax_identifier`),
  KEY `idx_tenants_tenant_name` (`tenant_name`),
  KEY `idx_tenants_status` (`status`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tenants`
--

LOCK TABLES `tenants` WRITE;
/*!40000 ALTER TABLE `tenants` DISABLE KEYS */;
INSERT INTO `tenants` VALUES (1,'00000000-0000-4000-8000-000000000001','INSURE01','Sunrise Medical Group','Sunrise Medical Group LLC','12-3456789','ACTIVE','ENTERPRISE','America/New_York','Sarah','Johnson','admin@sunrise.example','+1-212-555-0101','2026-09-09 23:08:24.522605','2026-09-09 23:08:24.522605'),(2,'00000000-0000-4000-8000-000000000002','INSURE02','NorthStar Health Partners','NorthStar Health Partners Inc','98-7654321','ACTIVE','PROFESSIONAL','America/Chicago','Michael','Brown','admin@northstar.example','+1-312-555-0102','2026-09-09 23:08:24.522605','2026-09-09 23:08:24.522605');
/*!40000 ALTER TABLE `tenants` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `user_locations`
--

DROP TABLE IF EXISTS `user_locations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_locations` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `user_id` bigint unsigned NOT NULL,
  `location_id` bigint unsigned NOT NULL,
  `assigned_by_user_id` bigint unsigned DEFAULT NULL,
  `is_primary` tinyint NOT NULL DEFAULT '0',
  `is_active` tinyint NOT NULL DEFAULT '1',
  `assigned_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_user_locations_user_id` (`user_id`),
  UNIQUE KEY `uk_user_locations_location_id` (`location_id`),
  KEY `idx_user_locations_assigned_by_user_id` (`assigned_by_user_id`),
  CONSTRAINT `fk_user_locations_assigned_by_user_id_3` FOREIGN KEY (`assigned_by_user_id`) REFERENCES `users` (`id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `fk_user_locations_location_id_2` FOREIGN KEY (`location_id`) REFERENCES `locations` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_user_locations_user_id_1` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user_locations`
--

LOCK TABLES `user_locations` WRITE;
/*!40000 ALTER TABLE `user_locations` DISABLE KEYS */;
INSERT INTO `user_locations` VALUES (1,1,1,1,1,1,'2026-09-09 23:08:27.492001');
/*!40000 ALTER TABLE `user_locations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `user_organizations`
--

DROP TABLE IF EXISTS `user_organizations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_organizations` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `user_id` bigint unsigned NOT NULL,
  `organization_id` bigint unsigned NOT NULL,
  `assigned_by_user_id` bigint unsigned DEFAULT NULL,
  `is_primary` tinyint NOT NULL DEFAULT '0',
  `is_active` tinyint NOT NULL DEFAULT '1',
  `assigned_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_user_organizations_user_id` (`user_id`),
  UNIQUE KEY `uk_user_organizations_organization_id` (`organization_id`),
  KEY `idx_user_organizations_assigned_by_user_id` (`assigned_by_user_id`),
  CONSTRAINT `fk_user_organizations_assigned_by_user_id_3` FOREIGN KEY (`assigned_by_user_id`) REFERENCES `users` (`id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `fk_user_organizations_organization_id_2` FOREIGN KEY (`organization_id`) REFERENCES `organizations` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_user_organizations_user_id_1` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user_organizations`
--

LOCK TABLES `user_organizations` WRITE;
/*!40000 ALTER TABLE `user_organizations` DISABLE KEYS */;
INSERT INTO `user_organizations` VALUES (1,1,1,1,1,1,'2026-09-09 23:08:27.552337');
/*!40000 ALTER TABLE `user_organizations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `user_roles`
--

DROP TABLE IF EXISTS `user_roles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_roles` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `user_id` bigint unsigned NOT NULL,
  `role_id` bigint unsigned NOT NULL,
  `assigned_by_user_id` bigint unsigned DEFAULT NULL,
  `assigned_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `expires_at` datetime(6) DEFAULT NULL,
  `is_active` tinyint NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_user_roles_user_id` (`user_id`),
  UNIQUE KEY `uk_user_roles_role_id` (`role_id`),
  KEY `idx_user_roles_assigned_by_user_id` (`assigned_by_user_id`),
  KEY `idx_user_roles_is_active` (`is_active`),
  CONSTRAINT `fk_user_roles_assigned_by_user_id_3` FOREIGN KEY (`assigned_by_user_id`) REFERENCES `users` (`id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `fk_user_roles_role_id_2` FOREIGN KEY (`role_id`) REFERENCES `roles` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_user_roles_user_id_1` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user_roles`
--

LOCK TABLES `user_roles` WRITE;
/*!40000 ALTER TABLE `user_roles` DISABLE KEYS */;
INSERT INTO `user_roles` VALUES (1,1,1,1,'2026-09-09 23:08:27.595910',NULL,1);
/*!40000 ALTER TABLE `user_roles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `user_uuid` char(36) NOT NULL,
  `tenant_id` bigint unsigned NOT NULL,
  `username` varchar(100) NOT NULL,
  `email` varchar(254) NOT NULL,
  `password_hash` varchar(255) NOT NULL,
  `first_name` varchar(100) NOT NULL,
  `middle_name` varchar(100) DEFAULT NULL,
  `last_name` varchar(100) NOT NULL,
  `phone` varchar(30) DEFAULT NULL,
  `job_title` varchar(150) DEFAULT NULL,
  `employee_number` varchar(100) DEFAULT NULL,
  `status` enum('PENDING','ACTIVE','LOCKED','INACTIVE') NOT NULL DEFAULT 'PENDING',
  `is_email_verified` tinyint NOT NULL DEFAULT '0',
  `last_login_at` datetime(6) DEFAULT NULL,
  `password_changed_at` datetime(6) DEFAULT NULL,
  `failed_login_attempts` int unsigned NOT NULL DEFAULT '0',
  `locked_until` datetime(6) DEFAULT NULL,
  `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `updated_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_users_user_uuid` (`user_uuid`),
  UNIQUE KEY `uk_users_1` (`tenant_id`,`username`),
  UNIQUE KEY `uk_users_2` (`tenant_id`,`email`),
  UNIQUE KEY `uk_users_3` (`tenant_id`,`employee_number`),
  KEY `idx_users_tenant_id` (`tenant_id`),
  KEY `idx_users_last_name` (`last_name`),
  KEY `idx_users_status` (`status`),
  CONSTRAINT `fk_users_tenant_id_1` FOREIGN KEY (`tenant_id`) REFERENCES `tenants` (`id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,'50000000-0000-4000-8000-000000000001',1,'sarah.johnson','sarah.johnson@sunrise.example','$2b$12$SunriseDemoHash000000000000000000000000000000000000','Sarah',NULL,'Johnson','+1-212-555-1101','Administrator','EMP001','ACTIVE',1,NULL,NULL,0,NULL,'2026-09-09 23:08:25.971728','2026-09-09 23:08:25.971728'),(2,'50000000-0000-4000-8000-000000000002',1,'david.lee','david.lee@sunrise.example','$2b$12$SunriseDemoHash000000000000000000000000000000000000','David',NULL,'Lee','+1-212-555-1102','Billing Specialist','EMP002','ACTIVE',1,NULL,NULL,0,NULL,'2026-09-09 23:08:25.971728','2026-09-09 23:08:25.971728'),(3,'50000000-0000-4000-8000-000000000003',1,'emily.davis','emily.davis@sunrise.example','$2b$12$SunriseDemoHash000000000000000000000000000000000000','Emily',NULL,'Davis','+1-212-555-1103','RCM Manager','EMP003','ACTIVE',1,NULL,NULL,0,NULL,'2026-09-09 23:08:25.971728','2026-09-09 23:08:25.971728'),(4,'50000000-0000-4000-8000-000000000004',2,'john.smith','john.smith@northstar.example','$2b$12$SunriseDemoHash000000000000000000000000000000000000','John',NULL,'Smith','+1-312-555-1104','Administrator','NS001','ACTIVE',1,NULL,NULL,0,NULL,'2026-09-09 23:08:25.971728','2026-09-09 23:08:25.971728');
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping routines for database 'insureflow_db'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-11-17 16:54:49
