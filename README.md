# DiscordSlashCommandStarter
Starting template for a Discord bot using slash commands.

## Setting Up
You need to create an application at [Discord](https://discord.com/developers/applications).

Then create a bot on the `Bot` tab on the lefthand side.

Copy the token from the page and place that in the appropriate `appsettings` file if you dare, or the preferred way, create a matching key with either `User Secrets` or `Environment Variables`.

On the Discord `OAuth2` tab, scroll to the bottom check the `applications.commands` box and copy the link below.

Paste this in your browser to invite the bot to your server.

In your Discord server with `Developer Mode` enabled in the `Advanced` tab, right-click your server's icon and click `Copy ID`, this is your Guild ID, which you can repeat the same steps above like you did with the bot token to place it either in the appsettings/user secrets/environment variables. 
