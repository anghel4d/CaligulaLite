using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace CaligulaLite.Services
{
    public class CommandHandler
    {
        // fields
        private readonly IConfiguration _config;
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _services;

        public CommandHandler(IServiceProvider services)
        {
            // populate fields
            _config = services.GetRequiredService<IConfiguration>();
            _commands = services.GetRequiredService<CommandService>();
            _client = services.GetRequiredService<DiscordSocketClient>();
            _services = services;

            // take action when command executed
            _commands.CommandExecuted += CommandExecutedAsync;

            // take action when we receive a message (so it can be processed and see if its a command)
            _client.MessageReceived += MessageReceivedAsync;
        }

        public async Task InitializeAsync()
        {
            // register modules that are public and inherit ModuleBase<T>.
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        // Take actions upon receiving messages
        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            // ensure we dont process system or other bot messages
            if(!(rawMessage is SocketUserMessage message) || (message.Source != MessageSource.User))
                return;

            // set argument position away from prefix
            var argPos = 0;

            // get prefix char
            char prefix = Char.Parse(_config["Prefix"]);

            //determine if message has valid prefix and adjust argPos based on prefix
            if (!(message.HasMentionPrefix(_client.CurrentUser, ref argPos) || message.HasCharPrefix(prefix, ref argPos))) 
                return;

            var context = new SocketCommandContext(_client, message);

            // execute command if match found
            await _commands.ExecuteAsync(context, argPos, _services);
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            // if a command isn't found log and exit method
            if(!command.IsSpecified)
            {
                System.Console.WriteLine($"Command failed to execute for [{context.User.Username}] <-> [{result.ErrorReason}]!");
                return;
            }

            // log success and exit
            if(result.IsSuccess)
            {
                System.Console.WriteLine($"Command [{command.Value.Name}] executed for -> [{context.User.Username}]");
                return;
            }

            // failure scenario: let the user know
            await context.Channel.SendMessageAsync($"Sorry, @{context.User.Username}... something went wrong -> [{result}]!");
        }
    }
}