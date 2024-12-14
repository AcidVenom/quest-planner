using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quest_planner
{
  public class SkillRequirements
  {
    int[] min_required;

    public SkillRequirements()
    {
      int num_skills = (int)Skill.kCount;

      min_required = new int[num_skills];
      for (int i = 0; i < num_skills; ++i)
      {
        min_required[i] = 0;
      }
    }

    public enum Skill
    {
      kAttack,
      kStrength,
      kDefence,
      kRanged,
      kPrayer,
      kMagic,
      kRunecrafting,
      kConstruction,
      kDungeoneering,
      kArchaeology,
      kConstitution,
      kAgility,
      kHerblore,
      kThieving,
      kCrafting,
      kFletching,
      kSlayer,
      kHunter,
      kDivination,
      kMining,
      kSmithing,
      kFishing,
      kCooking,
      kFiremaking,
      kWoodcutting,
      kFarming,
      kSummoning,
      kInvention,
      kNecromancy,
      kCount
    }

    public static string SkillToSkillName(Skill skill)
    {
      return skill.ToString().Remove(0, 1);
    }

    public static Skill SkillNameToSkill(string skill_name)
    {
      string lower = skill_name.ToLower();
      if (lower == "attack")
      {
        return Skill.kAttack;
      }
      else if (lower == "strength")
      {
        return Skill.kStrength;
      }
      else if (lower == "defence")
      {
        return Skill.kDefence;
      }
      else if (lower == "ranged")
      {
        return Skill.kRanged;
      }
      else if (lower == "prayer")
      {
        return Skill.kPrayer;
      }
      else if (lower == "magic")
      {
        return Skill.kMagic;
      }
      else if (lower == "runecrafting")
      {
        return Skill.kRunecrafting;
      }
      else if (lower == "construction")
      {
        return Skill.kConstruction;
      }
      else if (lower == "dungeoneering")
      {
        return Skill.kDungeoneering;
      }
      else if (lower == "archaeology")
      {
        return Skill.kArchaeology;
      }
      else if (lower == "constitution")
      {
        return Skill.kConstitution;
      }
      else if (lower == "agility")
      {
        return Skill.kAgility;
      }
      else if (lower == "herblore")
      {
        return Skill.kHerblore;
      }
      else if (lower == "thieving")
      {
        return Skill.kThieving;
      }
      else if (lower == "crafting")
      {
        return Skill.kCrafting;
      }
      else if (lower == "fletching")
      {
        return Skill.kFletching;
      }
      else if (lower == "slayer")
      {
        return Skill.kSlayer;
      }
      else if (lower == "hunter")
      {
        return Skill.kHunter;
      }
      else if (lower == "divination")
      {
        return Skill.kDivination;
      }
      else if (lower == "mining")
      {
        return Skill.kMining;
      }
      else if (lower == "smithing")
      {
        return Skill.kSmithing;
      }
      else if (lower == "fishing")
      {
        return Skill.kFishing;
      }
      else if (lower == "cooking")
      {
        return Skill.kCooking;
      }
      else if (lower == "firemaking")
      {
        return Skill.kFiremaking;
      }
      else if (lower == "woodcutting")
      {
        return Skill.kWoodcutting;
      }
      else if (lower == "farming")
      {
        return Skill.kFarming;
      }
      else if (lower == "summoning")
      {
        return Skill.kSummoning;
      }
      else if (lower == "invention")
      {
        return Skill.kInvention;
      }
      else if (lower == "necromancy")
      {
        return Skill.kNecromancy;
      }

      return Skill.kCount;
    }

    public int GetRequiredLevel(Skill skill)
    {
      return min_required[(int)skill];
    }

    public void SetRequiredLevel(Skill skill, int level)
    {
      int idx = (int)skill;
      min_required[idx] = Math.Max(min_required[idx], level);
    }

    public void Merge(SkillRequirements other)
    {
      for (int i = 0; i < (int)Skill.kCount; ++i)
      {
        min_required[i] = Math.Max(min_required[i], other.min_required[i]);
      }
    }

    public bool HasAnyLevel()
    {
      foreach (int level in min_required)
      {
        if (level > 0)
        {
          return true;
        }
      }

      return false;
    }
  }
}
