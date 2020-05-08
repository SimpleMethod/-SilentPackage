/*
 * Copyright  Michał Młodawski (SimpleMethod)(c) 2020.
 */
using System.Text;

namespace SilentPackage.Controllers.DocumentGenerator
{
    class DocumentNavGenerator
    {
        public DocumentNavGenerator()
        {

        }

        public string GenerateNav(bool process, bool webHistory, bool PrintScr, bool scanDirectory, int active)
        {
            StringBuilder tempBuilder = new StringBuilder("<li class=\"nav-item\"> <a class=\"nav-link\" href=\"./index.html\"> <span data-feather=\"layers\"></span>Strona główna</a></li>");

            if (process)
            {
                string tempActive = "";
                if (active == 0)
                {
                    tempActive = "active";
                }
                tempBuilder.AppendFormat("<li class=\"nav-item {0}\"> <a class=\"nav-link {0}\" href=\"./processList.html\"> <span data-feather=\"layers\"></span>Lista procesów</a></li>", tempActive);
            }

            if (webHistory)
            {
                string tempActive = "";
                if (active == 1)
                {
                    tempActive = "active";
                }
                tempBuilder.AppendFormat("<li class=\"nav-item\"> <a class=\"nav-link {0}\" href=\"./webHistoryList.html\"> <span data-feather=\"layers\"></span>Historia przegladania</a></li>", tempActive);
            }
            if (scanDirectory)
            {
                string tempActive = "";
                if (active == 2)
                {
                    tempActive = "active";
                }
                tempBuilder.AppendFormat("<li class=\"nav-item\"> <a class=\"nav-link {0} \" href=\"./directoryList.html\"> <span data-feather=\"layers\"></span>Lista katalogów</a></li>", tempActive);
            }
            if (PrintScr)
            {
                string tempActive = "";
                if (active == 3)
                {
                    tempActive = "active";
                }
                tempBuilder.AppendFormat("<li class=\"nav-item\"> <a class=\"nav-link {0}\" href=\"./screenshotsList.html\"> <span data-feather=\"layers\"></span>Lista zrzutów ekranowych</a></li>", tempActive);
            }
            return tempBuilder.ToString();
        }
    }
}
