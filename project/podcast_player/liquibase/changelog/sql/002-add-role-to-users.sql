--liquibase formatted sql

--changeset podcast:002-add-role-to-users
ALTER TABLE users ADD COLUMN IF NOT EXISTS role INTEGER NOT NULL DEFAULT 1;

--changeset podcast:002-add-role-comment
COMMENT ON COLUMN users.role IS 'Роль пользователя: 1=User, 2=Manager, 3=Admin';

