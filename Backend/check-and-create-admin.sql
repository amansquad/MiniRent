-- Check if admin user exists
SELECT Id, Username, FullName, Role, IsActive, CreatedAt
FROM Users
WHERE Username = 'admin';

-- If no admin exists, create one
-- Note: You'll need to run this manually if the user doesn't exist
-- The password hash below is for 'admin123'
IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'admin')
BEGIN
    INSERT INTO Users (Id, Username, PasswordHash, FullName, Email, Role, IsActive, CreatedAt)
    VALUES (
        NEWID(),
        'admin',
        '$2a$11$YourHashHere', -- This needs to be generated
        'System Administrator',
        'admin@minirent.com',
        0, -- Admin role
        1, -- IsActive
        GETUTCDATE()
    );
    PRINT 'Admin user created successfully';
END
ELSE
BEGIN
    PRINT 'Admin user already exists';
END
