using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace quest_planner
{
  public class QuestLister
  {
    protected struct QuestListing
    {
      public string name;
      public string url;
    }

    List<QuestListing> available_quests;

    public QuestLister(string url)
    {
      available_quests = new List<QuestListing>();

      HTTPRequester.HTMLResponse hr = HTTPRequester.GetURLBlocking(url);
      if (hr.Succeeded == true)
      {
        Parse(hr.Document);
      }

      available_quests = available_quests.OrderBy(a => a.name).ToList();
    }

    protected void Parse(HtmlDocument doc)
    {
      HtmlNode table = doc.DocumentNode.QuerySelector("table.wikitable");
      if (table == null)
      {
        return;
      }

      table = table.SelectSingleNode(table.XPath + "/tbody");
      
      foreach (HtmlNode quest in doc.DocumentNode.SelectNodes("//a"))
      {
        if (quest.ParentNode == null || quest.ParentNode.ParentNode == null)
        {
          continue;
        }

        // Well.. I apparently don't really know how this works, but whatever
        int child_index = quest.ParentNode.ParentNode.ChildNodes.IndexOf(quest.ParentNode);
        if (quest.ParentNode.ParentNode.ParentNode == table && child_index == 0)
        {
          QuestListing listing;
          listing.name = quest.InnerText;
          listing.url = listing.name != "" ? quest.GetAttributeValue("href", "") : "";

          if (listing.name != "" && listing.url != "")
          {
            available_quests.Add(listing);
          }
        }
      }
    }

    public void ListAvailableQuests()
    {
      int i = 0;
      foreach (QuestListing ql in available_quests)
      {
        ++i;
        Console.WriteLine("{0}.\t\t{1}", i, ql.name);
      }
    }

    public int GetQuestCount()
    {
      return available_quests.Count;
    }

    public string GetQuestName(int index)
    {
      return index >= available_quests.Count ? "undefined" : available_quests[index].name;
    }

    public string GetFullQuestURL(int index)
    {
      if (index < 0 || index >= available_quests.Count)
      {
        return "<error>";
      }

      return @"https://runescape.wiki" + available_quests[index].url;
    }

    public string GetFullQuestURL(string name)
    {
      string retrieval_name = name;
      retrieval_name = retrieval_name.Replace(" (quest)", "");

      int idx = FindQuestByName(retrieval_name);
      return GetFullQuestURL(idx);
    }

    public int FindQuestByName(string name)
    {
      for (int i = 0; i < available_quests.Count; ++i)
      {
        if (available_quests[i].name == name)
        {
          return i;
        }
      }

      return -1;
    }
  }
}
