/*
 * Copyright  Michał Młodawski (SimpleMethod)(c) 2020.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using SilentPackage.Models;

namespace SilentPackage.Controllers.DocumentGenerator
{
    class DocumentTableGenerator
    {
        public DocumentTableGenerator()
        {
          
        }

        public string GenerateTable<T>(Stack<T> stack, int option)
        {
            ComboModel comboModel = new ComboModel();
            string _table = "";
            int itelator = 0;
            while (stack.Count > 0)
            {
               
                if (option == 0)
                {
                    ProcessList processList = (ProcessList) (object)stack.Pop();
                    _table +="<h4>Data raportu: "+ DateTimeOffset.FromUnixTimeSeconds(processList.Timestamp) + "</h4>";  
                    _table += @"<div class=""table-responsive""><table class=""table table-striped table-sm""><thead><tr><th scope=""col"">#</th><th scope=""col"">ID</th><th scope=""col"">Nazwa</th><th scope=""col"">Data startu</th></tr></thead><tbody>";
                    foreach (var modelProcessList in processList.ProcessObjectList)
                    {
                        itelator++;
                        StringBuilder tempBuilder = new StringBuilder();
                        tempBuilder.AppendFormat(
                            @"<tr><th scope=""row"">{0}</th><td>{1}</td><td>{2}</td><td>{3}</td></tr>", itelator, modelProcessList.Id, modelProcessList.Name, modelProcessList.StartTime);
                        _table += tempBuilder.ToString();
                    }

                    _table += "</table> </div>";
                    itelator = 0;
                }

                if (option == 1) 
                {
                    BrowsingHistoryLists browsingHistory = (BrowsingHistoryLists)(object)stack.Pop();
                    _table += "<h4>Data raportu: " + DateTimeOffset.FromUnixTimeSeconds(browsingHistory.Timestamp) + "</h4>";
                    _table += @"<div class=""table-responsive""><table class=""table table-striped table-sm""><thead><tr><th scope=""col"">#</th><th scope=""col"">Nazwa strony www</th><th scope=""col"">Adres URL</th><th scope=""col"">Czas spędzony na witrynie</th><th scope=""col"">Czas ostatniej wizyty</th></tr></thead><tbody>";

                    foreach (var moBrowsingHistoryTab in browsingHistory.BrowsingHistoryList)
                    {
                        itelator++;
                        StringBuilder tempBuilder = new StringBuilder();
                        tempBuilder.AppendFormat(
                            @"<tr><th scope=""row"">{0}</th><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td></tr>", itelator, moBrowsingHistoryTab.GetTitle, moBrowsingHistoryTab.GetUrl, moBrowsingHistoryTab.GetDurationTime, DateTimeOffset.FromUnixTimeSeconds(moBrowsingHistoryTab.GetLastVisitTime));
                        _table += tempBuilder.ToString();
                    }
                    _table += "</table> </div>";
                    itelator = 0;
                }

                if (option==2)
                {
                    FileDirectoryList fileDirectory = (FileDirectoryList)(object)stack.Pop();
                    _table += "<h4>Data raportu: " + DateTimeOffset.FromUnixTimeSeconds(fileDirectory.Timestamp) + "</h4>";
                    _table += @"<div class=""table-responsive""><table class=""table table-striped table-sm""><thead><tr><th scope=""col"">#</th><th scope=""col"">Ścieżka dostępu</th><th scope=""col"">Data utworzenia </th><th scope=""col"">Data ostatniego dostępu</th><th scope=""col"">Data ostatniego zapisu</th></tr></thead><tbody>";

                    foreach (var moFileDirectory in fileDirectory.FileDirectories)
                    {
                        itelator++;
                        StringBuilder tempBuilder = new StringBuilder();
                        tempBuilder.AppendFormat(
                            @"<tr><th scope=""row"">{0}</th><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td></tr>", itelator, moFileDirectory.FullName, moFileDirectory.CreationTimeUtc, moFileDirectory.LastAccessTimeUtc, moFileDirectory.LastWriteTimeUtc);
                        _table += tempBuilder.ToString();
                    }
                    _table += "</table> </div>";
                    itelator = 0;
                }
                
            }

            return _table;
        }
        public string GenerateScreenshotsTable(List<Models.FileDirectory> list)
        {
            ComboModel comboModel = new ComboModel();
            string _table = "";
            int itelator = 0;
            _table += @"<div class=""table-responsive""><table class=""table table-striped table-sm""><thead><tr><th scope=""col"">#</th><th scope=""col"">Link</th><th scope=""col"">Data wykonania</th></tr></thead><tbody>";
            foreach (var screenshots in list)
            {
                itelator++;
                StringBuilder tempBuilder = new StringBuilder();
                tempBuilder.AppendFormat(
                    @"<tr><th scope=""row"">{0}</th><td><a href=""./screenshots/{1}"" target=""_blank"">Aktywność {1}</a></td><td>{2}</td></tr>", itelator, Path.GetFileName(screenshots.FullName), screenshots.CreationTimeUtc);
                _table += tempBuilder.ToString();
            }

            _table += "</table> </div>";
            
            return _table;
        }
    }
}
