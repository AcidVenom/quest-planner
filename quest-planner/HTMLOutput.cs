using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace quest_planner
{
  public class HTMLOutput
  {
    string body;
    public HTMLOutput(Quest quest, QuestLister lister)
    {
      body += "<!DOCTYPE HTML>";
      body += "<head>";
      body += "<title>Quest Planner</title>";
      body +=
      "<style>" +
        "*{ font-family: Verdana; }" +
        "body{ background-color: gainsboro; }" +
        "#quest{ border-left: 1px solid black; padding-left: 5px; } " +
        "#skill{ background-color: antiquewhite; border: 1px solid black; padding: 5px; min-width: 256px; }" +
        "label{ cursor: pointer; }" +
        "label:hover{ color: blue; }" +
        "h2{ color: darkred; }" +
        "a{ font-size: 10pt; color: red; }" +
        "a:visited { font-size: 10pt; color: brown; }" +
        "#excerpt{ color: gray; font-size: 11pt; }" +
      "</style>";
      body += "</head>";
      body += "<body>";

      string name = quest.GetName();
      body += "<h1 style='color: brown'>" + name + " <a href='" + lister.GetFullQuestURL(name) + "'>[wiki]</a></h1>";


      if (quest.HasAnyRequirements() == false)
      {
        body += "<h2>This quest has no requirements, go to the starting location to begin the quest!</h2>";
      }
      else
      {
        AppendQuestRequirements(quest, lister);
        AppendSkillRequirements(quest);
        AppendMiscRequirements(quest);
      }

      body +=
        "<hr><br><span id='excerpt'>" +
        "Thank you for using this tool!<br>" +
        "There might be edge-cases that I did not handle, thus I do not provide any guarantee that all required information is there.<br>" +
        "A known issue, for instance, is that miniquest linking does not function properly.<br><br>Enjoy your quests!<br><br>" +
        "- Plague Cow<br>"  +
        "</span>";

      body += "</body>";
    }

    protected void AppendQuestRequirements(Quest quest, QuestLister lister)
    {
      if (quest.HasQuestRequirements() == false)
      {
        return;
      }

      body += "<h2>Quest requirements:</h2>";
      body += "<br>";

      int quest_count = 0;
      quest.ListRequiredQuests(s =>
      {
        int indent = 0;
        foreach (char c in s)
        {
          if (c == '\t')
          {
            indent += 17;
          }
        }

        string label_id = "quest_input_" + quest_count;
        string quest_name = s;
        quest_name = quest_name.Replace("\t", "");
        quest_name = quest_name.Replace("\n", "");
        body += 
        "<span id='quest' style='margin-left: " + indent + "px'>" +
        "<input type='checkbox' id='" + label_id + "'> " +
        "<label for='" + label_id + "'>" + s + "</label> <a href='" + lister.GetFullQuestURL(quest_name) + "'>[wiki]</a>" +
        "</span><br>";

        ++quest_count;
      });

      if (quest.HasQuestPointRequirements() == true)
      {
        body += "<h2>Quest points:</h2>";
        body += "<ul>";

        foreach (KeyValuePair<string, int> qp in quest.GetQuestPointRequirements())
        {
          body += "<li>" + qp.Key + " (<b>" + qp.Value + "</b>) <a href='" + lister.GetFullQuestURL(qp.Key) + "'>[wiki]</a></li>";
        }

        body += "</ul>";
      }
    }

    protected void AppendSkillRequirements(Quest quest)
    {
      if (quest.HasSkillRequirements() == false)
      {
        return;
      }

      body += "<h2>Skill requirements:</h2>";
      body += "<br>";
      body += "<table>";

      int skill_count = 0;
      quest.ListSkillRequirements(s =>
      {
        string label_id = "skill_input_" + skill_count;
        string[] split = s.Split(':');
        body += "<tr><td id='skill'><input type='checkbox' id='" + label_id + "'></input> <label for='" + label_id + "'>" + split[0] + ":<b>" + split[1] + "</b></label></td></tr>";

        ++skill_count;
      });

      body += "</table>";
    }

    protected void AppendMiscRequirements(Quest quest)
    {
      if (quest.HasMiscRequirements() == false)
      {
        return;
      }

      body += "<h2>Other requirements:</h2>";
      body += "<ul>";
      quest.ListMiscRequirements(s =>
      {
        body += "<li>" + s + "</li>";
      });
      body += "</ul>";
    }

    public void Show()
    {
      string dt = DateTime.Now.ToString();
      dt = dt.Replace(':', '-');
      dt = dt.Replace('/', '-');
      dt = dt.Replace(" ", "_");

      string temp = Path.GetTempPath() + "quest_planner_" + dt + ".html";

      Console.WriteLine("Saving to: {0}", temp);

      using (StreamWriter sw = new StreamWriter(temp))
      {
        sw.Write(body);
      }

      System.Diagnostics.Process.Start(temp);
    }
  }
}
