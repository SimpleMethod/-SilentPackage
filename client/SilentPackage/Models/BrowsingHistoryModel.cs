/*
 * Copyright  Michał Młodawski (SimpleMethod)(c) 2020.
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace SilentPackage.Models
{
    public class BrowsingHistoryLists
    {
        public List<BrowsingHistoryTab> BrowsingHistoryList { get; set; }
        public long Timestamp { get; set; }
    }

    public class BrowsingHistoryTab
    {
        public BrowsingHistoryTab(string getUrl, string getTitle, long getLastVisitTime, long getDurationTime)
        {
            GetUrl = getUrl;
            GetTitle = getTitle;
            GetLastVisitTime = getLastVisitTime;
            GetDurationTime = getDurationTime;
        }

        public string GetUrl {get; set;}
        public string GetTitle { get; set; }
        public long GetLastVisitTime { get; set; }
        public long GetDurationTime { get; set; }
    }
}
