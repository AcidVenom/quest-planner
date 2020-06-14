using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace quest_planner
{
  public class Quest
  {
    string name;
    SkillRequirements skill_requirements;
    List<Quest> required_quests;
    int required_quest_points;
    List<string> misc_requirements;

    public delegate void ListDelegate(string s);

    public Quest(string url, List<string> already_parsed = null)
    {
      name = "";
      skill_requirements = new SkillRequirements();
      required_quests = new List<Quest>();
      required_quest_points = 0;
      misc_requirements = new List<string>();

      if (already_parsed == null)
      {
        already_parsed = new List<string>();
      }

      if (already_parsed.Contains(url))
      {
        return;
      }

      RetrieveData(url, already_parsed);
      Merge();
    }

    public bool IsValid()
    {
      return name != "";
    }

    public string GetName()
    {
      return name;
    }

    protected void RetrieveData(string url, List<string> already_parsed)
    {
      already_parsed.Add(url);
      HTTPRequester.HTMLResponse hr = HTTPRequester.GetURLBlocking(url);

      if (hr.Succeeded == true)
      {
        if (!hr.Document.DocumentNode.InnerHtml.Contains("quick guide"))
        {
          return;
        }

        if (Parse(hr.Document, already_parsed) == false)
        {
          Console.WriteLine("[Warning] Could not fully resolve parsing of quest '{0}'", url);
        }
      }
      else
      {
        Console.WriteLine("We were not able to retrieve all data, this might be due to a timeout (15 seconds), try again later");
      }
    }

    protected HtmlNode FindQuestRequirementsTable(HtmlDocument doc)
    {
      foreach (HtmlNode quest_header in doc.QuerySelectorAll("th.questdetails-header"))
      {
        if (quest_header.InnerText == "Requirements")
        {
          return quest_header.ParentNode.QuerySelector("td.questdetails-info");
        }
      }

      return null;
    }

    protected bool Parse(HtmlDocument doc, List<string> already_parsed)
    {
      var title = doc.DocumentNode.SelectSingleNode("//h1");
      name = title.InnerText;

      HtmlNode requirements_table = FindQuestRequirementsTable(doc);

      if (requirements_table == null)
      {
        return true;
      }

      if (ParseRequiredQuests(requirements_table, already_parsed) == false)
      {
        return false;
      }

      if (ParseOtherRequirements(requirements_table) == false)
      {
        return false;
      }

      return true;
    }

    protected bool ParseRequiredQuests(HtmlNode requirements_table, List<string> already_parsed)
    {
      HtmlNode quest_reqs = requirements_table.QuerySelector("table.questreq");
      if (quest_reqs == null)
      {
        return true;
      }

      Dictionary<string, HtmlNode> quest_urls = new Dictionary<string, HtmlNode>();

      HtmlNode top_list_item = quest_reqs.QuerySelector("li");

      foreach (HtmlNode quest_node in top_list_item.QuerySelectorAll("a"))
      {
        string quest_name = quest_node.InnerHtml;

        bool is_top_level = quest_node.ParentNode.ParentNode.ParentNode == top_list_item;
        bool is_not_self = quest_name != name;
        bool is_already_added = quest_urls.ContainsKey(quest_name);

        if (
          is_top_level == true
          && is_not_self == true
          && is_already_added == false)
        {
          string url = quest_node.GetAttributeValue("href", "/");
          quest_urls.Add(quest_name, quest_node);
        }
      }

      foreach (KeyValuePair<string, HtmlNode> pair in quest_urls)
      {
        string url = pair.Value.GetAttributeValue("href", "/");
        Quest new_quest = new Quest(@"https://runescape.wiki" + url, already_parsed);
        if (new_quest.IsValid())
        {
          required_quests.Add(new_quest);
        }
        else if (pair.Key.ToLower().Contains("quest points"))
        {
          string qp_string = pair.Value.ParentNode.InnerText.Split(' ').First();
          int quest_points = 0;

          if (int.TryParse(qp_string, out quest_points))
          {
            required_quest_points = quest_points;
          }
        }
      }

      return true;
    }

    protected bool ParseOtherRequirements(HtmlNode requirements_table)
    {
      if (requirements_table == null)
      {
        return false;
      }

      foreach (HtmlNode skill_req in requirements_table.QuerySelectorAll("ul"))
      {
        if (skill_req.ParentNode != requirements_table)
        {
          continue;
        }

        string[] split_reqs = skill_req.InnerText.Split('\n');

        foreach (string current in split_reqs)
        {
          string to_parse = current.ToLower();

          bool found = false;
          for (int i = 0; i < (int)SkillRequirements.Skill.kCount; ++i)
          {
            SkillRequirements.Skill current_skill = (SkillRequirements.Skill)i;
            string to_check = SkillRequirements.SkillToSkillName(current_skill);

            if (to_parse.Contains(to_check.ToLower()) == true)
            {
              string[] split_skill = to_parse.Split(' ');
              string level_string = split_skill.First();

              int required_level;
              if (int.TryParse(level_string, out required_level))
              {
                skill_requirements.SetRequiredLevel(current_skill, required_level);
                found = true;
              }

              break;
            }
          }

          if (found == false)
          {
            if (skill_req.InnerText != "None" && current.Length > 0)
            {
              misc_requirements.Add(current);
            }
          }
        }
      }

      return true;
    }

    protected void MergeMiscRequirement(string req)
    {
      foreach (string s in misc_requirements)
      {
        if (s == req)
        {
          return;
        }
      }

      misc_requirements.Add(req);
    }

    protected void Merge(Quest invoker = null)
    {
      if (invoker != null)
      {
        invoker.skill_requirements.Merge(skill_requirements);
        foreach (string s in misc_requirements)
        {
          invoker.MergeMiscRequirement(s);
        }
      }
      else
      {
        invoker = this;
      }

      foreach (Quest q in required_quests)
      {
        q.Merge(invoker);
      }
    }

    public void ListRequiredQuests(ListDelegate del = null, int depth = 0)
    {
      foreach (Quest q in required_quests)
      {
        string output = "";
        for (int i = 0; i < depth; ++i)
        {
          output += '\t';
        }

        output += q.GetName() + '\n';

        if (del == null)
        {
          Console.WriteLine(output);
        }
        else
        {
          del(output);
        }

        q.ListRequiredQuests(del, depth + 1);
      }
    }

    public void ListSkillRequirements(ListDelegate del = null)
    {
      for (int i = 0; i < (int)SkillRequirements.Skill.kCount; ++i)
      {
        SkillRequirements.Skill skill = (SkillRequirements.Skill)i;
        int level = skill_requirements.GetRequiredLevel(skill);
        if (level > 0)
        {
          string output = SkillRequirements.SkillToSkillName(skill) + ": " + level;
          if (del == null)
          {
            Console.WriteLine(output);
          }
          else
          {
            del(output);
          }
        }
      }
    }

    public void ListMiscRequirements(ListDelegate del = null)
    {
      foreach (string s in misc_requirements)
      {
        if (del == null)
        {
          Console.WriteLine(s);
        }
        else
        {
          del(s);
        }
      }
    }

    public void ListQuestPointRequirements(ListDelegate del = null)
    {
      foreach (Quest q in required_quests)
      {
        int required = q.required_quest_points;
        string output = q.name + ": " + required;

        if (required > 0)
        {
          if (del == null)
          {
            Console.WriteLine(output);
          }
          else
          {
            del(output);
          }
        }

        q.ListQuestPointRequirements(del);
      }
    }

    public bool HasQuestRequirements()
    {
      return required_quests.Count > 0;
    }
    
    public bool HasQuestPointRequirements()
    {
      foreach (Quest q in required_quests)
      {
        if (q.required_quest_points > 0)
        {
          return true;
        }
      }

      return false;
    }

    public Dictionary<string, int> GetQuestPointRequirements()
    {
      Dictionary<string, int> to_return = new Dictionary<string, int>();
      foreach (Quest q in required_quests)
      {
        if (q.required_quest_points > 0)
        {
          to_return.Add(q.name, q.required_quest_points);
        }
      }

      return to_return;
    }

    public bool HasSkillRequirements()
    {
      return skill_requirements.HasAnyLevel();
    }

    public bool HasMiscRequirements()
    {
      return misc_requirements.Count > 0;
    }

    public bool HasAnyRequirements()
    {
      return HasMiscRequirements() || HasQuestPointRequirements() || HasQuestRequirements() || HasSkillRequirements();
    }
  }
}
