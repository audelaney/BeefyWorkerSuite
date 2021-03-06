﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SvtTestSuite.Models
{
    public interface ISvtTest
    {
        int TestId { get; set; }
        int ClipNumber { get; set; }
        DateTime TestDate { get; set; }
        string OutputPath { get; set; }
        string[] ParamsInCmmd { get; }
    }
}