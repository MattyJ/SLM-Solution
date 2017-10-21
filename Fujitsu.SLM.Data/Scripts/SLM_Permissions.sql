﻿USE [master]
GO
CREATE LOGIN [g03\SA-FUJSQLSLMDB] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO

USE [SLM]
GO
CREATE USER [g03\SA-FUJSQLSLMDB] FOR LOGIN [g03\SA-FUJSQLSLMDB] WITH DEFAULT_SCHEMA=[dbo]
EXEC sp_addrolemember N'db_datareader', N'g03\SA-FUJSQLSLMDB'
EXEC sp_addrolemember N'db_datawriter', N'g03\SA-FUJSQLSLMDB'
GO
GRANT EXECUTE ON [spDeleteCustomer] TO "g03\SA-FUJSQLSLMDB"
GRANT EXECUTE ON [spDeleteResolver] TO "g03\SA-FUJSQLSLMDB"
GRANT EXECUTE ON [spDeleteServiceComponent] TO "g03\SA-FUJSQLSLMDB"
GRANT EXECUTE ON [spDeleteServiceDesk] TO "g03\SA-FUJSQLSLMDB"
GRANT EXECUTE ON [spDeleteServiceDeskContents] TO "g03\SA-FUJSQLSLMDB"
GRANT EXECUTE ON [spDeleteServiceDomain] TO "g03\SA-FUJSQLSLMDB"
GRANT EXECUTE ON [spDeleteServiceFunction] TO "g03\SA-FUJSQLSLMDB"
GO