using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;


namespace FirstDiscordBot
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public async Task Pincmd()
        {
            await Context.Message.ReplyAsync("ping"); //ответить тегая
        }
        [Command("del")]
        [Summary("Deletes a specified amount of messages")]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task DelMesAsync(int delnum)
        {
            var messages = Context.Channel.GetMessagesAsync(delnum).Flatten();
            foreach (var h in await messages.ToArrayAsync())
            {
                await this.Context.Channel.DeleteMessageAsync(h);
            }
        }
        [Command("createLog")]
        [RequireUserPermission(GuildPermission.Administrator)]
        private async Task Log()
        {
            var channel = Context.Guild.Channels.SingleOrDefault(x => x.Name == "log");

            if (channel == null)  
            {
                
                var newChannel = await Context.Guild.CreateTextChannelAsync("log");
                var id = Context.Message.Author.Id;
                var newChannelId = newChannel.Id;
                await ReplyAsync("Created channel ``'log'``");
                //await newChannel.AddPermissionOverwriteAsync(AddGuildUserProper.EveryoneRole, OverwritePermissions.DenyAll(newChannel));

            }
            else // reply that the channel already exists
            {
                await ReplyAsync("Unable to create channel ``log`` channel already exists.");
            }
        }
        
        [Command("random")]
        public async Task RandomCommand(int min , int max)
        {
            Random r = new Random();
            await Context.Message.ReplyAsync(r.Next(min, max).ToString());
        }
        [Command("avatar")]
        public async Task AvatarCommand(IGuildUser u)
        {
            var users = Context.Message.MentionedUsers;
            var subject = users.First();
            var embedBuiler = new EmbedBuilder()
            .WithImageUrl(subject.GetAvatarUrl( ImageFormat.Auto , size:300))
            .WithColor(Color.Green)
            .WithCurrentTimestamp();
            await Context.Message.ReplyAsync(embed: embedBuiler.Build());
        }
        [Command("kiss")]
        public async Task KissCommand(IGuildUser u)
        {
            Random random = new Random();
            var b = new EmbedBuilder().
              WithDescription($"**{Context.User.Mention} поцеловал(а) {u.Mention}!**\n");
           
            int num = random.Next(1, 4);
            switch (num)
            {
                case 1:
                    b.WithImageUrl("https://i.gifer.com/Dcnz.gif");
                break;
                    case 2:
                    b.WithImageUrl("https://c.tenor.com/I8kWjuAtX-QAAAAC/anime-ano.gif");
                    break;
                case 3:
                    b.WithImageUrl("https://c.tenor.com/NO6j5K8YuRAAAAAC/leni.gif");
                    break;
                case 4:
                    b.WithImageUrl("https://c.tenor.com/9jB6M6aoW0AAAAAS/val-ally-kiss.gif");
                    break;

            }
            Embed embed = b.Build();
            await ReplyAsync(embed: embed);
         }
        [Command("hit")]
        public async Task HitCommand(IGuildUser u)
        {
            Random random = new Random();
            var b = new EmbedBuilder().
              WithDescription($"**{Context.User.Mention} ударил(а) {u.Mention}!**\n");

            int num = random.Next(1, 4);
            switch (num)
            {
                case 1:
                    b.WithImageUrl("https://c.tenor.com/1T5bgBYtMgUAAAAS/head-hit-anime.gif");
                    break;
                case 2:
                    b.WithImageUrl("https://c.tenor.com/Y8_ITfFMQmMAAAAC/yue-arifureta.gif");
                    break;
                case 3:
                    b.WithImageUrl("https://c.tenor.com/Uw63A_uIWE8AAAAS/ijiranaide-nagatoro-anime.gif");
                    break;
                case 4:
                    b.WithImageUrl("https://c.tenor.com/g_9NDHUmUdgAAAAC/anime.gif");
                    break;

            }
            Embed embed = b.Build();
            await ReplyAsync(embed: embed);
         }
        [Command("hug")]
        public async Task HugCommand(IGuildUser u)
        {
            Random random = new Random();
            var b = new EmbedBuilder().
              WithDescription($"**{Context.User.Mention} обнял(а) {u.Mention}!**\n");

            int num = random.Next(1, 4);
            switch (num)
            {
                case 1:
                    b.WithImageUrl("https://c.tenor.com/XyMvYx1xcJAAAAAS/super-excited.gif");
                    break;
                case 2:
                    b.WithImageUrl("https://c.tenor.com/J7eGDvGeP9IAAAAS/enage-kiss-anime-hug.gif");
                    break;
                case 3:
                    b.WithImageUrl("https://c.tenor.com/lzKyZchfMzAAAAAS/anime-hug.gif");
                    break;
                case 4:
                    b.WithImageUrl("https://c.tenor.com/V3GQKvQjej0AAAAS/love-gif-sad.gif");
                    break;

            }
            Embed embed = b.Build();
            await ReplyAsync(embed: embed);
        }
        [Command("SonyaLox")]
        public async Task GetMyIdAsync()
        {
            var builder = new ComponentBuilder()
            .WithButton("Да", "1",ButtonStyle.Success)
            .WithButton("Нет", "2", ButtonStyle.Danger);
          
            await Context.Message.ReplyAsync("Соня лох?", components: builder.Build() ); 
        }
         
        [Command("Ban")]
        [RequireBotPermission(GuildPermission.BanMembers, ErrorMessage = "U dont can")]
        public async Task BanCommand(IGuildUser user , string reason  )
        {
       
            if (user == null)
            {
                await ReplyAsync("User == null");
                return;
            }
            var embedBuilder = new EmbedBuilder()
            .WithDescription($":white_check_mark: {user.Mention} В бан пошла\n**причина:** {reason}")
            .WithColor(new Color(255, 0, 0));
            Embed embed = embedBuilder.Build();
            await ReplyAsync(embed: embed);
            await Context.Guild.AddBanAsync(user,1,reason);

        }
        [Command("Kick")]
        [RequireBotPermission(GuildPermission.KickMembers, ErrorMessage = "U dont can")]
        public async Task KickCommand (IGuildUser user , string reason)
        {
            if(user == null)
            {
                await ReplyAsync("user == null");
                    return;
            }
            var embedBuilder = new EmbedBuilder()
             .WithDescription($":white_check_mark: {user.Mention} В кик пошла\n**причина:** {reason}")
             .WithColor(Color.DarkBlue);
            Embed embed = embedBuilder.Build();
            await ReplyAsync(embed: embed);
            // Channel
            //var channel = _client.channels.cache.find(channel => channel.name == "Log")
            //channel.send(message)
            await user.KickAsync();
        }

        [Command("mommy")]
        public async Task RoleMommy()
        {
            var user = Context.User;
            var role = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Мамка сервера");
            await (user as IGuildUser).AddRoleAsync(role);
            var embedBuilder = new EmbedBuilder()
           .WithDescription($":white_check_mark: {user.Mention} Ты получил роль {role.Name}")
           .WithColor(new Color(255, 0, 0));

        }
    }
}
