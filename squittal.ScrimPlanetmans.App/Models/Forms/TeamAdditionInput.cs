using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace squittal.ScrimPlanetmans.Models.Forms
{
    public class TeamAdditionInput
    {
        //[RegularExpression("^(\\[([A-Za-z0-9]{1,4})\\])$|^((([A-Za-z0-9]{1,32},?[ ]?)+))$")]
        //[RegularExpression("^(\\[([A-z0-9]{1,4})\\])$|^((([A-z0-9]{1,32},?[ ]?)+))$")]
        //[RegularExpression("^(\\[([A-z0-9]{1,4})\\])$|^([A-z0-9, ])*$")]
        //[RegularExpression("^(\\[([A-Za-z0-9]{1,4})\\])$|^([A-Za-z0-9, ])*$")]
        public string Value { get; set; }

        public TeamAdditionInputType InputType => GetInputType();

        public TeamAdditionInput()
        {
            Value = string.Empty;
        }

        public TeamAdditionInput(string input)
        {
            Value = input;
        }

        private TeamAdditionInputType GetInputType()
        {
            return GetInputType(Value);
        }

        public static TeamAdditionInputType GetInputType(string input)
        {
            //Console.WriteLine("Checking if Null or White Space");
            if (string.IsNullOrWhiteSpace(input))
            {
                //Console.WriteLine("Is Null or White Space");
                return TeamAdditionInputType.Null;
            }

            //Console.WriteLine("Checking if Outfit Alias");
            if (IsOutfitAlias(input))
            {
                //Console.WriteLine("Is Outfit Alias");
                return TeamAdditionInputType.Outfit;
            }

            //Console.WriteLine("Checking if Character List");
            if (IsCharacterList(input))
            {
                //Console.WriteLine("Is Character List");
                return TeamAdditionInputType.CharacterList;
            }

            //Console.WriteLine("Checking if Character");
            if (IsCharacter(input))
            {
                //Console.WriteLine("Is Character");
                return TeamAdditionInputType.Character;
            }

            //Console.WriteLine("Return Invalid");
            //else
            //{
                return TeamAdditionInputType.Invalid;
            //}
        }

        private static bool IsOutfitAlias(string input)
        {
            return Constants.OutfitAliasWithBracketsRegex.Match(input).Success;
        }

        private static bool IsCharacterList(string input)
        {
            return input.Contains(",") && Constants.CharacterInputListRegex.Match(input).Success;
        }

        private static bool IsCharacter(string input)
        {
            //return !input.Contains(",") && Constants.CharacterInputListRegex.Match(input).Success;
            return !input.Contains(",") && Constants.CharacterNameRegex.Match(input).Success;
        }

        public string GetOutfitAlias()
        {
            if (InputType != TeamAdditionInputType.Outfit)
            {
                return null;
            }

            return Constants.StripNonLettersOrDigits(Value);
        }

        public IEnumerable<string> GetCleanedCharacterList()
        {
            if (InputType != TeamAdditionInputType.CharacterList)
            {
                return null;
            }

            char[] characters = Value.ToCharArray();

            characters = Array.FindAll(characters, (c => (char.IsLetterOrDigit(c)
                                                                || c == ',')));

            return new string(characters).Split(",").ToList();
        }

        public string GetCharacter()
        {
            if (InputType != TeamAdditionInputType.Character)
            {
                return null;
            }

            return Constants.StripNonLettersOrDigits(Value);
        }
    }
}
