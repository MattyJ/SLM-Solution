DECLARE @Count INT

SET @Count = (SELECT COUNT(*) FROM [SecureApplications])

IF @Count = 0
	BEGIN

SET IDENTITY_INSERT [SecureApplications] ON

INSERT INTO [SecureApplications] ([Id], [Name]) VALUES (1, 'Fujitsu Role Administration')
INSERT INTO [SecureApplications] ([Id], [Name]) VALUES (2, 'Design Generator')
INSERT INTO [SecureApplications] ([Id], [Name]) VALUES (3, 'PALM')
INSERT INTO [SecureApplications] ([Id], [Name]) VALUES (4, 'SLM')

SET IDENTITY_INSERT [SecureApplications] OFF

SET IDENTITY_INSERT [Roles] ON

INSERT INTO [Roles] ([Id], [Name], [SecureApplicationId]) VALUES (1, 'Fujitsu Administration', 1 )
INSERT INTO [Roles] ([Id], [Name], [SecureApplicationId]) VALUES (2, 'ProjectArchitect', 2 )
INSERT INTO [Roles] ([Id], [Name], [SecureApplicationId]) VALUES (3, 'DesignStakeholder', 2 )
INSERT INTO [Roles] ([Id], [Name], [SecureApplicationId]) VALUES (4, 'ReferenceDataOwner', 2 )
INSERT INTO [Roles] ([Id], [Name], [SecureApplicationId]) VALUES (5, 'DesignAdministrator', 2 )
INSERT INTO [Roles] ([Id], [Name], [SecureApplicationId]) VALUES (6, 'Administrator', 3 )
INSERT INTO [Roles] ([Id], [Name], [SecureApplicationId]) VALUES (7, 'ProjectManager', 3 )
INSERT INTO [Roles] ([Id], [Name], [SecureApplicationId]) VALUES (8, 'Viewer', 3 )
INSERT INTO [Roles] ([Id], [Name], [SecureApplicationId]) VALUES (9, 'Administrator', 4 )
INSERT INTO [Roles] ([Id], [Name], [SecureApplicationId]) VALUES (10, 'Manager', 4 )
INSERT INTO [Roles] ([Id], [Name], [SecureApplicationId]) VALUES (11, 'Viewer', 4 )

SET IDENTITY_INSERT [Roles] OFF

SET IDENTITY_INSERT [Users] ON

INSERT INTO [Users] ([Id], [UserId], [DisplayName], [EmailAddress]) VALUES (1, 'DuerdenP', 'Peter Duerden', 'peter.duerden@uk.fujitsu.com')
INSERT INTO [Users] ([Id], [UserId], [DisplayName], [EmailAddress]) VALUES (2, 'MorganSJ', 'Stephen Morgan', 'stephen.morgan@uk.fujitsu.com')
INSERT INTO [Users] ([Id], [UserId], [DisplayName], [EmailAddress]) VALUES (3, 'JordanM', 'Matthew Jordan', 'matthew.jordan@uk.fujitsu.com')
INSERT INTO [Users] ([Id], [UserId], [DisplayName], [EmailAddress]) VALUES (4, 'BanisterR', 'Ray Banister', 'ray.banister@uk.fujitsu.com')
INSERT INTO [Users] ([Id], [UserId], [DisplayName], [EmailAddress]) VALUES (5, 'MorganK', 'Kate Morgan', 'kate.morgan@uk.fujitsu.com')
INSERT INTO [Users] ([Id], [UserId], [DisplayName], [EmailAddress]) VALUES (6, 'LockleyM', 'Matt Lockley', 'matt.lockley@uk.fujitsu.com')
INSERT INTO [Users] ([Id], [UserId], [DisplayName], [EmailAddress]) VALUES (7, 'SahotaA', 'Aman Sahota', 'amandip.sahota@uk.fujitsu.com')
INSERT INTO [Users] ([Id], [UserId], [DisplayName], [EmailAddress]) VALUES (8, 'ClarkI', 'Ian Clark', 'ian.clark@uk.fujitsu.com')
INSERT INTO [Users] ([Id], [UserId], [DisplayName], [EmailAddress]) VALUES (9, 'LocksmithD', 'Daniel Locksmith', 'daniel.locksmith@uk.fujitsu.com')
INSERT INTO [Users] ([Id], [UserId], [DisplayName], [EmailAddress]) VALUES (10, 'BleadsG', 'Gary Bleads', 'gary.bleads@uk.fujitsu.com')
INSERT INTO [Users] ([Id], [UserId], [DisplayName], [EmailAddress]) VALUES (11, 'HartM', 'Mark Hart', 'mark.hart@uk.fujitsu.com')

SET IDENTITY_INSERT [Users] OFF

SET IDENTITY_INSERT [UserRoles] ON

INSERT INTO [UserRoles] ([Id] ,[RoleId] ,[UserId]) VALUES (1, 1, 1)
INSERT INTO [UserRoles] ([Id] ,[RoleId] ,[UserId]) VALUES (2, 1, 2)
INSERT INTO [UserRoles] ([Id] ,[RoleId] ,[UserId]) VALUES (3, 1, 3)
INSERT INTO [UserRoles] ([Id] ,[RoleId] ,[UserId]) VALUES (4, 5, 7)
INSERT INTO [UserRoles] ([Id] ,[RoleId] ,[UserId]) VALUES (5, 2, 1)
INSERT INTO [UserRoles] ([Id] ,[RoleId] ,[UserId]) VALUES (6, 2, 2)
INSERT INTO [UserRoles] ([Id] ,[RoleId] ,[UserId]) VALUES (7, 2, 3)
INSERT INTO [UserRoles] ([Id] ,[RoleId] ,[UserId]) VALUES (8, 2, 4)
INSERT INTO [UserRoles] ([Id] ,[RoleId] ,[UserId]) VALUES (9, 3, 1)
INSERT INTO [UserRoles] ([Id] ,[RoleId] ,[UserId]) VALUES (10, 3, 2)
INSERT INTO [UserRoles] ([Id] ,[RoleId] ,[UserId]) VALUES (11, 3, 3)
INSERT INTO [UserRoles] ([Id] ,[RoleId] ,[UserId]) VALUES (12, 3, 4)
INSERT INTO [UserRoles] ([Id] ,[RoleId] ,[UserId]) VALUES (13, 4, 1)
INSERT INTO [UserRoles] ([Id] ,[RoleId] ,[UserId]) VALUES (14, 4, 2)
INSERT INTO [UserRoles] ([Id] ,[RoleId] ,[UserId]) VALUES (15, 4, 3)
INSERT INTO [UserRoles] ([Id] ,[RoleId] ,[UserId]) VALUES (16, 4, 4)
INSERT INTO [UserRoles] ([Id] ,[RoleId] ,[UserId]) VALUES (17, 5, 1)
INSERT INTO [UserRoles] ([Id] ,[RoleId] ,[UserId]) VALUES (18, 5, 2)
INSERT INTO [UserRoles] ([Id] ,[RoleId] ,[UserId]) VALUES (19, 5, 3)
INSERT INTO [UserRoles] ([Id] ,[RoleId] ,[UserId]) VALUES (20, 5, 4)
INSERT INTO [UserRoles] ([Id] ,[RoleId] ,[UserId]) VALUES (21, 2, 5)
INSERT INTO [UserRoles] ([Id] ,[RoleId] ,[UserId]) VALUES (22, 3, 5)
INSERT INTO [UserRoles] ([Id] ,[RoleId] ,[UserId]) VALUES (23, 4, 5)
INSERT INTO [UserRoles] ([Id] ,[RoleId] ,[UserId]) VALUES (24, 5, 5)
INSERT INTO [UserRoles] ([Id] ,[RoleId] ,[UserId]) VALUES (25, 2, 6)
INSERT INTO [UserRoles] ([Id] ,[RoleId] ,[UserId]) VALUES (26, 3, 6)
INSERT INTO [UserRoles] ([Id] ,[RoleId] ,[UserId]) VALUES (27, 4, 6)
INSERT INTO [UserRoles] ([Id] ,[RoleId] ,[UserId]) VALUES (28, 5, 6)
INSERT INTO [UserRoles] ([Id] ,[RoleId] ,[UserId]) VALUES (29, 2, 7)
INSERT INTO [UserRoles] ([Id] ,[RoleId] ,[UserId]) VALUES (30, 3, 7)
INSERT INTO [UserRoles] ([Id] ,[RoleId] ,[UserId]) VALUES (31, 4, 7)
INSERT INTO [UserRoles] ([Id] ,[RoleId] ,[UserId]) VALUES (32, 6, 3)
INSERT INTO [UserRoles] ([Id] ,[RoleId] ,[UserId]) VALUES (33, 7, 3)
INSERT INTO [UserRoles] ([Id] ,[RoleId] ,[UserId]) VALUES (34, 8, 3)
INSERT INTO [UserRoles] ([Id] ,[RoleId] ,[UserId]) VALUES (35, 6, 10)
INSERT INTO [UserRoles] ([Id] ,[RoleId] ,[UserId]) VALUES (36, 7, 10)
INSERT INTO [UserRoles] ([Id] ,[RoleId] ,[UserId]) VALUES (37, 8, 10)
INSERT INTO [UserRoles] ([Id] ,[RoleId] ,[UserId]) VALUES (38, 6, 11)
INSERT INTO [UserRoles] ([Id] ,[RoleId] ,[UserId]) VALUES (39, 7, 11)
INSERT INTO [UserRoles] ([Id] ,[RoleId] ,[UserId]) VALUES (40, 8, 11)
INSERT INTO [UserRoles] ([Id] ,[RoleId] ,[UserId]) VALUES (41, 9, 3)
INSERT INTO [UserRoles] ([Id] ,[RoleId] ,[UserId]) VALUES (42, 10, 3)
INSERT INTO [UserRoles] ([Id] ,[RoleId] ,[UserId]) VALUES (43, 11, 3)

SET IDENTITY_INSERT [UserRoles] OFF

END