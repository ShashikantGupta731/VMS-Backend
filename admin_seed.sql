
-- SQL Script to manually add the Admin user
-- Use this ONLY if you don't want to use EF Core Seeding

-- 1. Insert the Admin user
-- NOTE: UserId=1, DDOCode='ADMIN', Password='Admin@1234' (hashed)
INSERT INTO "Users" ("UserId", "Username", "PasswordHash", "Name", "DDOCode", "Enabled", "CreatedDate", "FailedAttempts", "IsGuest", "IsNonTreasuryDDO")
VALUES (1, 'admin', '$2a$11$v1eUXEH5k565XSNl.0exsOTBEfRQqB8Zzj/6WYFFWNaRLWkBog5PG', 'System Administrator', 'ADMIN', true, NOW(), 0, false, false);

-- 2. Link Admin user to the ADMN role (RoleId=2)
INSERT INTO "UserRoles" ("UserId", "RoleId")
VALUES (1, 2);
