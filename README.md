# quest-planner
RuneScape quest planner, utilizes the Wiki to recursively find out all quest requirements for a specific end-game quest. Useful for e.g. working towards 'Plague's End'

I am not entirely sure of scraping a Wiki like this is allowed, however, all calls are synchronous; thus is shouldn't be making too much requests to the
actual Wiki page. One request is done to retrieve the entirety of the quest listing (https://runescape.wiki/w/List_of_quests) and then there is a request
per recursive quest.

e.g. In the case of *All Fired Up*, it requires *Priest in Peril* to be completed. The tool will make a seperate request on both the quests.


I am also not too sure whether a tool like this exists; but the idea is that you can now easily plan a faraway quest as you will see all requirements
of the base quest, including its sub-quests.


There is no guarantee that I handle all the edge cases, nor are required items listed. I don't think I'll continue on this, but this could be
expanded with things like RuneMetrics lookups, nicer layouting, starting locations, etc.

Then again, this tool links to the Wiki for all those needs; it's just to give a general direction ;)


Thanks!
