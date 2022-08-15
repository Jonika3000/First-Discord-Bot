using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Interactions;
using Microsoft.Extensions.Hosting;
using FirstDiscordBot;
using Discord.Net;

class Program
{
    private DiscordSocketClient _client;
    private CommandService commands;
    private IServiceProvider services;


    static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

    public async Task MainAsync()
    {
        commands = new CommandService();
        var _config = new DiscordSocketConfig
        {
            AlwaysDownloadUsers = true,
            MessageCacheSize = 1000,
            UseInteractionSnowflakeDate = false,
            GatewayIntents =
                GatewayIntents.Guilds |
                GatewayIntents.GuildMembers |
                GatewayIntents.GuildMessageReactions |
                GatewayIntents.GuildMessages |
                GatewayIntents.GuildVoiceStates
        };

        _client = new DiscordSocketClient(_config);
        string token = "token";
        _client.Log += LogAsync;
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();
        _client.Ready += _client_Ready;

        await commands.AddModulesAsync(Assembly.GetEntryAssembly(), services);
       
 
        _client.MessageUpdated += MessageUpdated;
        _client.MessageDeleted += _client_MessageDeleted;
        _client.MessageReceived += HandleCommandAsync;
        _client.SlashCommandExecuted += _client_SlashCommandExecuted;
        _client.ButtonExecuted += _client_ButtonExecuted;

        await Task.Delay(-1);
    }
    private async Task LogAsync(LogMessage message)
           => Console.WriteLine(message.ToString());
  

    private async Task _client_ButtonExecuted(SocketMessageComponent arg)
    {
        switch (arg.Data.CustomId)
        {
            case "1":
                await arg.RespondAsync($"{arg.User.Mention} its true!");
                break;
        }
    }

    private async Task _client_Ready()
    { 
        var guildCommand1 = new SlashCommandBuilder();
        guildCommand1.WithName("help");
        guildCommand1.WithDescription("Command List");
        var guildCommand = new SlashCommandBuilder()
        .WithName("list-roles")
        .WithDescription("Lists all roles of a user.")
        .AddOption("user", ApplicationCommandOptionType.User, "The users whos roles you want to be listed", isRequired: true);
 
        try
        {
            await _client.CreateGlobalApplicationCommandAsync(guildCommand.Build());
            await _client.CreateGlobalApplicationCommandAsync(guildCommand1.Build());
        }
        catch (ApplicationCommandException exception)
        {

        }
     
    }

    private async Task _client_SlashCommandExecuted(SocketSlashCommand command)
    {
        switch (command.Data.Name)
        {
            case "list-roles":
                await HandleListRoleCommand(command);
                break;
            case "help":
                await HandleHelpCommand(command);
                break;

        }
    }


    private async Task HandleHelpCommand(SocketSlashCommand command)
    {

        var embedBuiler = new EmbedBuilder()
           .WithTitle("All commands:")
           .AddField("For users", "**!kiss @tag - u kiss someone**\n" +
           "**!hug @tag - u hug someone**\n" + "**!hit @tag - u hit someone**\n" + "**!avatar @tag - view user avatar**\n"
           + "**!random min,max - random num in the range\n**")
           .AddField("All commands for admin:\n", "**!kick @tag, reason- u kick someone**\n" +
           "**!ban @tag, time, reason - u ban someone**\n" + "**!del count - deletes a certain number of messages**\n"
           + "**!createLog - create a channel in which there will be deletion, message editing logs**")
           .WithColor(Color.Green)
           .WithCurrentTimestamp();

        await command.RespondAsync(embed: embedBuiler.Build());
    }
   
    private async Task HandleListRoleCommand(SocketSlashCommand command)
    {
        var guildUser = (SocketGuildUser)command.Data.Options.First().Value;
        var roleList = string.Join(",\n", guildUser.Roles.Where(x => !x.IsEveryone).Select(x => x.Mention));
        var embedBuiler = new EmbedBuilder()
            .WithAuthor(guildUser.ToString(), guildUser.GetAvatarUrl() ?? guildUser.GetDefaultAvatarUrl())
            .WithTitle("Roles")
            .WithDescription(roleList)
            .WithColor(Color.Green)
            .WithCurrentTimestamp();

        await command.RespondAsync(embed: embedBuiler.Build());
    }

    private async Task _client_MessageDeleted(Cacheable<IMessage, ulong> arg1, Cacheable<IMessageChannel, ulong> arg2)
    {
        IMessage msg = await arg1.GetOrDownloadAsync();
        var channel = msg.Channel;
        ulong GuidC = GetGuild((ISocketMessageChannel)channel);
        if (GuidC != 0)
        {
            var chnl = _client.GetChannel(GuidC) as IMessageChannel;
            if (msg != null)
            {
                Console.WriteLine($"Delete message: \"{msg.Content}\" , from \"{msg.Channel}\"");
                await chnl.SendMessageAsync($"Delete message: \"{msg.Content}\" , from \"{msg.Channel}\" , author:{msg.Author}");
            }
        }
        else
        {
            Console.WriteLine("No log channel");
        }
    }
    private ulong GetGuild(ISocketMessageChannel channel)
    {
        ulong tmp = 0;
        SocketGuild server = ((SocketGuildChannel)channel).Guild;//get server guid
        var guild = _client.GetGuild(server.Id);
        var channels = guild.Channels.Where(x => !(x is ICategoryChannel));//get list of text channels
        foreach (var c in channels)
        {
            if (c.Name == "log")
            {
                tmp = c.Id;
                break;
            }
        }
        return tmp;
    }
    private async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
    {
        ulong GuidC = GetGuild(channel);

        if (GuidC != 0)
        {
            var chnl = _client.GetChannel(GuidC) as IMessageChannel;
            var msg = await before.GetOrDownloadAsync();
            if (msg != null)
            {
                Console.WriteLine($"{msg} -> {after}");
                await chnl.SendMessageAsync($"Edit message: \"{msg}\"-> \"{after}\" , from \"{msg.Channel}\" author:\"{msg.Author}\"");
            }
        }
        else
        {
            Console.WriteLine("No log channel");
        }

    }
    private async Task HandleCommandAsync(SocketMessage arg)
    {
            var message = arg as SocketUserMessage;
            if(message != null)
            {
                var context = new SocketCommandContext(_client, message);
                if (message.Author.IsBot)
                    return;

                int argPos = 0;
                if (message.HasStringPrefix("!", ref argPos))
                {
                    var result = await commands.ExecuteAsync(context, argPos, services);
                    if (!result.IsSuccess)
                        Console.WriteLine(result.ErrorReason);
                    if (result.Error.Equals(CommandError.UnmetPrecondition))
                        await message.Channel.SendMessageAsync(result.ErrorReason);
                }
            }
        
    }
}
