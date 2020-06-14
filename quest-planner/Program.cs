//#define TEST_ALL_QUESTS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quest_planner
{
  class Program
  {
    static int Main(string[] args)
    {
      QuestLister lister = new QuestLister(@"https://runescape.wiki/w/List_of_quests");
      lister.ListAvailableQuests();
      int quest_count = lister.GetQuestCount();

#if TEST_ALL_QUESTS
      Console.WriteLine("===============[Test]===============");

      for (int i = 0; i < quest_count; ++i)
      {
        Console.WriteLine("\n> Showing requirements for '{0}', This might take a short time..\n", lister.GetQuestName(i));
        Quest chosen_quest = new Quest(lister.GetFullQuestURL(i));
      }

      Console.WriteLine("===============[/Test]===============");
#endif

      while (true)
      {
        Console.WriteLine("==============================================\n");
        Console.WriteLine("Please choose a quest to list the requirements of, or type 'exit' to quit:\n");

        string input = Console.ReadLine();

        if (input == "exit")
        {
          break;
        }

        int chosen = 0;
        if (int.TryParse(input, out chosen))
        {
          chosen -= 1;
          if (chosen < 0 || chosen >= quest_count)
          {
            Console.WriteLine("Invalid input, the valid options are 1 to {0}", quest_count);
            continue;
          }

          Console.WriteLine("\n> Showing requirements for '{0}', This might take a short time..\n", lister.GetQuestName(chosen));
          Quest chosen_quest = new Quest(lister.GetFullQuestURL(chosen));

          HTMLOutput output = new HTMLOutput(chosen_quest, lister);
          output.Show();
          
          Console.WriteLine("\n> ..Done!\n", lister.GetQuestName(chosen));
        }
        else
        {
          Console.WriteLine("Invalid input, only numbers are allowed to navigate to a quest");
        }
      }

      return 0;
    }
  }
}
