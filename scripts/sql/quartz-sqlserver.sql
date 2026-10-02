-- Quartz.NET 3.18.0 SQL Server / Azure SQL schema.
-- Source: https://github.com/quartznet/quartznet/blob/v3.18.0/database/tables/tables_sqlServer.sql
-- SPDX-License-Identifier: Apache-2.0
-- Copyright 2007 Marko Lahma. Full upstream license is included at the end.
--
-- Modified for Journey_of_faith: initialize only, reject an existing dbo.QRTZ_
-- schema, use the caller's selected database, and create everything atomically.
-- Table, constraint, and index definitions are from the pinned upstream source.
--
-- Run this entire script once in the database configured for Quartz, using
-- SSMS or sqlcmd with an explicit database selection. Requires CREATE TABLE
-- permission and ALTER permission on schema dbo. No application tables change.
-- Stop scheduler instances before initialization. This is not an upgrade script.

SET NOCOUNT ON;
SET XACT_ABORT ON;
SET ANSI_NULL_DFLT_ON ON;

IF @@TRANCOUNT <> 0
BEGIN
    THROW 51000, 'Run Quartz initialization outside an existing transaction.', 1;
END;

BEGIN TRY
    BEGIN TRANSACTION;

    IF EXISTS
    (
        SELECT 1
        FROM sys.tables AS tables
        INNER JOIN sys.schemas AS schemas ON schemas.schema_id = tables.schema_id
        WHERE schemas.name = N'dbo'
          AND tables.name LIKE N'QRTZ[_]%'
    )
    BEGIN
        THROW 51001, 'Quartz initialization refused: dbo.QRTZ_ tables already exist. Use a version-specific migration for an existing Quartz schema.', 1;
    END;

    CREATE TABLE [dbo].[QRTZ_CALENDARS] (
      [SCHED_NAME] nvarchar(120) NOT NULL,
      [CALENDAR_NAME] nvarchar(200) NOT NULL,
      [CALENDAR] varbinary(max) NOT NULL
    );

    CREATE TABLE [dbo].[QRTZ_CRON_TRIGGERS] (
      [SCHED_NAME] nvarchar(120) NOT NULL,
      [TRIGGER_NAME] nvarchar(150) NOT NULL,
      [TRIGGER_GROUP] nvarchar(150) NOT NULL,
      [CRON_EXPRESSION] nvarchar(120) NOT NULL,
      [TIME_ZONE_ID] nvarchar(80)
    );

    CREATE TABLE [dbo].[QRTZ_FIRED_TRIGGERS] (
      [SCHED_NAME] nvarchar(120) NOT NULL,
      [ENTRY_ID] nvarchar(140) NOT NULL,
      [TRIGGER_NAME] nvarchar(150) NOT NULL,
      [TRIGGER_GROUP] nvarchar(150) NOT NULL,
      [INSTANCE_NAME] nvarchar(200) NOT NULL,
      [FIRED_TIME] bigint NOT NULL,
      [SCHED_TIME] bigint NOT NULL,
      [PRIORITY] int NOT NULL,
      [STATE] nvarchar(16) NOT NULL,
      [JOB_NAME] nvarchar(150) NULL,
      [JOB_GROUP] nvarchar(150) NULL,
      [IS_NONCONCURRENT] bit NULL,
      [REQUESTS_RECOVERY] bit NULL,
      [EXECUTION_GROUP] nvarchar(200) NULL
    );

    CREATE TABLE [dbo].[QRTZ_PAUSED_TRIGGER_GRPS] (
      [SCHED_NAME] nvarchar(120) NOT NULL,
      [TRIGGER_GROUP] nvarchar(150) NOT NULL
    );

    CREATE TABLE [dbo].[QRTZ_SCHEDULER_STATE] (
      [SCHED_NAME] nvarchar(120) NOT NULL,
      [INSTANCE_NAME] nvarchar(200) NOT NULL,
      [LAST_CHECKIN_TIME] bigint NOT NULL,
      [CHECKIN_INTERVAL] bigint NOT NULL
    );

    CREATE TABLE [dbo].[QRTZ_LOCKS] (
      [SCHED_NAME] nvarchar(120) NOT NULL,
      [LOCK_NAME] nvarchar(40) NOT NULL
    );

    CREATE TABLE [dbo].[QRTZ_JOB_DETAILS] (
      [SCHED_NAME] nvarchar(120) NOT NULL,
      [JOB_NAME] nvarchar(150) NOT NULL,
      [JOB_GROUP] nvarchar(150) NOT NULL,
      [DESCRIPTION] nvarchar(250) NULL,
      [JOB_CLASS_NAME] nvarchar(250) NOT NULL,
      [IS_DURABLE] bit NOT NULL,
      [IS_NONCONCURRENT] bit NOT NULL,
      [IS_UPDATE_DATA] bit NOT NULL,
      [REQUESTS_RECOVERY] bit NOT NULL,
      [JOB_DATA] varbinary(max) NULL
    );

    CREATE TABLE [dbo].[QRTZ_SIMPLE_TRIGGERS] (
      [SCHED_NAME] nvarchar(120) NOT NULL,
      [TRIGGER_NAME] nvarchar(150) NOT NULL,
      [TRIGGER_GROUP] nvarchar(150) NOT NULL,
      [REPEAT_COUNT] int NOT NULL,
      [REPEAT_INTERVAL] bigint NOT NULL,
      [TIMES_TRIGGERED] int NOT NULL
    );

    CREATE TABLE [dbo].[QRTZ_SIMPROP_TRIGGERS] (
      [SCHED_NAME] nvarchar(120) NOT NULL,
      [TRIGGER_NAME] nvarchar(150) NOT NULL,
      [TRIGGER_GROUP] nvarchar(150) NOT NULL,
      [STR_PROP_1] nvarchar(512) NULL,
      [STR_PROP_2] nvarchar(512) NULL,
      [STR_PROP_3] nvarchar(512) NULL,
      [INT_PROP_1] int NULL,
      [INT_PROP_2] int NULL,
      [LONG_PROP_1] bigint NULL,
      [LONG_PROP_2] bigint NULL,
      [DEC_PROP_1] numeric(13,4) NULL,
      [DEC_PROP_2] numeric(13,4) NULL,
      [BOOL_PROP_1] bit NULL,
      [BOOL_PROP_2] bit NULL,
      [TIME_ZONE_ID] nvarchar(80) NULL
    );

    CREATE TABLE [dbo].[QRTZ_BLOB_TRIGGERS] (
      [SCHED_NAME] nvarchar(120) NOT NULL,
      [TRIGGER_NAME] nvarchar(150) NOT NULL,
      [TRIGGER_GROUP] nvarchar(150) NOT NULL,
      [BLOB_DATA] varbinary(max) NULL
    );

    CREATE TABLE [dbo].[QRTZ_TRIGGERS] (
      [SCHED_NAME] nvarchar(120) NOT NULL,
      [TRIGGER_NAME] nvarchar(150) NOT NULL,
      [TRIGGER_GROUP] nvarchar(150) NOT NULL,
      [JOB_NAME] nvarchar(150) NOT NULL,
      [JOB_GROUP] nvarchar(150) NOT NULL,
      [DESCRIPTION] nvarchar(250) NULL,
      [NEXT_FIRE_TIME] bigint NULL,
      [PREV_FIRE_TIME] bigint NULL,
      [PRIORITY] int NULL,
      [TRIGGER_STATE] nvarchar(16) NOT NULL,
      [TRIGGER_TYPE] nvarchar(8) NOT NULL,
      [START_TIME] bigint NOT NULL,
      [END_TIME] bigint NULL,
      [CALENDAR_NAME] nvarchar(200) NULL,
      [MISFIRE_INSTR] int NULL,
      [MISFIRE_ORIG_FIRE_TIME] bigint NULL,
      [EXECUTION_GROUP] nvarchar(200) NULL,
      [JOB_DATA] varbinary(max) NULL
    );

    ALTER TABLE [dbo].[QRTZ_CALENDARS] WITH NOCHECK ADD
      CONSTRAINT [PK_QRTZ_CALENDARS] PRIMARY KEY  CLUSTERED
      (
        [SCHED_NAME],
        [CALENDAR_NAME]
      );

    ALTER TABLE [dbo].[QRTZ_CRON_TRIGGERS] WITH NOCHECK ADD
      CONSTRAINT [PK_QRTZ_CRON_TRIGGERS] PRIMARY KEY  CLUSTERED
      (
        [SCHED_NAME],
        [TRIGGER_NAME],
        [TRIGGER_GROUP]
      );

    ALTER TABLE [dbo].[QRTZ_FIRED_TRIGGERS] WITH NOCHECK ADD
      CONSTRAINT [PK_QRTZ_FIRED_TRIGGERS] PRIMARY KEY  CLUSTERED
      (
        [SCHED_NAME],
        [ENTRY_ID]
      );

    ALTER TABLE [dbo].[QRTZ_PAUSED_TRIGGER_GRPS] WITH NOCHECK ADD
      CONSTRAINT [PK_QRTZ_PAUSED_TRIGGER_GRPS] PRIMARY KEY  CLUSTERED
      (
        [SCHED_NAME],
        [TRIGGER_GROUP]
      );

    ALTER TABLE [dbo].[QRTZ_SCHEDULER_STATE] WITH NOCHECK ADD
      CONSTRAINT [PK_QRTZ_SCHEDULER_STATE] PRIMARY KEY  CLUSTERED
      (
        [SCHED_NAME],
        [INSTANCE_NAME]
      );

    ALTER TABLE [dbo].[QRTZ_LOCKS] WITH NOCHECK ADD
      CONSTRAINT [PK_QRTZ_LOCKS] PRIMARY KEY  CLUSTERED
      (
        [SCHED_NAME],
        [LOCK_NAME]
      );

    ALTER TABLE [dbo].[QRTZ_JOB_DETAILS] WITH NOCHECK ADD
      CONSTRAINT [PK_QRTZ_JOB_DETAILS] PRIMARY KEY  CLUSTERED
      (
        [SCHED_NAME],
        [JOB_NAME],
        [JOB_GROUP]
      );

    ALTER TABLE [dbo].[QRTZ_SIMPLE_TRIGGERS] WITH NOCHECK ADD
      CONSTRAINT [PK_QRTZ_SIMPLE_TRIGGERS] PRIMARY KEY  CLUSTERED
      (
        [SCHED_NAME],
        [TRIGGER_NAME],
        [TRIGGER_GROUP]
      );

    ALTER TABLE [dbo].[QRTZ_SIMPROP_TRIGGERS] WITH NOCHECK ADD
      CONSTRAINT [PK_QRTZ_SIMPROP_TRIGGERS] PRIMARY KEY  CLUSTERED
      (
        [SCHED_NAME],
        [TRIGGER_NAME],
        [TRIGGER_GROUP]
      );

    ALTER TABLE [dbo].[QRTZ_TRIGGERS] WITH NOCHECK ADD
      CONSTRAINT [PK_QRTZ_TRIGGERS] PRIMARY KEY  CLUSTERED
      (
        [SCHED_NAME],
        [TRIGGER_NAME],
        [TRIGGER_GROUP]
      );

    ALTER TABLE [dbo].[QRTZ_BLOB_TRIGGERS] WITH NOCHECK ADD
      CONSTRAINT [PK_QRTZ_BLOB_TRIGGERS] PRIMARY KEY  CLUSTERED
      (
        [SCHED_NAME],
        [TRIGGER_NAME],
        [TRIGGER_GROUP]
      );

    ALTER TABLE [dbo].[QRTZ_CRON_TRIGGERS] ADD
      CONSTRAINT [FK_QRTZ_CRON_TRIGGERS_QRTZ_TRIGGERS] FOREIGN KEY
      (
        [SCHED_NAME],
        [TRIGGER_NAME],
        [TRIGGER_GROUP]
      ) REFERENCES [dbo].[QRTZ_TRIGGERS] (
        [SCHED_NAME],
        [TRIGGER_NAME],
        [TRIGGER_GROUP]
      ) ON DELETE CASCADE;

    ALTER TABLE [dbo].[QRTZ_SIMPLE_TRIGGERS] ADD
      CONSTRAINT [FK_QRTZ_SIMPLE_TRIGGERS_QRTZ_TRIGGERS] FOREIGN KEY
      (
        [SCHED_NAME],
        [TRIGGER_NAME],
        [TRIGGER_GROUP]
      ) REFERENCES [dbo].[QRTZ_TRIGGERS] (
        [SCHED_NAME],
        [TRIGGER_NAME],
        [TRIGGER_GROUP]
      ) ON DELETE CASCADE;

    ALTER TABLE [dbo].[QRTZ_SIMPROP_TRIGGERS] ADD
      CONSTRAINT [FK_QRTZ_SIMPROP_TRIGGERS_QRTZ_TRIGGERS] FOREIGN KEY
      (
    	[SCHED_NAME],
        [TRIGGER_NAME],
        [TRIGGER_GROUP]
      ) REFERENCES [dbo].[QRTZ_TRIGGERS] (
        [SCHED_NAME],
        [TRIGGER_NAME],
        [TRIGGER_GROUP]
      ) ON DELETE CASCADE;

    ALTER TABLE [dbo].[QRTZ_TRIGGERS] ADD
      CONSTRAINT [FK_QRTZ_TRIGGERS_QRTZ_JOB_DETAILS] FOREIGN KEY
      (
        [SCHED_NAME],
        [JOB_NAME],
        [JOB_GROUP]
      ) REFERENCES [dbo].[QRTZ_JOB_DETAILS] (
        [SCHED_NAME],
        [JOB_NAME],
        [JOB_GROUP]
      );

    -- Create indexes
    CREATE INDEX [IDX_QRTZ_T_G_J]                 ON [dbo].[QRTZ_TRIGGERS](SCHED_NAME, JOB_GROUP, JOB_NAME);
    CREATE INDEX [IDX_QRTZ_T_C]                   ON [dbo].[QRTZ_TRIGGERS](SCHED_NAME, CALENDAR_NAME);

    CREATE INDEX [IDX_QRTZ_T_N_G_STATE]           ON [dbo].[QRTZ_TRIGGERS](SCHED_NAME, TRIGGER_GROUP, TRIGGER_STATE);
    CREATE INDEX [IDX_QRTZ_T_STATE]               ON [dbo].[QRTZ_TRIGGERS](SCHED_NAME, TRIGGER_STATE);
    CREATE INDEX [IDX_QRTZ_T_N_STATE]             ON [dbo].[QRTZ_TRIGGERS](SCHED_NAME, TRIGGER_NAME, TRIGGER_GROUP, TRIGGER_STATE);
    CREATE INDEX [IDX_QRTZ_T_NEXT_FIRE_TIME]      ON [dbo].[QRTZ_TRIGGERS](SCHED_NAME, NEXT_FIRE_TIME);
    CREATE INDEX [IDX_QRTZ_T_NFT_ST]              ON [dbo].[QRTZ_TRIGGERS](SCHED_NAME, TRIGGER_STATE, NEXT_FIRE_TIME);
    CREATE INDEX [IDX_QRTZ_T_NFT_ST_MISFIRE]      ON [dbo].[QRTZ_TRIGGERS](SCHED_NAME, MISFIRE_INSTR, NEXT_FIRE_TIME, TRIGGER_STATE);
    CREATE INDEX [IDX_QRTZ_T_NFT_ST_MISFIRE_GRP]  ON [dbo].[QRTZ_TRIGGERS](SCHED_NAME, MISFIRE_INSTR, NEXT_FIRE_TIME, TRIGGER_GROUP, TRIGGER_STATE);

    CREATE INDEX [IDX_QRTZ_FT_INST_JOB_REQ_RCVRY] ON [dbo].[QRTZ_FIRED_TRIGGERS](SCHED_NAME, INSTANCE_NAME, REQUESTS_RECOVERY);
    CREATE INDEX [IDX_QRTZ_FT_G_J]                ON [dbo].[QRTZ_FIRED_TRIGGERS](SCHED_NAME, JOB_GROUP, JOB_NAME);
    CREATE INDEX [IDX_QRTZ_FT_G_T]                ON [dbo].[QRTZ_FIRED_TRIGGERS](SCHED_NAME, TRIGGER_GROUP, TRIGGER_NAME);

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;

/*
Upstream license (https://github.com/quartznet/quartznet/blob/v3.18.0/license.txt):

                                 Apache License
                           Version 2.0, January 2004
                        http://www.apache.org/licenses/

   TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION

   1. Definitions.

      "License" shall mean the terms and conditions for use, reproduction,
      and distribution as defined by Sections 1 through 9 of this document.

      "Licensor" shall mean the copyright owner or entity authorized by
      the copyright owner that is granting the License.

      "Legal Entity" shall mean the union of the acting entity and all
      other entities that control, are controlled by, or are under common
      control with that entity. For the purposes of this definition,
      "control" means (i) the power, direct or indirect, to cause the
      direction or management of such entity, whether by contract or
      otherwise, or (ii) ownership of fifty percent (50%) or more of the
      outstanding shares, or (iii) beneficial ownership of such entity.

      "You" (or "Your") shall mean an individual or Legal Entity
      exercising permissions granted by this License.

      "Source" form shall mean the preferred form for making modifications,
      including but not limited to software source code, documentation
      source, and configuration files.

      "Object" form shall mean any form resulting from mechanical
      transformation or translation of a Source form, including but
      not limited to compiled object code, generated documentation,
      and conversions to other media types.

      "Work" shall mean the work of authorship, whether in Source or
      Object form, made available under the License, as indicated by a
      copyright notice that is included in or attached to the work
      (an example is provided in the Appendix below).

      "Derivative Works" shall mean any work, whether in Source or Object
      form, that is based on (or derived from) the Work and for which the
      editorial revisions, annotations, elaborations, or other modifications
      represent, as a whole, an original work of authorship. For the purposes
      of this License, Derivative Works shall not include works that remain
      separable from, or merely link (or bind by name) to the interfaces of,
      the Work and Derivative Works thereof.

      "Contribution" shall mean any work of authorship, including
      the original version of the Work and any modifications or additions
      to that Work or Derivative Works thereof, that is intentionally
      submitted to Licensor for inclusion in the Work by the copyright owner
      or by an individual or Legal Entity authorized to submit on behalf of
      the copyright owner. For the purposes of this definition, "submitted"
      means any form of electronic, verbal, or written communication sent
      to the Licensor or its representatives, including but not limited to
      communication on electronic mailing lists, source code control systems,
      and issue tracking systems that are managed by, or on behalf of, the
      Licensor for the purpose of discussing and improving the Work, but
      excluding communication that is conspicuously marked or otherwise
      designated in writing by the copyright owner as "Not a Contribution."

      "Contributor" shall mean Licensor and any individual or Legal Entity
      on behalf of whom a Contribution has been received by Licensor and
      subsequently incorporated within the Work.

   2. Grant of Copyright License. Subject to the terms and conditions of
      this License, each Contributor hereby grants to You a perpetual,
      worldwide, non-exclusive, no-charge, royalty-free, irrevocable
      copyright license to reproduce, prepare Derivative Works of,
      publicly display, publicly perform, sublicense, and distribute the
      Work and such Derivative Works in Source or Object form.

   3. Grant of Patent License. Subject to the terms and conditions of
      this License, each Contributor hereby grants to You a perpetual,
      worldwide, non-exclusive, no-charge, royalty-free, irrevocable
      (except as stated in this section) patent license to make, have made,
      use, offer to sell, sell, import, and otherwise transfer the Work,
      where such license applies only to those patent claims licensable
      by such Contributor that are necessarily infringed by their
      Contribution(s) alone or by combination of their Contribution(s)
      with the Work to which such Contribution(s) was submitted. If You
      institute patent litigation against any entity (including a
      cross-claim or counterclaim in a lawsuit) alleging that the Work
      or a Contribution incorporated within the Work constitutes direct
      or contributory patent infringement, then any patent licenses
      granted to You under this License for that Work shall terminate
      as of the date such litigation is filed.

   4. Redistribution. You may reproduce and distribute copies of the
      Work or Derivative Works thereof in any medium, with or without
      modifications, and in Source or Object form, provided that You
      meet the following conditions:

      (a) You must give any other recipients of the Work or
          Derivative Works a copy of this License; and

      (b) You must cause any modified files to carry prominent notices
          stating that You changed the files; and

      (c) You must retain, in the Source form of any Derivative Works
          that You distribute, all copyright, patent, trademark, and
          attribution notices from the Source form of the Work,
          excluding those notices that do not pertain to any part of
          the Derivative Works; and

      (d) If the Work includes a "NOTICE" text file as part of its
          distribution, then any Derivative Works that You distribute must
          include a readable copy of the attribution notices contained
          within such NOTICE file, excluding those notices that do not
          pertain to any part of the Derivative Works, in at least one
          of the following places: within a NOTICE text file distributed
          as part of the Derivative Works; within the Source form or
          documentation, if provided along with the Derivative Works; or,
          within a display generated by the Derivative Works, if and
          wherever such third-party notices normally appear. The contents
          of the NOTICE file are for informational purposes only and
          do not modify the License. You may add Your own attribution
          notices within Derivative Works that You distribute, alongside
          or as an addendum to the NOTICE text from the Work, provided
          that such additional attribution notices cannot be construed
          as modifying the License.

      You may add Your own copyright statement to Your modifications and
      may provide additional or different license terms and conditions
      for use, reproduction, or distribution of Your modifications, or
      for any such Derivative Works as a whole, provided Your use,
      reproduction, and distribution of the Work otherwise complies with
      the conditions stated in this License.

   5. Submission of Contributions. Unless You explicitly state otherwise,
      any Contribution intentionally submitted for inclusion in the Work
      by You to the Licensor shall be under the terms and conditions of
      this License, without any additional terms or conditions.
      Notwithstanding the above, nothing herein shall supersede or modify
      the terms of any separate license agreement you may have executed
      with Licensor regarding such Contributions.

   6. Trademarks. This License does not grant permission to use the trade
      names, trademarks, service marks, or product names of the Licensor,
      except as required for reasonable and customary use in describing the
      origin of the Work and reproducing the content of the NOTICE file.

   7. Disclaimer of Warranty. Unless required by applicable law or
      agreed to in writing, Licensor provides the Work (and each
      Contributor provides its Contributions) on an "AS IS" BASIS,
      WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
      implied, including, without limitation, any warranties or conditions
      of TITLE, NON-INFRINGEMENT, MERCHANTABILITY, or FITNESS FOR A
      PARTICULAR PURPOSE. You are solely responsible for determining the
      appropriateness of using or redistributing the Work and assume any
      risks associated with Your exercise of permissions under this License.

   8. Limitation of Liability. In no event and under no legal theory,
      whether in tort (including negligence), contract, or otherwise,
      unless required by applicable law (such as deliberate and grossly
      negligent acts) or agreed to in writing, shall any Contributor be
      liable to You for damages, including any direct, indirect, special,
      incidental, or consequential damages of any character arising as a
      result of this License or out of the use or inability to use the
      Work (including but not limited to damages for loss of goodwill,
      work stoppage, computer failure or malfunction, or any and all
      other commercial damages or losses), even if such Contributor
      has been advised of the possibility of such damages.

   9. Accepting Warranty or Additional Liability. While redistributing
      the Work or Derivative Works thereof, You may choose to offer,
      and charge a fee for, acceptance of support, warranty, indemnity,
      or other liability obligations and/or rights consistent with this
      License. However, in accepting such obligations, You may act only
      on Your own behalf and on Your sole responsibility, not on behalf
      of any other Contributor, and only if You agree to indemnify,
      defend, and hold each Contributor harmless for any liability
      incurred by, or claims asserted against, such Contributor by reason
      of your accepting any such warranty or additional liability.

   END OF TERMS AND CONDITIONS

   APPENDIX: How to apply the Apache License to your work.

      To apply the Apache License to your work, attach the following
      boilerplate notice, with the fields enclosed by brackets "[]"
      replaced with your own identifying information. (Don't include
      the brackets!)  The text should be enclosed in the appropriate
      comment syntax for the file format. We also recommend that a
      file or class name and description of purpose be included on the
      same "printed page" as the copyright notice for easier
      identification within third-party archives.

   Copyright 2007 Marko Lahma

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

