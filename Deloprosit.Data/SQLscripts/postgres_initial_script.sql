INSERT INTO vader."Users"(
	"Nickname", "FirstName", "LastName", "BirthDate", "RegisterDate", "Country", "City", "UserTitle", "Info", "Avatar", "Email", "Password", "IsConfirmed")
	VALUES ('alex', null, null, null, null, null, null, null, null, null,'s9SovJPImunbRTz8OKtuwQ==','s9SovJPImunbRTz8OKtuwQ==','1');

INSERT INTO vader."Users"(
	"Nickname", "FirstName", "LastName", "BirthDate", "RegisterDate", "Country", "City", "UserTitle", "Info", "Avatar", "Email", "Password", "IsConfirmed")
	VALUES ('admin', null, null, null, null, null, null, null, null, null,'JtfP1IxKgKVGB4ADFXFnvA==','efavXKTzRTFnR7w69A7OJA==','1');

INSERT INTO vader."UserRoles" ("UserId", "RoleId")
VALUES (1, 1), (2, 2);