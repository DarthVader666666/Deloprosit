use DeloprositDb

INSERT INTO Users (Nickname, Email, Password, IsConfirmed)
VALUES ('alex', 's9SovJPImunbRTz8OKtuwQ==', 's9SovJPImunbRTz8OKtuwQ==', 'true'), ('admin', 'JtfP1IxKgKVGB4ADFXFnvA==', 'efavXKTzRTFnR7w69A7OJA==', 'true')

INSERT INTO UserRoles (UserId, RoleId)
VALUES (1, 1), (2, 2)