﻿using System;
using System.Linq;
using System.Threading.Tasks;
using PoGo.NecroBot.Logic.Common;
using PoGo.NecroBot.Logic.PoGoUtils;
using PoGo.NecroBot.Logic.State;

namespace PoGo.NecroBot.Logic.Service.TelegramCommand
{
    public class AllCommand : CommandMessage
    {
        public override string Command => "/all";
        public override bool StopProcess => true;
        public override TranslationString DescriptionI18NKey => TranslationString.TelegramCommandAllDescription;
        public override TranslationString MsgHeadI18NKey => TranslationString.TelegramCommandAllMsgHead;

        public AllCommand(TelegramUtils telegramUtils) : base(telegramUtils)
        {
        }

        public override async Task<bool> OnCommand(ISession session, string cmd, Action<string> callback)
        {
            string[] messagetext = cmd.Split(' ');

            if (messagetext[0].ToLower() == Command)
            {
                var answerTextmessage = "";
                var myPokemons = await session.Inventory.GetPokemons();
                var allMyPokemons = myPokemons.ToList();
                var allPokemons = await session.Inventory.GetHighestsCp(allMyPokemons.Count);

                if (messagetext.Length == 1)
                {
                    allPokemons = await session.Inventory.GetHighestsCp(allMyPokemons.Count);
                }
                else if (messagetext.Length == 2)
                {
                    if (messagetext[1] == "iv")
                    {
                        allPokemons = await session.Inventory.GetHighestsPerfect(allMyPokemons.Count);
                    }
                    else if (messagetext[1] != "cp")
                    {
                        answerTextmessage =
                            session.Translation.GetTranslation(TranslationString.UsageHelp, "/all [cp/iv]");
                    }
                }
                else
                {
                    answerTextmessage =
                        session.Translation.GetTranslation(TranslationString.UsageHelp, "/all [cp/iv]");
                }

                foreach (var pokemon in allPokemons)
                {
                    answerTextmessage += session.Translation.GetTranslation(TranslationString.ShowPokeTemplate,
                        pokemon.Cp, PokemonInfo.CalculatePokemonPerfection(pokemon).ToString("0.00"),
                        session.Translation.GetPokemonTranslation(pokemon.PokemonId));

                    if (answerTextmessage.Length > 3800)
                    {
                        callback(answerTextmessage);
                        answerTextmessage = "";
                    }
                }

                callback(answerTextmessage);
                return true;
            }
            return false;
        }
    }
}