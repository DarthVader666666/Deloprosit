INSERT INTO vader."Users"(
	"UserId", "Nickname", "FirstName", "LastName", "BirthDate", "RegisterDate", "Country", "City", "UserTitle", "Info", "Avatar", "Email", "Password", "IsConfirmed")
	VALUES (1, 'owner', null, null, null, null, null, null, null, null, null,'efavXKTzRTFnR7w69A7OJA==','JtfP1IxKgKVGB4ADFXFnvA==','1');

INSERT INTO vader."Users"(
	"UserId", "Nickname", "FirstName", "LastName", "BirthDate", "RegisterDate", "Country", "City", "UserTitle", "Info", "Avatar", "Email", "Password", "IsConfirmed")
	VALUES (2, 'admin', null, null, null, null, null, null, null, null, null,'JtfP1IxKgKVGB4ADFXFnvA==','efavXKTzRTFnR7w69A7OJA==','1');

INSERT INTO vader."UserRoles" ("UserId", "RoleId")
VALUES (1, 1), (2, 2)