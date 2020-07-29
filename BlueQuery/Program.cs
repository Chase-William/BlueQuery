using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using Newtonsoft.Json;

namespace BlueQuery
{
    public class Program
    {
        public static DiscordClient Client { get; set; }           // Our bot client
        public CommandsNextModule Commands { get; set; }    // Commands module
        
        /// <summary>
        ///     The bluequery Library.
        /// </summary>
        //public static BlueQueryLibrary.BlueQueryLibrary BQL { get; set; } = new BlueQueryLibrary.BlueQueryLibrary();

        static void Main(string[] args)
        {          
            var prog = new Program();                       
            prog.RunBotAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        ///     Initializes the bot and connects to discord.
        /// </summary>
        public async Task RunBotAsync()
        {
            // first, let's load our configuration file
            var json = "";
            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync();

            // next, let's load the values from that file
            // to our client's configuration
            var cfgjson = JsonConvert.DeserializeObject<ConfigJson>(json);

            // then we want to instantiate our client
            Client = new DiscordClient(new DiscordConfiguration
            {
                Token = cfgjson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true
            });

            // adding method addresses to eventhandler
            Client.Ready += this.Client_Ready;
            Client.GuildAvailable += this.Client_GuildAvailable;
            Client.ClientErrored += this.Client_ClientError;

            // setting up command configurations
            var ccfg = new CommandsNextConfiguration()
            {
                StringPrefix = cfgjson.CommandPrefix,
                EnableDms = false
            };

            // enabling commands on this bot
            Commands = Client.UseCommandsNext(ccfg);

            // Binding command classes
            Commands.RegisterCommands<Commands.General.Commands>();
            Commands.RegisterCommands<Commands.Crafting.CraftingCommands>();            
            Commands.RegisterCommands<Commands.TribeCommands>();

            // connecting and logging in
            await Client.ConnectAsync();

            // prevents premature quitting
            await Task.Delay(-1);
        }

        #region Client Event Handlers

        /// <summary>
        ///     Handles the client event for being available for use
        /// </summary>
        private Task Client_Ready(ReadyEventArgs e)
        {
            // let's log the fact that this event occured
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "ExampleBot", "Client is ready to process events.", DateTime.Now);

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Handles the clients event that a guild is available
        /// </summary>
        private Task Client_GuildAvailable(GuildCreateEventArgs e)
        {
            // let's log the name of the guild that was just
            // sent to our client
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "ExampleBot", $"Guild available: {e.Guild.Name}", DateTime.Now);

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Handles a client error
        /// </summary>
        private Task Client_ClientError(ClientErrorEventArgs e)
        {
            // let's log the details of the error that just 
            // occured in our client
            e.Client.DebugLogger.LogMessage(LogLevel.Error, "Kraumbot", $"Exception occured: {e.Exception.GetType()}: {e.Exception.Message}", DateTime.Now);

            return Task.CompletedTask;
        }

        #endregion
    }

    // this structure will hold data from config.json when loaded
    public struct ConfigJson
    {
        [JsonProperty("token")]
        public string Token { get; private set; }

        [JsonProperty("prefix")]
        public string CommandPrefix { get; private set; }

        //[JsonProperty("databaseFilePath")]
        //public string DatabaseFilePath { get; private set; }
    }
}

