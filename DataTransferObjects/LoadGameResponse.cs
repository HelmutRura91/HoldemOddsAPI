﻿namespace HoldemOddsAPI.DataTransferObjects
{
    public class LoadGameResponse
    {
        public PlayerStackInfo HighestStack {  get; set; }
        public PlayerStackInfo LowestStack { get; set; }
        public List<string> ListOfPlayersBehindEntryStack { get; set; }
        public List<string> ListOfPlayersAboveEntryStack { get; set; }
    }

    public class PlayerStackInfo
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }
}