﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelWorkbookSplitter.Functions;
using ExcelObject = Microsoft.Office.Interop.Excel;

namespace ExcelWorkbookSplitter
{
    class Program
    {
        static void Main(string[] args)
        {

            // Parse command line params
            string inFile = @"C:\Temp\Excel\test.xlsx";
            string outFile = "";
            outFile = @"C:\Temp\Excel\NewExcelFile.xlsx";
            string query = "";
            query = "select count(field1) as e1 from [data$]";

            bool info = !String.IsNullOrEmpty(query);
            bool resultToFile = !String.IsNullOrEmpty(outFile);

            // Open file and ...
            using (ExcelCore excelIn = new ExcelCore(inFile))
            {
                if (excelIn.IsInitialized())
                {
                    if (info)
                    {
                        // Display common information about Excel worksheets

                        List<string> worksheets = excelIn.GetWorksheets();

                        Console.WriteLine("List of available worksheets in file \"{0}\":", excelIn.FileName);
                        foreach (String name in worksheets)
                        {
                            Console.WriteLine("\t\"{0}\"", name);

                            ExcelObject.Worksheet worksheet = excelIn.GetWorksheet(name);

                            Console.WriteLine("\tLast row with data: {0}; Last column with data: {1}\n",
                                excelIn.GetCountOfRows(worksheet),
                                excelIn.GetCountOfCols(worksheet)
                            );
                        }
                    }
                    else
                    {
                        DataTable queryResult = new DataTable();
                        if (excelIn.RunSql(query, ref queryResult))
                        {
                            if (resultToFile)
                            {
                                // Option 1

                                // Save result to new file
                                using (ExcelCore excelOut = new ExcelCore())
                                {
                                    excelOut.NewFile(outFile);
                                    if (excelOut.IsInitialized())
                                    {
                                        if (excelOut.NewSheet("RESULT", WorksheerOrder.woFirst))
                                        {
                                            excelOut.DeleteSheet("Sheet1");

                                            //                       excelOut.PopulateData("RESULT", queryResult);

                                            excelOut.SaveFile();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // Option 2

                                // Verbose output -- show result from datatable
                                DisplayResult(queryResult);
                            }
                        }
                        else
                        {
                            Console.WriteLine("The an error has been occured during executing the sql query");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("The an error has been occured during accessing Excel file");
                }

                Console.ReadKey();

                return;
            }
        }

        private static void DisplayResult(DataTable dataTable)
        {
            foreach (DataRow dataRow in dataTable.Rows)
            {
                foreach (var item in dataRow.ItemArray)
                {
                    Console.Write(item + "\t");
                }
                Console.WriteLine();
            }
        }
    }
}
