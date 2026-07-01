CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260618222914_InitialCreate') THEN
    CREATE EXTENSION IF NOT EXISTS vector;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260618222914_InitialCreate') THEN
    CREATE TABLE documents (
        "Id" uuid NOT NULL,
        "FileName" character varying(260) NOT NULL,
        "ContentType" character varying(120) NOT NULL,
        "SizeInBytes" bigint NOT NULL,
        "Status" character varying(40) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_documents" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260618222914_InitialCreate') THEN
    CREATE TABLE roles (
        "Id" uuid NOT NULL,
        "Name" character varying(100) NOT NULL,
        CONSTRAINT "PK_roles" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260618222914_InitialCreate') THEN
    CREATE TABLE users (
        "Id" uuid NOT NULL,
        "Email" character varying(320) NOT NULL,
        "DisplayName" character varying(200) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_users" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260618222914_InitialCreate') THEN
    CREATE TABLE document_chunks (
        "Id" uuid NOT NULL,
        "DocumentId" uuid NOT NULL,
        "Content" text NOT NULL,
        "PageNumber" integer,
        "ChunkIndex" integer NOT NULL,
        "TokenCount" integer NOT NULL,
        "Embedding" vector(1024),
        "CreatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_document_chunks" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_document_chunks_documents_DocumentId" FOREIGN KEY ("DocumentId") REFERENCES documents ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260618222914_InitialCreate') THEN
    CREATE TABLE chat_sessions (
        "Id" uuid NOT NULL,
        "UserId" uuid NOT NULL,
        "Title" character varying(200),
        "CreatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_chat_sessions" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_chat_sessions_users_UserId" FOREIGN KEY ("UserId") REFERENCES users ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260618222914_InitialCreate') THEN
    CREATE TABLE chat_messages (
        "Id" uuid NOT NULL,
        "ChatSessionId" uuid NOT NULL,
        "Role" character varying(40) NOT NULL,
        "Content" text NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_chat_messages" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_chat_messages_chat_sessions_ChatSessionId" FOREIGN KEY ("ChatSessionId") REFERENCES chat_sessions ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260618222914_InitialCreate') THEN
    CREATE TABLE chat_feedback (
        "Id" uuid NOT NULL,
        "ChatMessageId" uuid NOT NULL,
        "Rating" character varying(40) NOT NULL,
        "Comment" character varying(1000),
        "CreatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_chat_feedback" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_chat_feedback_chat_messages_ChatMessageId" FOREIGN KEY ("ChatMessageId") REFERENCES chat_messages ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260618222914_InitialCreate') THEN
    CREATE INDEX "IX_chat_feedback_ChatMessageId" ON chat_feedback ("ChatMessageId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260618222914_InitialCreate') THEN
    CREATE INDEX "IX_chat_messages_ChatSessionId" ON chat_messages ("ChatSessionId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260618222914_InitialCreate') THEN
    CREATE INDEX "IX_chat_sessions_UserId" ON chat_sessions ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260618222914_InitialCreate') THEN
    CREATE INDEX "IX_document_chunks_DocumentId" ON document_chunks ("DocumentId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260618222914_InitialCreate') THEN
    CREATE INDEX "IX_document_chunks_Embedding" ON document_chunks USING hnsw ("Embedding" vector_cosine_ops);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260618222914_InitialCreate') THEN
    CREATE UNIQUE INDEX "IX_roles_Name" ON roles ("Name");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260618222914_InitialCreate') THEN
    CREATE UNIQUE INDEX "IX_users_Email" ON users ("Email");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260618222914_InitialCreate') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260618222914_InitialCreate', '10.0.9');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260620093928_AddDocumentStorageFields') THEN
    ALTER TABLE documents ADD "FailureReason" character varying(1000);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260620093928_AddDocumentStorageFields') THEN
    ALTER TABLE documents ADD "StoragePath" character varying(1000) NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260620093928_AddDocumentStorageFields') THEN
    ALTER TABLE documents ADD "StoredFileName" character varying(300) NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260620093928_AddDocumentStorageFields') THEN
    ALTER TABLE documents ADD "UpdatedAt" timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260620093928_AddDocumentStorageFields') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260620093928_AddDocumentStorageFields', '10.0.9');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627225601_AddDocumentMetadata') THEN
    ALTER TABLE documents ADD "CategoryId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627225601_AddDocumentMetadata') THEN
    ALTER TABLE documents ADD "DepartmentId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627225601_AddDocumentMetadata') THEN
    CREATE TABLE departments (
        "Id" uuid NOT NULL,
        "Name" character varying(160) NOT NULL,
        "Slug" character varying(160) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_departments" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627225601_AddDocumentMetadata') THEN
    CREATE TABLE document_categories (
        "Id" uuid NOT NULL,
        "Name" character varying(160) NOT NULL,
        "Slug" character varying(160) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_document_categories" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627225601_AddDocumentMetadata') THEN
    INSERT INTO departments ("Id", "CreatedAt", "Name", "Slug")
    VALUES ('11111111-1111-1111-1111-111111111111', TIMESTAMPTZ '2026-01-01T00:00:00+00:00', 'İnsan Kaynakları', 'human-resources');
    INSERT INTO departments ("Id", "CreatedAt", "Name", "Slug")
    VALUES ('22222222-2222-2222-2222-222222222222', TIMESTAMPTZ '2026-01-01T00:00:00+00:00', 'Finans', 'finance');
    INSERT INTO departments ("Id", "CreatedAt", "Name", "Slug")
    VALUES ('33333333-3333-3333-3333-333333333333', TIMESTAMPTZ '2026-01-01T00:00:00+00:00', 'Bilgi Teknolojileri', 'information-technology');
    INSERT INTO departments ("Id", "CreatedAt", "Name", "Slug")
    VALUES ('44444444-4444-4444-4444-444444444444', TIMESTAMPTZ '2026-01-01T00:00:00+00:00', 'Operasyon', 'operations');
    INSERT INTO departments ("Id", "CreatedAt", "Name", "Slug")
    VALUES ('55555555-5555-5555-5555-555555555555', TIMESTAMPTZ '2026-01-01T00:00:00+00:00', 'Hukuk', 'legal');
    INSERT INTO departments ("Id", "CreatedAt", "Name", "Slug")
    VALUES ('66666666-6666-6666-6666-666666666666', TIMESTAMPTZ '2026-01-01T00:00:00+00:00', 'Genel', 'general');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627225601_AddDocumentMetadata') THEN
    INSERT INTO document_categories ("Id", "CreatedAt", "Name", "Slug")
    VALUES ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', TIMESTAMPTZ '2026-01-01T00:00:00+00:00', 'Politika', 'policy');
    INSERT INTO document_categories ("Id", "CreatedAt", "Name", "Slug")
    VALUES ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', TIMESTAMPTZ '2026-01-01T00:00:00+00:00', 'Prosedür', 'procedure');
    INSERT INTO document_categories ("Id", "CreatedAt", "Name", "Slug")
    VALUES ('cccccccc-cccc-cccc-cccc-cccccccccccc', TIMESTAMPTZ '2026-01-01T00:00:00+00:00', 'Kılavuz', 'guide');
    INSERT INTO document_categories ("Id", "CreatedAt", "Name", "Slug")
    VALUES ('dddddddd-dddd-dddd-dddd-dddddddddddd', TIMESTAMPTZ '2026-01-01T00:00:00+00:00', 'Sözleşme', 'contract');
    INSERT INTO document_categories ("Id", "CreatedAt", "Name", "Slug")
    VALUES ('eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', TIMESTAMPTZ '2026-01-01T00:00:00+00:00', 'CV', 'cv');
    INSERT INTO document_categories ("Id", "CreatedAt", "Name", "Slug")
    VALUES ('ffffffff-ffff-ffff-ffff-ffffffffffff', TIMESTAMPTZ '2026-01-01T00:00:00+00:00', 'Diğer', 'other');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627225601_AddDocumentMetadata') THEN
    CREATE INDEX "IX_documents_CategoryId" ON documents ("CategoryId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627225601_AddDocumentMetadata') THEN
    CREATE INDEX "IX_documents_DepartmentId" ON documents ("DepartmentId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627225601_AddDocumentMetadata') THEN
    CREATE UNIQUE INDEX "IX_departments_Slug" ON departments ("Slug");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627225601_AddDocumentMetadata') THEN
    CREATE UNIQUE INDEX "IX_document_categories_Slug" ON document_categories ("Slug");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627225601_AddDocumentMetadata') THEN
    ALTER TABLE documents ADD CONSTRAINT "FK_documents_departments_DepartmentId" FOREIGN KEY ("DepartmentId") REFERENCES departments ("Id") ON DELETE SET NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627225601_AddDocumentMetadata') THEN
    ALTER TABLE documents ADD CONSTRAINT "FK_documents_document_categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES document_categories ("Id") ON DELETE SET NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627225601_AddDocumentMetadata') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260627225601_AddDocumentMetadata', '10.0.9');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628123306_AddChatSessionHistory') THEN
    ALTER TABLE chat_sessions DROP CONSTRAINT "FK_chat_sessions_users_UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628123306_AddChatSessionHistory') THEN
    ALTER TABLE chat_sessions ALTER COLUMN "UserId" DROP NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628123306_AddChatSessionHistory') THEN
    ALTER TABLE chat_sessions ADD "UpdatedAt" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628123306_AddChatSessionHistory') THEN
    ALTER TABLE chat_messages ADD "SourcesJson" jsonb;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628123306_AddChatSessionHistory') THEN
    ALTER TABLE chat_sessions ADD CONSTRAINT "FK_chat_sessions_users_UserId" FOREIGN KEY ("UserId") REFERENCES users ("Id") ON DELETE SET NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628123306_AddChatSessionHistory') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260628123306_AddChatSessionHistory', '10.0.9');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628182201_AddDocumentChunkMetadata') THEN
    ALTER TABLE documents ADD "PageCount" integer;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628182201_AddDocumentChunkMetadata') THEN
    ALTER TABLE document_chunks ADD "ChunkType" character varying(40) NOT NULL DEFAULT 'Fixed';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628182201_AddDocumentChunkMetadata') THEN
    ALTER TABLE document_chunks ADD "ClauseId" character varying(120);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628182201_AddDocumentChunkMetadata') THEN
    ALTER TABLE document_chunks ADD "EndPageNumber" integer;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628182201_AddDocumentChunkMetadata') THEN
    ALTER TABLE document_chunks ADD "Heading" character varying(300);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628182201_AddDocumentChunkMetadata') THEN
    ALTER TABLE document_chunks ADD "StartPageNumber" integer;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628182201_AddDocumentChunkMetadata') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260628182201_AddDocumentChunkMetadata', '10.0.9');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628184017_RemoveDocumentChunkPageNumber') THEN
    ALTER TABLE document_chunks DROP COLUMN "PageNumber";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628184017_RemoveDocumentChunkPageNumber') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260628184017_RemoveDocumentChunkPageNumber', '10.0.9');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628190018_AddChatSessionSoftDelete') THEN
    ALTER TABLE chat_sessions ADD "DeletedAt" timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628190018_AddChatSessionSoftDelete') THEN
    ALTER TABLE chat_sessions ADD "IsDeleted" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628190018_AddChatSessionSoftDelete') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260628190018_AddChatSessionSoftDelete', '10.0.9');
    END IF;
END $EF$;
COMMIT;

