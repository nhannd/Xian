#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0//

#endregion

// This file is auto-generated by the ClearCanvas.Model.SqlServer.CodeGenerator project.

namespace ClearCanvas.ImageServer.Model
{
    using System;
    using System.Collections.Generic;
    using ClearCanvas.ImageServer.Model.EntityBrokers;
    using ClearCanvas.ImageServer.Enterprise;
    using System.Reflection;

[Serializable]
public partial class StudyIntegrityReasonEnum : ServerEnum
{
      #region Private Static Members
      private static readonly StudyIntegrityReasonEnum _InconsistentData = GetEnum("InconsistentData");
      private static readonly StudyIntegrityReasonEnum _Duplicate = GetEnum("Duplicate");
      #endregion

      #region Public Static Properties
      /// <summary>
      /// Images must be reconciled because of inconsistent data.
      /// </summary>
      public static StudyIntegrityReasonEnum InconsistentData
      {
          get { return _InconsistentData; }
      }
      /// <summary>
      /// Duplicates were received and need to be reconciled.
      /// </summary>
      public static StudyIntegrityReasonEnum Duplicate
      {
          get { return _Duplicate; }
      }

      #endregion

      #region Constructors
      public StudyIntegrityReasonEnum():base("StudyIntegrityReasonEnum")
      {}
      #endregion
      #region Public Members
      public override void SetEnum(short val)
      {
          ServerEnumHelper<StudyIntegrityReasonEnum, IStudyIntegrityReasonEnumBroker>.SetEnum(this, val);
      }
      static public List<StudyIntegrityReasonEnum> GetAll()
      {
          return ServerEnumHelper<StudyIntegrityReasonEnum, IStudyIntegrityReasonEnumBroker>.GetAll();
      }
      static public StudyIntegrityReasonEnum GetEnum(string lookup)
      {
          return ServerEnumHelper<StudyIntegrityReasonEnum, IStudyIntegrityReasonEnumBroker>.GetEnum(lookup);
      }
      #endregion
}
}
