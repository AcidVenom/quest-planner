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

      Console.WriteLine("==============================================\n");

      Random r = new Random();

      Console.WriteLine("Would you like random quests? Press 'R'. If using the quest planner as usual; press any other key\n");
      if (Console.ReadKey(true).Key == ConsoleKey.R)
      {
        Console.WriteLine("\n--------------------------------------------------------------------------------------------------");
        Console.WriteLine("\n  Randomizer mode (Press any key, except 'Q' to generate a new random quest. Type 'Q' to exit)\n");
        Console.WriteLine("--------------------------------------------------------------------------------------------------\n");
        while (Console.ReadKey(true).Key != ConsoleKey.Q)
        {
          double random_number = r.NextDouble();
          int random_index = (int)Math.Floor(random_number * (double)quest_count);

          Console.WriteLine("Random quest: {0}", lister.GetQuestName(random_index));
        }

        return 0;
      }

      Console.WriteLine("\n--------------------------------------------------------------------------------------------------");
      Console.WriteLine("\n  Planner mode\n");
      Console.WriteLine("--------------------------------------------------------------------------------------------------\n");

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
