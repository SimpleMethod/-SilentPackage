using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.Text;
using System.Windows;
using SilentPackage.Models;

namespace SilentPackage.Controllers
{
    class DocumentTableGeneration
    {
        public DocumentTableGeneration()
        {
          
        }

        public string GenerateTable<T>(Stack<T> stack, int option)
        {
            ComboModel comboModel = new ComboModel();
            string _table = "";
            int itelator = 0;
            while (stack.Count > 0)
            {
                MessageBox.Show(stack.Count.ToString());
                if (option == 0)
                {
                    ProcessList processList = (ProcessList) (object)stack.Pop();
                    _table +="<h1>Data: "+ DateTimeOffset.FromUnixTimeSeconds(processList.Timestamp) + "</h1>";  
                    _table += @"<table class=""table""><thead><tr><th scope=""col"">#</th><th scope=""col"">ID</th><th scope=""col"">Nazwa</th><th scope=""col"">Data startu</th></tr></thead><tbody>";
                    foreach (var modelProcessList in processList.ProcessObjectList)
                    {
                        itelator++;
                        StringBuilder tempBuilder = new StringBuilder();
                        tempBuilder.AppendFormat(
                            @"<tr><th scope=""row"">{0}</th><td>{1}</td><td>{2}</td><td>{3}</td></tr>", itelator, modelProcessList.Id, modelProcessList.Name, modelProcessList.StartTime);
                        _table += tempBuilder.ToString();
                    }

                    _table += "</table>";
                    itelator = 0;
                }

                if (option == 1) 
                {
                    BrowsingHistoryLists browsingHistory = (BrowsingHistoryLists)(object)stack.Pop();
                    _table += "<h1>Data: " + DateTimeOffset.FromUnixTimeSeconds(browsingHistory.Timestamp) + "</h1>";
                    _table += @"<table class=""table""><thead><tr><th scope=""col"">#</th><th scope=""col"">Nazwa strony www</th><th scope=""col"">Adres URL</th><th scope=""col"">Czas spędzony na witrynie</th><th scope=""col"">Czas ostatniej wizyty</th></tr></thead><tbody>";

                    foreach (var moBrowsingHistoryTab in browsingHistory.BrowsingHistoryList)
                    {
                        itelator++;
                        StringBuilder tempBuilder = new StringBuilder();
                        tempBuilder.AppendFormat(
                            @"<tr><th scope=""row"">{0}</th><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td></tr>", itelator, moBrowsingHistoryTab.GetTitle, moBrowsingHistoryTab.GetUrl, moBrowsingHistoryTab.GetDurationTime, DateTimeOffset.FromUnixTimeSeconds(moBrowsingHistoryTab.GetLastVisitTime));
                        _table += tempBuilder.ToString();
                    }
                    _table += "</table>";
                    itelator = 0;
                }

                if (option==2)
                {
                    FileDirectoryList fileDirectory = (FileDirectoryList)(object)stack.Pop();
                    _table += "<h1>Data: " + DateTimeOffset.FromUnixTimeSeconds(fileDirectory.Timestamp) + "</h1>";
                    _table += @"<table class=""table""><thead><tr><th scope=""col"">#</th><th scope=""col"">Ścieżka dostępu</th><th scope=""col"">Data utworzenia </th><th scope=""col"">Data ostatniego dostępu</th><th scope=""col"">Data ostatniego zapisu</th></tr></thead><tbody>";

                    foreach (var moFileDirectory in fileDirectory.FileDirectories)
                    {
                        itelator++;
                        StringBuilder tempBuilder = new StringBuilder();
                        tempBuilder.AppendFormat(
                            @"<tr><th scope=""row"">{0}</th><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td></tr>", itelator, moFileDirectory.FullName, moFileDirectory.CreationTimeUtc, moFileDirectory.LastAccessTimeUtc, moFileDirectory.LastWriteTimeUtc);
                        _table += tempBuilder.ToString();
                    }
                    _table += "</table>";
                    itelator = 0;
                }
                
            }

            return _table;
        }

    }
}
