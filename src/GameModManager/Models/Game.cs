﻿using GameModManager.Services.Container;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace GameModManager.Models
{
 
    /// <summary>
    /// Class to represent a game to update it's mods
    /// </summary>
    public class Game
    {
        /// <summary>
        /// Path to the exe of the game to get the image from
        /// </summary>
        public string GameExe { get; private set; }

        /// <summary>
        /// The folder to place the downloaded files into
        /// </summary>
        public string TargetFolderPath { get; }

        /// <summary>
        /// The remote url to get the mod from
        /// </summary>
        public string RemoteUrl => url.Value.Url;

        /// <summary>
        /// The remote url to get the mod from
        /// </summary>
        private readonly Lazy<UrlOpener> url;

        /// <summary>
        /// The provider to use for loading the mod and updating the client
        /// </summary>
        public ModProvider DataProviderToUse { get; }

        /// <summary>
        /// The name of the game you are updating
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Last time the game was synced with the remote source
        /// </summary>
        public DateTime LastUpdateTime { get; private set; }

        /// <summary>
        /// The current locally installed version
        /// </summary>
        public Version LocalVersion { get; private set; }

        /// <summary>
        /// The current remote installed version
        /// </summary>
        public Version RemoteVersion { get; private set; }

        /// <summary>
        /// Create a new instance of the game class
        /// </summary>
        /// <param name="gameExe">The game exe to get the image from</param>
        /// <param name="name">The name of the game to show</param>
        /// <param name="targetFolderPath">The folder to place the download in</param>
        /// <param name="remoteUrl">The remote url to get the data from</param>
        /// <param name="dataProviderToUse">The data provider to use</param>
        public Game(string gameExe, string name, string targetFolderPath, string remoteUrl, ModProvider dataProviderToUse)
        {
            Name = name;
            GameExe = gameExe;
            TargetFolderPath = targetFolderPath;
            url = new Lazy<UrlOpener>(() => new UrlOpener(remoteUrl));
            DataProviderToUse = dataProviderToUse;

            LocalVersion = new Version(0, 0, 0);
            RemoteVersion = new Version(0, 0, 0);
        }

        /// <summary>
        /// Open the link in the browser
        /// </summary>
        /// <returns>Task you can await which will be done as soon as it was opened in the browser</returns>
        public async Task OpenLinkInBrowser()
        {
            await Task.Run(() => url.Value.OpenInBrowser()).ConfigureAwait(false);
        }

        /// <summary>
        /// Load the game image and return the data stream
        /// </summary>
        /// <returns>A stream with the game image</returns>
        public async Task<Stream> LoadGameImage()
        {
            return await Task<Stream>.Run(() =>
            {
                MemoryStream returnStream = new MemoryStream();
                if (!File.Exists(GameExe))
                {
                    returnStream.Close();
                    return returnStream;
                }
                Icon loadedIcon = Icon.ExtractAssociatedIcon(GameExe);
                loadedIcon.ToBitmap().Save(returnStream, ImageFormat.Png);
                returnStream.Position = 0;
                return returnStream;
            });
        }
    }
}
