// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Spreadsheet_Abylay_Dospayev
{
    /*
     * Author: Abylay Dospayev
     * WSU ID: 011858661
     * 
     * Entry point for the spreadsheet application.
     * This class initializes the application and starts the main form.
     */
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// Initializes application configuration and runs the main form.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}