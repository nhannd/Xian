create table Enterprise1.dbo.AuditLog_ (OID_ UNIQUEIDENTIFIER not null, Version_ INT not null, TimeStamp_ DATETIME not null, HostName_ NVARCHAR(255) null, Application_ NVARCHAR(255) null, Category_ NVARCHAR(255) not null, User_ NVARCHAR(255) null, Operation_ NVARCHAR(1000) null, Details_ NTEXT null, primary key (OID_))
create table Enterprise1.dbo.AuthorityGroup_ (OID_ UNIQUEIDENTIFIER not null, Version_ INT not null, Name_ NVARCHAR(100) not null, primary key (OID_))
alter table Enterprise1.dbo.AuthorityGroup_ add constraint UQ__AuthorityGroup____Name_ unique nonclustered (Name_)
create table Enterprise1.dbo.AuthorityGroupUser_ (UserOID_ UNIQUEIDENTIFIER not null, AuthorityGroupOID_ UNIQUEIDENTIFIER not null, primary key (AuthorityGroupOID_, UserOID_))
create table Enterprise1.dbo.AuthorityToken_ (OID_ UNIQUEIDENTIFIER not null, Version_ INT not null, Name_ NVARCHAR(400) not null, Description_ NVARCHAR(1024) null, primary key (OID_))
alter table Enterprise1.dbo.AuthorityToken_ add constraint UQ__AuthorityToken____Name_ unique nonclustered (Name_)
create table Enterprise1.dbo.AuthorityTokenAuthorityGroup_ (AuthorityGroupOID_ UNIQUEIDENTIFIER not null, AuthorityTokenOID_ UNIQUEIDENTIFIER not null, primary key (AuthorityGroupOID_, AuthorityTokenOID_))
create table Enterprise1.dbo.ConfigurationDocument_ (OID_ UNIQUEIDENTIFIER not null, Version_ INT not null, DocumentName_ NVARCHAR(255) not null, DocumentVersionString_ NVARCHAR(30) not null, User_ NVARCHAR(50) null, InstanceKey_ NVARCHAR(100) null, primary key (OID_))
create table Enterprise1.dbo.ConfigurationDocumentBody_ (DocumentOID_ UNIQUEIDENTIFIER not null, Version_ INT not null, DocumentText_ NTEXT null, primary key (DocumentOID_))
create table Enterprise1.dbo.ConfigurationSettingsGroup_ (OID_ UNIQUEIDENTIFIER not null, Version_ INT not null, Name_ NVARCHAR(255) not null, VersionString_ NVARCHAR(30) not null, Description_ NVARCHAR(1000) null, AssemblyQualifiedTypeName_ NVARCHAR(500) null, HasUserScopedSettings_ BIT not null, primary key (OID_))
create table Enterprise1.dbo.ConfigurationSettingsProperty_ (ConfigurationSettingsGroupOID_ UNIQUEIDENTIFIER not null, Name_ NVARCHAR(255) not null, TypeName_ NVARCHAR(255) not null, Scope_ NVARCHAR(255) not null, Description_ NVARCHAR(1000) null, DefaultValue_ NTEXT null, OID_ UNIQUEIDENTIFIER not null, primary key (OID_))
create table Enterprise1.dbo.ExceptionLog_ (OID_ UNIQUEIDENTIFIER not null, Version_ INT not null, TimeStamp_ DATETIME not null, HostName_ NVARCHAR(255) null, Application_ NVARCHAR(255) null, User_ NVARCHAR(255) null, Operation_ NVARCHAR(1000) null, ExceptionClass_ NVARCHAR(1000) null, Message_ NVARCHAR(4000) null, Details_ NTEXT null, AssemblyName_ NVARCHAR(500) null, AssemblyLocation_ NVARCHAR(500) null, primary key (OID_))
create table Enterprise1.dbo.User_ (OID_ UNIQUEIDENTIFIER not null, Version_ INT not null, UserName_ NVARCHAR(255) not null, PasswordSalt_ NVARCHAR(255) not null, PasswordSaltedHash_ NVARCHAR(255) not null, PasswordExpiryDate_ DATETIME null, DisplayName_ NVARCHAR(255) null, ValidFrom_ DATETIME null, ValidUntil_ DATETIME null, Enabled_ BIT not null, CreationTime_ DATETIME not null, LastLoginTime_ DATETIME null, primary key (OID_))
alter table Enterprise1.dbo.User_ add constraint UQ__User____UserName_ unique nonclustered (UserName_)
create table Enterprise1.dbo.UserSession_ (OID_ UNIQUEIDENTIFIER not null, UserOID_ UNIQUEIDENTIFIER not null, HostName_ NVARCHAR(255) null, Application_ NVARCHAR(255) null, SessionId_ NVARCHAR(255) not null, CreationTime_ DATETIME not null, ExpiryTime_ DATETIME not null, primary key (OID_))
alter table Enterprise1.dbo.UserSession_ add constraint UQ__UserSession____SessionId_ unique nonclustered (SessionId_)
alter table Enterprise1.dbo.ConfigurationDocument_ add constraint DocumentKey  unique (DocumentName_, DocumentVersionString_, User_, InstanceKey_)
alter table Enterprise1.dbo.ConfigurationSettingsGroup_ add constraint SettingsGroupKey  unique (Name_, VersionString_)
alter table Enterprise1.dbo.AuthorityGroupUser_  add constraint FKA6064D2AD2FAEF98 foreign key (UserOID_) references Enterprise1.dbo.User_
alter table Enterprise1.dbo.AuthorityGroupUser_  add constraint FKA6064D2A2214ACD3 foreign key (AuthorityGroupOID_) references Enterprise1.dbo.AuthorityGroup_
alter table Enterprise1.dbo.AuthorityTokenAuthorityGroup_  add constraint FK655B48B3635685CB foreign key (AuthorityTokenOID_) references Enterprise1.dbo.AuthorityToken_
alter table Enterprise1.dbo.AuthorityTokenAuthorityGroup_  add constraint FK655B48B32214ACD3 foreign key (AuthorityGroupOID_) references Enterprise1.dbo.AuthorityGroup_
alter table Enterprise1.dbo.ConfigurationDocumentBody_  add constraint FK955BF26A237D1C42 foreign key (DocumentOID_) references Enterprise1.dbo.ConfigurationDocument_
alter table Enterprise1.dbo.ConfigurationSettingsProperty_  add constraint FKEDA391995E715DEB foreign key (ConfigurationSettingsGroupOID_) references Enterprise1.dbo.ConfigurationSettingsGroup_
alter table Enterprise1.dbo.UserSession_  add constraint FKC86746FCD2FAEF98 foreign key (UserOID_) references Enterprise1.dbo.User_
create index IX_UserOID_AuthorityGroupOID_ on Enterprise1.dbo.AuthorityGroupUser_ (UserOID_, AuthorityGroupOID_)
create index IX_AuthorityGroupOID_UserOID_ on Enterprise1.dbo.AuthorityGroupUser_ (AuthorityGroupOID_, UserOID_)
create index IX_AuthorityTokenOID_AuthorityGroupOID_ on Enterprise1.dbo.AuthorityTokenAuthorityGroup_ (AuthorityTokenOID_, AuthorityGroupOID_)
create index IX_AuthorityGroupOID_AuthorityTokenOID_ on Enterprise1.dbo.AuthorityTokenAuthorityGroup_ (AuthorityGroupOID_, AuthorityTokenOID_)
create index IX_ConfigurationSettingsGroupOID_ on Enterprise1.dbo.ConfigurationSettingsProperty_ (ConfigurationSettingsGroupOID_)
create index IX_UserOID_ on Enterprise1.dbo.UserSession_ (UserOID_)
