using Discord.Commands;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace CaligulaLite.Modules
{
    public class DiceCommands : ModuleBase
    {
        [Command("roll")]
        [Alias("r")]
        public async Task RollCommand([Remainder]string args = null)
        {
            var user = Context.User;

            // Do nothing if arguments are empty
            if(args == null)
            {
                await ReplyAsync($"{user.Mention}\nYou have to type something in retard.");
                return;
            }
                
            string result = DiceRoll(args);
            
            await ReplyAsync($"{user.Mention}\n``{args}`` -> {result}");
        }

        private static string DiceRoll(string args = null)
        {
            StringBuilder result = new StringBuilder();
            Random rng = new Random();
            var data = args.Split('d');
            ulong k = String.IsNullOrEmpty(data[0]) ? 1 : ulong.Parse(data[0]);
            int x = int.Parse(data[1]);
            ulong sum = 0;
            ulong j = 0;
            do
            {
                int y = rng.Next(1, x + 1);
                sum += (ulong)y;
                j++;
            } while (j < k) ;

            return sum.ToString();
        }
    }
}