﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClearInsight.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start Test...");
            KpiEntryTest kpitest = new KpiEntryTest();
            try
            {
                //kpitest.TestErrorMessage();
                kpitest.TestImportsingleKpiEntry();
                //kpitest.TestImportbulkKpiEntries();
                //kpitest.TestImportKpiEntryAsync();
                //kpitest.TestImportKpiEntriesAsync();
                //kpitest.TestImportbulkKpiEntriesBatch();
                //kpitest.TestImportKpiEntriesAsyncBatch();
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            Console.WriteLine("End Test...");
            Console.ReadLine();
        }
    }
}
