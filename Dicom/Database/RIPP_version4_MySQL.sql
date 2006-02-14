-- Create ripp_version4 database.
create database ripp_version4;

-- Create new table Tag.
-- Tag : Table of Tag
-- 	TagId : Tag is identified by TagId
-- 	GroupElement : Tag has GroupElement
-- 	VR : Tag has VR
-- 	TagValue : Tag has TagValue
-- 	TypeCode : Tag is of Type
-- 	SequenceTagId : Tag has Tag
-- 	ValueLength : Tag has ValueLength
create table Tag (
  TagId integer unsigned not null auto_increment unique key,
  GroupElement integer not null,
  VR char(2) not null,
  TagValue varchar(255) null,
  TypeCode char(3) not null check (TypeCode in ('PAT', 'STU', 'SER', 'SOP')),
  SequenceTagId integer unsigned null check (SequenceTagId <> TagId),
  ValueLength integer not null,
  constraint Tag_PK primary key (TagId)
);

-- Create new table StudyTag.
-- StudyTag : Table of StudyTag
-- 	TagId : Tag is a StudyTag
-- 		Tag is a StudyTag
-- 		Role two (TagId) of fact: Tag is identified by {TagId}.
-- 	StudyUid : Study has StudyTag
create table StudyTag (
  ReferencedTagId integer unsigned not null unique key,
  ReferencedStudyUid varchar(64),
  constraint StudyTag_PK primary key (ReferencedTagId)
);

-- Create new table Study.
-- Study : Table of Study
-- 	StudyUid : Study is identified by Uid
-- 	PatientId : Patient has Study
create table Study (
	StudyUid varchar(64) not null unique key,
	PatientId varchar(64) not null,
  constraint Study_PK primary key (StudyUid)
);

-- Create new table SopTag.
-- SopTag : Table of SopTag
-- 	TagId : Tag is a SopTag
-- 		Tag is a SopTag
-- 		Role two (TagId) of fact: Tag is identified by {TagId}.
-- 	SopInstanceUid : SopInstance has SopTag
create table SopTag (
	ReferencedTagId integer unsigned not null unique key,
	ReferencedSopInstanceUid VARCHAR(64) null,
  constraint SopTag_PK primary key (ReferencedTagId)
);

-- Create new table SopInstance.
-- SopInstance : Table of SopInstance
-- 	SopInstanceUid : SopInstance is identified by Uid
-- 	SeriesUid : Series has SopInstance
create table SopInstance (
	SopInstanceUid varchar(64) not null unique key,
	ReferencedSeriesUid VARCHAR(64) not null,
  constraint SopInstance_PK primary key (SopInstanceUid)
);

-- Create new table SeriesTag.
-- SeriesTag : Table of SeriesTag
-- 	TagId : Tag is a SeriesTag
-- 		Tag is a SeriesTag
-- 		Role two (TagId) of fact: Tag is identified by {TagId}.
-- 	SeriesUid : Series has SeriesTag
create table SeriesTag (
	ReferencedTagId integer unsigned not null unique key,
	ReferencedSeriesUid varchar(64) null,
  constraint SeriesTag_PK primary key (ReferencedTagId)
);

-- Create new table Series.
-- Series : Table of Series
-- 	SeriesUid : Series is identified by Uid
-- 	StudyUid : Study has Series
create table Series (
	SeriesUid varchar(64) not null unique key,
	ReferencedStudyUid varchar(64) not null,
  constraint Series_PK primary key (SeriesUid)
);

-- Create new table PatientTag.
-- PatientTag : Table of PatientTag
-- 	TagId : Tag is a PatientTag
-- 		Tag is a PatientTag
-- 		Role two (TagId) of fact: Tag is identified by {TagId}.
-- 	PatientId : Patient has PatientTag
create table PatientTag (
	ReferencedTagId integer unsigned not null unique key,
	PatientId varchar(64) not null,
  constraint PatientTag_PK primary key (ReferencedTagId)
);

-- Add the remaining keys, constraints and indexes for the table Tag.
alter table Tag
  add constraint Tag_UC1
  unique (SequenceTagId);

-- Add foreign key constraints to table Tag.
alter table Tag
  add constraint Tag_Tag_FK1
  foreign key (SequenceTagId)
  references Tag (TagId)
;

  -- Add foreign key constraints to table StudyTag.
alter table StudyTag
	add constraint Study_StudyTag_FK1
  foreign key (ReferencedStudyUid)
	references Study (StudyUid)
;

alter table StudyTag
	add constraint Tag_StudyTag_FK1
  foreign key (ReferencedTagId)
	references Tag (TagId)
;

-- Add foreign key constraints to table SopTag.
alter table SopTag
	add constraint SopInstance_SopTag_FK1
  foreign key (ReferencedSopInstanceUid)
  references SopInstance (SopInstanceUid)
;

alter table SopTag
	add constraint Tag_SopTag_FK1
  foreign key (ReferencedTagId)
	references Tag (TagId)
;

-- Add foreign key constraints to table SopInstance.
alter table SopInstance
	add constraint Series_SopInstance_FK1
  foreign key (ReferencedSeriesUid)
  references Series (SeriesUid)
;

-- Add foreign key constraints to table SeriesTag.
alter table SeriesTag
	add constraint Series_SeriesTag_FK1
  foreign key (ReferencedSeriesUid)
  references Series (SeriesUid)
;

alter table SeriesTag
	add constraint Tag_SeriesTag_FK1
  foreign key (ReferencedTagId)
  references Tag (TagId)
;

-- Add foreign key constraints to table Series.
alter table Series
	add constraint Study_Series_FK1
  foreign key (ReferencedStudyUid)
  references Study (StudyUid)
;

-- Add foreign key constraints to table PatientTag.
alter table PatientTag
	add constraint Tag_PatientTag_FK1
  foreign key (ReferencedTagId)
  references Tag (TagId)
;

-- Create procedure/function PatientTag_equal1.
-- -- The constraint:
-- -- Implied equality constraint.
-- -- is enforced by the following DDL.
-- -- Warning! The target DBMS does not support create procedure or create function.
-- -- No code will be created during the DDL generation phase.
-- -- The constraint:
-- -- Implied equality constraint.
-- -- is enforced by the following DDL.
-- --     not exists (select * from Study X where
-- --                 not exists (select * from PatientTag Y
-- --                             where X.PatientId = Y.PatientId)) and
-- --     not exists (select * from PatientTag X where
-- --                 not exists (select * from Study Y
-- --                             where Y.PatientId = X.PatientId))